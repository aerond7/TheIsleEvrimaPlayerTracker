using System.Net.Sockets;
using System.Text;

namespace TheIsleEvrimaPlayerTracker.Core.Rcon
{
    public class EvrimaRconClient : IDisposable
    {
        private TcpClient? _client;
        private NetworkStream? _stream;
        private bool _isAuthorized = false;

        private readonly string host;
        private readonly int port;
        private readonly string password;
        private readonly int timeout;

        public EvrimaRconClient(string host,
            int port,
            string password,
            int timeout = 5000)
        {
            this.host = host;
            this.port = port;
            this.password = password;
            this.timeout = timeout;
        }

        public async Task<bool> ConnectAsync()
        {
            try
            {
                _client = new TcpClient();
                await _client.ConnectAsync(host, port);
                _stream = _client.GetStream();
                _stream.ReadTimeout = timeout;
                return await AuthorizeAsync();
            }
            catch
            {
                return false;
            }
        }

        private async Task<bool> AuthorizeAsync()
        {
            if (!_isAuthorized)
            {
                await SendPacketAsync($"\x01{password}\x00");
                var response = await ReadPacketAsync();
                if (!response.Contains("Password Accepted"))
                {
                    return false;
                }
                _isAuthorized = true;
                await ReconnectAsync();
                return true;
            }
            return false;
        }

        private void Disconnect()
        {
            _stream?.Close();
            _client?.Close();
        }

        private async Task ReconnectAsync()
        {
            Disconnect();
            await ConnectAsync();
        }

        private async Task SendPacketAsync(string data)
        {
            if (_stream == null)
            {
                throw new InvalidOperationException("Network stream is null");
            }

            byte[] buffer = Encoding.ASCII.GetBytes(data);
            await _stream.WriteAsync(buffer, 0, buffer.Length);
        }

        private async Task<string> ReadPacketAsync()
        {
            if (_stream == null)
            {
                throw new InvalidOperationException("Network stream is null");
            }

            byte[] buffer = new byte[4096];
            int bytesRead = await _stream.ReadAsync(buffer, 0, buffer.Length);
            return Encoding.ASCII.GetString(buffer, 0, bytesRead);
        }

        public async Task<string> SendCommandAsync(string commandName, string commandArgument = "")
        {
            var formattedCommand = commandName.ToLower().Trim();

            if (!EvrimaCommandByteMap.Map.TryGetValue(formattedCommand, out byte commandByte))
            {
                return $"Unknown command: {formattedCommand}";
            }

            string commandPacket = $"\x02{Convert.ToChar(commandByte)}{commandArgument}\x00";
            await SendPacketAsync(commandPacket);
            string response = await ReadPacketAsync();

            return GetFriendlyCommandResponse(formattedCommand, response, commandArgument);
        }

        public Task<string> SendCommandAsync(EvrimaCommand command, string commandArgument = "")
        {
            return SendCommandAsync(command.ToString(), commandArgument);
        }

        private string GetFriendlyCommandResponse(string commandName, string response, string input)
        {
            switch (commandName)
            {
                case "announce":
                    return $"Announced: {input}";
                case "updateplayables":
                    return response;
                case "ban":
                    return $"Banned: {input}";
                case "kick":
                    return $"Kicked: {input}";
                case "playerlist":
                    return response;
                case "save":
                    return "Server saved";
                case "custom":
                    return "Command executed";
                default:
                    return "Unknown command";
            }
        }

        public void Dispose()
        {
            Disconnect();
            _stream?.Dispose();
            _client?.Dispose();
        }
    }
}
