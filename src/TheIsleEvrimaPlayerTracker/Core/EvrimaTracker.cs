using System.Globalization;
using TheIsleEvrimaPlayerTracker.Core.Rcon;
using TheIsleEvrimaPlayerTracker.Configuration;
using TheIsleEvrimaPlayerTracker.Core.Discord;
using TheIsleEvrimaPlayerTracker.Core.RconExtensions;

namespace TheIsleEvrimaPlayerTracker.Core
{
    public class EvrimaTracker
    {
        private readonly ITrackerConfig trackerConfig;
        private readonly DiscordBot discordBot;

        private bool _tracking = false;
        private DateTime? _lastTrackingTime;

        public EvrimaTracker(ITrackerConfig trackerConfig)
        {
            this.trackerConfig = trackerConfig;
            discordBot = new DiscordBot(this.trackerConfig.DiscordBotToken);
        }

        public async Task StartTracking()
        {
            Logging.WriteLine("Starting tracker...");

            _tracking = true;

            Logging.WriteLine($"RCON will connect to {trackerConfig.RconHost}:{trackerConfig.RconPort}");

            Logging.WriteLine("Connecting to Discord bot...");
            try
            {
                await discordBot.ConnectAsync();
            }
            catch
            {
                throw new InvalidOperationException("Failed to connect to Discord bot. Check that your bot token is set correctly, or Discord is having an outage.");
            }
            Logging.WriteLine($"Discord bot '{discordBot.GetBotUsername()}' connected!");

            Logging.WriteLine("Tracker started. Press CTRL+C to stop and exit.");

            while (_tracking)
            {
                if (!_lastTrackingTime.HasValue
                    || _lastTrackingTime.HasValue && _lastTrackingTime.Value.AddMilliseconds(trackerConfig.TrackerInterval) < DateTime.Now)
                {
                    string output = GetDisplayStringByPattern(trackerConfig.TrackerDisplayPattern, 0, trackerConfig.TrackerMaxServerPlayers);

                    try
                    {
                        using (var rcon = new EvrimaRconClient(trackerConfig.RconHost, trackerConfig.RconPort, trackerConfig.RconPassword, trackerConfig.RconTimeout))
                        {
                            if (await rcon.ConnectAsync())
                            {
                                var players = await rcon.GetPlayerList();
                                _lastTrackingTime = DateTime.Now;
                                output = GetDisplayStringByPattern(trackerConfig.TrackerDisplayPattern, players.Count, trackerConfig.TrackerMaxServerPlayers);
                                Logging.WriteLine($"Tracker: {output}");
                            }
                            else
                            {
                                Logging.WriteLine($"RCON connection to {trackerConfig.RconHost}:{trackerConfig.RconPort} failed! Check and make sure your RCON connection details in the configuration file are correct.");
                            }
                        }
                    }
                    catch
                    {
                        Logging.WriteLine("Tracker: Connection lost, is the server offline?");
                    }
                    
                    await discordBot.SetActivity(output);
                }

                await Task.Delay(1000);
            }
        }

        public void StopTracking()
        {
            _tracking = false;
            Logging.WriteLine("Tracker stopped");
        }

        private string GetDisplayStringByPattern(string pattern, int online, int max)
        {
            return pattern.Replace("{online}", online.ToString(), ignoreCase: true, CultureInfo.InvariantCulture)
                          .Replace("{max}", max.ToString(), ignoreCase: true, CultureInfo.InvariantCulture);
        }
    }
}
