using Discord;
using Discord.WebSocket;
using System.Configuration;

namespace DiscordBot
{
    internal class Program
    {
        public static Task Main(string[] args) => new Program().MainAsync();

        private DiscordSocketClient? _client;

        public async Task MainAsync()
        {
            _client = new DiscordSocketClient();

            _client.Log += Log;

            var tokenFilePath = ConfigurationManager.AppSettings["BotTokenFilePath"];

            string token = File.ReadAllText(tokenFilePath);

            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            await Task.Delay(-1);
        }

        /// <summary>
        /// Handle Discord.Net event messages - Check log events for further information
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}