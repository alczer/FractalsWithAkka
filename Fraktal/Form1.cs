using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Fraktal
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            panel1.AutoScroll = true;
            pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
            textBox1.Text = "640";
            textBox2.Text = "640";
            radioButton1.Checked = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
           
            if (radioButton1.Checked)
            {
                var watch = System.Diagnostics.Stopwatch.StartNew();
                pictureBox1.Image = Program.generateFractal(Int32.Parse(textBox1.Text), Int32.Parse(textBox2.Text));
                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                label4.Text = elapsedMs.ToString() + " milisekundy";
            }
            else if (radioButton2.Checked)
            {
                var watch = System.Diagnostics.Stopwatch.StartNew();
                pictureBox1.Image = Program.useAkka(Int32.Parse(textBox1.Text), Int32.Parse(textBox2.Text));
                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                label4.Text = elapsedMs.ToString() + " milisekundy";

            }
            

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
