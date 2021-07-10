using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NaturalPersonAPI.Repository
{
    public interface IFileProcessingService
    {
        Task<string> UploadFileAsync(IFormFile file);
    }
}
