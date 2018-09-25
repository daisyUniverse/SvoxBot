using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Input;

namespace SvoxBot
{
    public partial class SvoxBot : Form
    {
        public SvoxBot()
        {
            InitializeComponent();
            var directories = Directory.GetDirectories(Directory.GetCurrentDirectory());
            int count = 0;
            foreach (var strings in directories)
            {
                string fundir = directories[count].Substring(directories[count].LastIndexOf(@"\") +1 );
                directories[count] = fundir;
                count++;
            }
            TermLine("Welcome to the Black Mesa Announcement System");
            TermLine("SvoxBot by Elisha Shaddock");
            TermLine("");
            TermLine("Current Available Soundpacks: ");
            TermLines(directories);
            TermLine("");
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == WM_NCHITTEST)
                m.Result = (IntPtr)(HT_CAPTION);
        }

        private const int WM_NCHITTEST = 0x84;
        private const int HT_CLIENT = 0x1;
        private const int HT_CAPTION = 0x2;

        private void applyButton_Click(object sender, EventArgs e)
        {
         string token = textBox1.Text;
         File.WriteAllText("token.txt", token);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            System.Environment.Exit(0);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void consoleButton_Click(object sender, EventArgs e)
        {

        }

        void textBox2_TextChanged(object sender, EventArgs e)
        {
            this.AcceptButton = enterCommand;
        }

        public void TermLines(string[] Line)
        {
            int count = 0;
            foreach (string lines in Line)
            {
                textBox3.AppendText(Line[count]);
                textBox3.AppendText(Environment.NewLine);
                count++;
            }
        }

        public void TermLine(string Line)
        {

           textBox3.AppendText(Line);
           textBox3.AppendText(Environment.NewLine);

        }

        private void enterCommand_Click(object sender, EventArgs e)
        {
            string command = textBox2.Text;

            if (command == "log"){ErrorLog();}
            if (command == "clear") { textBox3.ResetText(); }
            if (command == "token") { if(File.Exists("token.txt"))TermLine("Current bot token: "+File.ReadAllText("token.txt"));else TermLine("Token file missing!"); }

            if (command == "help")
            {
                TermLine("log - prints logfile to screen");
                TermLine("prefix - set bot trigger");
                TermLine("clear - clears screen");
                TermLine("token - prints current saved bot token");
                TermLine("sounds {soundpack} - prints sounds in soundpack");
                TermLine("help - what are you, stupid?");
                TermLine("packs - shows all available soundpacks");
            }

            if (command.Contains("sounds "))
            {
                string folder = command.Split(' ')[1];
                if (Directory.Exists(folder + @"\") && folder != "")
                {
                    TermLines(Directory.GetFiles(folder + @"\"));
                }
                else
                {
                    TermLine("Folder not found...");
                }
            }

            if (command.Contains("prefix "))
            {
                string prefix = command.Split(' ')[1];
                File.WriteAllText("prefix.txt", prefix);
                TermLine("New prefix set: " + prefix);
            }

            if (command.Contains("packs"))
            {
                var directories = Directory.GetDirectories(Directory.GetCurrentDirectory());
                int count = 0;
                foreach (var strings in directories)
                {
                    string fundir = directories[count].Substring(directories[count].LastIndexOf(@"\") + 1);
                    directories[count] = fundir;
                    count++;
                }
                TermLine("Current Available Soundpacks: ");
                TermLines(directories);
                TermLine("");
                TermLine("use sounds {foldername} to show all sounds in that pack");
            }


            textBox2.ResetText();

        }

        void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        public void ErrorLog()
        {
            string[] errorFile = File.ReadAllLines("log.txt");
            TermLines(errorFile);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked) { SvoxBot.ActiveForm.Width = 700; }

            if (!checkBox1.Checked) { SvoxBot.ActiveForm.Width = 229; }
        }
    }
}
