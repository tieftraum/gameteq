﻿using Microsoft.Extensions.Configuration;

namespace gRPC.Client
{
	public class Settings
	{
		public int BatchSize { get; set; }
		public string FilePath { get; set; } = string.Empty;
		public string ExtensionFilter { get; set; } = string.Empty;

		public static Settings Get()
		{
			var builder = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json", false, true)
				.Build();

			return builder.GetSection(nameof(Settings)).Get<Settings>()!;
		}
	}
}