# SvoxBot
A C# Discord bot for Half-Life fans

![alt](https://i.imgur.com/1OoXXSs.jpg)

# How to Run

1. go to releases and download the zip
2. create your own bot user and invite it to your server as seen [here](https://youtu.be/f7poEyI0lAQ)
3. Extract the SvoxBot zip, and run SvoxBot.exe
4. Copy your bot Token, enter it into the text box and press 'Save Token'
5. Restart the program. You should now see the bot in your server
6. type "sv_ vox test" in your server to test the bot

# Demo Video

[![Demo Video](https://img.youtube.com/vi/W_ySzLsUe-M/0.jpg)](https://youtu.be/W_ySzLsUe-M)

# What does it do?

it's a Discord bot that is made to run in windows that
combines .wav files in the order that you type them in discord.

It's made to be able to recreate the VOX lines from Half-Life

![alt](https://i.imgur.com/EoRCIrp.png)

syntax: {sv_} { desired wav subfolder } {words, words, words}

In order to use this you need to make your own bot account and get the token.
You can follow the instructions here to do that https://youtu.be/f7poEyI0lAQ

Right now all you need to do is to find your desired wav pack (Google is your friend) and extract it into a subfolder
with something you won't mind typing to trigger the command, for example, I have a HEV folder for the hev lines, so when I want to trigger that I type 

<sv_ hev hiss beep getmedkit>

Each generated sound will be saved in the working folder seperately. I'll add the option to overwrite itself later. Just makes it easier to keep hold of good lines, but it is wasteful.

Some notes: 
- You need to make sure you only use .WAV files that have the same bitrate and channels
- I plan on allowing much more customization, but I thought I would get a more basic version out now rather than later

Some goals:
- Allow zipped soundpacks
- custom trigger word
- GUI Console that logs usage (Steam 2004 look)
- Reduce the dll usage. I don't know why it's like that. I'm trying to figure it out
- Ability to show you all the words you messed up instead of just the first one
- help menu that scans and shows pages of available sounds in a given pack
