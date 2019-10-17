using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EndevFrameworkGraphicCore
{
    public partial class Form1 : Form
    {
        private GObjectHandler gHandler; 

        public Form1()
        {
            InitializeComponent();

            GObject.HoverType = HoverType.Border;
            GObject.HoverColor = Color.Red;

            gHandler = new GObjectHandler();

            gHandler.Add(new GObjectRectangle(
                pRectangle:     new Rectangle(50, 50, 100, 200), 
                pColor:         Color.DarkOrange, 
                pEnableDrag:    true
            ));

            gHandler.Add(new GObjectRectangle(
                pRectangle: new Rectangle(200, 200, 200, 100),
                pColor: Color.LimeGreen,
                pEnableDrag: true
            ));
        }

        private void tmrTick_Tick(object sender, EventArgs e)
        {
            gHandler.Tick();
            pbxMain.Invalidate();
        }

        private void pbxMain_Paint(object sender, PaintEventArgs e) { gHandler.Render(e.Graphics); }
        private void pbxMain_MouseMove(object sender, MouseEventArgs e) { gHandler.MouseMove(e); }
        private void pbxMain_MouseDown(object sender, MouseEventArgs e) {  gHandler.MouseDown(e); }
        private void pbxMain_MouseUp(object sender, MouseEventArgs e) { gHandler.MouseUp(e); }
    }
}
