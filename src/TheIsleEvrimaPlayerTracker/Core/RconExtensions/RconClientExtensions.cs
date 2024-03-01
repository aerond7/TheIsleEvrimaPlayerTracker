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
            var lines = response.Split("\n")
                                .Skip(1)
                                .ToArray();
            var eosIds = lines[0].Split(',')
                                 .Where(eos => !string.IsNullOrEmpty(eos))
                                 .ToArray();
            var names = lines[1].Split(',')
                                .Where(name => !string.IsNullOrEmpty(name))
                                .ToArray();

            for (int i = 0; i < eosIds.Length; i++)
            {
                result.Add(new ServerPlayer
                {
                    EosId = eosIds[i],
                    PlayerName = names[i]
                });
            }

            return result;
        }
    }
}
