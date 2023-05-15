using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace CreditCardApp
{
    public partial class Form1 : Form
    {
        private TextBox? ID;
        private TextBox? CSV;
        private TextBox? Date;
        private Button? Save;
        private TabControl tabControl;

        public Form1()
        {
            InitializeComponent();
            Form1_Load(this, EventArgs.Empty);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            tabControl = new TabControl();
            tabControl.Size = new System.Drawing.Size(600, 400);
            this.Controls.Add(tabControl);

            TabPage tabPage1 = new TabPage();
            tabPage1.Text = "Tab 1";
            tabPage1.Controls.Add(CreateTab1());
            tabControl.TabPages.Add(tabPage1);

            TabPage tabPage2 = new TabPage();
            tabPage2.Text = "Tab 2";
            tabPage2.Controls.Add(CreateTab2());
            tabControl.TabPages.Add(tabPage2);

        


        }
        private string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            Random random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

   
        private Control CreateTab1()
        {
            Panel panel = new Panel();
            panel.Size = new Size(600, 400);

            Label label1 = new Label();
            label1.Text = "Credit Card Number:";
            label1.Font = new Font("Arial", 12);
            label1.Location = new Point(10, 50);
            label1.AutoSize = true;
            panel.Controls.Add(label1);

            TextBox textBox1 = new TextBox();
            textBox1.Font = new Font("Arial", 12);
            textBox1.Location = new Point(250, 50);
            textBox1.Size = new Size(200, 30);
            panel.Controls.Add(textBox1);

            Label label2 = new Label();
            label2.Text = "CSV:";
            label2.Font = new Font("Arial", 12);
            label2.Location = new Point(10, 100);
            label2.AutoSize = true;
            panel.Controls.Add(label2);

            TextBox textBox2 = new TextBox();
            textBox2.Font = new Font("Arial", 12);
            textBox2.Location = new Point(250, 100);
            textBox2.Size = new Size(200, 30);
            textBox2.PasswordChar = '*'; // Ẩn CSV
            panel.Controls.Add(textBox2);

            Label label3 = new Label();
            label3.Text = "Expiration date (MM-YY):";
            label3.Font = new Font("Arial", 12);
            label3.Location = new Point(10, 150);
            label3.AutoSize = true;
            panel.Controls.Add(label3);

            TextBox textBox3 = new TextBox();
            textBox3.Font = new Font("Arial", 12);
            textBox3.Location = new Point(250, 150);
            textBox3.Size = new Size(200, 30);
            panel.Controls.Add(textBox3);

            Button button = new Button();
            button.Font = new Font("Arial", 12);
            button.Location = new Point(300, 200);
            button.Size = new Size(100, 40);
            button.Text = "Save";
            button.Click += (sender, e) =>
            {
                string connectionString = "Server=DESKTOP-3K158E8\\SQLEXPRESS;Database=token;Integrated Security=true";
                SqlConnection connection = new SqlConnection(connectionString);

                string creditCardNumber = textBox1.Text;
                string csv = textBox2.Text;
                string expirationDate = textBox3.Text;

                try
                {
                    // Kiểm tra định dạng số thẻ tín dụng
                    if (!Regex.IsMatch(creditCardNumber, @"^\d{16}$"))
                    {
                        MessageBox.Show("Invalid credit card number. Please enter 16 digits.");
                        return;
                    }

                    // Kiểm tra định dạng CSV
                    if (!Regex.IsMatch(csv, @"^\d{3}$"))
                    {
                        MessageBox.Show("Invalid CSV. Please check again.");
                        return;
                    }
                    // Kiểm tra định dạng ngày hết hạn
                    if (!Regex.IsMatch(expirationDate, @"^(0[1-9]|1[0-2])-(\d{2})$"))
                    {
                        MessageBox.Show("Invalid expiration date. Please re-enter in MM-YY format.");
                        return;
                    }

                    connection.Open();
                    string idToken = GenerateRandomString(16); // Tạo IDToken ngẫu nhiên
                    string csvToken = GenerateRandomString(12); // Tạo CSVToken ngẫu nhiên

                    // Kiểm tra tính duy nhất của IDToken và CSVToken
                    bool isIdTokenUnique = false;
                    bool isCsvTokenUnique = false;

                    while (!isIdTokenUnique || !isCsvTokenUnique)
                    {
                        SqlCommand idTokenCommand = new SqlCommand("SELECT COUNT(*) FROM CreditCardTable WHERE IDToken=@idToken", connection);
                        idTokenCommand.Parameters.AddWithValue("@idToken", idToken);

                        int idTokenCount = (int)idTokenCommand.ExecuteScalar();
                        if (idTokenCount == 0)
                        {
                            isIdTokenUnique = true;
                        }

                        SqlCommand csvTokenCommand = new SqlCommand("SELECT COUNT(*) FROM CreditCardTable WHERE CSVToken=@csvToken", connection);
                        csvTokenCommand.Parameters.AddWithValue("@csvToken", csvToken);

                        int csvTokenCount = (int)csvTokenCommand.ExecuteScalar();
                        if (csvTokenCount == 0)
                        {
                            isCsvTokenUnique = true;
                        }

                        connection.Close();
                    }

                    // Thực hiện thêm bản ghi thẻ tín dụng mới
                    SqlCommand insertCommand = new SqlCommand("INSERT INTO CreditCardTable (CreditCardNumber, CSV, ExpirationDate, IDToken, CSVToken) VALUES (@creditCardNumber, @csv, @expirationDate, @idToken, @csvToken)", connection);
                    insertCommand.Parameters.AddWithValue("@creditCardNumber", creditCardNumber);
                    insertCommand.Parameters.AddWithValue("@csv", csv);
                    insertCommand.Parameters.AddWithValue("@expirationDate", expirationDate);
                    insertCommand.Parameters.AddWithValue("@idToken", idToken);
                    insertCommand.Parameters.AddWithValue("@csvToken", csvToken);

                    connection.Open();
                    insertCommand.ExecuteNonQuery();
                    connection.Close();

                    MessageBox.Show("Credit card record added successfully!");

                    textBox1.Text = "";
                    textBox2.Text = "";
                    textBox3.Text = "";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error! An error occurred. Please try again later: " + ex.Message);
                }
            };

            panel.Controls.Add(button);

            return panel;
        }
        private Control CreateTab2()
        {
            Panel panel = new Panel();
            panel.Size = new System.Drawing.Size(600, 400);

            Label label1 = new Label();
            label1.Text = "Enter ID Token:";
            label1.Font = new System.Drawing.Font("Arial", 12);
            label1.Location = new System.Drawing.Point(10, 50);
            label1.AutoSize = true;
            panel.Controls.Add(label1);

            TextBox idTokenTextBox = new TextBox();
            idTokenTextBox.Font = new System.Drawing.Font("Arial", 12);
            idTokenTextBox.Location = new System.Drawing.Point(250, 50);
            idTokenTextBox.Size = new System.Drawing.Size(200, 30);
            panel.Controls.Add(idTokenTextBox);

            Label label2 = new Label();
            label2.Text = "Enter CSV Token:";
            label2.Font = new System.Drawing.Font("Arial", 12);
            label2.Location = new System.Drawing.Point(10, 100);
            label2.AutoSize = true;
            panel.Controls.Add(label2);

            TextBox csvTokenTextBox = new TextBox();
            csvTokenTextBox.Font = new System.Drawing.Font("Arial", 12);
            csvTokenTextBox.Location = new System.Drawing.Point(250, 100);
            csvTokenTextBox.Size = new System.Drawing.Size(200, 30);
            panel.Controls.Add(csvTokenTextBox);

            TextBox resultTextBox = new TextBox(); // Thêm dòng này để khởi tạo biến resultTextBox
            resultTextBox.Font = new System.Drawing.Font("Arial", 12);
            resultTextBox.Location = new System.Drawing.Point(10, 200);
            resultTextBox.Size = new System.Drawing.Size(440, 150);
            resultTextBox.Multiline = true;
            resultTextBox.ReadOnly = true;
            panel.Controls.Add(resultTextBox);

            Button searchButton = new Button();
            searchButton.Font = new System.Drawing.Font("Arial", 12);
            searchButton.Location = new System.Drawing.Point(300, 150);
            searchButton.Size = new System.Drawing.Size(100, 40);
            searchButton.Text = "Search";
            searchButton.Click += SearchButton_Click;
            panel.Controls.Add(searchButton);

            void SearchButton_Click(object sender, EventArgs e)
            {
                string connectionString = "Server=DESKTOP-3K158E8\\SQLEXPRESS;Database=token;Integrated Security=true";
                string idToken = idTokenTextBox.Text;
                string csvToken = csvTokenTextBox.Text;
                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        SqlCommand command = new SqlCommand("SELECT CreditCardNumber, ExpirationDate, CSV FROM CreditCardTable WHERE IDToken = @IDToken AND CSVToken = @CSVToken", connection);
                        command.Parameters.AddWithValue("@IDToken", idToken);
                        command.Parameters.AddWithValue("@CSVToken", csvToken);

                        SqlDataReader reader = command.ExecuteReader();

                        if (reader.Read())
                        {
                            string creditCardNumber = reader.GetString(0);
                            string expirationDate = reader.GetString(1);
                            string csv = reader.GetString(2);

                            resultTextBox.Text = $"Credit Card Number: {creditCardNumber}\r\nExpiration Date: {expirationDate}\r\nCSV: {csv}";
                        }
                        else
                        {
                            resultTextBox.Text = "Token information is invalid.";
                        }

                        reader.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                }
            }

            return panel;
        }


    }
}

        