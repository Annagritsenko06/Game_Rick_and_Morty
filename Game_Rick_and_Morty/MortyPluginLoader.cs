using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace RickAndMortyGame.GameCore
{
    internal static class MortyPluginLoader
    {
        public static void TryLoadMortyAssemblies()
        {
            try
            {
                string baseDir = AppContext.BaseDirectory;
                string solutionRoot = Path.GetFullPath(Path.Combine(baseDir, "..", "..", "..", ".."));

                var candidateDirs = new List<string>();
                string[] configs = new[] { "Debug", "Release" };
                foreach (var cfg in configs)
                {
                    candidateDirs.Add(Path.Combine(solutionRoot, "ClassicMorty", "bin", cfg, "net8.0"));
                    candidateDirs.Add(Path.Combine(solutionRoot, "LazyMorty", "bin", cfg, "net8.0"));
                }

                foreach (var dir in candidateDirs.Distinct())
                {
                    if (!Directory.Exists(dir))
                    {
                        continue;
                    }

                    foreach (var dll in Directory.EnumerateFiles(dir, "*.dll", SearchOption.TopDirectoryOnly))
                    {
                        string fileName = Path.GetFileName(dll);
                        if (!fileName.StartsWith("ClassicMorty", StringComparison.OrdinalIgnoreCase) &&
                            !fileName.StartsWith("LazyMorty", StringComparison.OrdinalIgnoreCase))
                        {
                            continue;
                        }

                        try
                        {
                            if (AppDomain.CurrentDomain.GetAssemblies().Any(a => SafeLocationEquals(a, dll)))
                            {
                                continue;
                            }
                            Assembly.LoadFrom(dll);
                        }
                        catch
                        {
                        }
                    }
                }
            }
            catch
            {
            }
        }

        private static bool SafeLocationEquals(Assembly assembly, string path)
        {
            try
            {
                return string.Equals(assembly.Location, path, StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        }
    }
}

