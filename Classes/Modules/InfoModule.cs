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

        // ReplyAsync is a method on ModuleBase

        [Group("sample")]
        public class SampleModule : ModuleBase<SocketCommandContext>
        {
            // ~sample userinfo --> foxbot#0282
            // ~sample userinfo @Khionu --> Khionu#8708
            // ~sample userinfo Khionu#8708 --> Khionu#8708
            // ~sample userinfo Khionu --> Khionu#8708
            // ~sample userinfo 96642168176807936 --> Khionu#8708
            // ~sample whois 96642168176807936 --> Khionu#8708
            [Command("userinfo")]
            [Alias("user", "whois")]
            public async Task UserInfoAsync(
                SocketUser? user = null)
            {
                var userInfo = user ?? Context.Client.CurrentUser;
                await ReplyAsync($"{userInfo.Username}#{userInfo.Discriminator}");
            }

            // ~sample serverUserList
            // ~sample asu
            // ~sample allUsers
            // ~sample serverUsers
            [Command("serverUserList")]
            [Alias ("allUsers", "serverUsers", "asu")]
            public async Task UserListAsync()
            {
                List<string> guildUsersNames = new List<string>();

                try
                {
                    var guild = Context.Client.Guilds.Where(g => g.Name == "The Goose Hut").First();
                    var guildUsers= guild.GetUsersAsync().ToListAsync().Result;
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
        }

        /// <summary>
        /// Nested submodules example
        /// </summary>
        [Group("admin")]
        public class AdminModule : ModuleBase<SocketCommandContext>
        {
            [Group("clean")]
            public class CleanModule : ModuleBase<SocketCommandContext>
            {
                // ~admin clean
                [Command]
                public async Task DefaultCleanAsync()
                {
                    // ...
                }

                // ~admin clean messages 15
                [Command("messages")]
                public async Task CleanAsync(int count)
                {
                    // ...
                }
            }
            // ~admin ban foxbot#0282
            [Command("ban")]
            public Task BanAsync(IGuildUser user) =>
                Context.Guild.AddBanAsync(user);
        }
    }
}
