using System.Collections;

namespace Handlebars.conf.Backends;

internal class EnvironmentBackend : IBackend
{
    public void LoadBackend(Dictionary<string, object> model, Config config, Config.Template template)
    {
        if (template.Keys == null)
        {
            return;
        }

        var envTable = EnvironmentHelper.GetLowercaseEnvironmentVariables();
        foreach (var key in template.Keys)
        {
            var lowerKey = key?.ToLowerInvariant();
            if (lowerKey != null && envTable.Contains(lowerKey))
            {
                model[lowerKey] = envTable[lowerKey]!;
            }
        }
    }
}
