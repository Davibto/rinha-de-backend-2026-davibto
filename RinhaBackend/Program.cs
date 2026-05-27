using RinhaBackend;           
using RinhaBackend.Models;
using RinhaBackend.Services;

var builder = WebApplication.CreateSlimBuilder(args);
builder.Logging.ClearProviders();
builder.Services.AddSingleton<DataLoaderService>();
builder.Services.AddSingleton<NormalizationService>();
builder.Services.AddSingleton<VpTreeService>();
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, RinhaJsonContext.Default);
});

var app = builder.Build();

var dataLoader = app.Services.GetRequiredService<DataLoaderService>();
dataLoader.LoadBinFile("Data/references.bin");

var vpTree = app.Services.GetRequiredService<VpTreeService>();

int raizId = vpTree.Build();
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

        return Results.Ok(new FraudScoreResponse(approved, fraudScore));
    }
    catch (Exception ex)
    {
        return Results.Problem($"Erro no processamento do score: {ex.Message}");
    }
});

app.Run();