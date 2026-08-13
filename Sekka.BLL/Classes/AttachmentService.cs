using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Sekka.BLL.Interfaces;

namespace Sekka.BLL.Classes
{
    public class AttachmentService : IAttachmentService
    {
        private readonly ILogger<AttachmentService> _logger;
        private readonly IWebHostEnvironment _environment;

        private readonly long _MaxFileSize = 5 * 1024 * 1024;

        private readonly string[] _AllowedExtension =
        {
            ".png",
            ".jpeg",
            ".jpg"
        };

        public AttachmentService(ILogger<AttachmentService> logger, IWebHostEnvironment environment)
        {
            _logger = logger;
            _environment = environment;
        }

        public bool DeleteAttachment(string fileName, string folderName)
        {
            var filePath = Path.Combine(
                _environment.WebRootPath,
                folderName,
                fileName);

            try
            {
                if (!File.Exists(filePath))
                    return false;

                File.Delete(filePath);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed To Delete File");

                return false;
            }
        }

        public (Stream stream, string ContentType)? GetAttachment(string fileName, string folderName)
        {
            if (string.IsNullOrWhiteSpace(fileName) || string.IsNullOrWhiteSpace(folderName))
                return null;


            var fullpath = Path.Combine(_environment.WebRootPath, folderName, fileName);

            if (!File.Exists(fullpath))
                return null;

            var stream = new FileStream(fullpath, FileMode.Open, FileAccess.Read);
            var extension = Path.GetExtension(fullpath).ToLowerInvariant();

            var contenttype = extension switch
            {
                ".png" => "image/png",
                ".jpg" or ".jpeg" => "image/jpeg",
                _ => "application/octet-stream"
            };

            return (stream, contenttype);
        }

        public async Task<string?> UploadAttachmentAsync(Stream fileStream, string fileName, string folderName, CancellationToken ct = default)
        {
            if (fileStream == null || !fileStream.CanRead)
                return null;

            // Check extension
            var extension = Path.GetExtension(fileName).ToLowerInvariant();

            if (!_AllowedExtension.Contains(extension))
            {
                _logger.LogError("File Rejected: This Extension Not Allowed");

                return null;
            }

            // Check size
            if (fileStream.Length > _MaxFileSize)
            {
                _logger.LogError("File Rejected: Too Large {Size} Bytes", fileStream.Length);
                return null;
            }

            // wwwroot/FolderName
            var uploadsfolder = Path.Combine(_environment.WebRootPath, folderName);

            Directory.CreateDirectory(uploadsfolder);

            // Generate unique file name
            var storedFileName = $"{Guid.NewGuid()}_{Path.GetFileName(fileName)}";

            var filePath = Path.Combine(uploadsfolder, storedFileName);

            try
            {
                await using var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);

                await fileStream.CopyToAsync(fs, ct);

                return storedFileName;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed To Upload Photo");
                return null;
            }
        }
    }
}