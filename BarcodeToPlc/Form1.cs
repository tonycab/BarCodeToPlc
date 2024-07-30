using Sharp7;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using BarcodeToPlc.BarCode;
using BarcodeToPlc.Parametrage;


namespace BarcodeToPlc
{
    public partial class Form1 : Form
    {

        ReaderBarcode Reader = new ReaderBarcode();

        BindingList<Port> listPort = new BindingList<Port>();

        Dictionary<string, Port> DictListPort = new Dictionary<string, Port>();

        Task DiscoveryReaderBarcode;

        bool Stop = true;

        ParamsApp p;

        string appDataPathFull;

        LoggerApp logger = LoggerApp.Factory();


        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            logger.eventlog += AffichageLogs;

            //Cache la fenetre
            this.WindowState = FormWindowState.Minimized;

            var appDataPath = System.Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            appDataPathFull = appDataPath + "\\SIIF\\BarcodeToPlc\\";
            
            logger.Info("STARTING APPLICATION");

            try { 

            if (!Directory.Exists(appDataPathFull))
            {
                Directory.CreateDirectory(appDataPathFull);
            }

            //Parametres de l'application
            if (!File.Exists(appDataPathFull + "Config.xml"))
            {
                p = new ParamsApp();
                p.AdressIP = "192.32.98.50";
                p.DBnumber = 5;
                p.CodeLenght = 12;
                p.RegexFilter = "^([A-Z]{3}_[0-9]{5}-[0-9]{2}){1}";
                p.SaveFromXml(appDataPathFull + "Config.xml");
            }

            //Chargement de parametres de l'application
            p = ParamsApp.LoadToXml(appDataPathFull + "Config.xml");

        


            UpdateParams(p, null);
            p.ChangParams += UpdateParams;

            //Recherche des ports Reader
            ReaderBarcode.EventAddReader += AddDeviceBarCode;
            ReaderBarcode.EventRemoveReader += RemoveDeviceBarCode;

            Reader.DataReceived += dataRecieve;



            //Task de recherche de port com pour lecteur code barre
            DiscoveryReaderBarcode = Task.Run(() => { while (Stop) { ReaderBarcode.DiscoveryReaderBarCode(); } });

        }catch(Exception ex)
            {
				logger.Error(ex.Message);
			}


        }

        private void AffichageLogs(DateTime arg1, LoggerApp.LogLevel arg2, string arg3)
        {
			String line;
			try
			{
				StreamWriter sw = new StreamWriter(appDataPathFull + "Logs.txt",true);
			
				sw.WriteLine($"{arg1.ToString().PadRight(16)} | {arg2.ToString().PadRight(10)} | {arg3}");

				//Close the file
				sw.Close();
			}
			catch (Exception e)
			{
				Console.WriteLine("Exception: " + e.Message);
			}
			finally
			{
				Console.WriteLine("Executing finally block.");
			}
		}

        private void UpdateParams(object sender, EventArgs e)
        {
            label2.Invoke(new Action(() => label2.Text = $"PLC : {p.AdressIP} | DB : {p.DBnumber}"));
        }



        //Appeler lorsqu'un nouveau port serie est détecté

        private void AddDeviceBarCode(PortEventArgs args)
        {
            try { 
            Console.WriteLine("new device :{0} - {1} ", args.Name, args.Description);

            if (args.Description.Contains("Barcode Scanner"))
            {

                Match ComPort = Regex.Match(args.Description, @"\bCOM\d+\b");

                Console.WriteLine("Connect to :{0} - {1}  - Port : {2}", args.Name, args.Description, ComPort.Value);


                textBox1.Invoke(new Action(() => textBox1.Text = args.Description));

                if (!Reader.IsOpen)
                {
                    Reader.PortName = ComPort.Value;
                    Reader.BaudRate = 9600;
                    Reader.Open();
                }
            }}catch(Exception e)
            {
                Console.WriteLine("Execption :{0} ", e);

                logger.Error(e.Message);
            }

        }

        //Appeler lorqu'un port serie n'est plus détécté
        private void RemoveDeviceBarCode(PortEventArgs args)
        {
            try { 
            Console.WriteLine("remove device :{0} - {1} ", args.Name, args.Description);

            if (textBox1.Text == args.Description)
            {
                textBox1.Invoke(new Action(() => textBox1.Text = ""));

                if (Reader.IsOpen)
                {
                    Reader.Close();
                }
            }
            }catch(Exception e) { 

                Console.WriteLine("Execption :{0} ", e);

                logger.Error(e.Message);
            }
        }



        private void dataRecieve(object sender, SerialDataReceivedEventArgs e)
        {

            try { 

            SerialPort DataCodeBarre = (SerialPort)sender;

            bool valid = true;

            var CodeBarre = DataCodeBarre.ReadLine();
            CodeBarre = CodeBarre.Replace("\r", "");

            ListViewItem item = new ListViewItem(DateTime.Now.ToString());

            item.SubItems.Add(CodeBarre);

            //Controle du code barre
            if (p.RegexFilter != "")
            {
                try
                {
                    Match r = Regex.Match(CodeBarre, p.RegexFilter);

                    if (r.Success)
                    {
                        item.SubItems.Add("OK");
                    }
                    else
                    {
                        item.SubItems.Add("NOK");
                        valid = false;
                    }
                }
                catch(Exception ex)
                {

                    logger.Error(ex.Message);
                    valid = false;
                    item.SubItems.Add("REGEX INVALID");

                }
            }
            else
            {
                item.SubItems.Add("OK");
            }


            //Ajoute a la liste view le code barre lu
            listView1.Invoke((new Action(() =>
            {
                listView1.Items.Add(item);
                listView1.EnsureVisible(listView1.Items.Count - 1);
                if (listView1.Items.Count > 20) listView1.Items.Remove(listView1.Items[0]);
            }
            )));
     

                //Connection au PLC
                var client = new S7Client();
                int connectionResult = client.ConnectTo(p.AdressIP, 0, 1);
   

            //Connection établie
            if (connectionResult == 0)
            {

                //Ajout du code barre dans le buffer
                byte[] buffer = new byte[p.CodeLenght + 13 + 2];

                //AJout le code barre
                buffer.SetStringAt(0, 12, CodeBarre);

                //Ajout de la date dans le buffer
                buffer.SetDTLAt(14, DateTime.Now);

                //Ajout de la validité du code barre dans le buffer
                buffer.SetBitAt(26, 0, valid);

                Console.WriteLine("Send to plc :{0} ", Encoding.ASCII.GetString(buffer));

                try
                {
                    //Ecriture dans le DB du PLC
                    client.DBWrite(p.DBnumber, 0, buffer.Length, buffer);

                    //Affichage d'envoi réussi
                    Task.Run(() =>
                    {

                        if (valid) { beeper(1, DataCodeBarre); } else { beeper(3, DataCodeBarre); };

                        label1.Invoke((Action)(() => { label1.BackColor = Color.Green; }));
                        Thread.Sleep(1000);
                        label1.Invoke((Action)(() => { label1.BackColor = SystemColors.Control; }));

                    });
                }
                catch (Exception ex)
                {

                    Task.Run(() =>
                    {

                        beeper(5, DataCodeBarre);

                        label1.Invoke((Action)(() => { label1.BackColor = Color.Red; }));
                        Thread.Sleep(1000);
                        label1.Invoke((Action)(() => { label1.BackColor = SystemColors.Control; }));

                    });


                    Console.WriteLine("Execption :{0} ", ex);
                    logger.Error(ex.Message);
                    }


            }
            else
            {
                Task.Run(() =>
                {

                    beeper(5, DataCodeBarre);

                    label1.Invoke((Action)(() => { label1.BackColor = Color.Red; }));
                    Thread.Sleep(1000);

                }

                    );

            }

            client.Disconnect();

            }
            catch (Exception ex)
            {
                Console.WriteLine("Execption :{0} ", ex);
                logger.Error(ex.Message);
            }
        }


        public void beeper(int number, SerialPort reader)
        {

            Task.Run(() =>
            {
                try
                {
                    Thread.Sleep(100);

                    for (int i = 0; i < number; i++)
                    {
                        Thread.Sleep(200);

                        reader.Write(new byte[] { 0x07 }, 0, 1);
                    }
                }
                catch (Exception ex)
                {

                    Console.WriteLine("Execption :{0} ", ex);
                    logger.Error(ex.Message);
                }

            });
        }



        private  void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
			logger.Info("CLOSE APPLICATION");
			Stop = false;
           DiscoveryReaderBarcode.Wait();
			logger.Info("STOP DISCOVERY COM");

            
		}

        private void quitterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }



        private void Form1_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                Hide();
                notifyIcon1.Visible = true;
            }
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            Show();
            this.WindowState = FormWindowState.Normal;
            notifyIcon1.Visible = false;
        }

        private void aProposToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form a = new About();
            a.ShowDialog();
        }

        private void configurationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form edit = new Edit(p);
            DialogResult r = edit.ShowDialog();
            if (r == DialogResult.OK)
            {
                p.SaveFromXml(appDataPathFull + "Config.xml");
            }
        }

        private void fichierToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
