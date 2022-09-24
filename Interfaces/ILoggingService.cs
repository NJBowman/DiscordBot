using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Interfaces
{
    public interface ILoggingService
    {
        Task Log(LogMessage msg);
        Task LogAsyncToClient(DiscordSocketClient client, LogMessage message);
    }
}
