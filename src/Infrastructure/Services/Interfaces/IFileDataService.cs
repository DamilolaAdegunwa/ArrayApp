using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArrayApp.Application.Common.Models;
using ArrayApp.Domain.Entities.FileAggregate;

namespace ArrayApp.Infrastructure.Services.Interfaces;
public interface IFileDataService
{
    Task<FileDataDto> UploadFileAsync(FileDataUploadDto fileUploadDto);
    Task<FileDataDto> GetFileByIdAsync(int fileId);
    Task DeleteFileAsync(int fileId);
}
