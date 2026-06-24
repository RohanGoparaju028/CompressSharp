using Amazon;
using Amazon.S3;
using Amazon.Runtime;
using dotenv.net;
using Amazon.SecurityToken;
using Amazon.SecurityToken.Model;
namespace CompressSharp.Auth;

public class Authenticate
{
    public async Task<bool> Verify()
    {
        try
        {
            DotEnv.Load(options: new DotEnvOptions(envFilePaths: new[] { ".env" }));
            var envVar = DotEnv.Read();
            var accessKeyId = envVar["ACCESS_KEY_ID"];
            var secret_acess_key_id = envVar["SECRET_ACCESS_KEY"];
            var credential = new BasicAWSCredentials(accessKeyId, secret_acess_key_id);
            var region = envVar["REGION"];
            var regionEndpoint = RegionEndpoint.GetBySystemName(region);
            using var stdClient = new AmazonSecurityTokenServiceClient(credential, regionEndpoint);
            var _ = await stdClient.GetCallerIdentityAsync(new GetCallerIdentityRequest());
            return true;
        }
        catch (AmazonSecurityTokenServiceException ex)
        {
            Console.WriteLine($"{ex.Message}");
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"{ex.Message}");
            return false;
        }
    }
}
