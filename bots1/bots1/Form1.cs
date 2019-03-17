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

namespace bots1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        int size;//размер карты
        bot1 []b;//массив ботов
        bool is_life=false;//проверяет, все ли умерли
        string path;
        static int[,] map;//карта
        /*индексация по карте:  0 - яд
                                1 - приграда
                                2 - бот
                                3 - еда
                                4 - пусто
                                */
        //что если значение индексов будет переменно?
        //

        private void button1_Click(object sender, EventArgs e)
        {
            //Start
            //path = System.IO.Path.GetFullPath("code.txt");
            //if (File.Exists(path) == false)
            //    File.Create(path);
            //else
            //{

            //}
            
            if(button1.Text=="Start")
            {
                if (is_life == false)
                {
                    b = new bot1[64];
                }
                else if (is_life == true)
                {
                    button1.Text = "Stop";
                    timer1.Enabled = true;
                }
            }
            else
            {
                button1.Text = "Start";
                timer1.Enabled = false;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {//Add bot
            //bot1 b = new bot1();
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //Refresh
            is_life = false;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 0)
                size = 20;
            if (comboBox1.SelectedIndex == 1)
                size = 50;
            if (comboBox1.SelectedIndex == 2)
                size = 100;
            if (comboBox1.SelectedIndex == 3)
                size = 200;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.Items.Add("20 x 20");
            comboBox1.Items.Add("50 x 50");
            comboBox1.Items.Add("100 x 100");
            comboBox1.Items.Add("200 x 200");
            comboBox1.SelectedItem = 0;
            comboBox1.Enabled = false;
            size = 20;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            int count = 0;//считает количество 
            for(int i=0;i<64;i++)
            {                
                if(b[i].alife)
                {
                    count++;
                    b[i].next_action();
                }
            }
            if(count<11)
            {
                for (int i = 0; i < 64; i++)
                {
                    b[i].new_generation();
                }
            }
        }


        
        class bot1
        {
            //blue
            public int life;//счетчик количества жизни
            public bool alife;//проверяем, жив ли бот
            public int x0, y0;//координаты данного бота
            private int i;//
            private int[] a;//массив последовательности действий
            private int direction;//направление, куда смотрит
            public bot1 (int[] array,bool mutation)
            {
                life = 50;
                a = array;
                alife = true;
                i = 0;
                direction = 0;
                /*
                 7  0  1
                 6  *  2
                 5  4  3
                 */
                if(mutation)
                {

                }
            }
            public void next_action()
            {
                life--;
                i++;
                go:
                if (i > 63)
                    i -= 64;
                /*
                     * i: 0 1 2 3 4 5 6 7 //остаток от деления i%8
                     *    ↓ ↓ ↓ ↓ ↓ ↓ ↓ ↓
                     * x: 0 + + + 0 - - - //просмотр по коорд. X на x0+x
                     * y: + + 0 - - - 0 + //просмотр по коорд. Y на y0+y
                 */
                if (a[i] < 8)
                {
                    //посмотреть
                    //i += (map[x0 + getX(a[i]), y0 + getY(a[i])]);
                    i += (map[x0 + getX(direction), y0 + getY(direction)]) + 1;
                    //если бот перед собой видет 
                }
                else if (a[i] < 16)
                {
                    //идти
                    x0 += getX(direction);
                    y0 += getY(direction);
                }
                else if (a[i] < 24)
                {
                    //взять
                    take(getX(direction), getY(direction));
                }
                else if (a[i] < 32)
                {
                    //преобразовать
                    redo(getX(direction), getY(direction));
                }
                else if (a[i] < 40)
                {
                    //поворот
                    direction += a[i] % 8;
                }
                else
                {
                    //безусловный переход нa a[i]
                    i += a[i];
                    goto go;
                }
            }

            private int getX(int index)
            {
                int ret;
                if (index == 0 || index == 4)
                {
                    ret = 0;
                }
                else if (index > 4)
                {
                    ret = -1;
                }
                else
                {
                    ret = +1;
                }
                return ret;
            }
            private int getY(int index)
            {
                int ret;
                index++;
                if (index == 3 || index == 7)
                {
                    ret = 0;
                }
                else if (index % 8 < 3)
                {
                    ret = 1;
                }
                else
                {
                    ret = -1;
                }
                return ret;
            }
            
            
            public void new_generation()
            {
                
            }

            //список действий:
            private void look()
            {
                if (map[x0 + getX(direction), y0 + getY(direction)] == 0)
                {//яд

                }
                if (map[x0 + getX(direction), y0 + getY(direction)] == 1)
                {//стена

                }
                if (map[x0 + getX(direction), y0 + getY(direction)] == 2)
                {//бот

                }
                if (map[x0 + getX(direction), y0 + getY(direction)] == 3)
                {//еда

                }
                if (map[x0 + getX(direction), y0 + getY(direction)] == 4)
                {//пусто

                }
            }
            private void take(int x,int y)
            {
                if (map[x0 + getX(a[i]), y0 + getY(a[i])] == 0)
                {//если "взять" яд, тогда жизнь бота уменьшается на 5
                    life -= 5;
                }
                else if (map[x0 + getX(a[i]), y0 + getY(a[i])] == 2)
                {//если "взять" другого бота, тогда жизнь этих ботов распределяется между ними по средней арифм.
                        
                }
                else if (map[x0 + getX(a[i]), y0 + getY(a[i])] == 3)
                {//если "взять" еду, тогда жизнь бота возрастает на 5
                    life += 5;
                }

            }
            private void redo(int x, int y)
            {
                if (map[x0 + getX(a[i]), y0 + getY(a[i])] == 0)
                {//если "переделать" яд, тогда он превратится в еду
                    map[x0 + getX(a[i]), y0 + getY(a[i])] = 3;
                }
                else if (map[x0 + getX(a[i]), y0 + getY(a[i])] == 3)
                {//если "переделать" еду, тогда он превратится в яд
                    map[x0 + getX(a[i]), y0 + getY(a[i])] = 0;
                }
                //если "переделать" стену, другого бота или пустоту тогда ничего не произойдет
            }
        }

        class food
        {//red
            public food(int s)
            {

            }
            public int life;
            private int x, y;
        }
        
    }
}
