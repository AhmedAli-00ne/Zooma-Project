using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zooma_Project
{
    internal class Ball
    {
        public Bitmap Img;
        public PointF Position;
        public RectangleF Rect;
        public int Type;
        public int CurveIndex;
        Random R = new Random();
        public Pen pp = new Pen(Brushes.Red);
        public Ball(PointF p)
        {
            Type = R.Next(0, 4);
            if(Type == 0)
            {
                Img = new Bitmap("BlueBall.png");
                Img = new Bitmap(Img, new Size(30, 30));
                Img.MakeTransparent();
            }
            else if(Type == 1)
            {
                Img = new Bitmap("RedBall.png");
                Img = new Bitmap(Img, new Size(30, 30));
                Img.MakeTransparent();
            }
            else if(Type == 2)
            {
                Img = new Bitmap("YellowBall.png");
                Img = new Bitmap(Img, new Size(30, 30));
                Img.MakeTransparent();
            }
            else
            {
                Img = new Bitmap("GreenBall.png");
                Img = new Bitmap(Img, new Size(30, 30));
                Img.MakeTransparent();
            }
            Position = p;
            CurveIndex = 0;
            Rect = new RectangleF(p, new Size(Img.Width, Img.Height));
        }
        public void ChangePosition(PointF pos)
        {
            PointF p = new PointF();

            p.X = pos.X - 5;
            p.Y = pos.Y - 5;
            Position = p;
            Rect = new RectangleF(p, new Size(Img.Width, Img.Height));
        }
        public void Draw(Graphics g)
        {
            g.DrawImage(Img, Position);
        }
        public void DrawRect(Graphics g)
        {
            g.DrawRectangle(pp, (int)Rect.X,(int)Rect.Y,(int)Rect.Width,(int)Rect.Height);
        }
    }
}
