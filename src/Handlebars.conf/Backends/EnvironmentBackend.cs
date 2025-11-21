using System.Collections;

namespace Handlebars.conf.Backends;

/// <summary>
/// Backend implementation that loads environment variables into the template model.
/// </summary>
internal class EnvironmentBackend : IBackend
{
    /// <summary>
    /// Loads specified environment variables into the Handlebars model.
    /// </summary>
    /// <param name="model">The Handlebars model dictionary to populate with data.</param>
    /// <param name="config">The application configuration.</param>
    /// <param name="template">The template configuration containing the list of environment variable keys to load.</param>
    public void LoadBackend(IDictionary<string, object> model, Config config, Config.Template template)
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
