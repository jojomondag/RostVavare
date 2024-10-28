using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Whisper.net;
using Whisper.net.Wave;

namespace RöstVävare.Services
{
    public class WhisperService
    {
        private readonly string _modelPath;

        public WhisperService(string modelPath = "ggml-large-v3-turbo.bin")
        {
            _modelPath = modelPath;
        }

        public Task DownloadModelAsync()
        {
            if (!File.Exists(_modelPath))
            {
                throw new FileNotFoundException($"Model file '{_modelPath}' not found. Please ensure it is placed in the correct directory.");
            }
            else
            {
                Console.WriteLine($"Using existing model file {_modelPath}.");
            }

            return Task.CompletedTask;
        }

        public string? LanguageIdentification(string fileName, string language = "auto")
        {
            try
            {
                using var factory = WhisperFactory.FromPath(_modelPath);

                var builder = factory.CreateBuilder()
                   .WithLanguage(language);

                using var processor = builder.Build();

                using var fileStream = File.OpenRead(fileName);

                var wave = new WaveParser(fileStream);

                var samples = wave.GetAvgSamples();

                var detectedLanguage = processor.DetectLanguage(samples) ?? "Unknown";
                return detectedLanguage;
            }
            catch (Exception ex)
            {
                return "An error occurred during language identification: " + ex.Message;
            }
        }

        public async Task TranscribeAsync(string fileName, string language = "auto", bool translate = false, Action<string>? onSegmentReceived = null)
        {
            try
            {
                using var factory = WhisperFactory.FromPath(_modelPath);

                var builder = factory.CreateBuilder()
                    .WithLanguage(language);

                if (translate)
                {
                    builder.WithTranslate();
                }

                using var processor = builder.Build();

                using var fileStream = File.OpenRead(fileName);

                await foreach (var segment in processor.ProcessAsync(fileStream, CancellationToken.None))
                {
                    onSegmentReceived?.Invoke($"New Segment: {segment.Start} ==> {segment.End} : {segment.Text}");
                }
            }
            catch (Exception ex)
            {
                onSegmentReceived?.Invoke("An error occurred during transcription: " + ex.Message);
            }
        }
    }
}
