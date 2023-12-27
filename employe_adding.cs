using Dapper;
using FirebirdSql.Data.FirebirdClient;
using Org.BouncyCastle.Crypto.Digests;
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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace EleCom
{
    public partial class employe_adding : Form
    {
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
        public employe_adding()
        {
            InitializeComponent();
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }
        private void textBox5_TextChanged(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if ((textBox1.Text != "") & (textBox2.Text != "") & (textBox3.Text != "") & (textBox4.Text != "") & (textBox5.Text != "") &(textBox6.Text != ""))
            {
                string dataToHash = textBox5.Text;
                var data = System.Text.Encoding.UTF8.GetBytes(dataToHash);
                MD5Digest hash = new MD5Digest();
                hash.BlockUpdate(data, 0, data.Length);
                byte[] result = new byte[hash.GetDigestSize()];
                hash.DoFinal(result, 0);
                FbConnection link = linkFb();
                FbTransaction addDetailsTransaction = link.BeginTransaction();

                string SQLCommandText = "INSERT INTO ROLES(LOGIN,PASSWORD,NUMBER_OF_ROLES) VALUES ('" + textBox4.Text + "','" + Hex.ToHexString(result) + "','" + textBox6.Text + "')";
                FbCommand addDetailsCommand = new FbCommand(SQLCommandText, link, addDetailsTransaction);
                addDetailsCommand.ExecuteNonQuery();
                addDetailsTransaction.Commit();

                int ID_ROLE = Convert.ToInt32(link.Query<String>("select MAX(ID_ROLE) from ROLES").Single());

                FbTransaction addDetailsTransaction2 = link.BeginTransaction();
                SQLCommandText = "INSERT INTO SOTRUDNIKI(SOTRUDNIK,TELEFON,ROLE,ID_ROLE) VALUES ('" + textBox1.Text + "','" + textBox2.Text + "','" + textBox3.Text + "','" + ID_ROLE + "')";
                addDetailsCommand = new FbCommand(SQLCommandText, link, addDetailsTransaction2);
                addDetailsCommand.ExecuteNonQuery();
                addDetailsTransaction2.Commit();
                this.Close();
            }
            else { MessageBox.Show("Не все поля заполнены"); }
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void employe_adding_Load(object sender, EventArgs e)
        {

        }
    }
}
