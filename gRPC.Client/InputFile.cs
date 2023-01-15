namespace gRPC.Client
{
	public class InputFile
	{
		public string Type { get; set; }
		public string WholeLine { get; set; }
		public InputFile(string type, string wholeLine)
		{
			Type = type;
			WholeLine = wholeLine;
		}
	}
}
