using System;
using System.Threading.Tasks;
using ArrayApp.Application.Common.Models;
using ArrayApp.Domain.Entities.FileAggregate;
using ArrayApp.Infrastructure.Repositories.Interfaces;
using ArrayApp.Infrastructure.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace ArrayApp.Infrastructure.Services;

public class FileDataService : IFileDataService
{
    private readonly ILogger<FileDataService> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public FileDataService(ILogger<FileDataService> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<FileDataDto> UploadFileAsync(FileDataUploadDto fileUploadDto)
    {
        _logger.LogInformation("Uploading file: {Name}", fileUploadDto.Name);
        var file = new FileData
        {
            Name = fileUploadDto.Name,
            Size = fileUploadDto.Size,
            MimeType = fileUploadDto.MimeType,
            Path = fileUploadDto.Path,
            Extension = fileUploadDto.Extension,
            CreatedAt = DateTimeOffset.UtcNow,
            ModifiedAt = DateTimeOffset.UtcNow
        };

        var saved = await _unitOfWork.FileDataBaseRepository.AddAsync(file);
        return MapToDto(saved);
    }

    public async Task<FileDataDto> GetFileByIdAsync(int fileId)
    {
        var file = await _unitOfWork.FileDataBaseRepository.GetByIdAsync(fileId);
        return file != null ? MapToDto(file) : new FileDataDto();
    }

    public async Task DeleteFileAsync(int fileId)
    {
        var file = await _unitOfWork.FileDataBaseRepository.GetByIdAsync(fileId);
        if (file != null)
        {
            await _unitOfWork.FileDataBaseRepository.DeleteAsync(file);
        }
    }

    private static FileDataDto MapToDto(FileData f) => new FileDataDto
    {
        Id = f.Id,
        Name = f.Name,
        Size = f.Size,
        MimeType = f.MimeType,
        Path = f.Path,
        Extension = f.Extension,
        CreatedAt = f.CreatedAt
    };
}
