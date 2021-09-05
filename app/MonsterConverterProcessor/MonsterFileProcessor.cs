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
        public ProcessorScanError ConvertMonsterFiles(string monsterDirectory, IMonsterConverter inputConverter, string outputDirectory, IMonsterConverter outputConverter, string otbmPath = null, ItemConversionMethod itemConversionMethod = ItemConversionMethod.KeepSouceIds, bool mirroredFolderStructure = false)
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
            if ((itemConversionMethod != ItemConversionMethod.KeepSouceIds) && (otbmPath != null))
            {
                OtbReader otbReader = new OtbReader();
                if (!otbReader.Read(otbmPath))
                {
                    return ProcessorScanError.OtbmReadFailed;
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

                    IList<string> lootConversionErrors = new List<string>();
                    if ((itemConversionMethod == ItemConversionMethod.UseClientIds) && (input.ItemIdType == ItemIdType.Server))
                    {
                        for (int i = 0; i < monster.Items.Count; i++)
                        {
                            LootItem item = monster.Items[i];
                            if (item.Id > 0)
                            {
                                var foundItems = itemMapping.FindByServerId(item.Id);
                                if (foundItems.Count == 0)
                                {
                                    lootConversionErrors.Add($"For given server id {item.ComboIdentifier} can't find client id");
                                }
                                else if (foundItems.Count >= 2)
                                {
                                    string itemList = string.Join(",", foundItems.Select(fi => fi.ClientId));
                                    lootConversionErrors.Add($"For given server id {item.ComboIdentifier} multiple client ids found {itemList}");
                                }
                                else
                                {
                                    item.Id = foundItems.First().ClientId;
                                }
                            }
                        }
                    }
                    else if ((itemConversionMethod == ItemConversionMethod.UseServerIds) && (input.ItemIdType == ItemIdType.Client))
                    {
                        for (int i = 0; i < monster.Items.Count; i++)
                        {
                            LootItem item = monster.Items[i];
                            if (item.Id > 0)
                            {
                                var foundItems = itemMapping.FindByClientId(item.Id);
                                if (foundItems.Count == 0)
                                {
                                    lootConversionErrors.Add($"For given client id {item.ComboIdentifier} can't find server id");
                                }
                                else if (foundItems.Count >= 2)
                                {
                                    string itemList = string.Join(",", foundItems.Select(fi => fi.ID));
                                    lootConversionErrors.Add($"For given client id {item.ComboIdentifier} multiple server ids found {itemList}");
                                }
                                else
                                {
                                    item.Id = foundItems.First().ID;
                                }
                            }
                        }
                    }

                    writeResult = output.WriteMonster(outputDir, ref monster);
                    if (lootConversionErrors.Count > 0)
                    {
                        writeResult.IncreaseError(ConvertError.Warning);
                    }
                    foreach (var msg in lootConversionErrors)
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
