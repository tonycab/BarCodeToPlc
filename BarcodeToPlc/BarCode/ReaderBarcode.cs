using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Management;
using System.ComponentModel;
using System.Threading;

namespace BarcodeToPlc.BarCode
{

    public class Port
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public Port(string name, string description)
        {
            Name = name;
            Description = description;
        }

        public Port(Port port)
        {
            Name = port.Name;
            Description = port.Description;
        }

    }

    public class PortEventArgs : EventArgs
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public PortEventArgs(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }

    public class ReaderBarcode : SerialPort
    {

        #region Propriete

        private static ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity WHERE Caption like '%(COM%'");

        public static Dictionary<String, String> DictArduinoList = new Dictionary<String, String>();
        #endregion

        #region Evennement
        public delegate void PortsEventHandler(PortEventArgs args);
        public static event PortsEventHandler EventAddReader;
        public static event PortsEventHandler EventRemoveReader;
        #endregion

        #region Constructeur
        public ReaderBarcode() : base() { }
        public ReaderBarcode(string comName, int baudRate) : base(comName, baudRate) { }
        #endregion

        

        /// <summary>
        /// Recherhe les ports com
        /// </summary>
        public static void DiscoveryReaderBarCode()
        {

            string[] portnames = SerialPort.GetPortNames();

            var ports = searcher.Get();


            Dictionary<String, String> DictArduinoDiscovery = new Dictionary<String, String>();

            //Creer un dictionnaire avec les ports com détectés
            foreach (var port in ports)
            {
                if (!DictArduinoDiscovery.ContainsKey(port.Properties["DeviceID"].Value.ToString()))
                {
                    DictArduinoDiscovery.Add(port.Properties["DeviceID"].Value.ToString(), port.Properties["Caption"].Value.ToString());
                }
            }

            //Recherche de nouveau port com
            foreach (var item in new Dictionary<String, String>(DictArduinoDiscovery))
            {
                if (!DictArduinoList.ContainsKey(item.Key))
                {
                    DictArduinoList.Add(item.Key, item.Value);
                    EventAddReader?.Invoke(new PortEventArgs(item.Key, item.Value));
                }
            }

            //Recherche de port com plus détecté
            foreach (var item in new Dictionary<String, String>(DictArduinoList))
            {
                if (!DictArduinoDiscovery.ContainsKey(item.Key))
                {
                    DictArduinoList.Remove(item.Key);
                    EventRemoveReader?.Invoke(new PortEventArgs(item.Key, item.Value));
                }
            }
            Thread.Sleep(1000);
        }

    }


}