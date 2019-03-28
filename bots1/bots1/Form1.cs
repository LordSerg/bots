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
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using OpenTK.Platform.Windows;

namespace bots1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public static int size;//размер карты
        bot1 []b;//массив ботов
        //bool is_life=false;//проверяет, все ли умерли
        string path;
        public static int[,] map;//карта
        int n_of_kinds_of_code = 0;//счетчик видов ДНК
        int[][] n_of_k_of_code;
        public static int yad;//кол-во яда на поле
        public static int food;//кол-во еды на поле
        int []c;//код
                /*индексация по карте:  0 - ничего
                                        1 - приграда
                                        2 - яд
                                        3 - еда
                                        4 - бот
                                        */
        //что если значение индексов будет переменно?
        
        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.Items.Add("20 x 20");
            //comboBox1.Items.Add("50 x 50");
            //comboBox1.Items.Add("100 x 100");
            //comboBox1.Items.Add("200 x 200");
            comboBox1.SelectedItem = 0;
            comboBox1.Enabled = false;
            size = 20;
            c = new int[64];
            GL.ClearColor(Color.Black);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            glControl1.SwapBuffers();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Start
            listBox1.Items.Clear();
            for (int i = 0; i < 65; i++)
                listBox1.Items.Add("");
            path = System.IO.Path.GetFullPath("code.txt");
            Random rnd = new Random();
            if (File.Exists(path) == false)
            {
                //File.Create(path);
                for (int i = 0; i < 64; i++)
                {
                    c[i] = rnd.Next(0, 64);
                }
                FileStream x = File.Create(path);
                StreamWriter w = new StreamWriter(x);
                string st = null;
                for (int i = 0; i < 64; i++)
                    st += c[i].ToString() + " ";
                w.WriteLine(st);
                w.Close();
                x.Close();
            }
            else if(button1.Text=="Start")
            {
                StreamReader r = new StreamReader(path);
                string[] s;
                s = new string[64];
                while (r.Peek() >= 0)
                    s = r.ReadLine().Split(' ');
                r.Close();
                for (int i = 0; i < 64; i++)
                    c[i] = Convert.ToInt32(s[i]);
            }

            if (button1.Text == "Start")
            {
                button1.Text = "Stop";
                //check map
                map = new int[size, size];
                map = map_maker("n1", size);
                int x, y, i = 0;
                b = new bot1[64];
                while (i < 64)//расставляем ботов
                {
                    x = rnd.Next(size);
                    y = rnd.Next(size);
                    if (map[x, y] == 0)
                    {
                        map[x, y] = 4;
                        b[i] = new bot1(c);
                        b[i].x0 = x;
                        b[i].y0 = y;
                        b[i].id = i;
                        i++;
                    }
                }
                i = 0;
                while (i < 20)//расставляем еду
                {
                    x = rnd.Next(size);
                    y = rnd.Next(size);
                    if (map[x, y] == 0)
                    {
                        map[x, y] = 3;
                        i++;
                    }
                }
                i = 0;
                while (i < 10)//расставляем яд
                {
                    x = rnd.Next(size);
                    y = rnd.Next(size);
                    if (map[x, y] == 0)
                    {
                        map[x, y] = 2;
                        i++;
                    }
                }
                timer1.Enabled = true;
            }
            else
            {
                button1.Text = "Start";
                timer1.Enabled = false;
                FileStream x = File.Create(path);
                StreamWriter w = new StreamWriter(x);
                string st = null;
                for (int i = 0; i < 64; i++)
                    st += c[i].ToString() + " ";
                w.WriteLine(st);
                w.Close();
                x.Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {//Choose map
            
        }

        private void button3_Click(object sender, EventArgs e)
        {//Refresh
            //is_life = false;
            for (int i = 0; i < 64; i++)
                b[i].alife = false;
            timer1.Enabled = false;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 0)
                size = 20;
            //if (comboBox1.SelectedIndex == 1)
            //    size = 50;
            //if (comboBox1.SelectedIndex == 2)
            //    size = 100;
            //if (comboBox1.SelectedIndex == 3)
            //    size = 200;
        }
        
        private void timer1_Tick(object sender, EventArgs e)
        {
            int x, y,i = 0;
            Random rnd = new Random();
            int count = 0;//считает количество живых
            food = 0;
            yad = 0;
            for (int u = 0; u < size; u++)
                for (int w = 0; w < size; w++)
                {
                    if (map[u, w] == 4)
                        count++;
                    if (map[u, w] == 3)
                        food++;
                    if (map[u, w] == 2)
                        yad++;
                }
            for (i = 0; i < 64; i++)
            {
                if (b[i].alife)
                {
                    b[i].next_action();
                    //if (b[i].alife)
                    //    count++;
                }
            }
            i = 0;
            label2.Text = "Num. of bots (now): " + count;
            if (count < 9)
            {//реинкорнация
                //проверяем, какой извидов ботов выжили 
                //(проверка на одинаковость генома)
                //****************
                //int[][] alife;
                //alife = new int[count][];
                //n_of_k_of_code = new int[count][];
                //int v = 0;
                //n_of_kinds_of_code = 0;
                //for (v = 0; v < count; v++)
                //{
                //    alife[v] = new int[64];
                //    n_of_k_of_code[v] = new int[64];
                //}
                //v = 0;
                //for (i = 0; i < 64; i++)
                //    if (b[i].alife)
                //    {
                //        alife[v] =  b[i].a;
                //        v++;
                //    }
                //if (count > 0)
                //{
                //    num_of_kinds_of_code(alife, count);
                //    c = n_of_k_of_code[n_of_kinds_of_code-1];
                //}
                //else
                //{
                //    //n_of_kinds_of_code = 1;
                //    //n_of_k_of_code = new int[1][];
                //    //n_of_k_of_code[0] = new int[64];
                //    //n_of_k_of_code[0] = c;
                //}
                //*****************
                for (int v = 0; v < 64; v++)
                    if (b[v].alife)
                        c = b[v].a;
                //убираем старых ботов
                map_maker("n1", size);
                
                //размещаем все разные виды заново no:(поравну каждого оставшегося вида)
                i = 0;
                int mut_i, mut;
                mut = rnd.Next(0,64);
                mut_i = rnd.Next(0,64);
                //for (int q = 0; q < n_of_kinds_of_code; q++)
                //{
                    while (i < /*(64/n_of_kinds_of_code)*(q+1)*/64)
                    {
                        x = rnd.Next(size);
                        y = rnd.Next(size);
                        if (map[x, y] == 0)
                        {
                            map[x, y] = 4;
                            if (i>= /*(64 / n_of_kinds_of_code) * (q + 1)-8*/56)
                            {
                                c[mut_i]=mut;
                            }
                            b[i] = new bot1(c);
                            b[i].x0 = x;
                            b[i].y0 = y;
                            b[i].id = i;
                            //добавляем мутантов
                            i++;
                        }
                    //}
                    //i = (64 / n_of_kinds_of_code) * (q + 1);
                }
            }
            //проверяем количество еды и яда на поле 
            if (yad < 5)
                for (int xx = 0; xx < 2; xx++)
                {
                    x = rnd.Next(size);
                    y = rnd.Next(size);
                    if (map[x, y] == 0)
                    {
                        map[x, y] = 2;
                        yad++;
                    }
                    else
                    {
                        xx--;
                    }
                }
            if (food < 15)
                for (int xx = 0; xx < 5; xx++)
                {
                    x = rnd.Next(size);
                    y = rnd.Next(size);
                    if (map[x, y] == 0)
                    {
                        map[x, y] = 3;
                        food++;
                    }
                    else
                    {
                        xx--;
                    }
                }
            
            //записываем отчет о каждом:
            //listBox1.Items.Clear();
            listBox1.Items[0]=("id\t|life\t|x\t|y");
            i = 0;
            while(i<64)
            {
                if (b[i].alife)
                    listBox1.Items[i+1]=(i + "\t|" + b[i].life + "\t|" + b[i].x0 + "\t|" + b[i].y0);
                else
                    listBox1.Items[i+1]=(i + "\t| I AM DEAD!!!");
                i++;
            }
            //рисуем
            GL.ClearColor(Color.Black);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            double o = 0.1;//для карты 20*20  о=0.1
            int x_i = 0, y_j = 0;
            for (double u = -1; u <= 1-o; u += o, x_i++)
            {
                y_j = 0;
                for (double j = 1; j >= -1+o; j -= o, y_j++)
                {
                    //GL.Begin(PrimitiveType.Lines);
                    //GL.Color3(Color.Black);
                    //GL.Vertex2(u, j);
                    //GL.Vertex2(u + o, j);
                    //GL.Vertex2(u + o, j);
                    //GL.Vertex2(u + o, j - o);
                    //GL.Vertex2(u + o, j - o);
                    //GL.Vertex2(u, j - o);
                    //GL.Vertex2(u, j - o);
                    //GL.Vertex2(u, j);
                    //GL.End();
                    GL.Begin(PrimitiveType.Polygon);
                    if (map[x_i, y_j] == 0)
                        GL.Color3(Color.White);
                    else if (map[x_i, y_j] == 1)
                        GL.Color3(Color.Gray);
                    else if (map[x_i, y_j] == 2)
                        GL.Color3(Color.Red);
                    else if (map[x_i, y_j] == 3)
                        GL.Color3(Color.Green);
                    else
                        GL.Color3(Color.Blue);
                    GL.Vertex2(u, j);
                    GL.Vertex2(u + o, j);
                    GL.Vertex2(u + o, j - o);
                    GL.Vertex2(u, j - o);
                    GL.End();
                }
            }
            glControl1.SwapBuffers();
        }

        void num_of_kinds_of_code(int [][]a,int num)
        {
            int[] k = new int[num];
            //int[] x = a[0];
            for (int i = 1; i < num; i++)
            {
                if (a[0] == a[i])
                    k[i] = 0;
                else
                    k[i] = 1;
            }
            n_of_k_of_code[n_of_kinds_of_code] = a[0];
            n_of_kinds_of_code++;
            int v=0;
            for(int i=0;i<num;i++)
                if (k[i] == 1)
                {
                    a[v] = a[i];
                    v++;
                }
            if(v>0)
            {
                num_of_kinds_of_code(a, v);
            }
        }

        int[,] map_maker(string name, int size)
        {//name is for finding txt-map path
            int[,] m;
            m = new int[size, size];
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                {
                    if (map[i, j] != 3 && map[i, j] != 2 && map[i,j]!=1)
                    {
                        map[i, j] = 0;
                    }
                }

            if (name == "n1" && size == 20)
            {
                //verticals:
                for (int j = 0; j < 5; j++)
                    m[12, j] = 1;
                for (int j = 2; j < 8; j++)
                    m[3, j] = 1;
                for (int j = 10; j < 15; j++)
                    m[11, j] = 1;
                for (int j = 14; j < 19; j++)
                    m[5, j] = 1;
                for (int j = 17; j < 20; j++)
                    m[12, j] = 1;
                //horizontales:
                for (int i = 0; i < 4; i++)
                    m[i, 2] = 1;
                for (int i = 15; i < 20; i++)
                    m[i, 2] = 1;
                for (int i = 3; i < 8; i++)
                    m[i, 7] = 1;
                for (int i = 7; i < 16; i++)
                    m[i, 10] = 1;
                for (int i = 5; i < 12; i++)
                    m[i, 14] = 1;
                for (int i = 0; i < 3; i++)
                    m[i, 17] = 1;
                for (int i = 12; i < 20; i++)
                    m[i, 17] = 1;
                /*
             0  0  0  0  0  0  0  0  0  0  0  0  1  0  0  0  0  0  0  0
             0  0  0  0  0  0  0  0  0  0  0  0  1  0  0  0  0  0  0  0
             1  1  1  1  0  0  0  0  0  0  0  0  1  0  0  1  1  1  1  1
             0  0  0  1  0  0  0  0  0  0  0  0  1  0  0  0  0  0  0  0
             0  0  0  1  0  0  0  0  0  0  0  0  1  0  0  0  0  0  0  0
             0  0  0  1  0  0  0  0  0  0  0  0  0  0  0  0  0  0  0  0
             0  0  0  1  0  0  0  0  0  0  0  0  0  0  0  0  0  0  0  0
             0  0  0  1  1  1  1  1  0  0  0  0  0  0  0  0  0  0  0  0
             0  0  0  0  0  0  0  0  0  0  0  0  0  0  0  0  0  0  0  0
             0  0  0  0  0  0  0  0  0  0  0  0  0  0  0  0  0  0  0  0
             0  0  0  0  0  0  0  1  1  1  1  1  1  1  1  1  0  0  0  0
             0  0  0  0  0  0  0  0  0  0  0  1  0  0  0  0  0  0  0  0
             0  0  0  0  0  0  0  0  0  0  0  1  0  0  0  0  0  0  0  0
             0  0  0  0  0  0  0  0  0  0  0  1  0  0  0  0  0  0  0  0
             0  0  0  0  0  1  1  1  1  1  1  1  0  0  0  0  0  0  0  0
             0  0  0  0  0  1  0  0  0  0  0  0  0  0  0  0  0  0  0  0
             0  0  0  0  0  1  0  0  0  0  0  0  0  0  0  0  0  0  0  0
             1  1  1  0  0  1  0  0  0  0  0  0  1  1  1  1  1  1  1  1
             0  0  0  0  0  1  0  0  0  0  0  0  1  0  0  0  0  0  0  0
             0  0  0  0  0  0  0  0  0  0  0  0  1  0  0  0  0  0  0  0
             */
            }
            return m;
        }
        
        class bot1
        {
            //blue
            public int life;//счетчик количества жизни
            public bool alife;//проверяем, жив ли бот
            public int x0, y0;//координаты данного бота
            public int id;//порядковый номер бота
            private int index;//счетчик итерации кода
            public int[] a;//массив последовательности действий или же ДНК
            private int direction;//направление, куда смотрит
            public bot1 (int[] array)
            {
                life = 100;
                a = array;
                alife = true;
                index = 0;
                direction = 0;
            }

            public void next_action()
            {
                life--;
            go:
                if (index > 63)
                    index -= 64;
                /*
                     * i: 0 1 2 3 4 5 6 7 //остаток от деления i%8
                     *    ↓ ↓ ↓ ↓ ↓ ↓ ↓ ↓
                     * x: 0 + + + 0 - - - //просмотр по коорд. X на x0+x
                     * y: + + 0 - - - 0 + //просмотр по коорд. Y на y0+y
                 */
                if (life > 0 && alife == true)
                {//если живой
                    //int getx = getX(direction);
                    //int gety = getY(direction);
                    if (a[index] < 8)
                    {
                        //посмотреть
                        //i += (map[x0 + getX(a[i]), y0 + getY(a[i])]);
                        index += (map[conv_map(x0 + getX(direction),size), conv_map(y0 + getY(direction),size)]) + 1;
                        //если бот перед собой видет
                    }
                    else if (a[index] < 16)
                    {
                        //идти
                        
                        if (map[conv_map(x0 + getX(a[index]+direction),size), conv_map(y0 + getY(a[index]+direction),size)] == 0)
                        {
                            map[x0, y0] = 0;
                            x0 = conv_map(x0 + getX(a[index] + direction), size);
                            y0 = conv_map(y0 + getY(a[index] + direction), size);
                            map[x0, y0] = 4;
                            life++;
                        }
                        else
                        {
                            life--;
                        }
                    }
                    else if (a[index] < 24)
                    {
                        //взять
                        take(conv_map(getX(direction),size), conv_map(getY(direction),size));
                    }
                    else if (a[index] < 32)
                    {
                        //преобразовать
                        redo(conv_map(getX(direction), size), conv_map(getY(direction), size));
                    }
                    else if (a[index] < 40)
                    {
                        //поворот
                        direction += a[index] % 8;
                    }
                    else
                    {
                        //безусловный переход нa a[i]
                        index += a[index];
                        life--;
                        goto go;
                    }
                    
                    index++;
                }
                else
                {//если жизни закончились, то бот превращается пустоту
                    alife = false;
                    map[x0, y0] = 0;
                    //food++;
                }
            }

            private int getX(int i)
            {
                int ret;
                if (i == 0 || i == 4)
                {
                    ret = 0;
                }
                else if (i > 4)
                {
                    ret = -1;
                }
                else
                {
                    ret = +1;
                }
                return ret;
            }
            private int getY(int i)
            {
                int ret;
                i++;
                if (i == 3 || i == 7)
                {
                    ret = 0;
                }
                else if (i % 8 < 3)
                {
                    ret = 1;
                }
                else
                {
                    ret = -1;
                }
                return ret;
            }
            private int conv_map(int coord,int s)
            {
                int ret = coord;
                if (coord > s-1)
                    ret = 0;
                if (coord < 0)
                    ret = s - 1;
                return ret;
            }
            //список действий:
            private void take(int x,int y)
            {
                if (map[conv_map(x0 + getX(a[index]),size), conv_map(y0 + getY(a[index]),size)] == 2)
                {//если "взять" яд, тогда жизнь бота уменьшается на 5
                    life -= 10;
                    map[conv_map(x0 + getX(a[index]), size), conv_map(y0 + getY(a[index]), size)] = 0;
                    yad--;
                }
                else if (map[conv_map(x0 + getX(a[index]), size), conv_map(y0 + getY(a[index]), size)] == 3)
                {//если "взять" еду, тогда жизнь бота возрастает на 5
                    life += 20;
                    map[conv_map(x0 + getX(a[index]), size), conv_map(y0 + getY(a[index]), size)] = 0;
                    food--;
                }
                //else if (map[conv_map(x0 + getX(a[index]), size), conv_map(y0 + getY(a[index]), size)] == 4)
                //{//если "взять" другого бота, тогда жизнь этих ботов распределяется между ними по средней арифм.

                //}
            }
            private void redo(int x, int y)
            {
                if (map[conv_map(x0 + getX(a[index]),size), conv_map(y0 + getY(a[index]),size)] == 2)
                {//если "переделать" яд, тогда он превратится в еду
                    map[conv_map(x0 + getX(a[index]),size), conv_map(y0 + getY(a[index]),size)] = 3;
                    yad--;
                    food++;
                }
                else if (map[conv_map(x0 + getX(a[index]),size), conv_map(y0 + getY(a[index]),size)] == 3)
                {//если "переделать" еду, тогда он превратится в яд
                    map[conv_map(x0 + getX(a[index]),size), conv_map(y0 + getY(a[index]),size)] = 2;
                    food--;
                    yad++;
                }
                //если "переделать" стену, другого бота или пустоту тогда ничего не произойдет
            }
        }
    }
}
