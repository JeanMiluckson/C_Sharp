using System;
using System.Text;
using Vosk;
using NAudio.Wave;
using System.Text.Json;

class Program
{
    static void Main(string[] args)
    {
        // 1. Carregar o modelo
        Vosk.Vosk.SetLogLevel(0);
        using var model = new Model("model");
        using var rec = new VoskRecognizer(model, 16000.0f);
        
        //2. Configure a captura do microfone
        using var capture = new WaveInEvent();
        capture.WaveFormat = new WaveFormat(16000, 1);

        capture.DataAvailable += (s, e) =>
        {
            if (rec.AcceptWaveform(e.Buffer, e.BytesRecorded))
            {
                var result = JsonDocument.Parse(rec.Result());
                string textoFinal = result.RootElement.GetProperty("text").GetString();
                if (!string.IsNullOrWhiteSpace(textoFinal))
                {
                    Console.WriteLine($"\n Texto: {textoFinal}");
                }
            }
            else
            {
                var partialResult = JsonDocument.Parse(rec.PartialResult());
                string textoParcial = partialResult.RootElement.GetProperty("partial").GetString();
                if (!string.IsNullOrWhiteSpace(textoParcial));
                {
                    Console.WriteLine($"\rOuvindo: {textoParcial}");
                }
            }

        };
        
        capture.StartRecording();
        Console.WriteLine("Gravador Iniciado! Fale no microfome...");
        Console.WriteLine("Presione [Enter] para sair.");
        Console.ReadLine();

        capture.StopRecording();
    }
}