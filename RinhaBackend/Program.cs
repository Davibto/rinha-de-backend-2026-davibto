using RinhaBackend.Models;
using RinhaBackend.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<DataLoaderService>();
builder.Services.AddSingleton<NormalizationService>();
builder.Services.AddSingleton<VpTreeService>();
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.SnakeCaseLower;
});

var app = builder.Build();

var dataLoader = app.Services.GetRequiredService<DataLoaderService>();
dataLoader.LoadBinFile("Data/references.bin");

var vpTree = app.Services.GetRequiredService<VpTreeService>();

int raizId = vpTree.CreateTree(0, dataLoader.Dataset.Length - 1);

app.MapGet("/ready", () => Results.Ok());



app.MapPost("/fraud-score", (FraudScoreRequest payload, NormalizationService normalizer, VpTreeService vpTree) =>
{
    try
    {
        sbyte[] vetorNormalizado = normalizer.NormalizePayload(payload);

        var vizinhos = vpTree.Search(vetorNormalizado, k: 5);

        int numeroDeFraudes = vizinhos.Count(v => v.IsFraud);

        double fraudScore = numeroDeFraudes / 5.0;

        bool approved = fraudScore < 0.6;

        return Results.Ok(new
        {
            approved = approved,
            fraud_score = fraudScore
        });
    }
    catch (Exception ex)
    {
        return Results.Problem($"Erro no processamento do score: {ex.Message}");
    }
});

app.Run();