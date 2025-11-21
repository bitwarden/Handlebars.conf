using YamlDotNet.Serialization;

namespace Handlebars.conf;

/// <summary>
/// Configuration model for Handlebars.conf application.
/// </summary>
internal class Config
{
    /// <summary>
    /// Gets or sets the list of templates to process.
    /// </summary>
    public List<Template>? Templates { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to load environment variables into the model.
    /// Defaults to true.
    /// </summary>
    public bool LoadEnvironmentVariables { get; set; } = true;

    /// <summary>
    /// Gets or sets the Handlebars helper categories to load.
    /// </summary>
    public string[]? HelperCategories { get; set; }

    /// <summary>
    /// Configuration for an individual template.
    /// </summary>
    internal class Template
    {
        /// <summary>
        /// Gets or sets the source template file path.
        /// </summary>
        [YamlMember(Alias = "src")]
        public string? SourceFile { get; set; }

        /// <summary>
        /// Gets or sets the inline source template text.
        /// </summary>
        [YamlMember(Alias = "src_text")]
        public string? SourceText { get; set; }

        /// <summary>
        /// Gets or sets the destination file path for the processed template.
        /// </summary>
        [YamlMember(Alias = "dest")]
        public string? Destination { get; set; }

        /// <summary>
        /// Gets or sets the list of keys to load from the backend.
        /// </summary>
        public string[]? Keys { get; set; }

        /// <summary>
        /// Gets or sets the backend type to use for loading template data.
        /// </summary>
        public BackendType? Backend { get; set; }
    }
}
