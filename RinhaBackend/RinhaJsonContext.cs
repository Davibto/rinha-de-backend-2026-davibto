using System.Text.Json.Serialization;
using RinhaBackend.Models; 

namespace RinhaBackend;

[JsonSourceGenerationOptions(
    WriteIndented = false,
    PropertyNamingPolicy = JsonKnownNamingPolicy.SnakeCaseLower,
    GenerationMode = JsonSourceGenerationMode.Default)]
[JsonSerializable(typeof(FraudScoreRequest))]
[JsonSerializable(typeof(FraudScoreResponse))] 
public partial class RinhaJsonContext : JsonSerializerContext
{
}