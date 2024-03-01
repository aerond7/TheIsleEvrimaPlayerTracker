using Config.Net;
using System.Reflection;
using TheIsleEvrimaPlayerTracker.Configuration;
using TheIsleEvrimaPlayerTracker.Constants;
using TheIsleEvrimaPlayerTracker.Core;
using TheIsleEvrimaPlayerTracker.Exceptions;

namespace TheIsleEvrimaPlayerTracker
{
    public class Program
    {
        static EvrimaTracker? Tracker { get; set; }

        static async Task Main(string[] args)
        {
            Console.Title = "The Isle EVRIMA Player Tracker";

            try
            {
                SetWorkingDirectory();
                ConfigureConsoleCancellation();
                var config = LoadConfiguration(TrackerConstants.IniConfigFileName, args);

                Tracker = new EvrimaTracker(config);
                await Tracker.StartTracking().ConfigureAwait(false);
            }
            catch (TrackerConfigurationException ex)
            {
                Logging.WriteLine($"ERROR: Your configuration has issues and the tracker cannot start with them: {ex.Message}");
            }
            catch (Exception ex)
            {
                Logging.WriteLine($"Critical error!\n{ex}");
            }

            Console.WriteLine("\nApplication stopped, press any key to exit...");
            Console.ReadKey();
        }

        static void SetWorkingDirectory()
        {
            var exePath = Assembly.GetExecutingAssembly().Location;
            if (exePath == null)
            {
                throw new InvalidOperationException("Could not locate the executing assembly path location");
            }

            string exeDir = Path.GetDirectoryName(exePath!)!;
            Directory.SetCurrentDirectory(exeDir);
        }

        static void ConfigureConsoleCancellation()
        {
            Console.CancelKeyPress += Console_CancelKeyPress;
        }

        private static void Console_CancelKeyPress(object? sender, ConsoleCancelEventArgs e)
        {
            Logging.WriteLine($"Stopping tracker and exiting...");

            if (Tracker != null)
            {
                Tracker.StopTracking();
            }

            Environment.Exit(0);
        }

        static ITrackerConfig LoadConfiguration(string iniFileName, string[] args)
        {
            if (DefaultConfigWriter.Write(TrackerConstants.IniConfigFileName, typeof(ITrackerConfig)))
            {
                Logging.WriteLine($"Configuration file not found, created default: {iniFileName}");
            }

            Logging.WriteLine($"Loading configuration from '{iniFileName}'...");

            var config = new ConfigurationBuilder<ITrackerConfig>()
                .UseCommandLineArgs(isCaseSensitive: false, args)
                .UseIniFile(TrackerConstants.IniConfigFileName)
                .Build();

            if (config.TrackerMaxServerPlayers <= 0)
            {
                Logging.WriteLine("WARNING: Your configuration is incomplete! Make sure to set 'MaxServerPlayers' to your server's max players cap in the configuration file.");
            }

            if (config.DiscordBotToken == "your_bot_token_here")
            {
                throw new TrackerConfigurationException("Tracker cannot run without a Discord bot token, set your bot's token into 'BotToken' in the configuration file.");
            }

            if (config.TrackerInterval < 10000)
            {
                config.TrackerInterval = 10000;
                Logging.WriteLine("WARNING: Tracker interval cannot be lower than 10000, the interval has been reset to the lowest allowed value.");
            }

            return config;
        }
    }
}