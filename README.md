# README

Tibia OT Monster Converter is a tool for converting monster files between the various formats used by open tibia servers.

## Technology
- .NET Core 3.1
- WPF
- Visual Studio 2019

## Status

Parsing and writing of nested loot is not yet supported by any formats. This is a low priority item as cipbia removed nested loot and its no longer commonly found in OT servers.

### Supported Input Formats
- [X] TFS XML
- [ ] TFS Rev Script Sys
- [ ] PyOT
- [ ] Long term goal [TibiaWiki](https://tibia.fandom.com/wiki/Main_Page)


### Supported Output Formats
- [ ] TFS XML
- [X] TFS Rev Script Sys
- [ ] PyOT as output (partical support)

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
