using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Printing;
using Mandelbrot;

namespace Mandelbrot
{
    public partial class Form1 : Form
    {

        private const int MAX = 256;      // max iterations
        private const double SX = -2.025; // start value real
        private const double SY = -1.125; // start value imaginary
        private const double EX = 0.6;    // end value real
        private const double EY = 1.125;  // end value imaginary
        private static int x1, y1, xs, ys, xe, ye;
        private static double xstart, ystart, xende, yende, xzoom, yzoom;
        private static bool action, rectangle, finished;
        private static float xy;
        //private Image picture;
        private Bitmap picture;
        private bool mousedown = false;
        private Graphics g;
        private Graphics g1;
        private Cursor c1, c2;
        private Pen pen;
        private Rectangle rect;

        


        private void pictureBox1_mousedown(object sender, MouseEventArgs e)
        {
            //e.consume();
            if (action)
            {
                mousedown = true;
                xs = e.X;
                ys = e.Y;
            }
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            //e.consume();

            if (action && mousedown)
            {
                xe = e.X;
                ye = e.Y;
                rectangle = true;
                pictureBox1.Refresh();
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            int z, w;

            //e.consume();
            if (action)
            {
                xe = e.X;
                ye = e.Y;
                if (xs > xe)
                {
                    z = xs;
                    xs = xe;
                    xe = z;
                }
                if (ys > ye)
                {
                    z = ys;
                    ys = ye;
                    ye = z;
                }
                w = (xe - xs);
                z = (ye - ys);
                if ((w < 2) && (z < 2)) initvalues();
                else
                {
                    if (((float)w > (float)z * xy)) ye = (int)((float)ys + (float)w / xy);
                    else xe = (int)((float)xs + (float)z * xy);
                    xende = xstart + xzoom * (double)xe;
                    yende = ystart + yzoom * (double)ye;
                    xstart += xzoom * (double)xs;
                    ystart += yzoom * (double)ys;
                }
                xzoom = (xende - xstart) / (double)x1;
                yzoom = (yende - ystart) / (double)y1;
                mandelbrot();
                rectangle = false;
                pictureBox1.Refresh();
                mousedown = false;
            }
        }

        private void mandelbrot()
        {

            int x, y;
            float h, b, alt = 0.0f;
            Pen pen = new Pen(Color.White);

            action = false;
            //this.Cursor = c1; // in java setCursor(c1)
            pictureBox1.Cursor = c2;

            //showStatus("Mandelbrot-Set will be produced - please wait..."); will do later
            for (x = 0; x < x1; x += 2)
            {
                for (y = 0; y < y1; y++)
                {
                    h = pointcolour(xstart + xzoom * (double)x, ystart + yzoom * (double)y); // hue value

                    if (h != alt)
                    {
                        b = 1.0f - h * h; // brightness

                        HSBcol.fromHSB(h, 0.8f, b); //convert hsb to rgb then make a Java Color
                        Color col = Color.FromArgb(Convert.ToByte(HSBcol.rChan), Convert.ToByte(HSBcol.gChan), Convert.ToByte(HSBcol.bChan));

                        pen = new Pen(col);

                        //djm end
                        //djm added to convert to RGB from HSB

                        alt = h;
                    }
                    g1.DrawLine(pen, new Point(x, y), new Point(x + 1, y)); // drawing pixel
                }
                //showStatus("Mandelbrot-Set ready - please select zoom area with pressed mouse.");
                Cursor.Current = c1;
                action = true;
            }

            pictureBox1.Image = picture;
        }

        private float pointcolour(double xwert, double ywert)
        {
            double r = 0.0, i = 0.0, m = 0.0;// real, imaginary, absolute value or distance
            int j = 0;

            while ((j < MAX) && (m < 4.0))
            {
                j++;
                m = r * r - i * i; // x^2 - y^2
                i = 2.0 * r * i + ywert; // 2xy + c
                r = m + xwert;
            }
            return (float)j / (float)MAX;
        }

        private void initvalues()
        {
            xstart = SX;
            ystart = SY;
            xende = EX;
            yende = EY;
            if ((float)((xende - xstart) / (yende - ystart)) != xy)
                xstart = xende - (yende - ystart) * (double)xy;

        }

        private HSB HSBcol;
        //private object editToolStripMenuItem;

        private void Form1_Load(object sender, EventArgs e)
        {
            HSB hsb = new HSB();
        }


        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            start();
        }

        private void start()
        {
            action = false;
            rectangle = false;
            initvalues();
            xzoom = (xende - xstart) / (double)x1;
            yzoom = (yende - ystart) / (double)y1;
            mandelbrot();
        }

        public void destroy() // delete all instances 
        {
            if (finished)
            {

                picture = null;
                g1 = null;
                c1 = null;
                c2 = null;
            }
        }


        

        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            e.Graphics.DrawImage(pictureBox1.Image, 0, 0);
        }

       

        private void saveStripMenuItem1_Click(object sender, EventArgs e)
        {
            SaveFileDialog f = new SaveFileDialog();
            f.Filter = "JPG(*.JPG) | *.JPG";
            if (f.ShowDialog() == DialogResult.OK)
            {
                picture.Save(f.FileName);
            }
        }

        
        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Stop();
        }

        

        private void restartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Restart();
        }

        private void reloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Stop();
            start();
        }

        


        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Stop();
        }




        private void Stop()
        {
            pictureBox1.Image = null;
            pictureBox1.Invalidate();
        }

        public Form1()
        {
            InitializeComponent();

            HSBcol = new HSB();
            this.pictureBox1.Size = new System.Drawing.Size(640, 480); // equivalent of setSize in java code
            finished = false;
            c1 = Cursors.WaitCursor;
            c2 = Cursors.Cross;
            x1 = pictureBox1.Width;
            y1 = pictureBox1.Height;
            xy = (float)x1 / (float)y1;
            picture = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            g1 = Graphics.FromImage(picture);
       
            finished = true;
            //editToolStripMenuItem.Enabled = false; 

            start();
         }


        private void pictureBox1_paint(object sender, PaintEventArgs e)
        {
            Update();
        }

        public void Update()
        {
            Image tempPic = Image.FromHbitmap(picture.GetHbitmap());
            Graphics g = Graphics.FromImage(tempPic);

            if (rectangle)
            {
                Pen pen = new Pen(Color.White);

                Rectangle rect;

                if (xs < xe)
                {

                    if (ys < ye)
                    {
                        rect = new Rectangle(xs, ys, (xe - xs), (ye - ys));
                    }
                    else
                    {
                        rect = new Rectangle
                            (xs, ye, (xe - xs), (ys - ye));
                    }
                }
                else
                {
                    if (ys < ye)
                    {
                        rect = new Rectangle
                            (xe, ys, (xs - xe), (ye - ys));
                    }
                    else
                    {
                        rect = new Rectangle
                            (xe, ye, (xs - xe), (ys - ye));
                    }
                }
                g.DrawRectangle(pen, rect);
                pictureBox1.Image = tempPic;

            }


        }

    }

    }


