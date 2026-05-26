using System.Text.Json.Serialization;

namespace RinhaBackend.Models
{
    public readonly record struct FraudScoreRequest(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("transaction")] Transaction Transaction,
    [property: JsonPropertyName("customer")] Customer Customer,
    [property: JsonPropertyName("merchant")] Merchant Merchant,
    [property: JsonPropertyName("terminal")] Terminal Terminal,
    [property: JsonPropertyName("last_transaction")] LastTransaction? LastTransaction
);

    public readonly record struct Transaction(
        [property: JsonPropertyName("amount")] float Amount,
        [property: JsonPropertyName("installments")] int Installments,
        [property: JsonPropertyName("requested_at")] DateTime RequestedAt
    );

    public readonly record struct Customer(
        [property: JsonPropertyName("avg_amount")] float AvgAmount,
        [property: JsonPropertyName("tx_count_24h")] int TxCount24h,
        [property: JsonPropertyName("known_merchants")] List<string> KnownMerchants
    );

    public readonly record struct Merchant(
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("mcc")] string Mcc,
        [property: JsonPropertyName("avg_amount")] float AvgAmount
    );

    public readonly record struct Terminal(
        [property: JsonPropertyName("is_online")] bool IsOnline,
        [property: JsonPropertyName("card_present")] bool CardPresent,
        [property: JsonPropertyName("km_from_home")] float KmFromHome
    );

    public readonly record struct LastTransaction(
        [property: JsonPropertyName("timestamp")] DateTime Timestamp,
        [property: JsonPropertyName("km_from_current")] float KmFromCurrent
    );
}