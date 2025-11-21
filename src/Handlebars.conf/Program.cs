using System.CommandLine;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;
using System.Collections;
using HandlebarsDotNet.Helpers.Enums;
using HandlebarsDotNet.Helpers;
using HandlebarsDotNet;
using Handlebars.conf.Backends;

namespace Handlebars.conf;

class Program
{
    static async Task<int> Main(string[] args)
    {
        // Set up command and parameters
        var configFileOption = new Option<FileInfo>(
            "--config",
            "-c")
        {
            Description = "The config file listing options and templates for Handlebars to process.",
            DefaultValueFactory = _ => File.Exists("/etc/hbs/config.yml") ?
                new FileInfo("/etc/hbs/config.yml") :
                new FileInfo("/etc/hbs/config.yaml")
        };

        var rootCommand = new RootCommand("Handlebars templates for config files.");
        rootCommand.Options.Add(configFileOption);

        rootCommand.SetAction(async (parseResult, cancellationToken) =>
        {
            var configFile = parseResult.GetValue(configFileOption);
            if (configFile == null)
            {
                return 1;
            }

            // Read config yaml file
            var config = await ReadConfigFileAsync(configFile);

            // Set up Handlebars
            var handlebarsContext = HandlebarsDotNet.Handlebars.Create();
            RegisterHandlebarsHelpers(handlebarsContext, config);

            // Process templates
            if (config.Templates != null)
            {
                foreach (var template in config.Templates)
                {
                    var model = GetHandlebarsModel(config, template);
                    var source = string.IsNullOrWhiteSpace(template.SourceText) ?
                        await File.ReadAllTextAsync(template.SourceFile!) : template.SourceText;
                    var sourceTemplate = handlebarsContext.Compile(source);
                    var result = sourceTemplate(model);
                    await File.WriteAllTextAsync(template.Destination!, result);
                }
            }

            return 0;
        });

        // Go
        var result = rootCommand.Parse(args);
        return await result.InvokeAsync();
    }

    static async Task<Config> ReadConfigFileAsync(FileInfo file)
    {
        var yaml = await File.ReadAllTextAsync(file.FullName);
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .Build();
        return deserializer.Deserialize<Config>(yaml);
    }

    static IDictionary<string, object> GetHandlebarsModel(Config config, Config.Template template)
    {
        var model = new Dictionary<string, object>();
        if (template.Backend.HasValue && template.Keys != null)
        {
            var backend = ResolveBackend(template.Backend.Value);
            backend?.LoadBackend(model, config, template);
        }
        if (config.LoadEnvironmentVariables)
        {
            AddEnvironmentVariablesToModel(model);
        }
        return model;
    }

    static void AddEnvironmentVariablesToModel(IDictionary<string, object> model)
    {
        // Need to lowercase all hash table key names due to bug here:
        // https://github.com/Handlebars-Net/Handlebars.Net/issues/521
        var envTable = new Hashtable();
        foreach (DictionaryEntry e in Environment.GetEnvironmentVariables())
        {
            envTable.Add(e.Key.ToString()!.ToLowerInvariant(), e.Value);
        }
        model["env"] = envTable;
    }

    static void RegisterHandlebarsHelpers(IHandlebars context, Config config)
    {
        if (config.HelperCategories != null && config.HelperCategories.Length > 0)
        {
            var categories = config.HelperCategories
                .Select(c => (Category)Enum.Parse(typeof(Category), c)).ToArray();
            HandlebarsHelpers.Register(context, categories);
        }
    }

    static IBackend? ResolveBackend(BackendType backend)
    {
        return backend switch
        {
            BackendType.Environment => new EnvironmentBackend(),
            _ => null,
        };
    }
}
