# Handlebars.conf

Handlebars templates for config files.

## Install

Download the latest release binary for your system from the [Releases page](https://github.com/bitwarden/Handlebars.conf/releases). Note that there are different binaries for various architectures on Linux, macOS, and Windows.

Linux x64 example:

```sh
curl -L -o hbs.zip https://github.com/bitwarden/Handlebars.conf/releases/download/v2024.2.0/hbs_linux-x64.zip
sudo unzip hbs.zip -d /usr/local/bin && rm hbs.zip
sudo chmod +x /usr/local/bin/hbs

hbs --help
```

NOTE: Release binaries with the `_dotnet` suffix are smaller in size, but require the .NET Core runtime to be installed on the host machine.

## Examples

### Basic Usage

Handlebars config file: `hbs.yaml`

```yaml
templates:
  - src: test.conf.hbs
    dest: test.conf
```

Source Handlebars template: `test.conf.hbs`.

Note: Environment variables are available under the `env` property.

```hbs
<Section>
    {{env.username}}
</Section>
```

Run command

```bash
hbs -c hbs.yaml
```

Destination output: `test.conf`

```
<Section>
    kyle
</Section>
```

### Multiple Templates

```yaml
templates:
  - src: foo.conf.hbs
    dest: foo.conf
  - src: bar.conf.hbs
    dest: bar.conf
  - src: baz.conf.hbs
    dest: baz.conf
```

## Handlebars Templating

Learn more about using Handlebars templates here: https://handlebarsjs.com

## Handlebars Helpers

You can load Handlebars helpers from the [Handlebars.Net Helpers library](https://github.com/Handlebars-Net/Handlebars.Net.Helpers) by specifying helper categories to load in your config file.

Config

```yaml
helper_categories:
  - String
  - Math
templates:
  - src: test.conf.hbs
    dest: test.conf
```

Template `test.conf.hbs`

```hbs
{{Math.Add 4 5}}

{{#if (String.IsNotNullOrWhitespace env.username)}}
{{String.Append env.username " is awesome."}}
{{/if}}

{{String.Coalesce "" "     " "value" " " "value2"}}

{{#each (String.Split "1,2, 3" ",")}}
Number: {{String Trim .}}
{{/each}}

{{#if (String.Equal env.username "tom")}}
user is tom.
{{else}}
user is {{String.Uppercase env.username}}.
{{/if}}
```

Result `test.conf`

```
9

kyle is awesome.

value

Number: 1
Number: 2
Number: 3

user is KYLE.
```

### Real World Example

You can see this tool in use with Bitwarden Unified's NGINX config here: https://github.com/bitwarden/self-host/tree/main/docker-unified/hbs
