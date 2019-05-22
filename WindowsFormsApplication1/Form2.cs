using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace WindowsFormsApplication1
{
    public partial class Form2 : Form
    {

        private int flag = 0;
        protected override void OnClosing(CancelEventArgs e)
        {
            if (flag == 0)// запрещаем закрывать если не вписано имя
            {
                base.OnClosing(e);
                e.Cancel = true;
            }
        }
        public Form2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox2.Text.Length != 0)
            {
                using (StreamWriter sw = new StreamWriter("name.txt", false, System.Text.Encoding.Default))// открываем файл для записи
                {
                    sw.WriteLine(textBox2.Text);
                }
                flag = 1;
                this.Close();
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
