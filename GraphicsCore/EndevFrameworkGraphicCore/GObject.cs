using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EndevFrameworkGraphicCore
{
    public enum HoverType
    {
        Border,
        Fill,
    }

    public abstract class GObject
    {
        public static int HoverBorderWidth { get; set; } = 2;
        public static Color HoverColor { get; set; } = Color.Red;
        public static HoverType HoverType { get; set; } = HoverType.Border;

        public static bool LeftClicked { get; set; } = false;
        public static bool RightClicked { get; set; } = false;

        public Rectangle Bounds { get; set; } = new Rectangle(0, 0, 0, 0);
        public bool Hover { get; set; } = false;
        public bool DragEnabled { get; set; } = false;
        

        public int X
        {
            get => Bounds.X;
            set => Bounds = new Rectangle(value, Y, Width, Height);
        }
        public int Y
        {
            get => Bounds.Y;
            set => Bounds = new Rectangle(X, value, Width, Height);
        }
        public int XLast
        {
            get => X + Width;
        }
        public int YLast
        {
            get => Y + Height;
        }
        public int Width 
        {
            get => Bounds.Width;
            set => Bounds = new Rectangle(X, Y, value, Height);
        }
        public int Height
        {
            get => Bounds.Height;
            set => Bounds = new Rectangle(X, Y, Width, value);
        }

        public abstract void Tick(params object[] pParameters);
        public abstract void Render(Graphics g);
        public virtual void MouseMove(MouseEventArgs e)
        {
            if (e.X > X && e.X < XLast && e.Y > Y && e.Y < YLast)
            {
                Hover = true;

                if (DragEnabled && LeftClicked)
                {


                    X = e.X - XL;
                    Y = e.Y - YL;
                }
            }                
            else Hover = false;


            
        }

        private int XL = 0;
        private int YL = 0;
        public virtual void MouseDown(MouseEventArgs e)
        {
            XL = e.X - X;
            YL = e.Y - Y;
            
        }

        public virtual void MouseUp(MouseEventArgs e)
        {

        }

    }
}
