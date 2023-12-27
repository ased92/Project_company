using Dapper;
using FirebirdSql.Data.FirebirdClient;
using Org.BouncyCastle.Utilities.Encoders;
using System;
using System.Collections;
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
    public partial class accountant_form : Form
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
        public accountant_form()
        {
            InitializeComponent();
            dataGridView1.SelectionChanged += dataGridView1_SelectionChanged;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string name = textBox1.Text.Trim(); // Получаем введенное пользователем имя из textBox1
            string address = textBox2.Text.Trim();
            if (string.IsNullOrEmpty(name) && string.IsNullOrEmpty(address))
            {
                MessageBox.Show("Ошибка.Введите ФИО или адрес для поиска.");
                return;
            }

            // Поиск абонента по адресу или ФИО
            DataTable searchResult = SearchAbonent(name, address);

            // Очистка DataGridView
            dataGridView1.DataSource = null;
            dataGridView1.Columns.Clear();

            // Загрузка результатов поиска в DataGridView
            dataGridView1.DataSource = searchResult;
            dataGridView1.Columns[0].HeaderText = "ФИО";
            dataGridView1.Columns[1].HeaderText = "Телефон";
            dataGridView1.Columns[2].HeaderText = "Баланс";
            dataGridView1.Columns[3].HeaderText = "Адрес";


            dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView1.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView1.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView1.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

        }

        private DataTable SearchAbonent(string name, string address)
        {
            
            FbConnection link = linkFb();
            DataTable searchResult = new DataTable();
            FbCommand com=null;

            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(address))
            {
                com = new FbCommand("SELECT ABONENTI.ABONENT, ABONENTI.TELEFON, BALANS.BALANS, ABONENTI.ADRES FROM ABONENTI JOIN BALANS ON ABONENTI.ID_BAL = BALANS.ID_BAL WHERE ABONENTI.ABONENT = @name AND ABONENTI.ADRES = @address", link);
                com.Parameters.AddWithValue("name", name);
                com.Parameters.AddWithValue("address", address);
            }
            else if (!string.IsNullOrEmpty(name))
            {
                com = new FbCommand("SELECT ABONENTI.ABONENT, ABONENTI.TELEFON, BALANS.BALANS, ABONENTI.ADRES FROM ABONENTI JOIN BALANS ON ABONENTI.ID_BAL = BALANS.ID_BAL WHERE ABONENTI.ABONENT = @name", link);
                com.Parameters.AddWithValue("name", name);
            }
            else if (!string.IsNullOrEmpty(address))
            {
                com = new FbCommand("SELECT ABONENTI.ABONENT, ABONENTI.TELEFON, BALANS.BALANS, ABONENTI.ADRES FROM ABONENTI JOIN BALANS ON ABONENTI.ID_BAL = BALANS.ID_BAL WHERE ABONENTI.ADRES = @address", link);
                com.Parameters.AddWithValue("address", address);
            }
            
           
            com.CommandType = CommandType.Text;
            FbDataReader read = com.ExecuteReader();
            searchResult.Load(read);

            return searchResult;
        }

        private void button3_Click(object sender, EventArgs e)
        {
                int count = this.dataGridView1.Columns.Count;
                for (int i = 0; i < count; i++)
                {
                    this.dataGridView1.Columns.RemoveAt(0);
                }
                FbConnection link = linkFb(); 
                int n = link.Query<int>("select count(ID_AB) from ABONENTI").Single();
                DataTable linkDB = new DataTable();
                FbCommand com = new FbCommand("select a.ABONENT, a.TELEFON, b.BALANS ,a.ADRES from ABONENTI a " + "inner join BALANS b on b.ID_BAL=a.ID_BAL ", link);
                com.CommandType = CommandType.Text;
                FbDataReader read = com.ExecuteReader();
                linkDB.Load(read);
                dataGridView1.DataSource = linkDB;
                dataGridView1.Columns[0].HeaderText = "ФИО";
                dataGridView1.Columns[1].HeaderText = "Телефон";
                dataGridView1.Columns[2].HeaderText = "Баланс";
                dataGridView1.Columns[3].HeaderText = "Адрес";


            dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView1.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView1.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView1.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                // Получаем выбранную строку
                DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];

                // Получаем значения из выбранной строки
                string abonent = selectedRow.Cells["ABONENT"].Value.ToString();
                float newBalance = Convert.ToSingle(textBox3.Text);

                // Находим id_bal по имени абонента
                int balanceId = FindBalanceId(abonent);
                if (balanceId != -1)
                {
                    // Обновляем баланс в DataGridView
                    selectedRow.Cells[2].Value = newBalance;

                    // Обновляем баланс в базе данных
                    UpdateBalanceInDatabase(balanceId, newBalance);
                }
                
            }
            else
            {
                // Вывод ошибки — текстовое поле пустое
                MessageBox.Show("Ошибка: Выберите абонента.");
            }
        }

        private int FindBalanceId(string abonent)
        {
            int balanceId = -1;

            string query = "SELECT ID_BAL FROM ABONENTI WHERE ABONENT = @Abonent";

            using (FbConnection connection = linkFb())
            {
                using (FbCommand command = new FbCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Abonent", abonent);

                    using (FbDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            balanceId = Convert.ToInt32(reader["ID_BAL"]);
                        }
                    }
                }
            }

            return balanceId;
        }

        private void UpdateBalanceInDatabase(int balanceId, float newBalance)
        {
            string query = "UPDATE BALANS SET BALANS = @NewBalance WHERE ID_BAL = @BalanceId";

            using (FbConnection connection = linkFb())
            {
                using (FbCommand command = new FbCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@NewBalance", newBalance);
                    command.Parameters.AddWithValue("@BalanceId", balanceId);

                    command.ExecuteNonQuery();
                }
            }
        }


        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            
        }
        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            

            if (dataGridView1.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];
                string balance = selectedRow.Cells[2].Value.ToString();              
                textBox3.Text = balance;
            }
            else
            {               
                textBox3.Clear();
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void accountant_form_Load(object sender, EventArgs e)
        {

        }
    }
}
