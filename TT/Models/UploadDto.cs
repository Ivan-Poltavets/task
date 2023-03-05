using System;
namespace TT.Models
{
	public class UploadDto
	{
		public IFormFile FormFile { get; set; }
		public string Path { get; set; }
	}
}

