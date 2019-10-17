using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EndevFrameworkGraphicCore
{
    class GObjectHandler
    {
        public List<GObject> GObjects = new List<GObject>();

        public GObject this[int idx]
        {
            get => GObjects[idx];
            set => GObjects.Insert(idx, value);
        }

        public void Add(GObject pObject)
        {
            GObjects.Add(pObject);
        }

        public void Tick(params object[] pParameters)
        {
            foreach (GObject obj in GObjects)
                obj.Tick(pParameters);
        }

        public void Render(Graphics g)
        {
            foreach (GObject obj in GObjects)
                obj.Render(g);
        }

        public void MouseMove(MouseEventArgs e)
        {
            foreach (GObject obj in GObjects)
                    obj.MouseMove(e);
        }

        public void MouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) GObject.LeftClicked = true;
            if (e.Button == MouseButtons.Right) GObject.RightClicked = true;

            foreach (GObject obj in GObjects)
                if (e.X > obj.X && e.X < obj.XLast &&
                e.Y > obj.Y && e.Y < obj.YLast)
                    obj.MouseDown(e);
        }

        public void MouseUp(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) GObject.LeftClicked = false;
            if (e.Button == MouseButtons.Right) GObject.RightClicked = false;

            foreach (GObject obj in GObjects)
                if (e.X > obj.X && e.X < obj.XLast &&
                e.Y > obj.Y && e.Y < obj.YLast)
                    obj.MouseUp(e);
        }
    }
}
