using Dapper;
using FirebirdSql.Data.FirebirdClient;
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
using static System.Windows.Forms.LinkLabel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace EleCom
{
    public partial class Controller_form : Form
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
        public Controller_form(string loginValue)
        {
            InitializeComponent();
            login = loginValue;
            UpdateTextBoxValue();
            textBox1.Enabled = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FbConnection link = linkFb();

            FbTransaction addDetailsTransaction = link.BeginTransaction();

            DataTable linkDB = new DataTable();
            string SQLCommandText = "INSERT INTO POKAZATELI(POKAZATEL_AB, DATA_AB, ID_AB) " +
                    "VALUES ('" + textBox3.Text + "','" + textBox5.Text + "','" + textBox8.Text + "')";
            FbCommand addDetailsCommand = new FbCommand(SQLCommandText, link, addDetailsTransaction);
            addDetailsCommand.ExecuteNonQuery();
            addDetailsTransaction.Commit();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            int count = this.dataGridView1.Columns.Count;
            for (int i = 0; i < count; i++)
            {
                this.dataGridView1.Columns.RemoveAt(0);
            }
            FbConnection link = linkFb(); 
            DataTable linkDB = new DataTable();
            FbCommand com = new FbCommand("select ABONENTI.ID_AB, ADRES, ABONENT, POKAZATEL_AB, DATA_AB from" +
                " ABONENTI INNER JOIN POKAZATELI ON POKAZATELI.ID_AB = ABONENTI.ID_AB", link);
            com.CommandType = CommandType.Text;
            FbDataReader read = com.ExecuteReader();
            linkDB.Load(read);
            dataGridView1.DataSource = linkDB;
            dataGridView1.Columns[0].HeaderText = "ID_Абонента ";
            dataGridView1.Columns[1].HeaderText = "Адресс ";
            dataGridView1.Columns[2].HeaderText = "Абонент ";
            dataGridView1.Columns[3].HeaderText = "Показания_АБ ";
            dataGridView1.Columns[4].HeaderText = "Дата внесения";
            dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView1.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView1.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView1.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView1.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            
        }
        private void UpdateTextBoxValue()
        {
            FbConnection link = linkFb();
            // Создаем команду для выполнения запроса к базе данных

            FbCommand command = new FbCommand($@"
            SELECT SOTRUDNIKI.SOTRUDNIK
            FROM SOTRUDNIKI
            JOIN ROLES ON SOTRUDNIKI.ID_ROLE = ROLES.ID_ROLE
            WHERE ROLES.LOGIN =  '{login}'", linkFb());
            {
                // Добавляем параметр для защиты от SQL-инъекций
                command.Parameters.Add("@Login", FbDbType.VarChar).Value = login;

                // Выполняем запрос и получаем результат
                using (FbDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        // Получаем значение имени сотрудника из столбца "SOTRUDNIK"
                        string SOTRUDNIK = reader.GetString(0); // Исправление здесь
                        textBox1.Text = SOTRUDNIK;
                    }
                    else
                    {
                        // Если запись не найдена, очищаем текстовое поле
                        textBox1.Text = string.Empty;
                    }
                }
            }
        }

        private void Controller_form_Load(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
