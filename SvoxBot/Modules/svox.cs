using Discord.Commands;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace SvoxBot.Modules
{
    // Just a ping to see if the bot is working
    public class svox : ModuleBase<SocketCommandContext>
    {
        [Command("help")]
        public async Task Test()
        {
            await ReplyAsync("**SVOXBOT**: A Bot for Half-Life fans\n" +
                             "Combines .wav files in the order that you specify, and uploads the file to Discord  \n\n" +
                             "`!say [soundpack] [words words words]`: Generate a sound file \n" +
                             "`!packs`: Show installed soundpacks \n" +
                             "`!sounds [soundpack]`: Show sounds in a soundpack");
        }
        
        [Command("about")]
        public async Task about()
        {
            await ReplyAsync("**SVOXBOT**: A Bot for Half-Life fans\n" +
                             "Combines .wav files in the order that you specify, and uploads the file to Discord  \n\n" +
                             "Created by Robin Universe \n" +
                             "https://github.com/robinuniverse/SvoxBot \n");
        }

        
    // Combine the .WAV files
    public static void Concatenate(string outputFile, IEnumerable<string> sourceFiles)
    {
        byte[] buffer = new byte[1024];
        WaveFileWriter waveFileWriter = null;
        try
        {
            foreach (string sourceFile in sourceFiles)
            {
                using (WaveFileReader reader = new WaveFileReader(sourceFile))
                {
                    if (waveFileWriter == null)
                    {
                        // first time in create new Writer
                        waveFileWriter = new WaveFileWriter(outputFile, reader.WaveFormat);
                    }
                    else
                    {
                        if (!reader.WaveFormat.Equals(waveFileWriter.WaveFormat))
                        {
                            throw new InvalidOperationException("Can't concatenate WAV Files that don't share the same format");
                        }
                    }
                    int read;
                    while ((read = reader.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        waveFileWriter.WriteData(buffer, 0, read);
                    }
                }
            }
        }
        finally
        {
            if (waveFileWriter != null)
            {
                waveFileWriter.Dispose();
            }
        }
    }
    
    public async Task ChunksUpto(string str, int maxChunkSize) {
        for (int i = 0; i < str.Length; i += maxChunkSize) 
            await ReplyAsync("```" + str.Substring(i, Math.Min(maxChunkSize, str.Length-i)) + "```");
    }

        // Split up the text and format it for the Concatinator to process
        public static string processText(string collection, string inputText, string outputFile)
        {
            bool errorstate = true;
            string phrase = inputText;
            string[] words = phrase.Split(' ');
            int count = 0;
            foreach (string file in words)
            {
                words[count] = words[count] + ".wav";
                words[count] = collection + @"\" + words[count];
                if (!File.Exists(words[count])) // Checks for missing files
                {
                    errorstate = true;
                    break;
                }
                else
                {
                    count++;
                    errorstate = false;
                }
            }
            if (errorstate == false) // Sends formatted filenames to the concatinator
            {
                List<string> list = new List<string>(words);
                IEnumerable<string> input = list;
                svox.Concatenate(outputFile + ".wav", input);
                string Error = null; // Sets error to null if nothing went wrong
                return Error;
            }
            else
            {
                string Error = "Error! File `" + words[count] + "` not found!"; // Makes an error message
                return Error; // Return Error so the command knows not to send a bad file
            }
        }
        
        [Command("sounds")]
        public async Task soundsCommand([Remainder] string text)
        {
            string[] words = text.Split(' ');
            string folder = text.Replace(words[0] + " ", "");

            if (Directory.Exists(folder + @"\") && folder != "")
            {
                string sounds = String.Join(", ", Directory.GetFiles(folder + @"\"));
                string sounds2 = sounds.Replace(folder + @"\", "").Replace(".wav","");
                if (sounds2.Length > 1950)
                    ChunksUpto(sounds2, 1950);
                else
                {
                    await ReplyAsync("```python\n" + sounds2 + "```");
                }
            } else
            {
                await ReplyAsync("Folder not found...");
            }
        }
        
        [Command("packs")]
        public async Task packsCommand()
        {
            var directories = Directory.GetDirectories(Directory.GetCurrentDirectory());
            int count = 0;
            foreach (var strings in directories)
            {
                string fundir = directories[count].Substring(directories[count].LastIndexOf(@"\") + 1);
                directories[count] = fundir;
                count++;
            }
            string packs = String.Join(", ",directories);
            await ReplyAsync("Current Available Soundpacks: `" + packs + "`");
        }

        [Command("say")] // The actual command
        public async Task svoxCommand([Remainder] string text)
        {
            string[] words = text.Split(' ');
            string rest = text.Replace(words[0] + " ", "");
            string ErrorState = processText(words[0], rest, rest);
            if (ErrorState == null) // Checks to see all the words were valid
            {
                processText(words[0], rest, rest); // Sends words down pipeline to get concatinated
                await Context.Channel.SendFileAsync(rest + ".wav"); // Send the file
                File.AppendAllText("log.txt", Context.User + " : " + rest + System.Environment.NewLine); // Add event to log
            }
            else
            {
                await ReplyAsync(processText(words[0], rest, rest)); // If it did Error, send error text to Discord
                File.AppendAllText("log.txt", Context.User + " : " + processText(words[0], rest, rest) + System.Environment.NewLine);
            }
        }

    }
}
