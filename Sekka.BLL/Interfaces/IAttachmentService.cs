namespace Sekka.BLL.Interfaces
{
    public interface IAttachmentService
    {

        Task<string?> UploadAttachmentAsync(Stream fileStream, string fileName, string folderName, CancellationToken ct = default);
        bool DeleteAttachment(string fileName, string folderName);
        (Stream stream, string ContentType)? GetAttachment(string fileName, string folderName);
    }
}
