using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.IO.Ports;

namespace Proba_poboru_danych
{
    public partial class Visualization : Form
    {
        private Thread DataThread;
        private Thread Time;
        private double[,] chanel1Array = new double[3,1000];
        private double[] ChartArray = new double[1000];
        public static int OK = 0, node_set = 0, first_save = 1;
        public static double peak_diference;
        
        public static int chart_points, buffor_length,chart_freq, chanel_auto;
        string[] ports = SerialPort.GetPortNames();
        public static string folder = "";
        public static int zapis = 0;
        public static int j=0;
        public static string wynik = "";
        string time , datetime , nazwa;
        public static string today;
        public static string pathString = "";
        mscl.Connection connection;
        mscl.BaseStation baseStation;
        mscl.WirelessNode node;
        //const String COM_PORT = ports[0];
        const int NODE_ADDRESS = 873;
        string error = "";
        OknoKonfiguracji window = new OknoKonfiguracji();



        public Visualization()
        {
            InitializeComponent();

            
        }

        //######ARRAY OPERATIONS######//
        public double peaktopeak(int chanel, double[,] tablica)
        {
            double[] tablica1 = new double[tablica.GetLength(1)];
            for (int i = 0; i < tablica.GetLength(1); i++)
            {
                tablica1[i] = tablica[chanel, i ];

            }
           double peak=tablica1.Max() - tablica1.Min();

            return peak;

        }
 
        public double[,] FIFO(double [,] tablica)
        {
            double[,] tablica1 = new double[tablica.GetLength(0), tablica.GetLength(1)];
            for (int j = 0; j < tablica.GetLength(0); j++)
            {
                for (int i = tablica.GetLength(1) - 1; i > 0; i--)
                {
                    tablica1[j, i - 1] = tablica[j, i];

                }
            }
            return tablica1;

        }

        public double[] copy(int chanel, double[,] tablica)
        {
            double[] tablica1 = new double[chart_points];
            for (int i = tablica.GetLength(1) - 1; i > tablica.GetLength(1) - chart_points; i--)
            {
                tablica1[chart_points - (tablica.GetLength(1) - i)] = tablica[chanel, i];

            }

            return tablica1;

        }

        //#######AUTO MEASUREMENT#####//
        public void Peak_check()
        {
            double peak_to_peak = peaktopeak(chanel_auto - 1, chanel1Array);

            if (pathString == "" || peak_to_peak < peak_diference)
            {
                zapis = 0;
                nazwa = "Pomiar" + time.Replace(":", "'");
                first_save = 1;
            }
            else
            {
                zapis = 1;
                if (first_save == 1)
                {
                    for (int i = 50; i > 1; i--)
                    {
                        using (System.IO.StreamWriter file = new System.IO.StreamWriter(pathString + "\\" + nazwa + ".txt", true))
                        {
                            wynik = "Pomiar:" + j + "ch1: " + chanel1Array[0, chanel1Array.GetLength(1) - i] + ";ch2:" + chanel1Array[1, chanel1Array.GetLength(1) - i] + ";ch3:" + chanel1Array[2, chanel1Array.GetLength(1) - i];
                            file.WriteLine(wynik);
                            j++;

                        }
                    }
                    first_save = 0;
                }

            }

            zmien_label(zapis);


            if (zapis == 1)
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(pathString + "\\" + nazwa + ".txt", true))
                {
                    wynik = "Pomiar:" + j + "ch1: " + chanel1Array[0, chanel1Array.GetLength(1) - 1] + ";ch2:" + chanel1Array[1, chanel1Array.GetLength(1) - 1] + ";ch3:" + chanel1Array[2, chanel1Array.GetLength(1) - 1];
                    file.WriteLine(wynik);
                    j++;

                }

            }
        }
        //########WINDOW CHANGES#######// 
        public void zmien_richTextBox1(string txt)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action<string>(zmien_richTextBox1), new object[] { txt });
                return;
            }
            richTextBox1.AppendText(txt);
            
        }

        public void zmien_label1(string txt)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action<string>(zmien_label1), new object[] { txt });
                return;
            }
            label2.Text = txt;

        }

        public void zmien_label(int txt)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action<int>(zmien_label), new object[] { txt });
                return;
            }
            if (txt == 1)
            {
                label2.Text = "ZAPIS";
                label2.ForeColor = System.Drawing.Color.Lime;
            }
            else
            {
                label2.Text = "OCZEKUJE";
                label2.ForeColor = System.Drawing.Color.Yellow;
            }
        }


        private void UpdateChart()
        {
            chart1.Series["Series1"].Points.Clear();
            if (comboBox1.SelectedItem.ToString() == "ch1")
            {
                ChartArray = copy(0, chanel1Array);
                chart1.Series["Series1"].Color = Color.Red;
            }

            if (comboBox1.SelectedItem.ToString() == "ch2")
            {
                ChartArray = copy(1, chanel1Array);
                chart1.Series["Series1"].Color = Color.Blue;
            }
            if (comboBox1.SelectedItem.ToString() == "ch3")
            {
                ChartArray = copy(2, chanel1Array);
                chart1.Series["Series1"].Color = Color.Green;
            }
            for (int i = 0; i < ChartArray.Length - 1; ++i)
            {
                chart1.Series["Series1"].Points.AddY(ChartArray[i]);
            }
        }
        //#######DEVICE CONFIGURATION######//

        private void Set_Idle()
        {
            try
            {
                zmien_richTextBox1(" Set to Idle");

                mscl.SetToIdleStatus status = node.setToIdle();

                while (!status.complete(300))
                { Console.Write("."); }

                switch (status.result())
                {
                    case mscl.SetToIdleStatus.SetToIdleResult.setToIdleResult_success:
                        break;

                    case mscl.SetToIdleStatus.SetToIdleResult.setToIdleResult_canceled:
                        break;
                    case mscl.SetToIdleStatus.SetToIdleResult.setToIdleResult_failed:
                    default:
                        break;
                }
            }
            catch (mscl.Error err)
            {
                zmien_richTextBox1("Error: " + err.Message);
            }
        }
        private void create_conection()
        {
            //create a Serial Connection with the specified COM Port, default baud rate of 921600
            connection = mscl.Connection.Serial(ports[0]);

            //create a BaseStation with the connection
            baseStation = new mscl.BaseStation(connection);

            //create a WirelessNode with the BaseStation we created
            node = new mscl.WirelessNode(NODE_ADDRESS, baseStation);
            node_set = 1;

            Set_Idle();
            //create a SyncSamplingNetwork object, giving it the BaseStation that will be the master BaseStation for the network
            mscl.SyncSamplingNetwork network = new mscl.SyncSamplingNetwork(baseStation);

            //add a WirelessNode to the network
            //	Note: The Node must already be configured for Sync Sampling before adding to the network, or else Error_InvalidConfig will be thrown.
            //TODO: Repeat this for all WirelessNodes that you want in the network
            zmien_richTextBox1("Adding node to the network..");

            network.addNode(node);

            //can get information about the network
            zmien_richTextBox1("\n Network info: ");
            zmien_richTextBox1("\n Network OK: " + network.ok().ToString());
            zmien_richTextBox1("\n Percent of Bandwidth: " + network.percentBandwidth().ToString() + "%");
            zmien_richTextBox1("\n Lossless Enabled: " + network.lossless().ToString());
            zmien_richTextBox1("\n High Capacity Mode: " + network.highCapacity().ToString());

            //apply the network configuration to every node in the network
            zmien_richTextBox1("\n Applying network configuration...");
            network.applyConfiguration();
            zmien_richTextBox1(" \n Done.");

            //start all the nodes in the network sampling. The master BaseStation's beacon will be enabled with the system time.
            //	Note: if you wish to provide your own start time (not use the system time), pass a mscl::Timestamp object as a second parameter to this function.
            //	Note: if you do not want to enable a beacon at this time, use the startSampling_noBeacon() function. (A beacon is required for the nodes to actually start sending data).
            zmien_richTextBox1(" \n Starting the network...");
            network.startSampling();
            zmien_richTextBox1(" \n Done.");
        }
        private void data_from_sweeps()
        {
             mscl.DataSweeps sweeps = baseStation.getData(10);

                        foreach (mscl.DataSweep sweep in sweeps)
                        {
                            
                            mscl.ChannelData data = sweep.data();
                            chanel1Array = FIFO(chanel1Array);
                            foreach (mscl.WirelessDataPoint dataPoint in data)
                            {
                                string chanelname = dataPoint.channelName();
                                char chanel_char = chanelname[2];
                                Int16 chanel = Int16.Parse("0"+chanel_char); 
                                chanel1Array[chanel - 1 , chanel1Array.GetLength(1) - 1] = dataPoint.as_double();
                                
                                
                             }
                            Peak_check();
                        }
        }

        // ##### MEASUREMENT ##### //

        public void getData()
        {

            int second = 0;


            while (OK == 0) { }
            Thread.Sleep(100);
            chanel1Array = new double[3,buffor_length];
            
            while (ports.Length == 0)
            {
                ports = SerialPort.GetPortNames();
                zmien_label1("Podłacz urządzenie");
                Thread.Sleep(1000);
                
            }
            
                try
                {
                    create_conection();

                    while (true)
                    {
                        data_from_sweeps();
                        
                            if (second > chart_freq)
                            {
                                second = 0;
                                if (chart1.IsHandleCreated)
                                {
                                    this.Invoke((MethodInvoker)delegate { UpdateChart(); });
                                }
                            }
                            else { second++; }                         
                        }
                    }
                
                catch (mscl.Error er)
                {
                    zmien_richTextBox1("Error: " + er.Message);
                    error = er.Message;
                }
            

            
            }

      // #### FOLDER CREATOR #### //
        
        private void TimeHandler()
        {
            int actual_buffor_length = 0;
            while (true)
            {
              if (buffor_length != actual_buffor_length)
                {
                    actual_buffor_length = buffor_length;
                    chanel1Array = new double[3,buffor_length];
                }

              Thread.Sleep(1000);
              datetime = DateTime.Now.ToString();
              string[] data = datetime.Split(new string[] { " " }, StringSplitOptions.None);
              time = data[1];

                
              string[] hour = data[1].Split(new string[] { ":" }, StringSplitOptions.None);
              if (today != data[0])
                {

                    while (folder == "") { }
                    pathString = System.IO.Path.Combine(folder, data[0]);
                    System.IO.Directory.CreateDirectory(pathString);
                    today = data[0]; 
              }
                
                
           
                
            }
        }

        // #### WINDOW EVENTS #### //
      
        private void Form1_Load(object sender, EventArgs e)
        {
            string[] chanels = { "ch1", "ch2", "ch3"};
            foreach (string chanel in chanels)
            {
                comboBox1.Items.Add(chanel);

            }
            comboBox1.SelectedItem = comboBox1.Items[0];

            
            
            Time = new Thread(new ThreadStart(this.TimeHandler));
            Time.IsBackground = true;
            Time.Start();

            DataThread = new Thread(getData);
            DataThread.IsBackground = true;
            DataThread.Start();


        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            OknoKonfiguracji window = new OknoKonfiguracji();
            window.Show();
        }

      

        private void Visualization_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (node_set == 1)
            { Set_Idle(); }     
        }

        private void Visualization_Shown(object sender, EventArgs e)
        {
            window.Show();
        }
     }

    }

