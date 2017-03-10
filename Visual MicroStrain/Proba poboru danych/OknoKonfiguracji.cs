using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Proba_poboru_danych
{
    public partial class OknoKonfiguracji : Form
    {
        public OknoKonfiguracji()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        public void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                MessageBox.Show(fbd.SelectedPath);
            textBox1.Text = fbd.SelectedPath;
            Visualization.folder = textBox1.Text;
            
            
        }
        public String GettxtForm2()
        {
            return textBox1.Text;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Visualization.OK = 1;
            string length = comboBox7.SelectedItem.ToString();
            Int32 len = Int32.Parse(length);
            Visualization.buffor_length = len;

            string chartpoints = comboBox1.SelectedItem.ToString();
            Int32 points = Int32.Parse(chartpoints);
            Visualization.chart_points = points;


            string freq = comboBox5.SelectedItem.ToString();
            Int32 intfreq = Int32.Parse(freq);
            Visualization.chart_freq = intfreq;

            string peak = textBox2.Text.Replace(".", ",");
            Double peaktopeak = Double.Parse(peak);
            Visualization.peak_diference = peaktopeak;

            string chanelname = comboBox6.Text.ToString();
            char chanel_char = chanelname[2];
            Int16 chanel = Int16.Parse("0" + chanel_char);
            Visualization.chanel_auto = chanel;
            this.Hide();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string[] bounds = { "100","200","300","500","800","1000" };
            foreach (string bound in bounds)
            {
                comboBox1.Items.Add(bound);

            }
            if(Visualization.chart_points == 0)
            { comboBox1.SelectedItem = comboBox1.Items[3]; }
            else { comboBox1.SelectedItem = Visualization.chart_points.ToString(); }
            

            string[] frequencies= { "1", "3", "5", "10", "15" };
            foreach (string frequency in frequencies)
            {
                comboBox5.Items.Add(frequency);

            }
            if (Visualization.chart_freq == 0)
            { comboBox5.SelectedItem = comboBox5.Items[0]; }
            else { comboBox5.SelectedItem = Visualization.chart_freq.ToString(); }

            string[] chanels = { "ch1", "ch2", "ch3"};
            foreach (string chanel in chanels)
            {
                comboBox6.Items.Add(chanel);

            }
            if (Visualization.chanel_auto == 0)
            { comboBox6.SelectedItem = comboBox6.Items[0]; }
            else { comboBox6.SelectedItem = "ch" + Visualization.chanel_auto.ToString(); }

            string[] lengths = { "1000", "2000","3000","5000","10000" };
            foreach (string length in lengths)
            {
                comboBox7.Items.Add(length);

            }
            if (Visualization.buffor_length == 0)
            { comboBox7.SelectedItem = comboBox7.Items[0]; }
            else { comboBox7.SelectedItem = Visualization.buffor_length.ToString(); }


            if (Visualization.peak_diference == 0)
            { textBox2.Text = "0,5"; }
            else { textBox2.Text = Visualization.peak_diference.ToString(); }
            


            if (Visualization.folder == "")
            { Visualization.folder = "C:\\Users\\Lunatyk\\Desktop";}
            textBox1.Text = Visualization.folder; 
            
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            MessageBox.Show("Zatwierdz OK");
            e.Cancel = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Hide();
        }
    }
}
