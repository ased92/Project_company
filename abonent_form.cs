using Dapper;
using FirebirdSql.Data.FirebirdClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.LinkLabel;

namespace EleCom
{
    public partial class abonent_form : Form
    {
        private string login;

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
        public abonent_form(string loginValue)
        {
            InitializeComponent();
            textBox1.Enabled = false;
            textBox2.Enabled = false;
            textBox3.Enabled = false;
            login = loginValue;
            inform();
        }

        public void abonent_form_Load(object sender, EventArgs e)
        {

        }
        private void inform()
        {
            FbCommand command = new FbCommand($@"
    SELECT ABONENTI.LICEVOY_SCHET, ABONENTI.ABONENT, BALANS.BALANS
    FROM ABONENTI
    JOIN BALANS ON ABONENTI.ID_BAL = BALANS.ID_BAL
    WHERE ABONENTI.ID_ROLE = (SELECT ID_ROLE FROM ROLES WHERE LOGIN = '{login}')", linkFb());
            FbDataAdapter adapter = new FbDataAdapter(command);
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);
            if (dataTable.Rows.Count > 0)
            {

                DataRow row = dataTable.Rows[0];
                // Получите значения полей
                string licevoySchet = row["LICEVOY_SCHET"].ToString();
                string abonent = row["ABONENT"].ToString();
                string balans = row["BALANS"].ToString();
                // Устанавливаем значения в TextBox
                textBox1.Text = $"{licevoySchet}";
                textBox2.Text = $"{abonent}";
                textBox3.Text = $"{balans}";
            }
            else
            {
                textBox1.Text = "Нет данных";
            }

        }
        private void button1_Click(object sender, EventArgs e)
        {

            
            
        }

        private void textBox1_TextChanged_1(object sender, EventArgs e)
        {
            
        }

        private void textBox2_TextChanged_1(object sender, EventArgs e)
        {
            
        }

        private void textBox3_TextChanged_1(object sender, EventArgs e)
        {
            
        }
    }
}
