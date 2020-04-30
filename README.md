# README

Tibia OT Monster Converter is a tool for converting monster files between the various formats used by open tibia servers.

## Technology
- .NET Core 3.1
- WPF
- Visual Studio 2019

## Status

Parsing and writing of nested loot is not yet supported by any formats. This is a low priority item as cipbia removed nested loot and its no longer commonly found in OT servers.

### Supported Input Formats

| Format                                               | Percent Complete | Notes                                                                                                                                                                                       |
|------------------------------------------------------|------------------|---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| TFS XML                                              | 95%              |  - Missing Support for variable haste<br/> - Missing support for converting attacks and defenses which are in scripts                                                                       |
| [PyOT](https://bitbucket.org/vapus/pyot/) | 0%               |                                                                                                                                                                                             |
| TFS revscriptsys                                     | 0%               |                                                                                                                                                                                             |
| [TibiaWiki](https://tibia.fandom.com/wiki/Main_Page) | 0%               |  - Helpful for keeping monsters up to date with cipbia<br/> - Monsters created from TibiaWiki will require corpse id, looktype, and spells to be created manually |

### Supported Output Formats

| Format           | Percent Complete | Notes                                                                                                                 |
|------------------|------------------|-----------------------------------------------------------------------------------------------------------------------|
| TFS XML          | 0%               |                                                                                                                       |
| [PyOT](https://bitbucket.org/vapus/pyot/) | 50%              | - Missing attacks and defenses                                                                                        |
| TFS revscriptsys | 90%              |  - Missing support for attacks and defenses with are in scripts<br/> - Attacks which cause conditions are not working |
| [TibiaWiki](https://tibia.fandom.com/wiki/Main_Page) | 0%               |                                                                                                                       |

## Graphical Interface
![Alt text](https://user-images.githubusercontent.com/5142635/80318493-86a70580-87d8-11ea-85dc-cfc4e3fe2754.png)

## Command Line
It should be possible to run this application on linux by ripping on the UI and recompiling.

```
PS C:\...\bin\Release\netcoreapp3.1> .\OTMonsterConverter.exe -help
Usage: OTMonsterConverter [OPTIONS]+

Options:
  -i, --inputDirectory=VALUE The directory of monster files to parse.
  -o, --outputDirectory=VALUE
                             The directory of monster files to parse.
      --inputFormat=VALUE    The starting monster file format.
      --outputFormat=VALUE   The format to converter the monster files to.
  -m, --MirrorFolders        True to mirror the folder structure of the input
                               directory
  -h, --help                 show this message and exit
```
