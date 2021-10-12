using MonsterConverterInterface;
using MonsterConverterInterface.MonsterTypes;
using OTLib.Collections;
using OTLib.OTB;
using OTLib.Server.Items;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MonsterConverterProcessor
{
    public class MonsterFileProcessor
    {
        // Events
        public event EventHandler<FileProcessorEventArgs> OnMonsterConverted;

        // Functions
        public ProcessorScanError ConvertMonsterFiles(string monsterDirectory, IMonsterConverter inputConverter, string outputDirectory, IMonsterConverter outputConverter, string otbmPath = null, ItemConversionMethod itemConversionMethod = ItemConversionMethod.KeepSourceIds, bool mirroredFolderStructure = false)
        {
            if ((inputConverter.FileSource == FileSource.LocalFiles) && (!Directory.Exists(monsterDirectory)))
            {
                return ProcessorScanError.InvalidInputDirectory;
            }

            if ((inputConverter.FileSource == FileSource.LocalFiles) &&
                (Path.GetFullPath(monsterDirectory) == Path.GetFullPath(outputDirectory)))
            {
                return ProcessorScanError.DirectoriesMatch;
            }

            if (!Directory.Exists(outputDirectory))
            {
                try
                {
                    Directory.CreateDirectory(outputDirectory);
                }
                catch (Exception)
                {
                    return ProcessorScanError.CouldNotCreateDirectory;
                }
            }

            string[] files  = inputConverter.GetFilesForConversion(monsterDirectory);
            if (inputConverter.FileSource == FileSource.Web)
            {
                mirroredFolderStructure = false;
            }
            if ((files != null) && (files.Length == 0))
            {
                return ProcessorScanError.NoMonstersFound;
            }

            ServerItemList itemMapping = null;
            if ((itemConversionMethod != ItemConversionMethod.KeepSourceIds) && (otbmPath != null))
            {
                OtbReader otbReader = new OtbReader();
                if (!otbReader.Read(otbmPath))
                {
                    return ProcessorScanError.OtbReadFailed;
                }
                itemMapping = otbReader.Items;
            }

            string destination;
            for (int i = 0; i < files.Length; i++)
            {
                string file = files[i];
                destination = FindExactFileDestination(monsterDirectory, outputDirectory, file, mirroredFolderStructure);
                var result  = ProcessFile(file, inputConverter, outputConverter, destination, itemConversionMethod, itemMapping);
                RaiseEvent(OnMonsterConverted, new FileProcessorEventArgs(result.Item1, result.Item2, i, files.Length));
            }

            return ProcessorScanError.Success;
        }

        private string FindExactFileDestination(string inputDirectory, string outputDirectory, string file, bool mirroredFolderStructure)
        {
            if (mirroredFolderStructure)
            {
                string nameOnly = Path.GetFileName(file);
                string subPath = file.Replace(nameOnly, "");
                subPath = subPath.Replace(inputDirectory, "");
                subPath = subPath.Substring(1, subPath.Length - 1);
                return Path.Combine(outputDirectory, subPath);
            }
            else
            {
                return outputDirectory;
            }
        }

        private Tuple<ConvertResultEventArgs, ConvertResultEventArgs> ProcessFile(string file, IMonsterConverter input, IMonsterConverter output, string outputDir, ItemConversionMethod itemConversionMethod, ServerItemList itemMapping)
        {
            ConvertResultEventArgs readResult = new ConvertResultEventArgs(file, ConvertError.Error, "Unknown error occured");
            ConvertResultEventArgs writeResult = new ConvertResultEventArgs("unknown", ConvertError.Error, "Unknown error occured");

            // ReadMonster and WriteMonster methods processors should do their best to catch and return meaningful errors
            try
            {
                readResult = input.ReadMonster(file, out Monster monster);
                if (readResult.Code != ConvertError.Error)
                {
                    if (!Directory.Exists(outputDir))
                    {
                        Directory.CreateDirectory(outputDir);
                    }

                    IList<string> itemConversionErrors = new List<string>();
                    ConvertCorpsesId(input, itemConversionMethod, itemMapping, monster, itemConversionErrors);
                    ConvertLootIds(input, itemConversionMethod, itemMapping, monster, itemConversionErrors);
                    ConvertChangeOutfitIds(input, itemConversionMethod, itemMapping, monster, itemConversionErrors);

                    writeResult = output.WriteMonster(outputDir, ref monster);
                    if (itemConversionErrors.Count > 0)
                    {
                        writeResult.IncreaseError(ConvertError.Warning);
                    }
                    foreach (var msg in itemConversionErrors)
                    {
                        writeResult.AppendMessage(msg);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error pasring {file}. Exception {ex.Message}");
            }
            return new(readResult, writeResult);
        }

        private static string ConvertItemId(ref ushort id, string idDescription, IMonsterConverter input, ItemConversionMethod itemConversionMethod, ServerItemList itemMapping, Monster monster)
        {
            string result = null;
            if ((itemConversionMethod == ItemConversionMethod.UseClientIds) && (input.ItemIdType == ItemIdType.Server))
            {
                if (id > 0)
                {
                    var foundItems = itemMapping.FindByServerId(id);
                    if (foundItems.Count == 0)
                    {
                        result = $"For given server id {idDescription} can't find client id";
                    }
                    else if (foundItems.Count >= 2)
                    {
                        string itemList = string.Join(",", foundItems.Select(fi => fi.ClientId));
                        result = $"For given server id {idDescription} multiple client ids found {itemList}";
                    }
                    else
                    {
                        id = foundItems.First().ClientId;
                    }
                }
            }
            else if ((itemConversionMethod == ItemConversionMethod.UseServerIds) && (input.ItemIdType == ItemIdType.Client))
            {
                if (id > 0)
                {
                    var foundItems = itemMapping.FindByClientId(id);
                    if (foundItems.Count == 0)
                    {
                        result = $"For given client id {idDescription} can't find server id";
                    }
                    else if (foundItems.Count >= 2)
                    {
                        string itemList = string.Join(",", foundItems.Select(fi => fi.ID));
                        result = $"For given client id {idDescription} multiple server ids found {itemList}";
                    }
                    else
                    {
                        id = foundItems.First().ID;
                    }
                }
            }
            return result;
        }

        private static void ConvertCorpsesId(IMonsterConverter input, ItemConversionMethod itemConversionMethod, ServerItemList itemMapping, Monster monster, IList<string> itemConversionErrors)
        {
            ushort corpseId = monster.Look.CorpseId;
            string result = ConvertItemId(ref corpseId, $"{corpseId}:{monster.Name}", input, itemConversionMethod, itemMapping, monster);
            if (result == null)
            {
                monster.Look.CorpseId = corpseId;
            }
            else
            {
                itemConversionErrors.Add(result);
            }
        }

        private static void ConvertLootIds(IMonsterConverter input, ItemConversionMethod itemConversionMethod, ServerItemList itemMapping, Monster monster, IList<string> itemConversionErrors)
        {
            for (int i = 0; i < monster.Items.Count; i++)
            {
                LootItem item = monster.Items[i];
                ushort itemId = item.Id;
                string result = ConvertItemId(ref itemId, item.ComboIdentifier, input, itemConversionMethod, itemMapping, monster);
                if (result == null)
                {
                    item.Id = itemId;
                }
                else
                {
                    itemConversionErrors.Add(result);
                }
            }
        }

        private static void ConvertChangeOutfitIds(IMonsterConverter input, ItemConversionMethod itemConversionMethod, ServerItemList itemMapping, Monster monster, IList<string> itemConversionErrors)
        {
            for (int i = 0; i < monster.Attacks.Count; i++)
            {
                Spell spell = monster.Attacks[i];
                if (spell.ItemId != null && spell.ItemId > 0)
                {
                    ushort itemId = (ushort)spell.ItemId;
                    string result = ConvertItemId(ref itemId, $"{itemId}:{spell}", input, itemConversionMethod, itemMapping, monster);
                    if (result == null)
                    {
                        spell.ItemId = itemId;
                    }
                    else
                    {
                        itemConversionErrors.Add(result);
                    }
                }
            }
        }

        protected bool RaiseEvent<T>(EventHandler<T> eventHandler, T args) where T : EventArgs
        {
            bool result = false;

            // Make a temporary copy of the event to avoid possibility of
            // a race condition if the last subscriber unsubscribes
            // immediately after the null check and before the event is raised.
            var handler = eventHandler;

            // Event will be null if there are no subscribers
            if (handler != null)
            {
                // Use the () operator to raise the event.
                handler(this, args);
                result = true;
            }
            return result;
        }
    }
}
