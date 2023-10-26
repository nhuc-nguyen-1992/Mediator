using System.ComponentModel;

namespace Nick.Mediator.Common.Extensions;

public static class TypeExtensions
{
    /// <summary>
    /// Determines whether given type is nullable
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>True if the given type is nullable, otherwise false</returns>
    public static bool IsNullableType(this Type type)
    {
        return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
    }

}

public static class StringExtensions
{
    public static string ToCamelCase(this string str)
    {
        var words = str.Split(new[] { "_", " " }, StringSplitOptions.RemoveEmptyEntries);

        words = words
            .Select(word => char.ToUpper(word[0]) + word.Substring(1))
            .ToArray();

        return string.Join(string.Empty, words);
    }
}

public static class TConverter
{
    public static T ChangeType<T>(object value)
    {
        return (T)ChangeType(typeof(T), value);
    }

    public static object ChangeType(Type t, object value)
    {
        var tc = TypeDescriptor.GetConverter(t);
        return tc.ConvertFrom(value);
    }
}