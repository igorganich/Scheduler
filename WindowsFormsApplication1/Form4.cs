using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;

namespace WindowsFormsApplication1
{
    public partial class Form4 : Form
    {
        public static string ToMonthName(DateTime dateTime)
        {
            return CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(dateTime.Month);
        }
        private class Item
        {
            public string Name;
            public int Value;
            public Item(string name, int value)
            {
                Name = name; Value = value;
            }
            public override string ToString()
            {
                // Generates the text shown in the combo box
                return Name;
            }
        }

        void create_days(string _year, int month)
        {
            int year = Convert.ToInt16(_year);
            comboBox3.Items.Clear();
            for (int i = 1; i <= DateTime.DaysInMonth(year, month); i++)
            {
                comboBox3.Items.Add(new Item(Convert.ToString(i), i));
            }
            comboBox3.SelectedIndex = 0;
        }

        void create_years_months()
        {
            for (int i = 0; i <= 2100 - 2019; i++)
                comboBox1.Items.Add(new Item(Convert.ToString(i + 2019), i));
            for (int i = 0; i < 12; i++)
            {
                DateTime smth = DateTime.MinValue.AddMonths(i);
                comboBox2.Items.Add(new Item(ToMonthName(smth), i));
            }
            for (int i = 0; i < 24; i++)
            {
                comboBox4.Items.Add(new Item(Convert.ToString(i), i));
            }
            for (int i = 0; i < 60; i++)
            {
                comboBox5.Items.Add(new Item(Convert.ToString(i), i));
                comboBox6.Items.Add(new Item(Convert.ToString(i), i));
            }
            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
            comboBox4.SelectedIndex = 0;
            comboBox5.SelectedIndex = 0;
            comboBox6.SelectedIndex = 0;
            create_days(comboBox1.SelectedItem.ToString(), comboBox2.SelectedIndex + 1);
            comboBox1.SelectedIndexChanged += new EventHandler(SelectedIndexChanged);
            comboBox2.SelectedIndexChanged += new EventHandler(SelectedIndexChanged);
        }
        public Form4()
        {
            InitializeComponent();
            create_years_months();
        }

        private void Form4_Load(object sender, EventArgs e)
        {
        }
        private void SelectedIndexChanged(object sender, EventArgs e)
        {
            create_days(comboBox1.SelectedItem.ToString(), comboBox2.SelectedIndex + 1);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string str = textBox1.Text;
            if (str.Length == 0)
            {
                MessageBox.Show("Enter message");
                status_calc.retstr = "";
            }
            else
            {
                status_calc.retstr = textBox1.Text;
                status_calc.al.set_alert(str, Convert.ToInt16(comboBox1.SelectedItem.ToString()), comboBox2.SelectedIndex + 1,
                    comboBox3.SelectedIndex + 1, comboBox4.SelectedIndex, comboBox5.SelectedIndex, comboBox6.SelectedIndex);
                this.Hide();
            }
            textBox1.Text = "";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            status_calc.retstr = "";
            textBox1.Text = "";
            this.Hide();
        }
    }
}
