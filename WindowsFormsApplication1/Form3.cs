using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form3 : Form
    {
        private string ret;

        class ButTextException : Exception
        {
            public ButTextException(string message)
                : base(message)
            { }
        }

        public string get_str 
        { 
            get
            {
                return (ret);
            }
        }

        public void update()
        {
            switch (status_calc.form3task)
            {
                case 1:
                    label1.Text = "Enter your task here";
                    break;
                case 2:
                    label1.Text = "Enter id of task to delete";
                    break;
                case 3:
                    label1.Text = "Enter id of task to set as done";
                    break;
                case 4:
                    label1.Text = "Enter number of months";
                    break;
                case 5:
                    label1.Text = "Enter number of year";
                    break;
            }
            textBox1.Text = "";
        }

        public Form3()
        {
            InitializeComponent();
            update();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            ret = "";
            this.Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (status_calc.form3task == 1)// проверяем должен ли быть текст или число
            {
                if (textBox1.Text.Length == 0)
                    return;
                ret = textBox1.Text;
                this.Hide();
            }
            else if (status_calc.form3task >= 2)
            {
                try
                {
                    if (Convert.ToInt16(textBox1.Text) <= 0)
                        throw (new ButTextException("Enter positive number"));
                    else if (Convert.ToInt16(textBox1.Text) > 12 && status_calc.form3task == 4)
                        throw (new ButTextException("Enter 1-12"));
                    else
                        ret = textBox1.Text;
                }
                catch (ButTextException ex)
                {
                    MessageBox.Show(ex.Message);
                    ret = "";
                }
                catch (Exception)
                {
                    MessageBox.Show("Bad number");
                    ret = "";
                }
                this.Hide();
            }
        }
    }
}