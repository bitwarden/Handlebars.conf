using System.Collections;

namespace Handlebars.conf.Backends;

internal class EnvironmentBackend : IBackend
{
    public void LoadBackend(Dictionary<string, object> model, Config config, Config.Template template)
    {
        if (template.Keys is null) return;

        var envTable = new Hashtable();
        foreach (DictionaryEntry e in Environment.GetEnvironmentVariables())
        {
            envTable.Add(e.Key.ToString()!.ToLowerInvariant(), e.Value);
        }
        foreach (var key in template.Keys)
        {
            if (key is null) continue;
            var lowerKey = key.ToLowerInvariant();
            if (envTable.Contains(lowerKey) && envTable[lowerKey] is { } value)
            {
                model[lowerKey] = value;
            }
        }
    }
}
