using System.Collections;

namespace Handlebars.conf.Backends;

internal class Environment : IBackend
{
    public void LoadBackend(Dictionary<string, object> model, Config config, Config.Template template)
    {
        var envTable = new Hashtable();
        foreach (DictionaryEntry e in System.Environment.GetEnvironmentVariables())
        {
            envTable.Add(e.Key.ToString().ToLowerInvariant(), e.Value);
        }
        foreach (var key in template.Keys)
        {
            var lowerKey = key?.ToLowerInvariant();
            if (envTable.Contains(lowerKey))
            {
                model[lowerKey] = envTable[lowerKey];
            }
        }
    }
}
