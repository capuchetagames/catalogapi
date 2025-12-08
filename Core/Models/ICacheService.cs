namespace Core.Models;

public interface ICacheService
{
    object? Get(string key);
    void Set(string key, object value);
    void Remove(string key);
}