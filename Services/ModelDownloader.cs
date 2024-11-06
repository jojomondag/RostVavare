using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

public enum GgmlType
{
    Tiny,
    TinyEn,
    Base,
    BaseEn,
    Small,
    SmallEn,
    Medium,
    MediumEn,
    LargeV1,
    LargeV2,
    LargeV3,
    LargeV3Turbo
}

public static class ModelDownloader
{
    private static readonly Lazy<HttpClient> httpClient = new(() => new HttpClient() { Timeout = Timeout.InfiniteTimeSpan });

    public static async Task DownloadModel(string fileName, GgmlType ggmlType)
    {
        Console.WriteLine($"Downloading Model {fileName}");
        using var modelStream = await GetGgmlModelAsync(ggmlType);
        using var fileWriter = File.OpenWrite(fileName);
        await modelStream.CopyToAsync(fileWriter);
    }

    private static async Task<Stream> GetGgmlModelAsync(GgmlType type, CancellationToken cancellationToken = default)
    {
        var modelName = GetModelName(type);
        var url = $"https://huggingface.co/sandrohanea/whisper.net/resolve/v3/classic/{modelName}.bin";
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        var response = await httpClient.Value.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        response.EnsureSuccessStatusCode();
#if NETSTANDARD
        return await response.Content.ReadAsStreamAsync();
#else
        return await response.Content.ReadAsStreamAsync(cancellationToken);
#endif
    }

    private static string GetModelName(GgmlType type)
    {
        return type switch
        {
            GgmlType.Tiny => "ggml-tiny",
            GgmlType.TinyEn => "ggml-tiny.en",
            GgmlType.Base => "ggml-base",
            GgmlType.BaseEn => "ggml-base.en",
            GgmlType.Small => "ggml-small",
            GgmlType.SmallEn => "ggml-small.en",
            GgmlType.Medium => "ggml-medium",
            GgmlType.MediumEn => "ggml-medium.en",
            GgmlType.LargeV1 => "ggml-large-v1",
            GgmlType.LargeV2 => "ggml-large-v2",
            GgmlType.LargeV3 => "ggml-large-v3",
            GgmlType.LargeV3Turbo => "ggml-large-v3-turbo",
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }
}