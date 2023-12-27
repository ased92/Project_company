using Dapper;
using FirebirdSql.Data.FirebirdClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EleCom
{
    public partial class adminform : Form
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
        public adminform()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form2 frm = new Form2();
            frm.Show();
        }

        public void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            employe_adding frm = new employe_adding();
            frm.Show();
        }

        public void button5_Click(object sender, EventArgs e)
        {          
            FbConnection link = linkFb(); int n = link.Query<int>("select count(ID_AB) from ABONENTI").Single();
            DataTable linkDB = new DataTable();
            FbCommand com = new FbCommand("select a.ID_AB, a.LICEVOY_SCHET, a.ABONENT, a.TELEFON, a.ADRES, b.BALANS from ABONENTI a " + "inner join BALANS b on b.ID_BAL=a.ID_BAL ", link);
            com.CommandType = CommandType.Text;
            FbDataReader read = com.ExecuteReader();
            linkDB.Load(read);
            dataGridView1.DataSource = linkDB;
            dataGridView1.Columns[0].HeaderText = "ID_AB";
            dataGridView1.Columns[1].HeaderText = "Лицевой счет";
            dataGridView1.Columns[2].HeaderText = "Абонент";
            dataGridView1.Columns[3].HeaderText = "Телефон";
            dataGridView1.Columns[4].HeaderText = "Адресс";
            dataGridView1.Columns[5].HeaderText = "Баланс";
            // Добавляем обработчик события для сохранения изменений в базе данных при редактировании
            dataGridView1.CellEndEdit += DataGridView1_CellEndEdit;
        }
        private void DataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            // Получаем измененную ячейку
            DataGridViewCell editedCell = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];

            // Получаем выбранную таблицу в DataGridView
            string tableName = ((DataTable)dataGridView1.DataSource).TableName;

            FbCommand updateCommand;
            if (tableName == "SOTRUDNIKI")
            {
                // Редактирование таблицы SOTRUDNIKI
                DataRowView editedRow = (DataRowView)dataGridView1.Rows[e.RowIndex].DataBoundItem;

                FbConnection link = linkFb();
                updateCommand = new FbCommand("UPDATE SOTRUDNIKI SET " + editedCell.OwningColumn.Name + " = @Value WHERE SOTRUDNIK = @Sotrudnik", link);
                updateCommand.Parameters.AddWithValue("@Value", editedCell.Value);
                updateCommand.Parameters.AddWithValue("@Sotrudnik", editedRow["SOTRUDNIK"]);
            }
            else ;
            {
                // Редактирование таблицы ABONENTI
                DataRowView editedRow = (DataRowView)dataGridView1.Rows[e.RowIndex].DataBoundItem;

                FbConnection link = linkFb();
                updateCommand = new FbCommand("UPDATE ABONENTI SET " + editedCell.OwningColumn.Name + " = @Value WHERE LICEVOY_SCHET = @LicevoySchet", link);
                updateCommand.Parameters.AddWithValue("@Value", editedCell.Value);
                updateCommand.Parameters.AddWithValue("@LicevoySchet", editedRow["LICEVOY_SCHET"]);
            }
            

            updateCommand.ExecuteNonQuery();
            ((DataTable)dataGridView1.DataSource).AcceptChanges();
        }

        public void button6_Click(object sender, EventArgs e)
        {
            int count = this.dataGridView1.Columns.Count;
            for (int i = 0; i < count; i++)
            {
                this.dataGridView1.Columns.RemoveAt(0);
            }
            FbConnection link = linkFb(); int n = link.Query<int>("select count(ID_SOTRUDNIKA) from SOTRUDNIKI").Single();
            DataTable linkDB = new DataTable();
            FbCommand com = new FbCommand("select a.SOTRUDNIK, a.TELEFON, a.ROLE from SOTRUDNIKI a ", link);
            com.CommandType = CommandType.Text;
            FbDataReader read = com.ExecuteReader();
            linkDB.Load(read);
            dataGridView1.DataSource = linkDB;
            dataGridView1.Columns[0].HeaderText = "Сотрудник";
            dataGridView1.Columns[1].HeaderText = "Телефон";
            dataGridView1.Columns[2].HeaderText = "Должность";
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            // Добавляем обработчик события для сохранения изменений в базе данных при редактировании
            dataGridView1.CellEndEdit += DataGridView1_CellEndEdit;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Получаем индекс выбранной строки
    int rowIndex = dataGridView1.CurrentRow.Index;

    // Получаем выбранную таблицу в DataGridView
    string tableName = ((DataTable)dataGridView1.DataSource).TableName;

    if (tableName == "SOTRUDNIKI")
    {
        // Удаление из таблицы SOTRUDNIKI
        DataRowView selectedRow = (DataRowView)dataGridView1.Rows[rowIndex].DataBoundItem;
        string sotrudnikId = selectedRow["SOTRUDNIK"].ToString();

        FbConnection link = linkFb();
        FbCommand deleteCommand = new FbCommand("DELETE FROM SOTRUDNIKI WHERE SOTRUDNIK = @Sotrudnik", link);
        deleteCommand.Parameters.AddWithValue("@Sotrudnik", sotrudnikId);
        deleteCommand.ExecuteNonQuery();
    }
    else if (tableName == "ABONENTI")
    {
        // Удаление из таблицы ABONENTI
        DataRowView selectedRow = (DataRowView)dataGridView1.Rows[rowIndex].DataBoundItem;
        string licevoySchet = selectedRow["LICEVOY_SCHET"].ToString();

        FbConnection link = linkFb();
        FbCommand deleteCommand = new FbCommand("DELETE FROM ABONENTI WHERE LICEVOY_SCHET = @LicevoySchet", link);
        deleteCommand.Parameters.AddWithValue("@LicevoySchet", licevoySchet);
        deleteCommand.ExecuteNonQuery();
    }

    // Удаление строки из DataGridView
    dataGridView1.Rows.RemoveAt(rowIndex);
        }
    }
}
