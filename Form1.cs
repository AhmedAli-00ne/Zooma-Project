using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Net.Http.Headers;
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
        Ball ShootingBall;
        bool ShootingProg = false;
        float XStart, XEnd, YStart, YEnd, DX, DY, M;
        int ExtraBall = 0;
        int TickCounter = 0;
        int HP = 100;
        bool Win = false;
        bool GameOver = false;
        int BallsShot = 0;
        int Score = 0;
        int WentIn = 0;
        public Form1()
        {
            InitializeComponent();
            this.Load += Form1_Load;
            this.Paint += Form1_Paint;
            T.Tick += T_Tick;
            this.MouseMove += Form1_MouseMove;
            this.MouseDown += Form1_MouseDown;
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Left && !ShootingProg)
            {
                Shoot(e.Location);
            }
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            float dx = e.X - (this.Width / 2);
            float dy = e.Y - (this.Height / 2);
            angle = (float)Math.Atan2(dy, dx);
        }

        private void T_Tick(object sender, EventArgs e)
        {
            if (!GameOver && !Win)
            {
                if (TickCounter % 25 == 0 && Balls.Count - ExtraBall + WentIn + Score - BallsShot <= 10)
                {
                    Balls.Add(new Ball(CurvePoints[0]));
                    Console.WriteLine(Balls.Count.ToString() + "  " + ExtraBall.ToString() + "  " + BallsShot.ToString());
                }
                MoveBalls();
                if (ShootingProg)
                {
                    MoveShootingBall();
                    ShootingBallCollision();
                }
                ChangeHero();
                CheckForGameOver();
                CheckForWin();
                TickCounter++;
                DoubleBuffer(this.CreateGraphics());
            }
            else if(GameOver)
            {
                DrawGameOver(this.CreateGraphics());
                T.Stop();
            }
            else if(Win)
            {
                DrawWin(this.CreateGraphics());
                T.Stop();
            }
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            DoubleBuffer(e.Graphics);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            offImage = new Bitmap(this.ClientSize.Width, this.ClientSize.Height);
            BackGround = new Bitmap(BackGround, new Size(this.ClientSize.Width, this.ClientSize.Height));
            float t_inc = 0.0001f;
            for (float t = 0.0f; t <= 1.0; t += t_inc)
            {
                PointF pnn = new PointF();
                pnn = MainCurve.CalcCurvePointAtTime(t);
                CurvePoints.Add(pnn);
            }
            Balls.Add(new Ball(CurvePoints[0]));
            ShootingBall = new Ball(new Point(295, 290));
            ShootingBall.pp = new Pen(Brushes.Blue);
            ChangeHero();
            T.Interval = 1;
            T.Start();
        }
        void MoveBalls()
        {
            if (Balls.Count > 0)
            {
                if (Balls[Balls.Count - 1].CurveIndex + 3 < CurvePoints.Count)
                {
                    Balls[Balls.Count - 1].ChangePosition(CurvePoints[Balls[Balls.Count - 1].CurveIndex]);
                    Balls[Balls.Count - 1].CurveIndex += 3;
                }
                else
                {
                    Balls.Clear();
                    HP -= 10;
                    WentIn++;
                    CheckForWin();
                    CheckForGameOver();
                }
                for (int i = Balls.Count - 2; i >= 0; i--)
                {
                    if (Balls[i].CurveIndex > Balls[i + 1].CurveIndex + 65)
                    {
                        continue;
                    }
                    else
                    {
                        if (Balls[i].CurveIndex + 3 < CurvePoints.Count)
                        {
                            Balls[i].ChangePosition(CurvePoints[Balls[i].CurveIndex]);
                            Balls[i].CurveIndex += 3;
                        }
                        else
                        {
                            Balls.RemoveAt(i);
                            HP -= 10;
                            WentIn++;
                        }
                    }

                }
            }
        }
        void ChangeHero()
        {
            if (!ShootingProg)
            {
                if (ShootingBall.Type == 0)
                {
                    ZumaHero = new Bitmap("Zuma Hero Blue.png");
                }
                else if (ShootingBall.Type == 1)
                {
                    ZumaHero = new Bitmap("Zuma Hero Red.png");
                }
                else if (ShootingBall.Type == 2)
                {
                    ZumaHero = new Bitmap("Zuma Hero Yellow.png");
                }
                else if (ShootingBall.Type == 3)
                {
                    ZumaHero = new Bitmap("Zuma Hero Green.png");
                }
            }
            else
            {
                ZumaHero = new Bitmap("Zuma Hero.png");
            }
            ZumaHero = new Bitmap(ZumaHero, new Size(200, 170));
            ZumaHero.MakeTransparent();
        }
        void Shoot(PointF p)
        {
            ShootingProg = true;
            XStart = 295;
            XEnd = p.X;
            YStart = 290;
            YEnd = p.Y;
            DX = XEnd - XStart;
            DY = YEnd - YStart;
            M = DY / DX;
        }
        void MoveShootingBall()
        {
            if (Math.Abs(DY) > Math.Abs(DX))
            {
                if (XStart < XEnd && YStart < YEnd)
                {
                    ShootingBall.Position.X += (1 / M)*4;
                    ShootingBall.Position.Y += 4;
                    ShootingBall.Rect = new RectangleF(ShootingBall.Position,ShootingBall.Rect.Size);
                }
                else if (XStart > XEnd && YStart > YEnd)
                {
                    ShootingBall.Position.X -= (1 / M) * 4;
                    ShootingBall.Position.Y -= 4;
                    ShootingBall.Rect = new RectangleF(ShootingBall.Position, ShootingBall.Rect.Size);

                }
                else if (XStart < XEnd && YStart > YEnd)
                {
                    ShootingBall.Position.X -= (1 / M) * 4;
                    ShootingBall.Position.Y -= 4;
                    ShootingBall.Rect = new RectangleF(ShootingBall.Position, ShootingBall.Rect.Size);
                }
                else if (XStart > XEnd && YStart < YEnd)
                {
                    ShootingBall.Position.X += (1 / M) * 4;
                    ShootingBall.Position.Y += 4;
                    ShootingBall.Rect = new RectangleF(ShootingBall.Position, ShootingBall.Rect.Size);
                }
            }
            else
            {
                if (XStart < XEnd && YStart < YEnd)
                {
                    ShootingBall.Position.X += 4;
                    ShootingBall.Position.Y += M * 4;
                    ShootingBall.Rect = new RectangleF(ShootingBall.Position, ShootingBall.Rect.Size);
                }
                else if (XStart > XEnd && YStart > YEnd)
                {
                    ShootingBall.Position.X -= 4;
                    ShootingBall.Position.Y -= M * 4;
                    ShootingBall.Rect = new RectangleF(ShootingBall.Position, ShootingBall.Rect.Size);
                }
                else if (XStart < XEnd && YStart > YEnd)
                {
                    ShootingBall.Position.X += 4;
                    ShootingBall.Position.Y += M * 4;
                    ShootingBall.Rect = new RectangleF(ShootingBall.Position, ShootingBall.Rect.Size);
                }
                else if (XStart > XEnd && YStart < YEnd)
                {
                    ShootingBall.Position.X -= 4;
                    ShootingBall.Position.Y -= M * 4;
                    ShootingBall.Rect = new RectangleF(ShootingBall.Position, ShootingBall.Rect.Size);
                }
            }
        }
        void ShootingBallCollision()
        {
            if(ShootingBall.Position.X > this.ClientSize.Width || ShootingBall.Position.X < 0)
            {
                ShootingProg = false;
                ShootingBall = new Ball(new Point(250, 250));
                ShootingBall.pp = new Pen(Brushes.Blue);
                return;
            }
            if(ShootingBall.Position.Y > this.ClientSize.Height || ShootingBall.Position.Y < 0)
            {
                ShootingProg = false;
                ShootingBall = new Ball(new Point(250, 250));
                ShootingBall.pp = new Pen(Brushes.Blue);
                return;
            }
            for(int i=0;i<Balls.Count;i++)
            {
                if (ShootingBall.Rect.IntersectsWith(Balls[i].Rect))
                {
                    Console.WriteLine("Made Intersection with ball : " + i.ToString());
                    ShootingBall.CurveIndex = Balls[i].CurveIndex;
                    for(int k =0;k<Balls.Count;k++)
                    {
                        Console.WriteLine(Balls[k].Type.ToString());
                    }
                    Console.WriteLine("Stop");
                    Balls.Insert(i, ShootingBall);
                    BallsShot++;
                    for (int k = 0; k < Balls.Count; k++)
                    {
                        Console.WriteLine(Balls[k].Type.ToString());
                    }
                    ShootingProg = false;
                    ShootingBall = new Ball(new Point(250, 250));
                    ShootingBall.pp = new Pen(Brushes.Blue);
                    FixSpacing(i);
                    return;
                }
            }
        }
        void FixSpacing(int index)
        {
            for(int i = index + 1; i <Balls.Count - 1;i++)
            {
                Balls[i].CurveIndex -= 60;
            }
            if (Balls[Balls.Count - 1].CurveIndex - 60 >= 0)
            {
                Balls[Balls.Count - 1].CurveIndex -= 60;
            }
            else
            {
                Balls.RemoveAt(Balls.Count - 1);
                ExtraBall++;
            }
            CheckForCollisionOfSameType(index);
        }
        void CheckForCollisionOfSameType(int index)
        {
            int left = index - 1;
            int right = index + 1;
            int leftindex = -1;
            int rightindex = -1;
            int StartRemove = -1;
            int EndRemove = -1;
            for(int i = left;i>0;i--)
            {
                if (Balls[i].Type == Balls[index].Type)
                {
                    leftindex = i;
                }
                else
                {
                    break;
                }
            }
            for(int i = right;i<Balls.Count;i++)
            {
                if (Balls[i].Type == Balls[index].Type)
                {
                    rightindex = i;
                }
                else
                {
                    break;
                }
            }
            if (leftindex != -1 && rightindex != -1)
            {
                StartRemove = leftindex;
                EndRemove = rightindex;
            }
            else if (leftindex != -1 && rightindex == -1)
            {
                StartRemove = leftindex;
                EndRemove = index;
            }
            else if (leftindex == -1 && rightindex != -1)
            {
                StartRemove = index;
                EndRemove = rightindex;
            }
            else
            {
                return;
            }
            if (Math.Abs(StartRemove - EndRemove) >= 2)
            {
                Score += Math.Abs(StartRemove - EndRemove) + 1;
                Balls.RemoveRange(StartRemove, Math.Abs(StartRemove - EndRemove) + 1);
            }
        }
        void CheckForGameOver()
        {
            if(HP <=0)
            {
                GameOver = true;
                return;
            }
        }
        void CheckForWin()
        {
            if (Balls.Count == 0 && HP>= 1)
            {
                Win = true;
                return;
            }
        }
        void DrawGameOver(Graphics g)
        {
            g.DrawString("Game Over", new Font("Arial", 50), Brushes.Red, new PointF(100, 100));
        }
        void DrawWin(Graphics g)
        {
            g.DrawString("You Win", new Font("Arial", 50), Brushes.Red, new PointF(100, 100));
        }
        void DrawScene(Graphics g)
        {
            g.Clear(Color.White);
            g.DrawImage(BackGround, 0, 0);
            for(int i=0;i<Balls.Count;i++)
            {
                Balls[i].Draw(g);
            }
            if(ShootingProg)
            {
                ShootingBall.Draw(g);
            }
            g.DrawString("Score : " + Score.ToString(), new Font("Arial", 20), Brushes.Red, new PointF(10, 10));
            g.DrawString("HP : ", new Font("Arial", 20), Brushes.Green, new PointF(500, 10));
            g.DrawRectangle(Pens.Black, 570, 15, 200, 20);
            for(int i = 0; i<HP/10;i+=1)
            {
                g.FillRectangle(Brushes.Green, 572 + (20 *i), 17, 15, 17);
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
