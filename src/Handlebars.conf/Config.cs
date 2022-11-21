using YamlDotNet.Serialization;

namespace Handlebars.conf;

internal class Config
{
    public List<Template> Templates { get; set; }
    public bool LoadEnvironmentVariables { get; set; } = true;

    internal class Template
    {
        [YamlMember(Alias = "src")]
        public string Source { get; set; }
        [YamlMember(Alias = "dest")]
        public string Destination { get; set; }
    }
}
