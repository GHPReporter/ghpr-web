﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static GhprWeb.EmbeddedResources.Resources;

namespace GhprWeb.EmbeddedResources
{
    public class ResourceExtractor
    {
        public string DestinationPath { get; private set; }
        public bool ReplaceExisting { get; private set; }
        public Resource[] CurrentResources { get; private set; }

        public ResourceExtractor(string destPath = "", bool replaceExisting = false, Resource[] resources = null)
        {
            DestinationPath = destPath;
            ReplaceExisting = replaceExisting;
            CurrentResources = resources;
        }

        public void Extract(Resource resource, string destinationPath = "", bool replaceExisting = false)
        {
            if (destinationPath.Equals(""))
            {
                destinationPath = DestinationPath;
            }

            ExtractResources(GetNames(resource), destinationPath, replaceExisting);

        }

        public void Extract(Resource[] resources, string destinationPath = "", bool replaceExisting = false)
        {
            if (destinationPath.Equals(""))
            {
                destinationPath = DestinationPath;
            }
            foreach (var resource in resources)
            {
                ExtractResources(GetNames(resource), destinationPath, replaceExisting);
            }
        }

        private void ExtractResource(string embeddedFileName, string destinationPath, bool replaceExisting)
        {
            var currentAssembly = GetType().Assembly;
            var arrResources = GetType().Assembly.GetManifestResourceNames();
            Directory.CreateDirectory(destinationPath);
            var destinationFullPath = Path.Combine(destinationPath, embeddedFileName);

            if (File.Exists(destinationFullPath) && !replaceExisting) return;

            foreach (var resourceName in arrResources.Where(resourceName => resourceName.ToUpper().EndsWith(embeddedFileName.ToUpper())))
            {
                using (var resourceToSave = currentAssembly.GetManifestResourceStream(resourceName))
                {
                    using (var output = File.Create(destinationFullPath))
                    {
                        resourceToSave?.CopyTo(output);
                    }
                    resourceToSave?.Close();
                }
            }
        }

        private void ExtractResources(IEnumerable<string> embeddedFileNames, string destinationPath, bool replaceExisting)
        {
            foreach (var embeddedFileName in embeddedFileNames)
            {
                ExtractResource(embeddedFileName, destinationPath, replaceExisting);
            }
        }

        public static List<string> GetNames(Resource resource)
        {
            switch (resource)
            {
                case Resource.JQuery:
                    return new List<string> { "jquery-1.11.0.min.js"};

                case Resource.Octicons:
                    return new List<string>
                        {
                            "octicons.css",
                            "octicons.eot",
                            "octicons.svg",
                            "octicons.ttf",
                            "octicons.woff"
                        };
                case Resource.Primer:
                    return new List<string> { "primer.css" };
                case Resource.SvgJs:
                    return new List<string> { "svg.min.js" };
                case Resource.Tablesort:
                    return new List<string> { "tablesort.min.js" };
                default:
                    throw new ArgumentOutOfRangeException(nameof(resource), resource, null);
            }
        }

        public List<string> GetResoucrePaths(Resource resource)
        {
            return GetNames(resource).Select(name => Path.Combine(DestinationPath, name)).ToList();
        }

        public List<string> GetResoucresPaths(Resource[] resources)
        {
            var paths = new List<string>();
            foreach (var resource in resources)
            {
                paths.AddRange(GetResoucrePaths(resource));
            }
            return paths;
        }
    }
}
