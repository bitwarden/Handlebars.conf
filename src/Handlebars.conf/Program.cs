using System.CommandLine;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;
using System.Collections;
using HandlebarsDotNet.Helpers.Enums;
using HandlebarsDotNet.Helpers;
using HandlebarsDotNet;
using Handlebars.conf.Backends;
using Microsoft.Extensions.Logging;

namespace Handlebars.conf;

class Program
{
    static async Task<int> Main(string[] args)
    {
        // Set up logging
        using var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder
                .AddConsole()
                .SetMinimumLevel(LogLevel.Information);
        });
        var logger = loggerFactory.CreateLogger<Program>();

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
            try
            {
                var configFile = parseResult.GetValue(configFileOption);
                if (configFile == null)
                {
                    logger.LogError("Config file path is required.");
                    Console.Error.WriteLine("Error: Config file path is required.");
                    return 1;
                }

                logger.LogInformation("Reading config file: {ConfigFile}", configFile.FullName);

                // Read config yaml file
                Config config;
                try
                {
                    config = await ReadConfigFileAsync(configFile, cancellationToken);
                }
                catch (FileNotFoundException)
                {
                    logger.LogError("Config file not found: {ConfigFile}", configFile.FullName);
                    Console.Error.WriteLine($"Error: Config file not found: {configFile.FullName}");
                    return 1;
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error reading config file");
                    Console.Error.WriteLine($"Error reading config file: {ex.Message}");
                    return 1;
                }

                // Validate configuration
                if (config.Templates == null || config.Templates.Count == 0)
                {
                    logger.LogError("No templates defined in config file");
                    Console.Error.WriteLine("Error: No templates defined in config file.");
                    return 1;
                }

                foreach (var template in config.Templates)
                {
                    if (string.IsNullOrWhiteSpace(template.SourceFile) && string.IsNullOrWhiteSpace(template.SourceText))
                    {
                        logger.LogError("Template must have either 'src' or 'src_text' defined");
                        Console.Error.WriteLine("Error: Template must have either 'src' (source file) or 'src_text' (inline text) defined.");
                        return 1;
                    }

                    if (string.IsNullOrWhiteSpace(template.Destination))
                    {
                        logger.LogError("Template must have 'dest' defined");
                        Console.Error.WriteLine("Error: Template must have 'dest' (destination) defined.");
                        return 1;
                    }
                }

                logger.LogInformation("Processing {TemplateCount} template(s)", config.Templates.Count);

                // Set up Handlebars
                var handlebarsContext = HandlebarsDotNet.Handlebars.Create();
                RegisterHandlebarsHelpers(handlebarsContext, config);

                // Process templates
                if (config.Templates != null)
                {
                    foreach (var template in config.Templates)
                    {
                        try
                        {
                            var destination = template.Destination ?? "unknown";
                            logger.LogDebug("Processing template: {Destination}", destination);

                            var model = GetHandlebarsModel(config, template);
                            var source = string.IsNullOrWhiteSpace(template.SourceText) ?
                                await File.ReadAllTextAsync(template.SourceFile!, cancellationToken) : template.SourceText;
                            var sourceTemplate = handlebarsContext.Compile(source);
                            var result = sourceTemplate(model);
                            await File.WriteAllTextAsync(template.Destination!, result, cancellationToken);

                            logger.LogInformation("Successfully processed template: {Destination}", destination);
                        }
                        catch (FileNotFoundException ex)
                        {
                            logger.LogError("Template file not found: {FileName}", ex.FileName);
                            Console.Error.WriteLine($"Error: Template file not found: {ex.FileName}");
                            return 1;
                        }
                        catch (Exception ex)
                        {
                            logger.LogError(ex, "Error processing template");
                            Console.Error.WriteLine($"Error processing template: {ex.Message}");
                            return 1;
                        }
                    }
                }

                logger.LogInformation("All templates processed successfully");
                return 0;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error");
                Console.Error.WriteLine($"Unexpected error: {ex.Message}");
                return 1;
            }
        });

        // Go
        var result = rootCommand.Parse(args);
        return await result.InvokeAsync();
    }

    static async Task<Config> ReadConfigFileAsync(FileInfo file, CancellationToken cancellationToken = default)
    {
        var yaml = await File.ReadAllTextAsync(file.FullName, cancellationToken);
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
        model["env"] = EnvironmentHelper.GetLowercaseEnvironmentVariables();
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
