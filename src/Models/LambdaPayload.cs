namespace ObjectAnalysis.Models;

public record LambdaPayload
{
    public string ObjectKey { get; init; }
    public float Confidence { get; init; }
}
