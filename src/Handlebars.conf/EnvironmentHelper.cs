using System.Collections;

namespace Handlebars.conf;

internal static class EnvironmentHelper
{
    public static Hashtable GetLowercaseEnvironmentVariables()
    {
        // Need to lowercase all hash table key names due to bug here:
        // https://github.com/Handlebars-Net/Handlebars.Net/issues/521
        var envTable = new Hashtable();
        foreach (DictionaryEntry e in Environment.GetEnvironmentVariables())
        {
            envTable.Add(e.Key.ToString()!.ToLowerInvariant(), e.Value);
        }
        return envTable;
    }
}
