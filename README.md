# Handlebars.conf

Handlebars templates for config files.

## Examples

### Basic usage

Handlebars config file: `hb.yaml`

```
templates:
  - source: test.conf.hbs
    destination: test.conf
```

Source Handlebars template: `test.conf.hbs`

```
<Section>
    {{env.username}}
</Section>
```

Run command

```
hbs --config hb.yaml
```

Destination output: `test.conf`

```
<Section>
    kyle
</Section>
```
