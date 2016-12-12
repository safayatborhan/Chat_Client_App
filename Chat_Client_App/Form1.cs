using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;

namespace Chat_Client_App
{
    public partial class Form1 : Form
    {
        Socket sck;
        EndPoint eplocal, epremote;
        public Form1()
        {
            InitializeComponent();
            sck = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            sck.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            textBox1.Text = getlocalip();
            textBox3.Text = getlocalip();
        }
        private string getlocalip()
        {
            IPHostEntry host;
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            return "127.0.0.1";
        }
        private void messagecallback(IAsyncResult aresult)
        {
            try
            {
                int size = sck.EndReceiveFrom(aresult, ref epremote);
                if (size > 0)
                {
                    byte[] recieveddata = new byte[1464];
                    recieveddata = (byte[])aresult.AsyncState;
                    ASCIIEncoding eencoding = new ASCIIEncoding();
                    string recievedmessage = eencoding.GetString(recieveddata);
                    listBox1.Items.Add("Friend : "+recievedmessage);
                }
                byte[] buffer = new byte[1500];
                sck.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref epremote, new AsyncCallback(messagecallback), buffer);
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.ToString());
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
                byte[] msg = new byte[1500];
                msg = enc.GetBytes(textBox5.Text);
                sck.Send(msg);
                listBox1.Items.Add("You : " + textBox5.Text);
                textBox5.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                eplocal = new IPEndPoint(IPAddress.Parse(textBox1.Text), Convert.ToInt32(textBox2.Text));
                sck.Bind(eplocal);
                epremote = new IPEndPoint(IPAddress.Parse(textBox3.Text), Convert.ToInt32(textBox4.Text));
                sck.Connect(epremote);
                byte[] buffer = new byte[1500];
                sck.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref epremote, new AsyncCallback(messagecallback), buffer);
                button1.Text = "Connected";
                button1.Enabled = false;
                Send.Enabled = true;
                textBox5.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("pp", "About", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("pp", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void exitToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void getIPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string str;
            str = getlocalip();
            MessageBox.Show(str, "My IP", MessageBoxButtons.OK, MessageBoxIcon.Question);
        }

        private void textBox5_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                try
                {
                    System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
                    byte[] msg = new byte[1500];
                    msg = enc.GetBytes(textBox5.Text);
                    sck.Send(msg);
                    listBox1.Items.Add("You : " + textBox5.Text);
                    textBox5.Clear();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
