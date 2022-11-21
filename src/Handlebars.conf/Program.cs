using System.CommandLine;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;
using System.Collections;

namespace Handlebars.conf;

class Program
{
    static async Task<int> Main(string[] args)
    {
        var configFileOption = new Option<FileInfo>(
            name: "--config",
            description: "The config file listing templates and options for Handlebars to process.");

        var rootCommand = new RootCommand("Handlebars templates for config files.");
        rootCommand.AddOption(configFileOption);

        rootCommand.SetHandler(async (configFile) =>
        {
            var config = await ReadConfigFileAsync(configFile);
            foreach (var template in config.Templates)
            {
                var model = GetHandlebarsModel(config, template);
                var source = await File.ReadAllTextAsync(template.Source);
                var sourceTemplate = HandlebarsDotNet.Handlebars.Compile(source);
                var result = sourceTemplate(model);
                await File.WriteAllTextAsync(template.Destination, result);
            }
        }, configFileOption);

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
}
