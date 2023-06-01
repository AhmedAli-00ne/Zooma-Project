using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrayNotify;

namespace Zooma_Project
{
    public partial class Form1 : Form
    {
        Bitmap offImage;
        Timer T = new Timer();
        float angle = 0;
        Bitmap BackGround = new Bitmap("Zuma 1.jpg");
        Bitmap ZumaHero = new Bitmap("Zuma Hero.png");
        Curve MainCurve = new Curve();
        List<PointF> CurvePoints = new List<PointF>();
        List<Ball> Balls = new List<Ball>();
        int TickCounter = 0;
        public Form1()
        {
            InitializeComponent();
            this.Load += Form1_Load;
            this.Paint += Form1_Paint;
            T.Tick += T_Tick;
            this.MouseMove += Form1_MouseMove;
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            float dx = e.X - (this.Width / 2);
            float dy = e.Y - (this.Height / 2);
            angle = (float)Math.Atan2(dy, dx);
        }

        private void T_Tick(object sender, EventArgs e)
        {
            if(TickCounter % 20 == 0 && Balls.Count <30)
            {
                Balls.Add(new Ball(CurvePoints[0]));
            }
            MoveBalls();
            TickCounter++;
            DoubleBuffer(this.CreateGraphics());
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            DoubleBuffer(e.Graphics);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            offImage = new Bitmap(this.ClientSize.Width, this.ClientSize.Height);
            BackGround = new Bitmap(BackGround, new Size(this.ClientSize.Width, this.ClientSize.Height));
            ZumaHero = new Bitmap(ZumaHero, new Size(200, 170));
            ZumaHero.MakeTransparent();
            float t_inc = 0.0001f;
            for (float t = 0.0f; t <= 1.0; t += t_inc)
            {
                PointF pnn = new PointF();
                pnn = MainCurve.CalcCurvePointAtTime(t);
                CurvePoints.Add(pnn);
            }
            Balls.Add(new Ball(CurvePoints[0]));
            T.Interval = 1;
            T.Start();
        }
        void RotateHeroToCursor(Point CursorLocation)
        {
            
        }
        void MoveBalls()
        {
            for (int i = 0; i < Balls.Count; i++)
            {
                Balls[i].ChangePosition(CurvePoints[Balls[i].CurveIndex]);
                Balls[i].CurveIndex+=3;
            }
        }
        void DrawScene(Graphics g)
        {
            g.Clear(Color.White);
            g.DrawImage(BackGround, 0, 0);
            MainCurve.DrawCurve(g);
            for(int i=0;i<Balls.Count;i++)
            {
                Balls[i].Draw(g);
            }
            g.TranslateTransform(295, 290);
            g.RotateTransform(angle * 180 / (float)Math.PI);
            g.DrawImage(ZumaHero, -ZumaHero.Width / 2, -ZumaHero.Height / 2);
            g.TranslateTransform(-295, -290);
        }
        void DoubleBuffer(Graphics g)
        {
            Graphics g2 = Graphics.FromImage(offImage);
            DrawScene(g2);
            g.DrawImage(offImage, 0, 0);
        }
    }
}
