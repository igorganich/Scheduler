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
using System.Collections;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        Form3 newForm_3 = new Form3();
        Form4 f4 = new Form4();
        System.Windows.Forms.Timer timer;
        Button[] btns;
        public delegate bool update_task(DateTime date, string str);// обьявляем делегаты функций 
        public delegate void insert_func(DateTime date);
        public Form1()
        {
            InitializeComponent();
        }

        private void init_name()
        {
            try// пробуем прочитать name.txt
            {
                using (StreamReader sr = new StreamReader("name.txt"))
                {
                    label1.Text = "Hello, " + sr.ReadToEnd();// меняем текст в привестствии
                }
            }
            catch (Exception)/* если не удалось прочитать - открываем вторую форму, создаем файл в ней. записываем туда имя и вызываем
                 этот же метод*/
            {
                Form2 newForm = new Form2();
                newForm.ShowDialog();// вызываем вторую форму для заполнения имени
                init_name();
            }
        }

        private void DynamicButton_Click(object sender, EventArgs e)//обрабатываем нажатие на кнопку с датой в календаре
        {
            btns[status_calc.cur_date.Day].BackColor = SystemColors.Control;
            Button bt = sender as Button;
            status_calc.cur_date = new DateTime(status_calc.cur_date.Year, status_calc.cur_date.Month, Convert.ToInt16(bt.Text));
            btns[Convert.ToInt16(bt.Text)].BackColor = SystemColors.GradientActiveCaption;
            insert_day_data(status_calc.cur_date);// вставляем инфо на экран в соответствующую колонку
            insert_week_data(status_calc.cur_date);
        }

        private Button create_but_cal(int i, int l, int start) // создаем кнопку с определенными координатами
        {
            Button dynamicButton = new Button();
            // Set Button properties
            dynamicButton.BackColor = SystemColors.Control;
            dynamicButton.Height = 33;
            dynamicButton.Width = 33;
            dynamicButton.Location = new Point(10 + 61 * ((i + start - 2) % 7), l);
            dynamicButton.Text = Convert.ToString(i);
            dynamicButton.Name = "DynamicButton" + Convert.ToString(i);
            dynamicButton.Font = new Font("Georgia", 8);
            // Add a Button Click Event handler
            dynamicButton.Click += new EventHandler(DynamicButton_Click);// добавляем функцию когда кнопка нажимается
            Controls.Add(dynamicButton);// делаем кнопку видимой 
            dynamicButton.BringToFront();
            return (dynamicButton);
        }

        private void create_buttons_date(DateTime date)// высчитываем положение кнопки на экране и создаем кнопку
        {
            for (int i = 1; btns != null && btns[i] != null; i++)
            {
                this.Controls.Remove(btns[i]);// удаляем старые кнопки
            }
            btns = new Button[33];
            int day = 0;
            int year = 0;
            int month = 0;
            list_from_date.form_date(date.ToString(), ref day, ref month, ref year);
            DateTime first_of_month = new DateTime(year, month, 1, 0, 0, 0);
            int start = Convert.ToInt32(first_of_month.DayOfWeek) + 1;
            label4.Text = "SUN             MON            TUE             WED            THU             FRI             SAT";
            for (int i = 1, l = 0; i <= DateTime.DaysInMonth(year, month); i++)// по одному генерируем кнопки
            {
                btns[i] = create_but_cal(i, 90 + 33 * l, start);
                if ((start + i) % 7 == 1)
                    l++;
            }
            btns[day].BackColor = SystemColors.GradientActiveCaption;       // меняем цвет "особых" кнопок (сегодня и обрабатываемая дата)
            if (date.Month == DateTime.Today.Month && date.Year == DateTime.Today.Year)
            {
                list_from_date.form_date(DateTime.Today.ToString(), ref day, ref month, ref year);
                btns[day].ForeColor = SystemColors.ActiveCaption;
            }
            label3.Text = Convert.ToString(date.Month);
            label5.Text = Convert.ToString(date.Year);
        }

        private void save_data(int id)// здесь сохраняем данные про задачи с помощью сериализации 
        {
            BinaryFormatter formatter = new BinaryFormatter();
            if (id == 0 || id == 1)
                using (FileStream fs = new FileStream("day_list.dat", FileMode.OpenOrCreate))  
                {
                    formatter.Serialize(fs, status_calc.my_list_day);//сохраняем обьект с помощью сериализации
                }
            if (id == 0 || id == 2)
                using (FileStream fs = new FileStream("week_list.dat", FileMode.OpenOrCreate))
                {
                    formatter.Serialize(fs, status_calc.my_list_week);
                }
            if (id == 0 || id == 3)
                using (FileStream fs = new FileStream("habits_list.dat", FileMode.OpenOrCreate))
                {
                    formatter.Serialize(fs, status_calc.my_list_habits);
                }
            if (id == 0 || id == 4)
                using (FileStream fs = new FileStream("alert.dat", FileMode.OpenOrCreate))
                {
                    formatter.Serialize(fs, status_calc.al);
                }
        }

        private void load_day_data(string path)
        {
            BinaryFormatter formatter = new BinaryFormatter();// обьявляем бинарный форматтер
            try
            {
                using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
                {
                    status_calc.my_list_day = (list_day)formatter.Deserialize(fs); //сериализируем обьект в файл по пути path
                }
            }
            catch (SerializationException)
            {
                status_calc.my_list_day = new list_day();
                save_data(1);
            }
            return;
        }

        private void load_week_data(string path)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            try
            {
                using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
                {
                    status_calc.my_list_week = (list_week)formatter.Deserialize(fs);
                }
            }
            catch (SerializationException)
            {
                status_calc.my_list_week = new list_week();
                save_data(2);
            }
            return;
        }

        private void load_alert_data(string path)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            try
            {
                using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
                {
                    status_calc.al = (alert)formatter.Deserialize(fs);
                }
            }
            catch (SerializationException)
            {
                status_calc.al = new alert();
                save_data(4);
            }
            return;
        }

        private void load_habits_data(string path)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            try
            {
                using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
                {
                    status_calc.my_list_habits = (list_habits)formatter.Deserialize(fs);
                }
            }
            catch (SerializationException)
            {
                status_calc.my_list_habits = new list_habits();
                save_data(3);
            }
            return;
        }

        private void insert_day_data(DateTime date)// вставляем в поле информацию по текущей дате
        {
            textBox1.Text = status_calc.my_list_day.get_list(date);
            label7.Text = status_calc.my_list_day.get_prod();
        }

        private void insert_week_data(DateTime date)
        {
            textBox2.Text = status_calc.my_list_week.get_list(date);
        }

        private void insert_habits_data(DateTime date)
        {
            textBox3.Text = status_calc.my_list_habits.get_list(date);
        }

        private void create_timer()
        {
            timer = new System.Windows.Forms.Timer();
            timer.Interval = 1000;//
            timer.Tick += new EventHandler(timer_Tick); //подписываемся на события Tick
            timer.Start();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            if (status_calc.al.is_date())
            {
                button11.Text = status_calc.al.get_alert();
                timer.Stop();
            }
        }

        private static void Real_Alert(object obj)
        {
            MessageBox.Show("smth");
        }

        private void insert_alert_data()
        {
            if (status_calc.al.is_changed())
            {
                if (status_calc.al.is_date())
                {
                    button11.Text = status_calc.al.get_alert();
                }
                else
                {
                    button11.Text = "Waiting for alert. Press here to change alert";
                    create_timer();
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);// добавляем функцию, которая запустится когда программа завершится
            init_name();// записываем имя в приветствие
            create_buttons_date(status_calc.cur_date);// генерируем кнопки на календаре
            load_day_data("day_list.dat");// загружаем из файла инфо про задачи на дни
            load_week_data("week_list.dat");// загружаем из файла инфо про задачи на недели
            load_habits_data("habits_list.dat");// загружаем из файла инфо про задачи на недели
            load_alert_data("alert.dat");
            insert_day_data(status_calc.cur_date);// вставляем инфо на экран в соответствующую колонку
            insert_week_data(status_calc.cur_date);
            insert_habits_data(status_calc.cur_date);
            insert_alert_data();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click_1(object sender, EventArgs e)
        {

        }

        private void label1_Click_2(object sender, EventArgs e)
        {

        }

        private void Form1_FormClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            save_data(0);
        }
        // тут мы обрабатываем кнопки add new/ delete / set as done в зависимости от того какие функции мы передали 
        private void update_tasks(int id_task, update_task func, insert_func func2, DateTime date, string str)
        {
            status_calc.form3task = id_task;
            newForm_3.update();
            newForm_3.ShowDialog();
            status_calc.retstr = newForm_3.get_str;
            if (status_calc.retstr == null|| status_calc.retstr.Length == 0)// проверяем не пустая ли строка 
                return;
            func(date, status_calc.retstr);// вызываем метод по делегату апдейта в словаре
            func2(date);// вызываем метод по делегату апдейта в форме, метод которой передали
            status_calc.retstr = "";
        }
        // дальше делаем одно и то же, только меняем в параметрах какой функцией сохранять изменения, в зависимости от нажатой кнопки
        private void button3_Click(object sender, EventArgs e)
        {
            update_tasks(3, new update_task(status_calc.my_list_day.done_one), new insert_func(insert_day_data), status_calc.cur_date, status_calc.retstr);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            update_tasks(1, new update_task(status_calc.my_list_day.set_new), new insert_func(insert_day_data), status_calc.cur_date, status_calc.retstr);
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            update_tasks(2, new update_task(status_calc.my_list_day.delete_one), new insert_func(insert_day_data), status_calc.cur_date, status_calc.retstr);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            update_tasks(1, new update_task(status_calc.my_list_week.set_new), new insert_func(insert_week_data), status_calc.cur_date, status_calc.retstr);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            update_tasks(2, new update_task(status_calc.my_list_week.delete_one), new insert_func(insert_week_data), status_calc.cur_date, status_calc.retstr);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            update_tasks(1, new update_task(status_calc.my_list_habits.set_new), new insert_func(insert_habits_data), status_calc.cur_date, status_calc.retstr);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            update_tasks(2, new update_task(status_calc.my_list_habits.delete_one), new insert_func(insert_habits_data), status_calc.cur_date, status_calc.retstr);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            update_tasks(3, new update_task(status_calc.my_list_habits.done_one), new insert_func(insert_habits_data), status_calc.cur_date, status_calc.retstr);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            status_calc.form3task = 4;
            Form3 newForm = new Form3();
            newForm.ShowDialog();
            if (status_calc.retstr.Length == 0)
                return;
            status_calc.cur_date = new DateTime(status_calc.cur_date.Year, Convert.ToInt16(status_calc.retstr), 1);
            create_buttons_date(status_calc.cur_date);// генерируем кнопки на календаре
            insert_day_data(status_calc.cur_date);// вставляем инфо на экран в соответствующую колонку
            insert_week_data(status_calc.cur_date);
            status_calc.retstr = "";
        }

        private void button10_Click(object sender, EventArgs e)
        {
            status_calc.form3task = 5;
            Form3 newForm = new Form3();
            newForm.ShowDialog();
            if (status_calc.retstr.Length == 0)
                return;
            status_calc.cur_date = new DateTime(Convert.ToInt16(status_calc.retstr), status_calc.cur_date.Month, 1);
            create_buttons_date(status_calc.cur_date);// генерируем кнопки на календаре
            insert_day_data(status_calc.cur_date);// вставляем инфо на экран в соответствующую колонку
            insert_week_data(status_calc.cur_date);
            status_calc.retstr = "";
        }

        private void button11_Click(object sender, EventArgs e)
        {
            if (status_calc.al.is_date())
            {
                button11.Text = "No alert. Press here to set up alert";
                status_calc.al = new alert();
                MessageBox.Show("Alert was deleted");            
            }
            else
            {
                f4.ShowDialog();
                if (status_calc.retstr.Length != 0)
                {
                    create_timer();
                    button11.Text = "Waiting for alert. Press here to change alert";
                }
            }
        }
    }
    public interface Ilist_calen
    {
        string get_list(DateTime date);
        bool set_new(DateTime date, string newstr);
        bool delete_one(DateTime date, string index);
    }

    public interface Ilist_doneable
    {
        bool done_one(DateTime date, string index);
    }

    [Serializable]// обьявляем класс таким который можно сериализовать
    public abstract class list_calen : Ilist_calen
    {
        public abstract string get_list(DateTime date);
        public abstract bool set_new(DateTime date, string newstr);
        public abstract bool delete_one(DateTime date, string index);
    }

    [Serializable]
    public abstract class list_from_date : list_calen
    {
        protected Dictionary<string, ArrayList> dict_list;
        public static void form_date(string str, ref int day, ref int month, ref int year)// здесь мы парсим дату на день + месяц + год
        {
            str = str.Substring(0, str.Length - 8);
            string[] compo = str.Split(new char[] { '.' });
            day = Convert.ToInt16(compo[0]);
            month = Convert.ToInt16(compo[1]);
            year = Convert.ToInt16(compo[2]);
        }
        protected void set_new_by_key(string date, string newstr)
        {
            if (dict_list.ContainsKey(date))// проверяем существование такого ключа
            {
                ArrayList smth = dict_list[date];
                smth.Add(newstr);
            }
            else// если нет - создаем ключ
            {
                ArrayList smth = new ArrayList();
                smth.Add(newstr);
                dict_list.Add(date, smth);
            }
        }
        protected string get_list_by_key(string date)
        {
            string ret = "";
            try// проверяем наличие тасков для этого ключа
            {
                ArrayList strings = dict_list[date];
                if (strings != null)
                    for (int i = 0; i < strings.Count; i++)
                        ret += Convert.ToString(i + 1) + ". " + (string)strings[i] + "\r\n";
                if (ret.Length == 0)
                    throw (new Exception());
                return (ret);
            }
            catch (Exception)
            {
                return ("No tasks for this date");
            }    
        }
        protected bool delete_one_by_key(string date, string index)
        {
            ArrayList smth;
            if (dict_list.ContainsKey(date))
            {
                smth = dict_list[date];
            }
            else
            {
                MessageBox.Show("No subjects with this id");
                return false;
            }
            if (smth.Count + 1 > Convert.ToInt16(index) && Convert.ToInt16(index) >= 1)
            {
                smth.RemoveAt(Convert.ToInt16(index) - 1);
            }
            else
            {
                MessageBox.Show("No subjects with this id");
                return false;
            }
            return true;
        }
    }

    [Serializable]
    public class list_day : list_from_date, Ilist_doneable
    {     
        private int count_tasks = 0;
        private int count_done = 0;
        public list_day()
        {
            dict_list = new Dictionary<string, ArrayList>();
        }

        public string get_prod()
        {
            return ("Your productivity for all time: " + (count_tasks == 0 ? "0" : Convert.ToString((double)count_done / (double)count_tasks * 100)) + "%");
        }

        public override bool set_new(DateTime date_not_str, string newstr)
        {
            int day = 0;
            int month = 0;
            int year = 0;
            form_date(date_not_str.ToString(), ref day, ref month, ref year);
            string date = Convert.ToString(day) + Convert.ToString(month) + Convert.ToString(year);
            set_new_by_key(date, newstr);
            count_tasks += 1;
            return true;
        }

        public override string get_list(DateTime date)// из даты получаем стринг
        {
            int day = 0;
            int month = 0;
            int year = 0;
            form_date(date.ToString(), ref day, ref month, ref year);
            string date2 = Convert.ToString(day) + Convert.ToString(month) + Convert.ToString(year);
            return (get_list_by_key(date2));
        }

        public override bool delete_one(DateTime date, string index)
        {
            if (this.delete_one1(date, index) == true)
                count_tasks -= 1;
            else
                return false;
             return true;
        }
        public bool done_one(DateTime date, string index)
        {
            if (this.delete_one(date, index) == true)
            {
                count_tasks += 1;
                count_done += 1;
            }
            return (true);
        }

        private bool delete_one1(DateTime date, string index)
        {
            int day = 0;
            int month = 0;
            int year = 0;
            form_date(date.ToString(), ref day, ref month, ref year);// получаем значения день - месяц - год от даты. 
            string newstr = Convert.ToString(day) + Convert.ToString(month) + Convert.ToString(year);
            return (delete_one_by_key(newstr, index));
        }
    }

    [Serializable]
    public class list_week : list_from_date
    {
        public list_week()
        {
            dict_list = new Dictionary<string, ArrayList>();
        }
        public override bool set_new(DateTime date_not_str, string newstr)
        {
            int day = 0;
            int month = 0;
            int year = 0;
            form_date(date_not_str.ToString(), ref day, ref month, ref year);
            DateTime first_of_month = new DateTime(year, month, 1, 0, 0, 0);
            int start = Convert.ToInt32(first_of_month.DayOfWeek) + 1;
            double week = (start + day - 2) / 7;// получаем какая по счету неделя соответствует этому дню
            if (week != Convert.ToInt16(week))
                week += 1;
            string date = Convert.ToString(Convert.ToInt16(week)) + Convert.ToString(month) + Convert.ToString(year);
            set_new_by_key(date, newstr);
            return true;
        }
        public override string get_list(DateTime date)
        {
            int day = 0;
            int month = 0;
            int year = 0;
            form_date(date.ToString(), ref day, ref month, ref year);
            DateTime first_of_month = new DateTime(year, month, 1, 0, 0, 0);// делаем то же что и методом выше
            int start = Convert.ToInt32(first_of_month.DayOfWeek) + 1;
            double week = (start + day - 2) / 7;
            if (week != Convert.ToInt16(week))
                week += 1;
            string date2 = Convert.ToString(Convert.ToInt16(week)) + Convert.ToString(month) + Convert.ToString(year);
            return (get_list_by_key(date2));
        }

        public override bool delete_one(DateTime date, string index)// удаление как в дне только считаем не день а неделю
        {
            int day = 0;
            int month = 0;
            int year = 0;
            form_date(date.ToString(), ref day, ref month, ref year);
            DateTime first_of_month = new DateTime(year, month, 1, 0, 0, 0);
            int start = Convert.ToInt32(first_of_month.DayOfWeek) + 1;
            double week = (start + day) / 7;
            if (week != Convert.ToInt16(week))
                week += 1;
            string newstr = Convert.ToString(Convert.ToInt16(week)) + Convert.ToString(month) + Convert.ToString(year);
            return (delete_one_by_key(newstr, index));
        }
    }

    [Serializable]
    public class list_habits : list_calen, Ilist_doneable
    {
        private DateTime lastdate;// для проверки поменялся ли "сегодняшний" день
        private ArrayList all_habits;
        private ArrayList not_done;
        public list_habits()
        {
            all_habits = new ArrayList();
            not_done = new ArrayList();
        }

        private string get_str()// прибавляем строки
        {
            string ret = "";
            for (int i = 0; i < not_done.Count; i++)
                ret += Convert.ToString(i + 1) + ". " + (string)not_done[i] + "\r\n";
            return (ret);
        }

        public override string get_list(DateTime date)// получаем список тасков которые еще не выполнены
        {
            string ret = "";
            if (lastdate == DateTime.Today)
            {
                ret = get_str();
            }
            else
            {
                lastdate = DateTime.Today;
                not_done = new ArrayList();
                for (int i = 0; i < all_habits.Count; i++)
                    not_done.Add(all_habits[i]);
                ret = get_str();
            }
            return (ret);
        }
        public override bool set_new(DateTime date, string newstr)
        {
            all_habits.Add(newstr);
            not_done.Add(newstr);
            return true;
        }
        public override bool delete_one(DateTime date, string index)
        {
            if (not_done.Count + 1 > Convert.ToInt16(index))
            {
                string forcmp = (string)not_done[Convert.ToInt16(index) - 1];
                for (int i = 0; i < all_habits.Count; i++)
                {
                    if (((string)(all_habits[i])).CompareTo(forcmp) == 1)
                    {
                        all_habits.RemoveAt(i);
                        break;
                    }
                }
                not_done.RemoveAt(Convert.ToInt16(index) - 1);
            }
            else
            {
                MessageBox.Show("No habits with this id for this day");
                return false;
            }
            return true;
        }
        public bool done_one(DateTime date, string index)
        {
            if (not_done.Count + 1 > Convert.ToInt16(index) && Convert.ToInt16(index) >= 1)
            {
                not_done.RemoveAt(Convert.ToInt16(index) - 1);
            }
            else
            {
                MessageBox.Show("No habits with this id for this day");
                return false;
            }
            return true;
        }
    }
    [Serializable]
    public class alert
    {
        DateTime date;
        string al_str;
        public alert()
        {
            date = DateTime.MaxValue;   
        }
        public void set_alert(string str, int year, int month, int day, int hour, int minute, int seconds)
        {
            al_str = str;
            date = new DateTime(year, month, day, hour, minute, seconds);
        }
        public string get_alert()
        {
            return (al_str);
        }
        public bool is_date()
        {
            return (date < DateTime.Now);
        }
        public bool is_changed()
        {
            return (date != DateTime.MaxValue);
        }
        public int dif()
        {
            return ((date - DateTime.Now).Milliseconds);
        }
    }

    public class status_calc
    {
        public static list_day my_list_day;
        public static list_week my_list_week;
        public static list_habits my_list_habits;
        public static alert al;
        public static DateTime cur_date = DateTime.Today;
        public static string retstr = "";
        public static int form3task = 0;
    }
}
