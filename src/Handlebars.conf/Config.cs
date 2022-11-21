namespace Handlebars.conf;

internal class Config
{
    public List<Template> Templates { get; set; }
    public bool LoadEnvironmentVariables { get; set; } = true;

    internal class Template
    {
        public string Source { get; set; }
        public string Destination { get; set; }
    }
}
