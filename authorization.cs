using Dapper;
using FirebirdSql.Data.FirebirdClient;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EleCom
{
    public partial class authorization : Form
    {
        public string login;
        public FbConnection linkFb()
        {
            FbConnection.ClearAllPools();
            FbConnectionStringBuilder csb = new FbConnectionStringBuilder();
            csb.DataSource = "localhost";
            csb.Port = 3050;
            csb.Database = @"C:\Users\Admin\Desktop\kursovaya\kursovaya\kursovaya\KKK.FDB";
            csb.UserID = "SYSDBA";
            csb.Password = "masterkey";
            csb.ServerType = FbServerType.Default;
            FbConnection db = new FbConnection(csb.ToString());
            db.Open();
            return (db);

        }
        public authorization()
        {
            InitializeComponent();
        }
        
        private void button1_Click(object sender, EventArgs e)
        {

            string dataToHash = textBox2.Text;
            var data = System.Text.Encoding.UTF8.GetBytes(dataToHash);
            MD5Digest hash = new MD5Digest();
            hash.BlockUpdate(data, 0, data.Length);
            byte[] result = new byte[hash.GetDigestSize()];
            hash.DoFinal(result, 0);
            FbConnection link = linkFb();
            string doc = "";
            string type = "";
            login = textBox1.Text;
            try
            {
                doc = link.Query<String>("select PASSWORD from ROLES where LOGIN='" + textBox1.Text + "'").Single();
                
            }
            catch { }
            if (doc == Hex.ToHexString(result) || doc == "admin" && textBox2.Text != "")
            {
                type = link.Query<String>("select NUMBER_OF_ROLES from ROLES where LOGIN='" + textBox1.Text + "'").Single();
                this.Hide();
                if (type == "3")
                {
                    abonent_form f1 = new abonent_form(login);
                    f1.ShowDialog();
                    this.Close();
                }
                if (type == "2")
                {
                    Controller_form f1 = new Controller_form(login);
                    f1.ShowDialog();
                    this.Close();
                }
                if (type == "4")
                {
                    accountant_form f1 = new accountant_form();
                    f1.ShowDialog();
                    this.Close();
                }
                if (type == "1")
                {
                    adminform f1 = new adminform();
                    f1.ShowDialog();
                    this.Close();
                }
                
            }
            else
            {
                { MessageBox.Show("Неверный логин или пароль"); }
            }

        }


        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void authorization_Load(object sender, EventArgs e)
        {

        }
    }
}
