using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArrayApp.Application.Common.Models;
using ArrayApp.Infrastructure.Services.Interfaces;

namespace ArrayApp.Infrastructure.Services;
public class FileDataService : IFileDataService
{
    public Task DeleteFileAsync(int fileId)
    {
        throw new NotImplementedException();
    }

    public Task<FileDataDto> GetFileByIdAsync(int fileId)
    {
        throw new NotImplementedException();
    }

    public Task<FileDataDto> UploadFileAsync(FileDataUploadDto fileUploadDto)
    {
        throw new NotImplementedException();
    }
}
