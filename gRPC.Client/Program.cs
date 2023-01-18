using Google.Protobuf.WellKnownTypes;
using gRPC.Server;
using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.Client.Configuration;
using Newtonsoft.Json.Linq;
using static gRPC.Server.FileWriter;

namespace gRPC.Client
{
	internal class Program
	{
		private static readonly Settings Settings = Settings.Get();
		private static void Main()
		{
			var watcher = new Watcher(Settings.FilePath, Settings.ExtensionFilter);

			watcher.Start();
			watcher.Created += Watcher_Created;

			Console.WriteLine("Press 'q' to quit the sample.");

			while (Console.Read() != 'q')
			{ }

			watcher.Stop();
		}

		private static async void Watcher_Created(object sender, FileSystemEventArgs e)
		{
			var defaultMethodConfig = new MethodConfig
			{
				Names = { MethodName.Default },
				RetryPolicy = new RetryPolicy
				{
					MaxAttempts = 5,
					InitialBackoff = TimeSpan.FromSeconds(1),
					MaxBackoff = TimeSpan.FromSeconds(5),
					BackoffMultiplier = 1.5,
					RetryableStatusCodes = { StatusCode.Unavailable }
				}
			};
			var channel = GrpcChannel.ForAddress(Settings.GrpcServerAddress, new GrpcChannelOptions
			{
				ServiceConfig = new ServiceConfig { MethodConfigs = { defaultMethodConfig } }
			});
			var client = new FileWriterClient(channel);

			await WriteFileInformationAsync(client, e.FullPath);
			await GetAllFileTypesAsync(client);
			await GetObjectsByFileTypeStreamAsync(client);
		}

		private static async Task WriteFileInformationAsync(FileWriterClient client, string filePath)
		{
			using var call = client.WriteFileInputInformation();

			foreach (var line in File.ReadLines(filePath))
			{
				var type = (string)JObject.Parse(line)["type"]!;

				await call.RequestStream.WriteAsync(new WriteFileRequest
				{
					Type = type,
					Data = line
				});
			}

			await call.RequestStream.CompleteAsync();

			var response = await call.ResponseAsync;

			if (response.Success)
				File.Delete(filePath);

			Console.WriteLine($"File {filePath} deleted");
		}

		private static async Task GetObjectsByFileTypeStreamAsync(FileWriterClient client)
		{
			using var es = client.GetObjectsByFileTypeStream(new GetObjectsByFileTypeRequest { FileType = "eventlog" });

			Console.WriteLine("\nObjects by type eventlog\n");

			await foreach (var response in es.ResponseStream.ReadAllAsync(CancellationToken.None))
			{
				Console.WriteLine(response.Data);
				await Task.Delay(1000);
			}
		}

		private static async Task GetAllFileTypesAsync(FileWriterClient client)
		{
			var data = await client.GetAllFileTypesAsync(new Empty());

			Console.WriteLine("\nList of all object types.\n");

			foreach (var type in data.Types_)
			{
				Console.WriteLine(type);
				await Task.Delay(1000);
			}
		}
	}
}