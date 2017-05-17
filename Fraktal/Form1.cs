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

namespace Fraktal
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
            textBox1.Text = "800";
            textBox2.Text = "800";

            String strHostName = string.Empty;
            // Getting Ip address of local machine...
            // First get the host name of local machine.
            strHostName = Dns.GetHostName();
            Console.WriteLine("Local Machine's Host Name: " + strHostName);
            // Then using host name, get the IP address list..
            IPHostEntry ipEntry = Dns.GetHostEntry(strHostName);
            IPAddress[] addr = ipEntry.AddressList;
            for (int i = 0; i < addr.Length; i++)
            {
                comboBox1.Items.Add(addr[i].ToString());
            }
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
           
            var watch = System.Diagnostics.Stopwatch.StartNew();
            pictureBox1.Image = Program.useAkka(Int32.Parse(textBox1.Text), Int32.Parse(textBox2.Text), "single");
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            label4.Text = elapsedMs.ToString() + " milisekundy";
        }
        
        private void button2_Click(object sender, EventArgs e)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            Program.runRemote(comboBox1.Text);
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            label4.Text = elapsedMs.ToString() + " milisekundy";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            pictureBox1.Image = Program.runLocal(Int32.Parse(textBox1.Text), Int32.Parse(textBox2.Text), comboBox1.Text, "local");
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            label4.Text = elapsedMs.ToString() + " milisekundy";
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            pictureBox1.Image = Program.generateFractal(Int32.Parse(textBox1.Text), Int32.Parse(textBox2.Text));
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            label4.Text = elapsedMs.ToString() + " milisekundy";
        }

        private void button5_Click(object sender, EventArgs e)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            pictureBox1.Image = Program.useAkka(Int32.Parse(textBox1.Text), Int32.Parse(textBox2.Text), "single2");
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            label4.Text = elapsedMs.ToString() + " milisekundy";
        }

        private void button6_Click(object sender, EventArgs e)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            pictureBox1.Image = Program.runLocal(Int32.Parse(textBox1.Text), Int32.Parse(textBox2.Text), comboBox1.Text, "local2");
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            label4.Text = elapsedMs.ToString() + " milisekundy";
        }
    }
}
