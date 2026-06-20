using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace HostUtility;

public static class BanWords
{
    private static List<string> Swears_EN;

    public static void Initialize()
    {
        Swears_EN = LoadFilter("en.txt");
    }

    private static List<string> LoadFilter(string fileName)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // Find the exact resource name - see step 3 if unsure
        var resourceName = assembly.GetManifestResourceNames()
            .First(n => n.EndsWith(fileName));

        using var stream = assembly.GetManifestResourceStream(resourceName);
        using var reader = new StreamReader(stream!);

        return reader.ReadToEnd().Split("\n", StringSplitOptions.RemoveEmptyEntries).ToList();
    }

    private static List<string> GetSwearsList()
    {
        return Swears_EN;
    }

    public static bool ContainsSwear(string input)
    {
        foreach (var swear in GetSwearsList())
        {
            if (input.Contains($" {swear} ", StringComparison.OrdinalIgnoreCase) || input.StartsWith($"{swear} ", StringComparison.OrdinalIgnoreCase) || input.EndsWith($" {swear}", StringComparison.OrdinalIgnoreCase) || input == swear)
            {
                return true;
            }
        }
        return false;
    }
}