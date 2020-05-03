# README

Tibia OT Monster Converter is a tool for converting monster files between the various formats used by open tibia servers.

## Technology
- .NET Core 3.1
- WPF
- Visual Studio 2019

## Status

Parsing and writing of nested loot is not yet supported by any formats. This is a low priority item as cipbia removed nested loot and its no longer commonly found in OT servers.

### Supported Formats

| Format                                               | Input % Complete | Output % Complete | Notes                                                                                                                                                            |
| ---------------------------------------------------- | ---------------- | ----------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| TFS XML                                              | [95%](https://github.com/soul4soul/ot-monster-converter/wiki/TFS-XML-Input-Status)              | [0%](https://github.com/soul4soul/ot-monster-converter/wiki/TFS-XML-Output-Status)                | - Most common OT Monster format which has been around for over a decade  |
| [PyOT](https://bitbucket.org/vapus/pyot/)            | [0%](https://github.com/soul4soul/ot-monster-converter/wiki/PyOT-Input-Status)               | [70%](https://github.com/soul4soul/ot-monster-converter/wiki/PyOT-Output-Status)               | - This format can be consider dead as PyOT development has ceased. Unless development is picked back up support for this format is unlikely to be completed.                                                                                                                                                      |
| TFS revscriptsys                                     | [0%](https://github.com/soul4soul/ot-monster-converter/wiki/TFS-revscriptsys-Input-Status)               | [90%](https://github.com/soul4soul/ot-monster-converter/wiki/TFS-revscriptsys-Output-Status)               | - Very new OT monster format that was theorized many years ago. In the future there is a good chance it will replace TFS XML completely. This output type will likely be the mosted used.                |
| [TibiaWiki](https://tibia.fandom.com/wiki/Main_Page) | 0%               | 0%                | - Helpful for keeping monsters up to date with cipbia<br/> - See The [Infobox Creature Template](https://tibia.fandom.com/wiki/Template:Infobox_Creature) for information about TibiaWiki Format <br/> - Monsters created from TibiaWiki will require corpse id, looktype, and spells to be created manually |


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


## Contributing
Improvements and bug fixes are welcome, via pull requests  
For questions, suggestions and bug reports, submit an issue.

Another to contribute to this project is by contributing to [TibiaWiki](https://tibia.fandom.com) to improve creature information.

![image](https://vignette.wikia.nocookie.net/tibia/images/d/d9/Tibiawiki_Small.gif/revision/latest?cb=20150129101832&path-prefix=en)
