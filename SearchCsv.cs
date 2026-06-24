using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using System.IO;
using System.IO.Compression;
using System.Net;
using dotenv.net;

namespace CompressSharp.Search;

public class SearchCsv
{
    private readonly AmazonS3Client _s3;
    public SearchCsv(BasicAWSCredentials credentials, RegionEndpoint region)
    {
        this._s3 = new AmazonS3Client(credentials, region);
    }
    public async Task FindCsv(string bucketName, string filename)
    {
        try
        {
            var request = new GetObjectMetadataRequest
            {
                BucketName = bucketName,
                Key = filename
            };

            await _s3.GetObjectMetadataAsync(request);
            await CompressAndDownload(bucketName, filename);
            Console.WriteLine("Process complete.");
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            Console.WriteLine($"Error: File '{filename}' not found.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    internal async Task CompressAndDownload(string bucketName, string filename)
    {
        var downloadPath = Path.Combine(Directory.GetCurrentDirectory(), filename);
        string compressedPath = downloadPath + ".gz";

        var request = new GetObjectRequest
        {
            BucketName = bucketName,
            Key = filename
        };

        using (var response = await _s3.GetObjectAsync(request))
        using (var compressedFileStream = File.Create(compressedPath))
        using (var compressionStream = new GZipStream(compressedFileStream, CompressionMode.Compress))
        {
            await response.ResponseStream.CopyToAsync(compressionStream);
        }
    }
}
