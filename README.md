# Handlebars.conf

Handlebars templates for config files.

## Install

Download the latest release binary for your system from the [Releases page](https://github.com/kspearrin/Handlebars.conf/releases).

```sh
curl -L -o hbs.zip https://github.com/kspearrin/Handlebars.conf/releases/download/v1.0.0/hbs_linux-x64.zip
unzip hbs.zip -d /usr/local/bin && rm hbs.zip
chmod +x /usr/local/bin/hbs

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

Source Handlebars template: `test.conf.hbs`

```hbs
<Section>
    {{env.username}}
</Section>
```

Run command

```bash
hbs --config hbs.yaml
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
<Section>
    {{String.Append env.username " is awesome."}}
    {{String.Lowercase "SOMETHING"}}
    {{Math.Add 4 5}}
</Section>
```

Result `test.conf`

```
<Section>
    kyle is awesome.
    something
    9
</Section>
```
