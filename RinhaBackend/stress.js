import http from 'k6/http';
import { check, sleep } from 'k6';

export const options = {
  stages: [
    { duration: '10s', target: 50 },  // Ramp-up: sobe para 50 usuários simultâneos em 10s
    { duration: '30s', target: 50 },  // Estabiliza: mantém 50 usuários por 30s
    { duration: '10s', target: 0 },   // Ramp-down: desce para 0 usuários em 10s
  ],
};

export default function () {
  const url = 'http://localhost:9999/fraud-score';
  
    const payload = JSON.stringify({
        "id": "tx-123456",
        "transaction": {
            "amount": 150.50,
            "installments": 1,
            "requested_at": new Date().toISOString()
        },
        "customer": {
            "avg_amount": 120.00,
            "tx_count_24h": 3,
            "known_merchants": ["merch-A", "merch-B"]
        },
        "merchant": {
            "id": "merch-A",
            "mcc": "5411",
            "avg_amount": 200.00
        },
        "terminal": {
            "is_online": true,
            "card_present": false,
            "km_from_home": 12.5
        },
        "last_transaction": {
            "timestamp": new Date(Date.now() - 3600000).toISOString(), // 1 hora atrás
            "km_from_current": 5.0
        }
    });

  const params = {
    headers: {
      'Content-Type': 'application/json',
    },
  };

  const res = http.post(url, payload, params);

  check(res, {
    'status é 200': (r) => r.status === 200,
  });
    if (res.status !== 200) {
        console.log(`ERRO STATUS ${res.status}: ${res.body}`);
    }

  sleep(0.01); 
}