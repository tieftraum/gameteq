using Grpc.Core;

namespace gRPC.Server.Services
{
	public class FileService : FileWriter.FileWriterBase
	{
		public override async Task<WriteFileResponse> WriteFileInputInformation(IAsyncStreamReader<WriteFileRequest> requestStream, ServerCallContext context)
		{
			while (await requestStream.MoveNext() && !context.CancellationToken.IsCancellationRequested)
			{
				var fileInput = requestStream.Current;
				Console.WriteLine("DATA\n");
				Console.WriteLine(fileInput.Type);
				Console.WriteLine(fileInput.Data);
			}

			return new WriteFileResponse { Success = true };
		}
	}
}