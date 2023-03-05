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

		public List<Folder> ImportFolders(IFormFile file, string path)
		{
			var lines = new List<string>();
			var folders = new List<Folder>();
			var separator = ',';
			using (var stream = new StreamReader(file.OpenReadStream()))
			{
				while (stream.Peek() >= 0)
				{
					lines.Add(stream.ReadLine());
				}
			}

			int separatorIndex;
			foreach (var line in lines)
			{
				separatorIndex = line.IndexOf(separator);
				var folderPath = line.Substring(separatorIndex + 1, line.Length - (separatorIndex + 1));
				if (folderPath.Length > 1)
				{
					folderPath = folderPath.Substring(1, folderPath.Length - 1);
				}
				else
				{
					folderPath = string.Empty;
				}
				
				folders.Add(new Folder
				{
					Name = line.Substring(0, separatorIndex),
					Path = path + folderPath
				});
			}
			return folders;
		}

		public async Task ExportFolders(string currentFoldersPath)
		{
			var folders = await _context.Folders.Where(x => x.Path.StartsWith(currentFoldersPath)).ToListAsync();
			var defaultFileName = "exportedCatalogs.txt";
            var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), defaultFileName);
			using (var stream = new StreamWriter(filePath))
			{
				foreach (var folder in folders)
				{
					stream.WriteLine($"{folder.Name},{folder.Path}");
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

