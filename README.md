# Handlebars.conf

Handlebars templates for config files.

## Install

Download the latest release binary for your system from the [Releases page](https://github.com/kspearrin/Handlebars.conf/releases).

## Examples

### Basic usage

Handlebars config file: `hbs.yaml`

```
templates:
  - src: test.conf.hbs
    dest: test.conf
```

Source Handlebars template: `test.conf.hbs`

```
<Section>
    {{env.username}}
</Section>
```

Run command

```
hbs --config hbs.yaml
```

Destination output: `test.conf`

```
<Section>
    kyle
</Section>
```
