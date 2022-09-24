using Discord;
using Discord.Commands;
using Discord.WebSocket;
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
            // ~sample square 20 -> 400
            [Command("square")]
            [Summary("Squares a number.")]
            public async Task SquareAsync(
                [Summary("The number to square.")]
                int num)
            {
                // We can also access the channel from the Command Context.
                await Context.Channel.SendMessageAsync($"{num}^2 = {Math.Pow(num, 2)}");
            }

            // ~sample userinfo --> foxbot#0282
            // ~sample userinfo @Khionu --> Khionu#8708
            // ~sample userinfo Khionu#8708 --> Khionu#8708
            // ~sample userinfo Khionu --> Khionu#8708
            // ~sample userinfo 96642168176807936 --> Khionu#8708
            // ~sample whois 96642168176807936 --> Khionu#8708
            [Command("userinfo")]
            [Summary
            ("Returns info about the current user, or the user parameter, if one passed.")]
            [Alias("user", "whois")]
            public async Task UserInfoAsync(
                [Summary("The (optional) user to get info from")]
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
            [Summary("Returns a list of users within the specified server")]
            [Alias ("allUsers", "serverUsers", "asu")]
            public async Task UserListAsync()
            {
                var guildUsers = Context.Guild.Users;
                var guildUsersNames = guildUsers.Select(u => u.Username);

                await ReplyAsync(string.Join("\n", guildUsersNames));
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
