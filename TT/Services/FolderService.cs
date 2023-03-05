using System;
using Microsoft.EntityFrameworkCore;
using TT.Models;
using TT.Models.Context;


namespace TT.Services
{
	public class FolderService
	{
		private readonly ApplicationDbContext _context;

		public FolderService(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<List<Folder>> GetFoldersAsync(string path)
		{
			ConfigurePath(ref path);
			var folders = await _context.Folders
				.Where(x => x.Path.Equals(path))
				.ToListAsync();
			return folders;
		}

		public async Task AddFoldersAsync(List<Folder> folders)
		{
			await _context.AddRangeAsync(folders);
			await _context.SaveChangesAsync();
		}

		public async Task<List<Folder>> ImportFoldersAsync(IFormFile file, string path)
		{
			var lines = new List<string>();
			var importFolders = new List<Folder>();
			var existingFolders = await _context.Folders.ToListAsync();

			using (var stream = new StreamReader(file.OpenReadStream()))
			{
				while (stream.Peek() >= 0)
				{
					lines.Add(await stream.ReadLineAsync());
				}
			}

			foreach (var line in lines)
			{
				var splitedLine = line.Split(',', StringSplitOptions.RemoveEmptyEntries);
				var folderName = splitedLine[0];
				var folderPath = splitedLine[1];

				if (folderPath.Length > 1)
				{
					folderPath = folderPath.Substring(1, folderPath.Length - 1);
				}
				else
				{
					folderPath = string.Empty;
				}

				folderPath = path + folderPath;

				if (existingFolders.FirstOrDefault(x => x.Name == folderName && x.Path == folderPath) is null)
				{
					importFolders.Add(new Folder
					{
						Name = folderName,
						Path = folderPath
					});
				}
			}
			return importFolders;
		}

		public async Task ExportFoldersAsync(string currentFoldersPath)
		{
			var folders = await _context.Folders.Where(x => x.Path.StartsWith(currentFoldersPath)).ToListAsync();
			var defaultFileName = "exportedCatalogs.txt";
            var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), defaultFileName);
			using (var stream = new StreamWriter(filePath))
			{
				foreach (var folder in folders)
				{
					await stream.WriteLineAsync($"{folder.Name},{folder.Path}");
				}
			}
		}

		private void ConfigurePath(ref string path)
		{
			if (path.Equals("/"))
			{
				return;
			}
			path = $"/{path}";
		}
	}
}

