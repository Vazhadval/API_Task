using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NaturalPersonAPI.Repository
{
    public class FileProcessingService : IFileProcessingService
    {
        private readonly IWebHostEnvironment _env;

        public FileProcessingService(IWebHostEnvironment env)
        {
            _env = env;
        }
        public async Task<string> UploadFileAsync(IFormFile file)
        {
            string uploadsFolder = $"{_env.WebRootPath}/uploads";

            string fileName = Path.GetFileNameWithoutExtension(file.FileName) + Guid.NewGuid();
            string extension = Path.GetExtension(file.FileName);

            string filePath = Path.Combine(uploadsFolder, fileName + extension);

            string relativePath = $"/uploads/{fileName}{extension}";

            using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                try
                {
                    await file.CopyToAsync(fileStream);
                }
                catch (Exception)
                {

                    return string.Empty;
                }
            }


            return relativePath;
        }
    }
}
