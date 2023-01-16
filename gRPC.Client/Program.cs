using Grpc.Net.Client;
using gRPC.Server;
using Newtonsoft.Json.Linq;

namespace gRPC.Client
{
	internal class Program
	{
		private static readonly Settings Settings = Settings.Get();
		private static void Main(string[] args)
		{
			var watcher = new Watcher(Settings.FilePath, Settings.ExtensionFilter);

			watcher.Start();
			watcher.Created += Watcher_Created;

			Console.WriteLine("Press 'q' to quit the sample.");

			while (Console.Read() != 'q')
			{}

			watcher.Stop();
		}

		private static async void Watcher_Created(object sender, FileSystemEventArgs e)
		{
			var channel = GrpcChannel.ForAddress(Settings.GrpcServerAddress);
			
			var client = new FileWriter.FileWriterClient(channel);

			using var call = client.WriteFileInputInformation();

			foreach (var line in File.ReadLines(e.FullPath))
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

			Console.WriteLine(response);
		}
	}
}