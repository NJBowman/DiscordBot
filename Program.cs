using Discord;
using Discord.Commands;
using Discord.Net.Providers.WS4Net;
using Discord.WebSocket;
using DiscordBot.Classes.Modules;
using DiscordBot.Classes.Services;
using DiscordBot.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using System.Reflection;

namespace DiscordBot
{
    internal class Program
    {
        public static Task Main(string[] args) => new Program().MainAsync();

        private DiscordSocketClient _client;
        private readonly ILoggingService _loggingService;
        private readonly CommandService _commands;
        private readonly IServiceProvider _services;

        private Program()
        {
            _loggingService = new LoggingService();
            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Info,
                MessageCacheSize = 50,
                WebSocketProvider = WS4NetProvider.Instance
            });
            _commands = new CommandService(new CommandServiceConfig
            {
                LogLevel = LogSeverity.Info,
                CaseSensitiveCommands = false,
            });
            _services = ConfigureServices();
        }

        public async Task MainAsync()
        {
            _client.Log += _loggingService.Log;
            _commands.Log += _loggingService.Log;

            var tokenFilePath = ConfigurationManager.AppSettings["BotTokenFilePath"];
            Console.WriteLine($"Bot token recieved from filepath: {tokenFilePath}");

            string token = File.ReadAllText(tokenFilePath);

            await InitCommands();

            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            await Task.Delay(-1);
        }

        private static IServiceProvider ConfigureServices()
        {
            var map = new ServiceCollection()
                .AddTransient<ILoggingService, LoggingService>();

            return map.BuildServiceProvider();
        }

        private async Task InitCommands()
        {
            // Either search the program and add all Module classes that can be found.
            // Module classes MUST be marked 'public' or they will be ignored.
            // You also need to pass your 'IServiceProvider' instance now,
            // so make sure that's done before you get here.
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
            // Or add Modules manually if you prefer to be a little more explicit:
            //await _commands.AddModuleAsync<InfoModule>(_services);
            // Note that the first one is 'Modules' (plural) and the second is 'Module' (singular).

            // Subscribe a handler to see if a message invokes a command.
            _client.MessageReceived += HandleCommandAsync;
        }

        private async Task HandleCommandAsync(SocketMessage arg)
        {
            // Bail out if it's a System Message.
            var msg = arg as SocketUserMessage;
            if (msg == null) return;

            // We don't want the bot to respond to itself or other bots.
            if (msg.Author.Id == _client.CurrentUser.Id || msg.Author.IsBot) return;

            // Create a number to track where the prefix ends and the command begins
            int pos = 0;
            // Replace the '!' with whatever character
            // you want to prefix your commands with.
            // Uncomment the second half if you also want
            // commands to be invoked by mentioning the bot instead.
            if (msg.HasCharPrefix('!', ref pos) /* || msg.HasMentionPrefix(_client.CurrentUser, ref pos) */)
            {
                // Create a Command Context.
                var context = new SocketCommandContext(_client, msg);

                // Execute the command. (result does not indicate a return value, 
                // rather an object stating if the command executed successfully).
                var result = await _commands.ExecuteAsync(context, pos, _services);

                // Uncomment the following lines if you want the bot
                // to send a message if it failed.
                // This does not catch errors from commands with 'RunMode.Async',
                // subscribe a handler for '_commands.CommandExecuted' to see those.
                //if (!result.IsSuccess && result.Error != CommandError.UnknownCommand)
                //    await msg.Channel.SendMessageAsync(result.ErrorReason);
            }
        }
    }
}