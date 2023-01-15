using Microsoft.Extensions.Configuration;
using System.IO;
using System.Text.Json;
using static System.Net.Mime.MediaTypeNames;
using Newtonsoft.Json.Linq;

namespace gRPC.Client
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			var settings = Settings.Get();
			var watcher = new Watcher(settings.FilePath, settings.ExtensionFilter);
			
			watcher.Start();
			watcher.Created += Watcher_Created;

			Console.WriteLine("Press 'q' to quit the sample.");

			while (Console.Read() != 'q')
			{

			}

			watcher.Stop();
		}


		private static void Watcher_Created(object sender, FileSystemEventArgs e)
		{
			if (CheckIfFileAlreadyExists(Path.GetDirectoryName(e.FullPath)!, Path.GetFileName(e.FullPath))) 
			{
				Console.WriteLine("File with that name already exists in the directory.\nPlease try file with another name...");
				return;
			}

			Console.WriteLine($"File created: {e.FullPath}");

			var serializedFileInfo = new List<InputFile>();

			foreach (var line in File.ReadLines(e.FullPath))
			{
				var jObject = JObject.Parse(line);
				var type = (string)jObject["type"]!;
				serializedFileInfo.Add(new InputFile(type, line));
			}
		}

		private static bool CheckIfFileAlreadyExists(string directory, string fileName)
		{
			return Directory.EnumerateFiles(directory).Any(fullPath => Path.GetFileName(fullPath) == fileName);
		}
	}
}