using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DailyProgrammer_258
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //chat.freenode.net:6667
            //Nickname : DBrown24601
            //Username : DBrown24601
            //Real Name : Dan Brown
            if (!backgroundWorker1.IsBusy)
            {
                // Start the asynchronous operation.
                backgroundWorker1.RunWorkerAsync();
                //Change button to Send/Disconnect
                button1.Text = "Send";
            }
        }

        void writeToBox(string text)
        {
            System.Console.WriteLine(text);
            richTextBox1.AppendText(text + "\n");
        }

        void writeToNamebox(string name)
        {
            nameBox.AppendText(name + "\n");
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
             
            string host = "chat.freenode.net";
            int port = 6667;
            string nick = "DBrown24601";
            string user = "DBrown24601";
            string real = "DBrown24601";

            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPHostEntry ipHostInfo = Dns.Resolve(host);
            IPAddress ipAddress = ipHostInfo.AddressList[0];

            IPEndPoint ipe = new IPEndPoint(ipAddress, port);

            string recieve = "";
            string message = "";
            byte[] bytes = new byte[8192];

            s.Connect(ipe);

            backgroundWorker1.ReportProgress(0,("Connected to " + s.RemoteEndPoint.ToString()));

            message = "NICK " + nick + "\nUSER " + user + " 0 * :" + real + "\nJOIN #reddit-dailyprogrammer\n";
            s.Send(Encoding.ASCII.GetBytes(message));
            backgroundWorker1.ReportProgress(0, message);
            
            while (true)
            {
                recieve = Encoding.ASCII.GetString(bytes, 0, s.Receive(bytes));
                backgroundWorker1.ReportProgress(0, recieve);

                if (recieve.Contains("353"))
                {
                    string[] substrs = recieve.Split(':');
                    string[] names = substrs[6].Split(' ');
                    foreach (var name in names)
                        backgroundWorker1.ReportProgress(1, name);
                }

                if (recieve.StartsWith("PING"))
                {
                    s.Send(Encoding.ASCII.GetBytes(recieve.Replace("PING", "PONG")));
                    backgroundWorker1.ReportProgress(0, "Pong sent");
                }
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage.ToString() == "0")
            {
                writeToBox((string)e.UserState);
            }

            if (e.ProgressPercentage.ToString() == "1")
            {
                writeToNamebox((string)e.UserState);
            }
            
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled == true)
            {
                writeToBox("Canceled!");
            }
            else if (e.Error != null)
            {
                writeToBox("Error: " + e.Error.Message);
            }
            else
            {
                writeToBox("Done!");
            }
        }
    }
}
