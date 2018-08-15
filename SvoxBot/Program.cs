using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using NAudio.Wave;

namespace SvoxBot
{
    class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>

        [STAThread]
        static void Main(String[] args)
        {

            if (!File.Exists("token.txt"))
            {
                MessageBox.Show("Token Not Set! Please set a token and restart...");
                OpenMainWindow();
            }
            else
            {
                OpenMainWindow();
                new Program().RunBotAsync().GetAwaiter().GetResult();
            }
            
        }

        private static void OpenMainWindow()
        {
            new Thread(() => new SvoxBot().ShowDialog()).Start();

            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new SvoxBot());
            //var thread = new Thread(OpenMainWindow);
            //thread.SetApartmentState(ApartmentState.STA);
            //thread.Start();
        }

        private DiscordSocketClient _client;
        private CommandService _commands;
        private IServiceProvider _services;

        public async Task RunBotAsync()
        {
           string token = File.ReadAllText("token.txt");

            _client = new DiscordSocketClient();
            _commands = new CommandService();
            _services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_commands)
                .BuildServiceProvider();

            string botToken = token;

            await RegisterCommandsAsync();

            await _client.LoginAsync(TokenType.Bot, botToken);

            await _client.StartAsync();

            await Task.Delay(-1);

        }

        public async Task RegisterCommandsAsync()
        {
            _client.MessageReceived += HandleCommandAsync;

            await _commands.AddModulesAsync(Assembly.GetEntryAssembly());
        }

        private async Task HandleCommandAsync(SocketMessage arg)
        {
            var message = arg as SocketUserMessage;

            if (message is null || message.Author.IsBot) return;

            int argPos = 0; 

            if (message.HasStringPrefix("`",ref argPos) || message.HasMentionPrefix(_client.CurrentUser, ref argPos))
            {
                var context = new SocketCommandContext(_client, message);

                var result = await _commands.ExecuteAsync(context, argPos, _services);

                if (!result.IsSuccess) { Console.WriteLine("Nah"); }
            }
        }
    }

}
