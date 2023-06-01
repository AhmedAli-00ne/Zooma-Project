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
        Bitmap Img;
        PointF Position;
        int Type;
        public int CurveIndex;
        Random R = new Random();
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
        }
        public void ChangePosition(PointF pos)
        {
            PointF p = new PointF();

            p.X = pos.X - 5;
            p.Y = pos.Y - 5;
            Position = p;
        }
        public void Draw(Graphics g)
        {
            g.DrawImage(Img, Position);
        }
    }
}
