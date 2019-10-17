using EndevFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestForm
{
    public partial class Form1 : Form
    {
        private BindingManager bm = new BindingManager();

        public Form1()
        {
            InitializeComponent();

            bm.Bind(textBox1, "Text", "SimpleBind1");
            bm.Bind(listBox1, "Items", "DataBind3");
            

            bm.LoadFromFile(@"C:\Users\zivi\Desktop\test.ini");

            bm.FillBindings();
        }

        private void Button1_Click(object sender, EventArgs e)
        {

        }
    }
}
