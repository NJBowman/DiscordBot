using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Interfaces;

namespace DiscordBot.Classes.Services
{
    public class LoggingService : ILoggingService
    {
        public LoggingService(DiscordSocketClient client, CommandService command)
        {
            client.Log += LogAsync;
            command.Log += LogAsync;
        }

        public LoggingService()
        {
        }

        private Task LogAsync(LogMessage message)
        {
            if (message.Exception is CommandException cmdException)
            {
                Console.WriteLine($"[Command/{message.Severity}] {cmdException.Command.Aliases.First()}"
                    + $" failed to execute in {cmdException.Context.Channel}.");
                Console.WriteLine(cmdException);
            }
            else
                Console.WriteLine($"[General/{message.Severity}] {message}");

            return Task.CompletedTask;
        }

        public Task LogAsyncToClient(DiscordSocketClient client , LogMessage message)
        {
            client.Log += LogAsync;

            return LogAsync(message);
        }


        /// <summary>
        /// Handle Discord.Net event messages - Check log events for further information
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public Task Log(LogMessage msg)
        {
            switch (msg.Severity)
            {
                case LogSeverity.Critical:
                case LogSeverity.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LogSeverity.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogSeverity.Info:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case LogSeverity.Verbose:
                case LogSeverity.Debug:
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    break;
            }
            Console.WriteLine($"{DateTime.Now,-19} [{msg.Severity,8}] {msg.Source}: {msg.Message} {msg.Exception}");
            Console.ResetColor();

            return Task.CompletedTask;
        }
    }
}
