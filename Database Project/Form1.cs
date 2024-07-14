using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;
using System.Windows.Input;
using System.Diagnostics;
using System.Data.Common;
using System.Net.NetworkInformation;


namespace Database_Project
{

    public partial class Form1 : Form

    {
        string connectionString = @"Server=PCR\MSSQLSERVER1;Database=LibraryDB;Integrated Security=True;";

        public Form1()
        {
            InitializeComponent();

        }
        private void RefreshDataGrid()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM Books", con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                dgvBooks.DataSource = dt;
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'libraryDBDataSet1.Books' table. You can move, or remove it, as needed.
            this.booksTableAdapter1.Fill(this.libraryDBDataSet1.Books);


        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            // Get values from textboxes
            string title = TxtTitle.Text; // Assuming there's a TextBox named txtTitle
            string author = TxtAuthor.Text; // Assuming there's a TextBox named txtAuthor
            string genre = TxtGenre.Text;
            int YearPublished;
            if (!int.TryParse(TxtYear.Text, out YearPublished))
            {
                MessageBox.Show("Please enter a valid year.");
                return;
            }
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string query = "INSERT INTO Books (Title, Author,Genre,YearPublished) VALUES (@Title, @Author,@Genre,@YearPublished)";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Title", title);
                    cmd.Parameters.AddWithValue("@Author", author);
                    cmd.Parameters.AddWithValue("@Genre", genre);
                    cmd.Parameters.AddWithValue("@YearPublished", YearPublished);
                    cmd.ExecuteNonQuery();
                    RefreshDataGrid(); // Refresh DataGridView after update
                }
            }

        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            // Ensure a row is selected in the DataGridView
            if (dgvBooks.SelectedRows.Count > 0)
            {
                // Get the ID from the selected row
                int id = Convert.ToInt32(dgvBooks.SelectedRows[0].Cells["iDDataGridViewTextBoxColumn"].Value);

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    string query = "UPDATE Books SET Title=@Title, Author=@Author, Genre=@Genre, YearPublished=@YearPublished WHERE ID=@iDDataGridViewTextBoxColumn";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        // Add parameters
                        cmd.Parameters.AddWithValue("@Title", TxtTitle.Text.Trim());
                        cmd.Parameters.AddWithValue("@Author", TxtAuthor.Text.Trim());
                        cmd.Parameters.AddWithValue("@Genre", TxtGenre.Text.Trim());
                        cmd.Parameters.AddWithValue("@YearPublished", TxtYear.Text.Trim());
                        cmd.Parameters.AddWithValue("@iDDataGridViewTextBoxColumn", id); // Set the ID parameter

                        try
                        {
                            cmd.ExecuteNonQuery();
                            MessageBox.Show("Record updated successfully!");
                            RefreshDataGrid(); // Refresh DataGridView after update
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error updating record: " + ex.Message);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a row to update.");
            }
        }
        private void LoadData(int id)
        {
            ID = id;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    // Define the SQL query to retrieve data for a specific ID
                    string selectQuery = "SELECT Title, Author, YearPublished, Genre FROM Books WHERE ID = @iDDataGridViewTextBoxColumn";
                    SqlCommand command = new SqlCommand(selectQuery, connection);
                    command.Parameters.AddWithValue("@iDDataGridViewTextBoxColumn", id);

                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        // Populate form fields with data from the database
                        TxtTitle.Text = reader["Title"].ToString();
                        TxtAuthor.Text = reader["Author"].ToString();
                        TxtYear.Text = reader["YearPublished"].ToString();
                        TxtGenre.Text = reader["Genre"].ToString();
                    }
                    else
                    {
                        MessageBox.Show("Record not found.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading data: " + ex.Message);
                }
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgvBooks.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dgvBooks.SelectedRows[0];
                int id = Convert.ToInt32(selectedRow.Cells["iDDataGridViewTextBoxColumn"].Value); // Assuming "ID" is the column name for your primary key

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    string deleteQuery = "DELETE FROM Books WHERE ID = @iDDataGridViewTextBoxColumn";
                    using (SqlCommand cmd = new SqlCommand(deleteQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@iDDataGridViewTextBoxColumn", id);

                        try
                        {
                            cmd.ExecuteNonQuery();
                            MessageBox.Show("Record deleted successfully!");
                            RefreshDataGrid(); // Refresh DataGridView after deletion
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error deleting record: " + ex.Message);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a row to delete.");
            }

        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            TxtTitle.Text = string.Empty;
            TxtAuthor.Text = string.Empty;
            TxtGenre.Text = string.Empty;
            TxtYear.Text = string.Empty;

            // Optionally, you can clear DataGridView selection
            dgvBooks.ClearSelection();
    }

        private void dgvBooks_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
