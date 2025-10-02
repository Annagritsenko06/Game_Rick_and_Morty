using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace RickAndMortyGame.GameCore
{
    internal static class MortyPluginLoader
    {
        public static void TryLoadMortyAssemblies(string mortyName)
        {
            try
            {
                string dllPath = mortyName switch
                {
                    "ClassicMorty" => @"D:\5 СЕМ\стажировка\task 4\Game_Rick_and_Morty\ClassicMorty\bin\Debug\net8.0\ClassicMorty.dll",
                    "LazyMorty" => @"D:\5 СЕМ\стажировка\task 4\Game_Rick_and_Morty\LazyMorty\bin\Debug\net8.0\LazyMorty.dll",
                    _ => string.Empty
                };

                if (string.IsNullOrWhiteSpace(dllPath) || !File.Exists(dllPath))
                {
                    Console.WriteLine($"Assembly for {mortyName} was not found.");
                    return;
                }

                if (AppDomain.CurrentDomain.GetAssemblies()
                    .Any(a => SafeLocationEquals(a, dllPath)))
                {
                    Console.WriteLine($"Assembly {mortyName} is already loaded.");
                    return;
                }

                Assembly.LoadFrom(dllPath);
                Console.WriteLine($"Morty assembly loaded: {dllPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in TryLoadMortyAssemblies: {ex.Message}");
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

