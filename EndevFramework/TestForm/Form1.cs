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

            

            bm.AddBinding(textBox1, "test");
            bm.AddBinding(metroTextBox1, "Text", typeof(string), "test2");

            bm.LoadBinding(@"C:\Users\zivi\Desktop\myIni.ini");

            bm.FillBindings();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            bm.SaveBindings(@"C:\Users\zivi\Desktop\myIni.ini");

        }
    }
}
