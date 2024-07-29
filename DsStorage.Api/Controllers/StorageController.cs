using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Supabase.Storage;
using Supabase.Storage.Interfaces;

namespace DsStorage.Api;

[Route("[controller]")]
[ApiController]
public class StorageController(IStorageClient<Bucket, FileObject> client, IOptions<StorageOptions> options) : ControllerBase
{
    readonly StorageOptions options = options.Value;
    readonly IStorageClient<Bucket, FileObject> client = client;

    [HttpPost("upload")]
    public async Task<ActionResult<string>> UploadFileToDefaultBucket(IFormFile file, CancellationToken ct) =>
        Ok(await UploadToStorage(file, ct: ct));

    [HttpPost("upload/{bucketName}/keep-file-name")]
    public async Task<ActionResult<string>> UploadFileToBucket(string bucketName, IFormFile file, string? internalDirPath = null, CancellationToken ct = default) =>
        Ok(await UploadToStorage(file, bucketName, true, internalDirPath, ct));

    [HttpDelete("remove/{bucketName}/{fileName}")]
    public async Task<ActionResult<string>> Remove(string bucketName, string fileName)
    {
        if (client.GetBucket(bucketName) == null) return Problem();

        return Ok(await client.From(bucketName).Remove(fileName));
    }

    [HttpDelete("remove")]
    public async Task<ActionResult<string>> RemoveAllGuids()
    {
        var allBuckets = (await client.ListBuckets())?.Select(x => x.Name);
        if (allBuckets == null) return Ok();

        foreach (var b in allBuckets)
            if (Guid.TryParse(b, out var _))
            {
                var files = await client.From(b).List();
                if (files == null) continue;
                foreach (var f in files)
                    if (f.Name != null)
                        await client.From(b).Remove(f.Name);

                await client.DeleteBucket(b);
            }
        
        return Ok();
    }

    async Task<string> UploadToStorage(IFormFile file, string? bucketName = null, bool keepFileName = false, string? internalDirPath = null, CancellationToken ct = default)
    {
        bucketName ??= options.DefaultBucket;
        if (await client.GetBucket(bucketName) == null)
            await client.CreateBucket(bucketName, new() { Public = true });

        var bucket = client.From(bucketName);
        var fileName = keepFileName ? file.FileName : $"{NewId.NextGuid()}{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        if (internalDirPath != null)
            fileName = Path.Combine(internalDirPath, fileName);

        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream, ct);
        byte[] fileBytes = memoryStream.ToArray();

        await bucket.Upload(fileBytes, fileName, new() { Upsert = true } );
        return fileName;
    }
}
