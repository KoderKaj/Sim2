using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace Sim2
{
    public partial class Form1 : Form
    {
        int maxX, maxY, count = 20;
        bool decreaseSpeed = true;
        List<Test> tests = new List<Test> { };
        List<Test> testRando = new List<Test>();
        List<Leader> leaders = new List<Leader>();
        List<Brush> colours = new List<Brush>()
        {
            Brushes.Firebrick,
            Brushes.Black,
            Brushes.Red,
            Brushes.Green,
            Brushes.Blue,
            Brushes.Magenta,
            Brushes.Yellow,
            Brushes.Navy
        };
        public Form1()
        {
            InitializeComponent();
            maxX = this.ClientSize.Width;
            maxY = this.ClientSize.Height;
            leaders.Add(new Leader(maxX / 2, maxY / 2));
            float a = 5;
            float scale = 0.1f;
            for (int i = 0; i < count; i++)
            {
                tests.Add(new Test(20, 20).setSpeed(a));
                tests[i].colour = colours[i%8];
                if (decreaseSpeed)
                {
                    a -= scale;
                }
            }
        }

        private void update_Tick(object sender, EventArgs e)
        {
            Test prev = leaders[0];//tests[tests.Count-1];
            var rand = new Random();
            foreach (Test test in tests)
            {
                if (test.move(prev))
                {
                    testRando.Add(test);
                    testRando.Add(prev);
                }
                prev = test;
            }
            if (testRando.Count > 0)
            {
                foreach (Test test in testRando)
                {
                    test.x = rand.Next(0, maxX);
                    test.y = rand.Next(0, maxY);
                }
                testRando.Clear();
            }
            leaders[0].move();
            this.Invalidate();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            foreach (Test test in tests)
            {
                g.FillEllipse(test.colour, new RectangleF(test.x, test.y, test.size, test.size));
            }
            Leader temp = leaders[0];
            g.FillEllipse(temp.colour, new RectangleF(temp.x, temp.y, temp.size, temp.size));
        }
    }
    public class Cell
    {
        public float x, y, speed, size;
        public Brush colour;
        public Cell()
        {

        }
    }
    public class Leader : Test
    {
        public float midX, midY, rad = 150, speedX, speedY;
        private bool up = false, right = true;
        public Leader(float newX, float newY) : base(newX, newY)
        {
            colour = Brushes.Purple;
            size = 30;
            speed = 1;
            midX = newX; 
            midY = newY;
            x = midX;
            y = midY;
        }
        float angle = 0;
        public void move()
        { 
            x = rad * (float)Math.Sin(angle) + midX;
            y = rad * (float)Math.Cos(angle) + midY;
            if (angle >= 360) { angle = 0; }
            else { angle += 0.034f; } //tan-1(target speed / radius) -> (float)Math.Atan2(5,150)
        }                   
    }
    public class Test : Cell
    {
        float socialDistancing = 50;
        public Test(float newX, float newY) : base()
        {
            size = 30;
            colour = Brushes.Aqua;
            speed = 10;
            x = newX;
            y = newY;
        }
        public Test setSpeed(float newSpeed)
        {
            speed = newSpeed;
            return this;
        }
        public bool move(Test target)
        {
            float diffX = (target.x + target.size / 2) - (x + size / 2);
            float diffY = (y + size / 2) - (target.y + target.size / 2);
            float magnitude = (float)Math.Sqrt(diffX * diffX + diffY * diffY);
            if (magnitude > target.size / 2)
            {
                diffX /= magnitude;
                diffY /= magnitude;
                float distance = magnitude - target.size / 2;
                if(distance > socialDistancing)
                {
                    x += diffX * speed;
                    y -= diffY * speed;
                }
                else if (distance > socialDistancing * 0.66)
                {
                    x += diffX * speed * 0.75f;
                    y -= diffY * speed * 0.75f;
                }
                else if (distance > socialDistancing * 0.33)
                {
                    x += diffX * speed * 0.5f;
                    y -= diffY * speed * 0.5f;
                }
                else
                {
                    x += diffX * speed * 0.25f;
                    y -= diffY * speed * 0.25f;
                }
                return false;
            }
            else
            {               //Collision
                return true;
            }
        }
    }
}
