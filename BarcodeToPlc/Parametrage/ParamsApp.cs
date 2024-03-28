using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace BarcodeToPlc.Parametrage
{
    public class ParamsApp
    {
        private string adressIP;

        private string ipPattern = @"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b";
        private int dBnumber=5;
        private int codeLenght=12;
        private string regexFilter="";

        public event EventHandler ChangParams;

        public string AdressIP
        {
            get
            {
                return adressIP;
            }
            set
            {
                if (value != null)
                {
                    Match r = Regex.Match(value, ipPattern);
                    if (r.Success)
                    {
                        adressIP = value;
                        ChangParams?.Invoke(this, EventArgs.Empty);
                    }
                }
            }
        }

        public int DBnumber
        {
            get
            {
                return dBnumber;
            }
            set
            {
                dBnumber = value;
                ChangParams?.Invoke(this, EventArgs.Empty);
            }
        }

        public int CodeLenght
        {
            get
            {
                return codeLenght;
            }
            set
            {
                codeLenght = value;
                ChangParams?.Invoke(this, EventArgs.Empty);
            }
        }

        public string RegexFilter
        {
            get
            {
                return regexFilter;
            }
            set
            {
                regexFilter = value;
                ChangParams?.Invoke(this, EventArgs.Empty);
            }
        }


        public XElement ToXmlElement()
        {
            XElement xElement = new XElement(nameof(ParamsApp));
            xElement.SetElementValue(nameof(AdressIP), AdressIP);
            xElement.SetElementValue(nameof(DBnumber), DBnumber);
            xElement.SetElementValue(nameof(CodeLenght), CodeLenght);
            xElement.SetElementValue(nameof(RegexFilter), RegexFilter);
            return xElement;
        }

        public static ParamsApp FromXml(XElement xElement)
        {
            ParamsApp p = new ParamsApp();

            xElement = xElement.Element(nameof(ParamsApp));

            p.AdressIP = xElement.Element(nameof(AdressIP))?.Value;
            p.DBnumber = int.Parse(xElement.Element(nameof(DBnumber))?.Value, CultureInfo.InvariantCulture);
            p.CodeLenght = int.Parse(xElement.Element(nameof(CodeLenght))?.Value, CultureInfo.InvariantCulture);
            p.RegexFilter = xElement.Element(nameof(RegexFilter))?.Value;

            return p;

        }

        public void SaveFromXml(string path)
        {
            XElement xmlRoot = new XElement("CodeBarToPlc");
            XDocument xDocument = new XDocument(xmlRoot);

            xmlRoot.Add(this.ToXmlElement());

            xDocument.Save(path);

        }

        public static ParamsApp LoadToXml(string path)
        {
            XDocument xDocument = XDocument.Load(path);

            return FromXml(xDocument.Root);

        }
    }
}
