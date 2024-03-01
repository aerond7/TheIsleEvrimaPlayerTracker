using System.Reflection;

namespace TheIsleEvrimaPlayerTracker.Configuration
{
    public static class DefaultConfigWriter
    {
        public static bool Write(string fileName, Type configType)
        {
            if (File.Exists(fileName))
            {
                return false;
            }

            var writer = new StreamWriter(fileName);

            var groupedOptions = configType.GetProperties()
                                           .Select(p => p.GetCustomAttribute<Config.Net.OptionAttribute>())
                                           .Where(o => o != null)
                                           .GroupBy(o => o!.Alias!.Split('.')[0])
                                           .ToList();

            foreach (var options in groupedOptions)
            {
                writer.WriteLine($"[{options.Key}]");

                foreach (var option in options)
                {
                    writer.WriteLine($"{option!.Alias!.Split('.')[1]}={option.DefaultValue}");
                }

                writer.Write("\n");
            }

            writer.Flush();
            writer.Close();

            return true;
        }
    }
}
