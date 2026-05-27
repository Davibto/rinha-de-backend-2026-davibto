using System.Text.Json.Serialization;

namespace RinhaBackend.Models;

public readonly record struct FraudScoreResponse(
    [property: JsonPropertyName("approved")] bool Approved,
    [property: JsonPropertyName("fraud_score")] double FraudScore
);