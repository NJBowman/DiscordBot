using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Classes.Modules
{
    public class InfoModule : ModuleBase<SocketCommandContext>
    {

        // ~say hello world -> hello world
        [Command("say")]
        [Summary("Echoes a message.")]
        public Task SayAsync([Remainder][Summary("The text to echo")] string echo)
            => ReplyAsync(echo);

        [Group("users")]
        public class UserModule : ModuleBase<SocketCommandContext>
        {
            // ~users all
            [Command("all")]
            public async Task UserListAsync()
            {
                List<string> guildUsersNames = new List<string>();

                try
                {
                    var guild = Context.Client.Guilds.Where(g => g.Name == "The Goose Hut").First();
                    var guildUsers = guild.GetUsersAsync().ToListAsync().Result;
                    guildUsersNames = guildUsers[0].Select(u => u.Username).ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message.ToString());
                }
                finally
                {
                    if (guildUsersNames.Count > 0)
                    {
                        await ReplyAsync(string.Join("\n", guildUsersNames));
                    }
                    else
                    {
                        await ReplyAsync("Bot couldn't find anyone");
                    }
                }
            }

            //~users bitchRole
            [Command("bitchRole")]
            public async Task BitchListAsync()
            {
                var guild = Context.Client.Guilds.Where(g => g.Name == "The Goose Hut").First();
                var littleBitchRole = guild.Roles.Where(r => r.Name == "Little Bitch").First();
                var guildUsers = guild.GetUsersAsync().ToListAsync().Result;
                var littleBitchUsers = guildUsers[0].Where(u => u.RoleIds.Contains(littleBitchRole.Id)).ToList();

                if (littleBitchUsers.Count > 0)
                {
                    await ReplyAsync(string.Join("\n", littleBitchUsers));
                }
                else
                {
                    await ReplyAsync("Bot couldn't find any bitches");
                }
            }
        }

        [Group("events")]
        public class EventModule : ModuleBase<SocketCommandContext>
        {
            // ~events createEvent
            [Command("createEvent")]
            public async Task CreateEventAsync(string eventName, int daysInFuture, string eventLocation)
            {
                var guild = Context.Client.Guilds.Where(g => g.Name == "The Goose Hut").First();
                var guildEvent = await guild
                    .CreateEventAsync(eventName,
                    DateTimeOffset.UtcNow.AddDays(daysInFuture),
                    GuildScheduledEventType.Stage,
                    endTime: DateTimeOffset.UtcNow.AddDays(daysInFuture + 1),
                    location: eventLocation);

                await ReplyAsync(string.Join("\n", guild.Events));
            }
        }

        [Group("channel")]
        public class ChannelModule : ModuleBase<SocketCommandContext>
        {
            // ~channel createChannelPost
            [Command("createChannelPost")]
            public async Task ChannelPostAsync(string channelName)
            {
                var guild = Context.Client.Guilds.Where(g => g.Name == "The Goose Hut").First();
                var selectedChannels = guild.Channels.Where(c => c.Name == channelName);

                if (selectedChannels.Count() == 0)
                {
                    await ReplyAsync("No Channels with that name");
                }
                else if (selectedChannels.Count() > 1)
                {
                    await ReplyAsync($"More than one channel found, please be more specific: \n {string.Join("/n", selectedChannels.Select(c => c.Name))}");
                }
                else 
                {
                    //Create a channel post...
                    // may have to be a slash command in channel with admin permissions applied
                }


            }
        }
    }
}
