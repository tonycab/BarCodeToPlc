using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using BarcodeToPlc.Parametrage;

namespace BarcodeToPlc
{
    public partial class Edit : Form
    {
        public Edit()
        {
            InitializeComponent();

            this.Text = "Modification paramètres";

            Params = new ParamsApp();

        }


        private ParamsApp Params;
        public Edit(ParamsApp paramsApp)
        {

            InitializeComponent();

            this.Text = "Modification paramètres";

            Params = paramsApp;

            textBox1.Text = Params.AdressIP;
            textBox2.Text = Params.DBnumber.ToString();
            textBox4.Text = Params.CodeLenght.ToString();
            textBox3.Text = Params.RegexFilter;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Params.AdressIP = textBox1.Text;
            Params.DBnumber = int.Parse(textBox2.Text);
            Params.CodeLenght = int.Parse(textBox4.Text);
            Params.RegexFilter = textBox3.Text;

            this.DialogResult = DialogResult.OK;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void textBox1_Validating(object sender, CancelEventArgs e)
        {

            string ipPattern = @"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b";

            Match r = Regex.Match(textBox1.Text, ipPattern);

            if (!r.Success)
            {
                e.Cancel = true;
                textBox1.Focus();
                errorProvider1.SetError(textBox1, "Adresse IP incorrect");
            }
            else
            {
                e.Cancel = false;
                errorProvider1.SetError(textBox1, "");
            }

        }

        private void textBox2_Validating(object sender, CancelEventArgs e)
        {
            int dbn;

            if (!int.TryParse(textBox2.Text, out dbn) | dbn < 1)
            {
                e.Cancel = true;
                textBox2.Focus();
                errorProvider1.SetError(textBox2, "Numéro de db incorrect");
            }
            else
            {
                e.Cancel = false;
                errorProvider1.SetError(textBox2, "");
            }

        }


        Regex r;
        private void textBox3_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                if (textBox3.Text != "")
                {
                    r = new Regex(textBox3.Text);
                    e.Cancel = false;
                    errorProvider1.SetError(textBox3, "");
                }
            }
            catch (Exception ex)
            {
                e.Cancel = true;
                textBox2.Focus();
                errorProvider1.SetError(textBox3, "regex incorrect");
            }
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void textBox4_Validating(object sender, CancelEventArgs e)
        {

            int CodeLenght;

            if (!int.TryParse(textBox4.Text, out CodeLenght) | CodeLenght < 8 | CodeLenght > 16)
            {
                e.Cancel = true;
                textBox4.Focus();
                errorProvider1.SetError(textBox4, "Nombre incorrect");
            }
            else
            {
                e.Cancel = false;
                errorProvider1.SetError(textBox4, "");
            }

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }
    }
}

