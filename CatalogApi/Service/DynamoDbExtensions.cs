using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;

namespace CatalogApi.Service;

public static class DynamoDbExtensions
{
    public static IServiceCollection AddDynamoDb(this IServiceCollection services, IConfiguration configuration)
    {
        var useLocal       = configuration.GetValue<bool>("DynamoDb:UseLocal");
        var localUrl = configuration["DynamoDb:LocalUrl"];
        var profile  = configuration["DynamoDb:ProfileName"];
        var region   = configuration["DynamoDb:Region"];
        //var tableName      = configuration["DynamoDb:TableName"];
        
        if (useLocal)
        {
            services.AddSingleton<IAmazonDynamoDB>(new AmazonDynamoDBClient(new BasicAWSCredentials("fake", "fake"),
                new AmazonDynamoDBConfig { ServiceURL = localUrl }));
        }

        if (!string.IsNullOrWhiteSpace(profile))
        {
            var credentials = ResolveProfileCredentials(profile);
    
            services.AddSingleton<IAmazonDynamoDB>(new AmazonDynamoDBClient(credentials, RegionEndpoint.GetBySystemName(region)));    
        }

        
        services.AddSingleton<IDynamoDBContext>(provider =>
        {
            var client = provider.GetRequiredService<IAmazonDynamoDB>();
        
            return new DynamoDBContext(client);
        });

         // services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Information));
        
        return services;
    }

    private static AWSCredentials ResolveProfileCredentials(string? profileName)
    {
        var chain = new CredentialProfileStoreChain();
        if (chain.TryGetAWSCredentials(profileName, out var credentials))
        {
            return credentials;
        }

        throw new InvalidOperationException(
            $"Profile '{profileName}' não encontrado em ~/.aws/credentials ou ~/.aws/config.\n" +
            $"Execute: aws configure --profile {profileName}");
    }
}