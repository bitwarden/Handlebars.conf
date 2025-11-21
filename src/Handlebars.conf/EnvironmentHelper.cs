using System.Collections;

namespace Handlebars.conf;

/// <summary>
/// Helper class for working with environment variables.
/// </summary>
internal static class EnvironmentHelper
{
    /// <summary>
    /// Gets all environment variables with their keys converted to lowercase.
    /// </summary>
    /// <returns>A hashtable containing environment variables with lowercase keys.</returns>
    /// <remarks>
    /// Keys are lowercased due to a bug in Handlebars.Net: https://github.com/Handlebars-Net/Handlebars.Net/issues/521
    /// </remarks>
    public static Hashtable GetLowercaseEnvironmentVariables()
    {
        var envTable = new Hashtable();
        foreach (DictionaryEntry e in Environment.GetEnvironmentVariables())
        {
            envTable.Add(e.Key.ToString()!.ToLowerInvariant(), e.Value);
        }
        return envTable;
    }
}
