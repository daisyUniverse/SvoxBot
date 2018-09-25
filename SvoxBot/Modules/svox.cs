using Discord.Commands;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace SvoxBot.Modules
{
    // just a ping to see if the bot is working
    public class svox : ModuleBase<SocketCommandContext>
    {
        [Command("ping")]
        public async Task Test()
        {
            await ReplyAsync("y");
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
                if (!File.Exists(words[count]))
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

            if (errorstate == false)
            {
                List<string> list = new List<string>(words);
                IEnumerable<string> input = list;
                svox.Concatenate(outputFile + ".wav", input);
                string Error = null;
                return Error;
            }
            else
            {
                string Error = "Error! File `" + words[count] + "` not found!";
                return Error;
            }
        }

        // The actual command

        [Command("_")]
        public async Task svoxCommand([Remainder] string text)
        {
            
            string[] words = text.Split(' ');

            string rest = text.Replace(words[0] + " ", "");
            string ErrorState = processText(words[0], rest, rest);

            if (ErrorState == null)
            {
                processText(words[0], rest, rest);
                await Context.Channel.SendFileAsync(rest + ".wav");
                File.AppendAllText("log.txt", Context.User + " : " + rest + System.Environment.NewLine);
            }
            else
            {
                await ReplyAsync(processText(words[0], rest, rest));
                File.AppendAllText("log.txt", Context.User + " : " + processText(words[0], rest, rest) + System.Environment.NewLine);
            }
        }

    }
}
