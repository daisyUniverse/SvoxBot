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
    [Command("ping")]
    public async Task Test(){ await ReplyAsync("hewwo"); }

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

        [Command("_")] // The actual command
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
