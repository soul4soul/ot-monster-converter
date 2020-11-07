using Microsoft.VisualStudio.Composition;
using MonsterConverterInterface;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Reflection;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OTMonsterConverter
{
    class PluginHelper
    {
        private static PluginHelper instance = null;
        private static readonly object lockobject = new object();

        public IList<IMonsterConverter> Converters { get; private set; }

        private PluginHelper()
        {
        }

        public static PluginHelper Instance
        {
            get
            {
                lock (lockobject)
                {
                    if (instance == null)
                    {
                        instance = new PluginHelper();
                        instance.FindConverters();
                    }
                    return instance;
                }
            }
        }

        private async void FindConverters()
        {
            // Build up a catalog of MEF parts
            var catalog = await CreateProductCatalogAsync();

            // Assemble the parts into a valid graph.
            var config = CompositionConfiguration.Create(catalog);

            // Prepare an ExportProvider factory based on this graph.
            var epf = config.CreateExportProviderFactory();

            // Create an export provider, which represents a unique container of values.
            // You can create as many of these as you want, but typically an app needs just one.
            var exportProvider = epf.CreateExportProvider();

            // Obtain converts exported from the assemblies in the catalog
            Converters = exportProvider.GetExportedValues<IMonsterConverter>().ToList();
        }

        /// <summary>
        /// The MEF discovery module to use (which finds both MEFv1 and MEFv2 parts).
        /// </summary>
        private readonly PartDiscovery discoverer = PartDiscovery.Combine(
            new AttributedPartDiscovery(Resolver.DefaultInstance, isNonPublicSupported: true),
            new AttributedPartDiscoveryV1(Resolver.DefaultInstance));

        /// <summary>
        /// Gets the names of assemblies that belong to the application .exe folder.
        /// </summary>
        /// <returns>A list of assembly names.</returns>
        private static IEnumerable<string> GetAssemblyNames()
        {
            string directoryToSearch = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
            foreach (string file in Directory.EnumerateFiles(directoryToSearch, "*.dll"))
            {
                string assemblyFullPath = null;
                try
                {
                    var assemblyName = AssemblyName.GetAssemblyName(file);
                    if (assemblyName != null)
                    {
                        // This is stupid and defeats the purpose of dynamically loading converters
                        if ((assemblyName.Name == "MonsterConverterPyOt") ||
                            (assemblyName.Name == "MonsterConverterTfsRevScriptSys") ||
                            (assemblyName.Name == "MonsterConverterTfsXml") ||
                            (assemblyName.Name == "MonsterConverterTibiaWiki"))
                        {
                            assemblyFullPath = Path.Combine(directoryToSearch, $"{assemblyName.Name}.dll");
                        }
                    }
                }
                catch (Exception)
                {
                }

                if (assemblyFullPath != null)
                {
                    yield return assemblyFullPath;
                }
            }
        }

        /// <summary>
        /// Creates a catalog with all the assemblies from the application .exe's directory.
        /// </summary>
        /// <returns>A task whose result is the <see cref="ComposableCatalog"/>.</returns>
        private async Task<ComposableCatalog> CreateProductCatalogAsync()
        {
            var assemblyNames = GetAssemblyNames();
            var assemblies = assemblyNames.Select(Assembly.LoadFrom);
            var discoveredParts = await this.discoverer.CreatePartsAsync(assemblies);
            var catalog = ComposableCatalog.Create(Resolver.DefaultInstance)
                .AddParts(discoveredParts);
            return catalog;
        }
    }
}
