using Google.Protobuf.WellKnownTypes;
using gRPC.Server.Options;
using Grpc.Core;
using Microsoft.Extensions.Options;

namespace gRPC.Server.Services
{
	public class FileService : FileWriter.FileWriterBase
	{
		private readonly ApplicationOptions _applicationOptions;
		public FileService(IOptionsMonitor<ApplicationOptions> configuration)
		{
			_applicationOptions = configuration.CurrentValue;
		}
		public override async Task<WriteFileResponse> WriteFileInputInformation(IAsyncStreamReader<WriteFileRequest> requestStream, ServerCallContext context)
		{
			var dataDict = new Dictionary<string, List<string>>();

			while (await requestStream.MoveNext() && !context.CancellationToken.IsCancellationRequested)
			{
				var fileInput = requestStream.Current;

				if (!dataDict.ContainsKey(fileInput.Type))
					dataDict.Add(fileInput.Type, new List<string>());

				dataDict[fileInput.Type].Add(fileInput.Data);
			}

			var writeTasks = new List<Task>();
			
			foreach (var group in dataDict.GroupBy(x => x.Key))
			{
				foreach (var (type, typeData) in group)
				{
					var folderToSave = $"{_applicationOptions.JsonFileStoragePath}\\{type}";
;					if (!Directory.Exists(folderToSave))
						Directory.CreateDirectory(folderToSave);

					writeTasks.Add(File.AppendAllLinesAsync(folderToSave + $"\\{DateTime.Now:yy-MM-dd}", typeData));
				}
			}

			await Task.WhenAll(writeTasks);

			return new WriteFileResponse { Success =  true };
		}
		public override Task<FileTypes> GetAllFileTypes(Empty request, ServerCallContext context)
		{
			var data = new FileTypes();

			foreach (var directory in Directory.EnumerateDirectories(_applicationOptions.JsonFileStoragePath))
			{
				data.Types_.Add(Path.GetFileName(directory));
			}

			return Task.FromResult(data);
		}
		public override async Task GetObjectsByFileTypeStream(GetObjectsByFileTypeRequest request, 
			IServerStreamWriter<GetObjectsByFileTypeResponse> responseStream, 
			ServerCallContext context)
		{
			var directoryFullPath = $"{_applicationOptions.JsonFileStoragePath}\\{request.FileType}";

			foreach (var fileFullPath in Directory.EnumerateFiles(directoryFullPath))
			{
				foreach (var readLine in File.ReadLines(fileFullPath))
				{
					await responseStream.WriteAsync(new GetObjectsByFileTypeResponse
					{
						Type = request.FileType,
						Data = readLine
					});
				}
			}
		}
	}
}