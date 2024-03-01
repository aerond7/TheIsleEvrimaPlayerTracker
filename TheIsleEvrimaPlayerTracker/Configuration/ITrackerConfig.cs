using Config.Net;

namespace TheIsleEvrimaPlayerTracker.Configuration
{
    public interface ITrackerConfig
    {
        [Option(Alias = "Tracker.Interval", DefaultValue = 30000)]
        int TrackerInterval { get; set; }

        [Option(Alias = "Tracker.MaxServerPlayers", DefaultValue = 0)]
        int TrackerMaxServerPlayers { get; }

        [Option(Alias = "Tracker.DisplayPattern", DefaultValue = "{online} / {max}")]
        string TrackerDisplayPattern { get; }

        [Option(Alias = "Rcon.Host", DefaultValue = "127.0.0.1")]
        string RconHost { get; }

        [Option(Alias = "Rcon.Port", DefaultValue = 8888)]
        int RconPort { get; }

        [Option(Alias = "Rcon.Password", DefaultValue = "your_rcon_password_here")]
        string RconPassword { get; }

        [Option(Alias = "Rcon.Timeout", DefaultValue = 5000)]
        int RconTimeout { get; }

        [Option(Alias = "Discord.BotToken", DefaultValue = "your_bot_token_here")]
        string DiscordBotToken { get; }
    }
}
