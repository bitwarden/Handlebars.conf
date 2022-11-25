using YamlDotNet.Serialization;

namespace Handlebars.conf;

internal class Config
{
    public List<Template> Templates { get; set; }
    public bool LoadEnvironmentVariables { get; set; } = true;
    public string[] HelperCategories { get; set; }

    internal class Template
    {
        [YamlMember(Alias = "src")]
        public string SourceFile { get; set; }
        [YamlMember(Alias = "src_text")]
        public string SourceText { get; set; }
        [YamlMember(Alias = "dest")]
        public string Destination { get; set; }
        public string[] Keys { get; set; }
        public BackendType? Backend { get; set; }
    }
}
