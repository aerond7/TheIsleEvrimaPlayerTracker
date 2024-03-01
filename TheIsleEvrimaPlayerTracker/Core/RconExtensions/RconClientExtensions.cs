using TheIsleEvrimaPlayerTracker.Core.Models;
using TheIsleEvrimaPlayerTracker.Core.Rcon;

namespace TheIsleEvrimaPlayerTracker.Core.RconExtensions
{
    public static class RconClientExtensions
    {
        public static async Task<List<ServerPlayer>> GetPlayerList(this EvrimaRconClient client)
        {
            var result = new List<ServerPlayer>();

            var response = await client.SendCommandAsync(EvrimaCommand.PlayerList);
            var lines = response.Replace(",", string.Empty)
                                .Split("\n")
                                .Skip(1)
                                .ToArray();

            for (int i = 0; i < lines.Length; i = i + 2)
            {
                result.Add(new ServerPlayer
                {
                    EosId = lines[i],
                    PlayerName = lines[i + 1]
                });
            }

            return result;
        }
    }
}
