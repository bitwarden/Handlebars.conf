namespace Handlebars.conf.Backends;

/// <summary>
/// Interface for backend implementations that load data into template models.
/// </summary>
internal interface IBackend
{
    /// <summary>
    /// Loads data from the backend into the Handlebars model.
    /// </summary>
    /// <param name="model">The Handlebars model dictionary to populate with data.</param>
    /// <param name="config">The application configuration.</param>
    /// <param name="template">The template configuration containing backend-specific settings.</param>
    void LoadBackend(IDictionary<string, object> model, Config config, Config.Template template);
}
