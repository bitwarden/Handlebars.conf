namespace Handlebars.conf.Backends;

internal interface IBackend
{
    void LoadBackend(Dictionary<string, object> model, Config config, Config.Template template);
}
