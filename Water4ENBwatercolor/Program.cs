using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;
using System.Drawing;
using System.Reflection;
using System.Linq;

namespace Water4ENBwatercolor;

public class Program
{
    // Wasser-Farbwerte für "Natural Shades of Skyrim"
    private static readonly Color SunriseColor = Color.FromArgb(137, 160, 171);
    private static readonly Color DayColor = Color.FromArgb(175, 216, 237);
    private static readonly Color SunsetColor = Color.FromArgb(105, 142, 154);
    private static readonly Color NightColor = Color.FromArgb(31, 63, 75);

    public static async Task<int> Main(string[] args)
    {
        return await SynthesisPipeline.Instance
            .AddPatch<ISkyrimMod, ISkyrimModGetter>(RunPatch)
            .SetTypicalOpen(GameRelease.SkyrimSE, "WaterColorPatcher.esp")
            .Run(args);
    }

    public static void RunPatch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
    {
        int patchedCount = 0;

        // Iteriere durch alle Wetter-Records in der Load Order
        foreach (var weatherGetter in state.LoadOrder.PriorityOrder.Weather().WinningOverrides())
        {
            // Erstelle einen Override im Patch-Mod
            var weather = state.PatchMod.Weathers.GetOrAddAsOverride(weatherGetter);

            // In .NET 6 / Mutagen nutzen wir System.Drawing.Color oder 
            // setzen die Komponenten direkt, falls Mutagen eigene Typen nutzt.
            // Hier setzen wir die WaterMultiplier-Werte:


            // Ausgabe der eigenschaften und methoden von weather Objekt hier
            /*
            var type = weather.WaterMultiplierColor.GetType();
            Console.WriteLine($"=== Weather-Type: {type.FullName} ===");

            var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                            .OrderBy(p => p.Name);
            Console.WriteLine("Properties:");
            foreach (var p in props)
            {
                object? value = null;
                try
                {
                    value = p.GetValue(weather);
                }
                catch
                {
                    // manche Getter können Exceptions werfen; dann nur den Namen ausgeben
                }
                Console.WriteLine($"  {p.PropertyType.Name} {p.Name} = {value}");
            }

            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                              .Where(m => !m.IsSpecialName) // get_/set_ usw. ausblenden
                              .OrderBy(m => m.Name);
            Console.WriteLine("Methods:");
            foreach (var m in methods)
            {
                var parameters = string.Join(", ",
                    m.GetParameters().Select(pm => $"{pm.ParameterType.Name} {pm.Name}"));
                Console.WriteLine($"  {m.ReturnType.Name} {m.Name}({parameters})");
            }
            */

            weather.WaterMultiplierColor.Sunrise = SunriseColor;
            weather.WaterMultiplierColor.Day = DayColor;
            weather.WaterMultiplierColor.Sunset = SunsetColor;
            weather.WaterMultiplierColor.Night = NightColor;
            /*
            weather.WaterMultiplierSunrise = SunriseColor;
            weather.WaterMultiplierDay = DayColor;
            weather.WaterMultiplierSunset = SunsetColor;
            weather.WaterMultiplierNight = NightColor;
            */
            patchedCount++;
        }

        Console.WriteLine($"Erfolgreich {patchedCount} Wetter-Einträge mit 'Water for ENB'-Farben angepasst.");
    }
}