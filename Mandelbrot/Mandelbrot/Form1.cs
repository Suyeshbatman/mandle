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
using System.Xml.Linq;
using System.Xml;
using System.IO;

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
      
        Rectangle rect = new Rectangle(0, 0, 0, 0);
        private int j;

        


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
                if ((w < 2) && (z < 2))
                {
                    initvalues();
                }
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
            try
            { 
            int x, y;
            float h, b, alt = 0.0f;
            Pen pen = new Pen(Color.White);

           // action = false;
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
                    pictureBox1.Image = picture;
                }
            }

            catch
            {
                
            }
            
        }

        private float pointcolour(double xwert, double ywert)
        {
            double r = 0.0, i = 0.0, m = 0.0;// real, imaginary, absolute value or distance
            int p;
            p = j;

            while ((p < MAX) && (m < 4.0))
            {
                p++;
                m = r * r - i * i; // x^2 - y^2
                i = 2.0 * r * i + ywert; // 2xy + c
                r = m + xwert;
            }
            return (float)p / (float)MAX;
        }

        private void initvalues()
        {
            xstart = SX;
            ystart = SY;
            xende = EX;
            yende = EY;
            if ((float)((xende - xstart) / (yende - ystart)) != xy)
            {
                xstart = xende - (yende - ystart) * (double)xy;
            }
              

        }

        private HSB HSBcol;

        public int[] Entries { get; private set; }

        private void Form1_Load(object sender, EventArgs e)
        {
            HSB hsb = new HSB();
        }

        private void greenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            j = 80;
            mandelbrot();
            Refresh();
        }

        private void yellowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            j = 20;
            mandelbrot();
            Refresh();
        }

        private void redToolStripMenuItem_Click(object sender, EventArgs e)
        {
            j = 0;
            mandelbrot();
            Refresh();
        }

        private void purpleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            j = 190;
            mandelbrot();
            Refresh();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            j = j + 1;
            mandelbrot();
            Refresh();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            timer1.Start();
            
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Created by Suyesh Bhatta", "Mandelbrot Fractal with Color Palette and Color cycling"
                );
        }

        private void button2_Click(object sender, EventArgs e)
        {
            timer1.Stop();
        }

        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            start();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog f = new SaveFileDialog();
            f.Filter = "JPG(*.JPG) | *.JPG";
            if (f.ShowDialog() == DialogResult.OK)
            {
                picture.Save(f.FileName);
            }
        }

        private void blueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            j = 150;
            mandelbrot();
            Refresh();
        }

        private void stateSaveToolStripMenuItem_Click(object sender, EventArgs e)
        {

            SaveFileDialog f = new SaveFileDialog();
            f.Filter = "XML Files(*.XML) | *.XML";
            if (f.ShowDialog() == DialogResult.OK)

                try
                {
                XmlWriter writer = XmlWriter.Create("state.xml");
                writer.WriteStartDocument();
                writer.WriteStartElement("states");
                writer.WriteElementString("xstart", xstart.ToString());
                writer.WriteElementString("ystart", ystart.ToString());
                writer.WriteElementString("xzoom", xzoom.ToString());
                writer.WriteElementString("yzoom", yzoom.ToString());
                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Flush();
                writer.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void start()
        {
            action = false;
            rectangle = false;
           initvalues();
              xzoom = (xende - xstart) / (double)x1;
              yzoom = (yende - ystart) / (double)y1;
              mandelbrot();


            String exists = "state.xml";
            if (File.Exists(exists))
            { 
            
           try
           {
                    XmlDocument state = new XmlDocument();
                    state.Load("state.xml");
                    foreach (XmlNode node in state)
                    {
                        xstart = Convert.ToDouble(node["xstart"]?.InnerText);
                        ystart = Convert.ToDouble(node["ystart"]?.InnerText);
                        xzoom = Convert.ToDouble(node["xzoom"]?.InnerText);
                        yzoom = Convert.ToDouble(node["yzoom"]?.InnerText);
                    }
                    mandelbrot();
                    Refresh();
                    
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
        }
            else
            {
                initvalues();
        xzoom = (xende - xstart) / (double) x1;
        yzoom = (yende - ystart) / (double) y1;
                mandelbrot();
    }

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

       

      

        

        private void restartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Restart();
        }

        private void reloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            String exists = "state.xml";
            if (File.Exists(exists))
            {
                File.Delete("state.xml");
            }
            pictureBox1.Image = null;
            
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
       
        
            

            start();
         }


        private void pictureBox1_paint(object sender, PaintEventArgs e)
        {
           
            Image tempPic = Image.FromHbitmap(picture.GetHbitmap());
            Graphics g1 = Graphics.FromImage(tempPic);

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
                g1.DrawRectangle(pen, rect);
                pictureBox1.Image = tempPic;

            }


        }

    }

    }


