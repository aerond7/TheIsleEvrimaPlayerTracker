using DSharpPlus;
using DSharpPlus.Entities;
using Microsoft.Extensions.Logging;

namespace TheIsleEvrimaPlayerTracker.Core.Discord
{
    public class DiscordBot
    {
        private readonly DiscordClient bot;

        private DiscordActivity _activity;

        public DiscordBot(string token)
        {
            _activity = new DiscordActivity("EvrimaTrackerBot", ActivityType.Playing);

            var config = new DiscordConfiguration
            {
                Token = token,
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                MinimumLogLevel = LogLevel.Error,
                Intents = DiscordIntents.None
            };

            bot = new DiscordClient(config);
        }

        public async Task ConnectAsync()
        {
            await bot.ConnectAsync();
        }

        public async Task DisconnectAsync()
        {
            await bot.DisconnectAsync();
        }

        public async Task SetActivity(string text)
        {
            _activity.Name = text;
            if (!bot.IsConnected)
            {
                return;
            }
            await bot.UpdateStatusAsync(_activity);
        }

        public async Task ClearActivity()
        {
            if (!bot.IsConnected)
            {
                return;
            }
            await bot.UpdateStatusAsync();
        }

        public string GetBotUsername()
            => bot.CurrentUser.Username;
    }
}
