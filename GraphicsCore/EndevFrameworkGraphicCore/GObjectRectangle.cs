using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EndevFrameworkGraphicCore
{
    class GObjectRectangle : GObjectBasic
    {
        public GObjectRectangle(Rectangle pRectangle, Color pColor, bool pFill = true, bool pEnableDrag = false)
        {
            Bounds = pRectangle;
            Color = pColor;
            Fill = pFill;
            DragEnabled = pEnableDrag;
        }
        public override void MouseDown(MouseEventArgs e)
        {
            base.MouseDown(e);
        }

        public override void MouseMove(MouseEventArgs e)
        {
            base.MouseMove(e);
        }

        public override void MouseUp(MouseEventArgs e)
        {
            base.MouseUp(e);
        }

        public override void Render(Graphics g)
        {  
            if (Fill) g.FillRectangle(new SolidBrush(Color), Bounds);
            else g.DrawRectangle(new Pen(Color), Bounds);

            if (Hover)
            {
                if (HoverType == HoverType.Border)
                {
                    g.FillRectangle(new SolidBrush(HoverColor), new Rectangle(X - HoverBorderWidth, Y - HoverBorderWidth, Width + 2 * HoverBorderWidth, Height + 2 * HoverBorderWidth));
                    g.FillRectangle(new SolidBrush(Color), Bounds);
                }
                if (HoverType == HoverType.Fill)
                {
                    g.FillRectangle(new SolidBrush(HoverColor), Bounds);
                }
            }
        }

        public override void Tick(params object[] pParameters)
        {
            
        }
    }
}
