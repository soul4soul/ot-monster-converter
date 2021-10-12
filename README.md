# README

Tibia OT Monster Converter is a tool for converting monster files between the various formats used by open tibia servers.

## Technology

- .NET 5
- WPF
- Visual Studio 2019

## Supported Formats

| Format | Input % Complete | Output % Complete | Notes |
| - | - | - | - |
| TFS XML                                              | [95%](https://github.com/soul4soul/ot-monster-converter/wiki/TFS-XML-Input-Status)           | [95%](https://github.com/soul4soul/ot-monster-converter/wiki/TFS-XML-Output-Status)            | - Most common OT Monster format which has been around for over a decade  |
| [PyOT](https://bitbucket.org/vapus/pyot/)            | [0%](https://github.com/soul4soul/ot-monster-converter/wiki/PyOT-Input-Status)               | [80%](https://github.com/soul4soul/ot-monster-converter/wiki/PyOT-Output-Status)               | - This format can be consider dead as PyOT development has ceased. Unless development is picked back up support for this format is unlikely to be completed. |
| TFS revscriptsys                                     | [0%](https://github.com/soul4soul/ot-monster-converter/wiki/TFS-revscriptsys-Input-Status)   | [95%](https://github.com/soul4soul/ot-monster-converter/wiki/TFS-revscriptsys-Output-Status)   | - Very new OT monster format that was theorized many years ago. In the future there is a good chance it will replace TFS XML completely. This is likely the output type that most users of this program will use. <br/> - Opentibiabr RevScriptSys format is not completely compatible with TFS RevScriptSys format |
| [TibiaWiki](https://tibia.fandom.com/wiki/Main_Page) | [85%](https://github.com/soul4soul/ot-monster-converter/wiki/TibiaWiki-Input-Status)         | [90%](https://github.com/soul4soul/ot-monster-converter/wiki/TibiaWiki-Output-Status)          | - Helpful for keeping monsters up to date with cipbia<br/> - See The [Infobox Creature Template](https://tibia.fandom.com/wiki/Template:Infobox_Creature) for information about TibiaWiki Format <br/> - Monsters created from TibiaWiki will require corpse id, looktype, and spells to be created manually |
| Cip Mon                                              | [95%](https://github.com/soul4soul/ot-monster-converter/wiki/Cip-Mon-Input-Status)           | [0%](https://github.com/soul4soul/ot-monster-converter/wiki/Cip-Mon-Output-Status)             | - Format is for input purposes to easily generte monsters as they were in the 7.7 days |

## Graphical User Interface

The executable named `OTMonsterConverter.exe` is the GUI program. The GUI program works on Windows.

![Sample GUI Image](https://user-images.githubusercontent.com/5142635/120939976-c2551400-c6e8-11eb-8e53-10ad7f68ea5e.png)

## Command Line Interface

The executable named `otmc.exe` is the CLI program. The CLI program works on Windows and Linux.

```ps
PS C:\Users\...\Release> .\otmc.exe
OT Monster Converter:
Developed By Soul4Soul
Repository located at https://github.com/soul4soul/ot-monster-converter

Usage: otmc [OPTIONS]+

Options:
  -i, --inputDirectory=VALUE The directory of monster files to parse.
  -o, --outputDirectory=VALUE
                             The output directory of the new monster files.
      --inputFormat=VALUE    The starting input monster file format.
      --outputFormat=VALUE   The desired monster file format.
      --otbPath=VALUE        The path to an otb file.
      --itemIdFormat=VALUE   Desired item id types used for the converted
                               monser loot.
  -m, --MirrorFolders        Mirror the folder structure of the input directory,
                                otherwise flat folder structure is output
  -h, --help                 show this message and exit

Input Formats: TibiaWiki, TFS XML, Cip Mon
Output Formats: TFS RevScriptSys, TibiaWiki, TFS XML, pyOT
Item Id Formats: KeepSourceIds, UseServerIds, UseClientIds
```

## Contributing

Improvements and bug fixes are welcome, via pull requests
For questions, suggestions and bug reports, submit an issue.

Another way to contribute to this project is by contributing to [TibiaWiki](https://tibia.fandom.com) to improve creature information.

![image](https://vignette.wikia.nocookie.net/tibia/images/d/d9/Tibiawiki_Small.gif/revision/latest?cb=20150129101832&path-prefix=en)
