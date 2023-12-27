using Dapper;
using FirebirdSql.Data.FirebirdClient;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Utilities.Collections;
using Org.BouncyCastle.Utilities.Encoders;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Windows.Forms.LinkLabel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace EleCom
{
    public partial class Form2 : Form
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
        public Form2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
                if ((textBox1.Text != "") & (textBox2.Text != "") & (textBox3.Text != "") & (textBox4.Text != "") & (textBox7.Text != ""))
                {
                    string dataToHash = textBox6.Text;
                    var data = System.Text.Encoding.UTF8.GetBytes(dataToHash);
                    MD5Digest hash = new MD5Digest();
                    hash.BlockUpdate(data, 0, data.Length);
                    byte[] result = new byte[hash.GetDigestSize()];
                    hash.DoFinal(result, 0);
                    FbConnection link = linkFb();

                    FbTransaction addDetailsTransaction = link.BeginTransaction();

                    string SQLCommandText = "INSERT INTO ROLES(LOGIN,PASSWORD,NUMBER_OF_ROLES) " +
                    "VALUES ('" + textBox5.Text + "','" + Hex.ToHexString(result) + "','" + textBox7.Text + "')";

                    FbCommand addDetailsCommand = new FbCommand(SQLCommandText, link, addDetailsTransaction);
                    addDetailsCommand.ExecuteNonQuery();
                    addDetailsTransaction.Commit();

                    int ID_ROLE = Convert.ToInt32(link.Query<String>("select MAX(ID_ROLE) from ROLES").Single());

                    FbTransaction addDetailsTransaction1 = link.BeginTransaction();
                    SQLCommandText = "INSERT INTO BALANS(BALANS) VALUES ('" + textBox8.Text +  "')";
                    addDetailsCommand = new FbCommand(SQLCommandText, link, addDetailsTransaction1);
                    addDetailsCommand.ExecuteNonQuery();
                    addDetailsTransaction1.Commit();

                    int ID_BAL = Convert.ToInt32(link.Query<String>("select MAX(ID_BAL) from BALANS").Single());

                    FbTransaction addDetailsTransaction2 = link.BeginTransaction();
                    SQLCommandText = "INSERT INTO ABONENTI(LICEVOY_SCHET,ABONENT,TELEFON,ADRES,ID_ROLE,ID_BAL) VALUES ('" + textBox1.Text + "','" + textBox2.Text + "','" + textBox3.Text + "','" + textBox4.Text + "','" + ID_ROLE + "','" + ID_BAL + "')";
                    addDetailsCommand = new FbCommand(SQLCommandText, link, addDetailsTransaction2);
                    addDetailsCommand.ExecuteNonQuery();
                    addDetailsTransaction2.Commit();
                    this.Close();
                }
            else { MessageBox.Show("Не все поля заполнены"); }
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox8_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox9_TextChanged(object sender, EventArgs e)
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

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
