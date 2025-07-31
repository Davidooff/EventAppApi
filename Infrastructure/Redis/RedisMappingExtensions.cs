using StackExchange.Redis;

namespace Infrastructure.Redis;

public static class RedisMappingExtensions
{
    public static HashEntry[] ToHashEntries(this object obj)
    {
        var properties = obj.GetType().GetProperties();
        var hashEntries = new List<HashEntry>();

        foreach (var property in properties)
        {
            // --- CRITICAL: Ignore collections and other complex types ---
            if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(ICollection<>))
                continue; // Skip collections
        
            var value = property.GetValue(obj);
            if (value != null)
            {
                // --- SOLUTION for Enums ---
                // If the property is an enum, store its integer value.
                if (property.PropertyType.IsEnum)
                {
                    hashEntries.Add(new HashEntry(property.Name, (int)value));
                }
                else
                {
                    hashEntries.Add(new HashEntry(property.Name, value.ToString()));
                }
            }
        }
        return hashEntries.ToArray();
    }

    public static T ConvertFromHash<T>(this HashEntry[] hashEntries) where T : new()
    {
        var obj = new T();
        var properties = typeof(T).GetProperties();
        var hashDict = hashEntries.ToDictionary(h => h.Name.ToString(), h => h.Value);

        foreach (var property in properties)
        {
            if (hashDict.TryGetValue(property.Name, out var value))
            {
                if (!value.HasValue) continue;

                // --- SOLUTION for Enums ---
                // If the target property is an enum, parse it from the integer value.
                if (property.PropertyType.IsEnum)
                {
                    property.SetValue(obj, Enum.ToObject(property.PropertyType, (int)value));
                }
                else
                {
                    // Use Convert.ChangeType for robust type conversion
                    Console.WriteLine($"working on {property.Name}");
                    var convertedValue = Convert.ChangeType(value, property.PropertyType);
                    property.SetValue(obj, convertedValue);
                }
            }
        }
        return obj;
    }
}