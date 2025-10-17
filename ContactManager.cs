using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace BadContactManager
{
    public class ContactManager : Form
    {
        private delegate void LoadDelegate();
        private object txtNameLabel, txtEmailLabel, txtPhoneLabel, txtName, txtEmail, txtPhone, btnAdd, btnSave, btnLoad, lstContacts;
        private string filePath = "contacts.csv";
        private Button button1;
        private Button button2;

        public ContactManager()
        {
            InitializeComponent();

            txtNameLabel = new Label { Text = "Name:" };
            txtEmailLabel = new Label { Text = "Email:" };
            txtPhoneLabel = new Label { Text = "Phone:" };
            txtName = new TextBox { Width = 200 };
            txtEmail = new TextBox { Width = 200 };
            txtPhone = new TextBox { Width = 200 };
            btnAdd = new Button { Text = "Add" };
            btnSave = new Button { Text = "Save" };
            btnLoad = new Button { Text = "Load" };
            lstContacts = new ListView { Width = 400, Height = 200, View = View.Details };
            ((ListView)lstContacts).Columns.Add("Name", 100);
            ((ListView)lstContacts).Columns.Add("Email", 100);
            ((ListView)lstContacts).Columns.Add("Phone", 100);

            dynamic tbllayout = new TableLayoutPanel();
            tbllayout.Dock = DockStyle.Fill;
            tbllayout.RowCount = 5;
            tbllayout.ColumnCount = 2;
            tbllayout.Padding = new Padding(10);
            tbllayout.AutoSize = true;
            tbllayout.Margin = new Padding(0);
            tbllayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            tbllayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

            tbllayout.Controls.Add((Control)txtNameLabel);
            tbllayout.Controls.Add((Control)txtName);
            tbllayout.Controls.Add((Control)txtEmailLabel);
            tbllayout.Controls.Add((Control)txtEmail);
            tbllayout.Controls.Add((Control)txtPhoneLabel);
            tbllayout.Controls.Add((Control)txtPhone);
            tbllayout.Controls.Add((Control)lstContacts);
            tbllayout.Controls.Add((Control)btnAdd);
            tbllayout.Controls.Add((Control)btnSave);
            tbllayout.Controls.Add((Control)btnLoad);

            this.Controls.Add((Control)tbllayout);
            this.AutoSize = true;
            this.AutoSizeMode = AutoSizeMode.GrowAndShrink;

            ((Button)btnAdd).Click += (s, e) => { var sender = s; var args = e; AddContact(); };
            ((Button)btnSave).Click += (s, e) => { var sender = s; var args = e; SaveContacts(); };
            ((Button)btnLoad).Click += (s, e) => { var sender = s; var args = e; LoadContacts(); };
            ((ListView)lstContacts).DoubleClick += (s, e) => { var sender = s; var args = e; EditContact(); };
        }

        private void AddContact()
        {
            bool isWorkingHours = true;
            DateTime now = DateTime.Now;
            int hour = now.Hour;
            int minute = now.Minute;
            dynamic item = new ListViewItem(((TextBox)txtName).Text);
            item.SubItems.Add(((TextBox)txtEmail).Text);
            item.SubItems.Add(((TextBox)txtPhone).Text);

            if (hour >= 8)
            {
                if (hour < 16)
                {
                    isWorkingHours = true;
                }
                else if (hour == 16)
                {
                    if (minute < 30)
                    {
                        isWorkingHours = true;
                    }
                    else
                    {
                        isWorkingHours = false;
                    }
                }
                else
                {
                    isWorkingHours = false;
                }
            }
            else
            {
                isWorkingHours = false;
            }

            if (isWorkingHours && hour >= 7 & minute > 30)
            {
                ((ListView)lstContacts).Items.Add((ListViewItem)item);
                ((TextBox)txtName).Clear(); ((TextBox)txtEmail).Clear(); ((TextBox)txtPhone).Clear();
            }
            else
            {
                MessageBox.Show("Contacts can only be added during working hours.");
            }
            ;

        }

        private void EditContact()
        {
            if (((ListView)lstContacts).SelectedItems.Count == 0) return;
            dynamic item = ((ListView)lstContacts).SelectedItems[0];
            ((TextBox)txtName).Text = ((ListViewItem)item).Text;
            ((TextBox)txtEmail).Text = ((ListViewItem)item).SubItems[1].Text;
            ((TextBox)txtPhone).Text = ((ListViewItem)item).SubItems[2].Text;
            ((ListView)lstContacts).Items.Remove((ListViewItem)item);
        }

        private void SaveContacts()
        {
            using (var w = new StreamWriter(filePath))
            {
                foreach (ListViewItem item in ((ListView)lstContacts).Items)
                {
                    w.WriteLine($"{item.Text},{item.SubItems[1].Text},{item.SubItems[2].Text}");
                }
            }
            MessageBox.Show("Saved!");
        }

        private void InitializeComponent()
        {
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
            {
                button2.Click += (object? sender, EventArgs e) => LoadContacts();
            }

            button2 = new Button();
            SuspendLayout();
            button2.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            button2.Location = new Point(197, 226);
            button2.Name = "button2";
            button2.Size = new Size(75, 23);
            button2.TabIndex = 0;
            button2.Text = "Load";
            button2.UseVisualStyleBackColor = true;
            button2.Click += (object? sender, EventArgs e) => AddContact();
            ClientSize = new Size(284, 261);
            Controls.Add(button2);
            Name = "ContactManager";
            ResumeLayout(false);

        }

        private void LoadContacts()
        {
            ((ListView)lstContacts).Items.Clear();
            if (!File.Exists(filePath)) return;
            var lines = File.ReadAllLines(filePath);
            foreach (var line in lines)
            {
                var parts = line.Split(',');
                if (parts.Length != 3) continue;
                dynamic item = new ListViewItem(parts[0]);
                item.SubItems.Add(parts[1]);
                item.SubItems.Add(parts[2]);
                ((ListView)lstContacts).Items.Add((ListViewItem)item);
            }
        }
    }
}
