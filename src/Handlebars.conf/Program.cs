using System.CommandLine;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;
using System.Collections;
using HandlebarsDotNet.Helpers.Enums;
using HandlebarsDotNet.Helpers;
using HandlebarsDotNet;

namespace Handlebars.conf;

class Program
{
    static async Task<int> Main(string[] args)
    {
        // Set up command and parameters
        var configFileOption = new Option<FileInfo>(
            name: "--config",
            () => File.Exists("/etc/hbs/config.yml") ? new FileInfo("/etc/hbs/config.yml") :
                new FileInfo("/etc/hbs/config.yaml"),
            description: "The config file listing options and templates for Handlebars to process.");
        configFileOption.AddAlias("-c");

        var rootCommand = new RootCommand("Handlebars templates for config files.");
        rootCommand.AddOption(configFileOption);

        rootCommand.SetHandler(async (configFile) =>
        {
            // Read config yaml file
            var config = await ReadConfigFileAsync(configFile);

            // Set up Handlebars
            var handlebarsContext = HandlebarsDotNet.Handlebars.Create();
            RegisterHandlebarsHelpers(handlebarsContext, config);

            // Process templates
            foreach (var template in config.Templates)
            {
                var model = GetHandlebarsModel(config, template);
                var source = string.IsNullOrWhiteSpace(template.SourceText) ?
                    await File.ReadAllTextAsync(template.SourceFile) : template.SourceText;
                var sourceTemplate = handlebarsContext.Compile(source);
                var result = sourceTemplate(model);
                await File.WriteAllTextAsync(template.Destination, result);
            }
        }, configFileOption);

        // Go
        return await rootCommand.InvokeAsync(args);
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
        // TODO: Load model from various config sources
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
        var table = new Hashtable();
        foreach (DictionaryEntry e in Environment.GetEnvironmentVariables())
        {
            table.Add(e.Key.ToString().ToLowerInvariant(), e.Value);
        }
        model["env"] = table;
    }

    static void RegisterHandlebarsHelpers(IHandlebars context, Config config)
    {
        if (config.HelperCategories != null && config.HelperCategories.Length > 0)
        {
            var categories = config.HelperCategories
                .Select(c => (Category)Enum.Parse(typeof(Category), c)).ToArray();
            HandlebarsHelpers.Register(context, categories);

            if (categories.Contains(Category.String))
            {
                // Overload String equality helpers since they do not allow null values
                context.RegisterHelper("String.Equal", (context, arguments) =>
                {
                    var value1 = arguments[0] as string;
                    var value2 = arguments[1] as string;
                    return value1 == value2;
                });

                context.RegisterHelper("String.NotEqual", (context, arguments) =>
                {
                    var value1 = arguments[0] as string;
                    var value2 = arguments[1] as string;
                    return value1 != value2;
                });

                // Add new helpers

                context.RegisterHelper("String.Coalesce", (context, arguments) =>
                {
                    foreach (var arg in arguments)
                    {
                        if (arg != null)
                        {
                            if (arg is string s)
                            {
                                if (!string.IsNullOrWhiteSpace(s))
                                {
                                    return arg;
                                }
                            }
                            else
                            {
                                return arg;
                            }
                        }
                    }
                    return null;
                });
            }
        }
    }
}
