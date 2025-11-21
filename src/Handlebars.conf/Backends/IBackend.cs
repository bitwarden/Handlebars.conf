namespace Handlebars.conf.Backends;

internal interface IBackend
{
    void LoadBackend(IDictionary<string, object> model, Config config, Config.Template template);
}
