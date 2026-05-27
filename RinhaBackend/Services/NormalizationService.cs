
using RinhaBackend.Models;

namespace RinhaBackend.Services
{
    public class NormalizationService
    {
        private readonly Dictionary<string, float> _mccRisk = new()
        {
            { "5411", 0.15f }, { "5812", 0.30f }, { "5912", 0.20f }, { "5944", 0.45f },
            { "7801", 0.80f }, { "7802", 0.75f }, { "7995", 0.85f }, { "4511", 0.35f },
            { "5311", 0.25f }, { "5999", 0.50f }
        };

        public sbyte[] NormalizePayload(FraudScoreRequest payload)
        {
            sbyte[] v = new sbyte[14];
            sbyte Q(float val) => (sbyte)Math.Round(val * 127f);

            // 0: amount
            v[0] = Q(Math.Clamp(payload.Transaction.Amount / 10000f, 0f, 1f));

            // 1: installments
            v[1] = Q(Math.Clamp(payload.Transaction.Installments / 12f, 0f, 1f));

            // 2: amount_vs_avg 
            float avgAmount = payload.Customer.AvgAmount > 0 ? payload.Customer.AvgAmount : 1f;
            v[2] = Q(Math.Clamp((payload.Transaction.Amount / avgAmount) / 10f, 0f, 1f));

            // 3: hour_of_day 
            v[3] = Q(payload.Transaction.RequestedAt.ToUniversalTime().Hour / 23.0f);

            // 4: day_of_week (seg=0, dom=6)
            int day = ((int)payload.Transaction.RequestedAt.DayOfWeek + 6) % 7;
            v[4] = Q(day / 6.0f);

      
            if (!payload.LastTransaction.HasValue)
            {
                v[5] = -127; 
                v[6] = -127;
            }
            else
            {
                // 5: minutes_since_last_tx
                float minutes = (float)(payload.Transaction.RequestedAt - payload.LastTransaction.Value.Timestamp).TotalMinutes;
                if (minutes < 0) minutes = 0;
                v[5] = Q(Math.Clamp(minutes / 1440f, 0f, 1f));

                // 6: km_from_last_tx
                v[6] = Q(Math.Clamp((float)payload.LastTransaction.Value.KmFromCurrent / 1000f, 0f, 1f));
            }

            // 7: km_from_home
            v[7] = Q(Math.Clamp((float)payload.Terminal.KmFromHome / 1000f, 0f, 1f));

            // 8: tx_count_24h
            v[8] = Q(Math.Clamp(payload.Customer.TxCount24h / 20f, 0f, 1f));

            // 9: is_online
            v[9] = payload.Terminal.IsOnline ? (sbyte)127 : (sbyte)0;

            // 10: card_present
            v[10] = payload.Terminal.CardPresent ? (sbyte)127 : (sbyte)0;

            // 11: unknown_merchant
            bool isKnown = payload.Customer.KnownMerchants?.Contains(payload.Merchant.Id) ?? false;
            v[11] = isKnown ? (sbyte)0 : (sbyte)127;

            // 12: mcc_risk (busca no dicionario, se nao achar usa 0.5)
            float risk = _mccRisk.GetValueOrDefault(payload.Merchant.Mcc, 0.5f);
            v[12] = Q(risk);

            // 13: merchant_avg_amount
            v[13] = Q(Math.Clamp(payload.Merchant.AvgAmount / 10000f, 0f, 1f));

            return v;
        }
    }
}