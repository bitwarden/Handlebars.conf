using System.CommandLine;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;

namespace Handlebars.conf;

class Program
{
    static async Task<int> Main(string[] args)
    {
        var configFileOption = new Option<FileInfo>(
            name: "--config",
            description: "The file to read and display on the console.");

        var rootCommand = new RootCommand("Handlebars templates for config files.");
        rootCommand.AddOption(configFileOption);

        rootCommand.SetHandler(async (configFile) =>
        {
            var configArray = await ReadConfigFileAsync(configFile);
            foreach (var config in configArray)
            {
                var source = await File.ReadAllTextAsync(config.Source);
                var sourceTemplate = HandlebarsDotNet.Handlebars.Compile(source);
                var result = sourceTemplate(new Dictionary<string, object>
                {
                    { "env", Environment.GetEnvironmentVariables() }
                });
                await File.WriteAllTextAsync(config.Destination, result);
            }
        }, configFileOption);

        return await rootCommand.InvokeAsync(args);
    }

    static async Task<Config[]> ReadConfigFileAsync(FileInfo file)
    {
        var yaml = await File.ReadAllTextAsync(file.FullName);
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .Build();
        return deserializer.Deserialize<Config[]>(yaml);
    }
}
