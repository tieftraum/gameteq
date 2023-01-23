namespace gRPC.Client
{
	public class Watcher
	{
		private readonly FileSystemWatcher _watcher = new();
		public event FileSystemEventHandler? Created;
		public Watcher(string path, string filter)
		{
			_watcher.Path = path;
			_watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
			_watcher.Filter = filter;
			_watcher.Created += OnCreated;
		}
		public void Start() => _watcher.EnableRaisingEvents = true;
		public void Stop() => _watcher.EnableRaisingEvents = false;
		private void OnCreated(object source, FileSystemEventArgs e)
		{
			Created?.Invoke(this, e);
		}
	}
}
