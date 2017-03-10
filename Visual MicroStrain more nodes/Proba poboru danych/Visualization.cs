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
        private double[,] Node1Array = new double[3,1000];
        private double[,] Node2Array = new double[3, 1000];
        private double[,] Node3Array = new double[3, 1000];
        private double[,] Node4Array = new double[3, 1000];
        private double[,] Node5Array = new double[3, 1000];
        private double[] ChartArray = new double[1000];
        public static int OK = 0, node_set = 0, first_save = 1;
        public static double peak_diference;
        
        public static int chart_points, buffor_length,chart_freq, chanel_auto, node_auto;
        string[] ports = SerialPort.GetPortNames();
        public static string folder = "";
        public static int zapis = 0;
        public static int j =0,j1=0,j2=0,j3=0,j4=0,j5 =0;
        public static string wynik = "";
        string time , datetime , nazwa;
        public static string today;
        public static string pathString = "", timepathString ="";
        mscl.Connection connection;
        mscl.BaseStation baseStation;
        mscl.WirelessNode node;
        mscl.WirelessNode node1;
        mscl.WirelessNode node2;
        mscl.WirelessNode node3;
        mscl.WirelessNode node4;
        mscl.WirelessNode node5;
        //const String COM_PORT = ports[0];
        const int NODE_ADDRESS1 = 873;
        const int NODE_ADDRESS2 = 873;
        const int NODE_ADDRESS3 = 873;
        const int NODE_ADDRESS4 = 873;
        const int NODE_ADDRESS5 = 873;
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
        public void Peak_check(double[,] NodeArray, int actual_node)
        {
            double peak_to_peak = 0;
                switch (node_auto)
                {
                    case 1:
                        peak_to_peak = peaktopeak(chanel_auto - 1, Node1Array);
                        break;
                    case 2:
                        peak_to_peak = peaktopeak(chanel_auto - 1, Node2Array);
                        break;
                    case 3:
                        peak_to_peak = peaktopeak(chanel_auto - 1, Node3Array);
                        break;
                    case 4:
                        peak_to_peak = peaktopeak(chanel_auto - 1, Node4Array);
                        break;
                    case 5:
                        peak_to_peak = peaktopeak(chanel_auto - 1, Node5Array);
                        break;
                    }
            // zwraca wartośc róznicy miedzy najwieksza i najmniejsza wartoscia w tablicy na wybranym kanale
            
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
                    timepathString = System.IO.Path.Combine(pathString, nazwa);
                    System.IO.Directory.CreateDirectory(timepathString);

                    for (int i = 50; i > 0; i--)
                    {
                        using (System.IO.StreamWriter file = new System.IO.StreamWriter(timepathString + "\\" + "Node1" + ".txt", true))
                        {
                            wynik = "Pomiar:" + j1 + "ch1: " + Node1Array[0, Node1Array.GetLength(1) - i] + ";ch2:" + Node1Array[1, Node1Array.GetLength(1) - i] + ";ch3:" + Node1Array[2, Node1Array.GetLength(1) - i];
                            file.WriteLine(wynik);
                            j1++;

                        }

                        using (System.IO.StreamWriter file = new System.IO.StreamWriter(timepathString + "\\" + "Node2" + ".txt", true))
                        {
                            wynik = "Pomiar:" + j2 + "ch1: " + Node2Array[0, Node2Array.GetLength(1) - i] + ";ch2:" + Node2Array[1, Node2Array.GetLength(1) - i] + ";ch3:" + Node2Array[2, Node2Array.GetLength(1) - i];
                            file.WriteLine(wynik);
                            j2++;

                        }

                        using (System.IO.StreamWriter file = new System.IO.StreamWriter(timepathString + "\\" + "Node3" + ".txt", true))
                        {
                            wynik = "Pomiar:" + j3 + "ch1: " + Node3Array[0, Node3Array.GetLength(1) - i] + ";ch2:" + Node3Array[1, Node3Array.GetLength(1) - i] + ";ch3:" + Node3Array[2, Node3Array.GetLength(1) - i];
                            file.WriteLine(wynik);
                            j3++;

                        }

                        using (System.IO.StreamWriter file = new System.IO.StreamWriter(timepathString + "\\" + "Node4" + ".txt", true))
                        {
                            wynik = "Pomiar:" + j4 + "ch1: " + Node4Array[0, Node4Array.GetLength(1) - i] + ";ch2:" + Node4Array[1, Node4Array.GetLength(1) - i] + ";ch3:" + Node4Array[2, Node4Array.GetLength(1) - i];
                            file.WriteLine(wynik);
                            j4++;

                        }

                        using (System.IO.StreamWriter file = new System.IO.StreamWriter(timepathString + "\\" + "Node5"  + ".txt", true))
                        {
                            wynik = "Pomiar:" + j5 + "ch1: " + Node5Array[0, Node5Array.GetLength(1) - i] + ";ch2:" + Node5Array[1, Node5Array.GetLength(1) - i] + ";ch3:" + Node5Array[2, Node5Array.GetLength(1) - i];
                            file.WriteLine(wynik);
                            j5++;

                        }
                    }
                    first_save = 0;
                }

            }

            zmien_label(zapis);


            if (zapis == 1)
            {
                switch (actual_node)
                {
                    case 1:
                        j = j1;
                        j1++;
                        break;
                    case 2:
                        j = j2;
                        j2++;
                        break;
                    case 3:
                        j = j3;
                        j3++;
                        break;
                    case 4:
                        j = j4;
                        j4++;
                        break;
                    case 5:
                        j = j5;
                        j5++;
                        break;
                }
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(timepathString + "\\" + "Node" +actual_node.ToString() + ".txt", true))
                {
                    wynik = "Pomiar:" + j + "ch1: " + NodeArray[0, NodeArray.GetLength(1) - 1] + ";ch2:" + NodeArray[1, NodeArray.GetLength(1) - 1] + ";ch3:" + NodeArray[2, NodeArray.GetLength(1) - 1];
                    file.WriteLine(wynik);
                    

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
            


            string chanelname = comboBox1.Text.ToString();
            string[] chanel_name = chanelname.Split(new string[] { ":" }, StringSplitOptions.None);
            string node = chanel_name[0];
            string chanel = chanel_name[1];
            char node_char = node[4];
            Int16 node_int = Int16.Parse("0" + node_char);
            char chanel_char = chanel[2];
            Int16 chanel_int = Int16.Parse("0" + chanel_char);


            switch (node_int)
            {
                case 1:
                    ChartArray = copy(chanel_int -1, Node1Array);
                    break;
                case 2:
                    ChartArray = copy(chanel_int - 1, Node2Array);
                    break;
                case 3:
                    ChartArray = copy(chanel_int - 1, Node3Array);
                    break;
                case 4:
                    ChartArray = copy(chanel_int - 1, Node4Array);
                    break;
                case 5:
                    ChartArray = copy(chanel_int - 1, Node5Array);
                    break;
            }


            switch (chanel_int)
            {
                case 1:
                    chart1.Series["Series1"].Color = Color.Red;
                    break;
                case 2:
                    chart1.Series["Series1"].Color = Color.Green;
                    break;
                case 3:
                    chart1.Series["Series1"].Color = Color.Blue;
                    break;
                
            }



            for (int i = 0; i < ChartArray.Length - 1; ++i)
            {
                chart1.Series["Series1"].Points.AddY(ChartArray[i]);
            }
        }
        //#######DEVICE CONFIGURATION######//
        int num_node = 1;
        private void Set_Idle()
        {
            do
            {
                switch (num_node)
                {
                    case 1:
                        node = node1;
                        break;
                    case 2:
                        node = node2;
                        break;
                    case 3:
                        node = node3;
                        break;
                    case 4:
                        node = node4;
                        break;
                    case 5:
                        node = node5;
                        break;

                }
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
                num_node++;
            } while (num_node < 6);
        }
        private void create_conection()
        {
            //create a Serial Connection with the specified COM Port, default baud rate of 921600
            connection = mscl.Connection.Serial(ports[0]);

            //create a BaseStation with the connection
            baseStation = new mscl.BaseStation(connection);

            //create a WirelessNode with the BaseStation we created
            node1 = new mscl.WirelessNode(NODE_ADDRESS1, baseStation);
            node2 = new mscl.WirelessNode(NODE_ADDRESS2, baseStation);
            node3 = new mscl.WirelessNode(NODE_ADDRESS3, baseStation);
            node4 = new mscl.WirelessNode(NODE_ADDRESS4, baseStation);
            node5 = new mscl.WirelessNode(NODE_ADDRESS5, baseStation);
            node_set = 1;

            Set_Idle();
            //create a SyncSamplingNetwork object, giving it the BaseStation that will be the master BaseStation for the network
            mscl.SyncSamplingNetwork network = new mscl.SyncSamplingNetwork(baseStation);

            //add a WirelessNode to the network
            //	Note: The Node must already be configured for Sync Sampling before adding to the network, or else Error_InvalidConfig will be thrown.
            //TODO: Repeat this for all WirelessNodes that you want in the network
            zmien_richTextBox1("Adding node to the network..");

            network.addNode(node1);
            network.addNode(node2);
            network.addNode(node3);
            network.addNode(node4);
            network.addNode(node5);

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



        
       // #### SWEEPS ### //
        private void data_from_sweeps()
        {
            int actual_node = 0;
             mscl.DataSweeps sweeps = baseStation.getData(10);

                        foreach (mscl.DataSweep sweep in sweeps)
                        {
                            double[,] NodeArray = new double[3,buffor_length];
                            switch (sweep.nodeAddress())
                            { 
                                case 873:
                                    NodeArray = Node1Array;
                                    actual_node = 1;
                                    break;
                                case 874:
                                    NodeArray = Node2Array;
                                    actual_node = 2;
                                    break;
                                case 875:
                                    NodeArray = Node3Array;
                                    actual_node = 3;
                                    break;
                                case 876:
                                    NodeArray = Node4Array;
                                    actual_node = 4;
                                    break;
                                case 877:
                                    NodeArray = Node5Array;
                                    actual_node = 5;
                                    break;
                            }
                            mscl.ChannelData data = sweep.data();
                            NodeArray = FIFO(NodeArray);
                            foreach (mscl.WirelessDataPoint dataPoint in data)
                            {
                                string chanelname = dataPoint.channelName();
                                char chanel_char = chanelname[2];
                                Int16 chanel = Int16.Parse("0"+chanel_char); 
                                NodeArray[chanel - 1 , NodeArray.GetLength(1) - 1] = dataPoint.as_double();
                                
                                
                             }
                            switch (sweep.nodeAddress())
                            {
                                case 873:
                                    Node1Array = NodeArray;
                                    break;
                                case 874:
                                    Node2Array = NodeArray;
                                    break;
                                case 875:
                                    Node3Array = NodeArray;
                                  
                                    break;
                                case 876:
                                    Node4Array = NodeArray;
                                 
                                    break;
                                case 877:
                                    Node5Array = NodeArray;
                                    
                                    break;
                            }
                            // mamy tablice node arrey z 1 noda z wlasciwymi danymi
                            Peak_check(NodeArray,actual_node);
                        }
        }

        // ##### MEASUREMENT ##### //

        public void getData()
        {

            int second = 0;


            while (OK == 0) { }
            Thread.Sleep(100);
            Node1Array = new double[3, buffor_length];
            Node2Array = new double[3, buffor_length];
            Node3Array = new double[3, buffor_length];
            Node4Array = new double[3, buffor_length];
            Node5Array = new double[3, buffor_length];
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
                        data_from_sweeps();//otrzymywanie danych
                        
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
                    Node1Array = new double[3, buffor_length];
                    Node2Array = new double[3, buffor_length];
                    Node3Array = new double[3, buffor_length];
                    Node4Array = new double[3, buffor_length];
                    Node5Array = new double[3,buffor_length];
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
            string[] chanels = { "Node1:ch1", "Node1:ch2", "Node1:ch3", "Node2:ch1", "Node2:ch2", "Node2:ch3", "Node3:ch1", "Node3:ch2", "Node3:ch3", "Node4:ch1", "Node4:ch2", "Node4:ch3", "Node5:ch1", "Node5:ch2", "Node5:ch3" };
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

