using Amazon.DynamoDBv2;

namespace CatalogApi.Service.DynamoLogging;

public static class DynamoDbLoggerExtensions
{
    public static ILoggingBuilder AddDynamoDbLogger(this ILoggingBuilder builder, IAmazonDynamoDB client, string tableName, LogLevel minLevel = LogLevel.Warning)
    {
        builder.AddProvider(new DynamoDbLoggerProvider(client, tableName, minLevel));

        return builder;
    }
}