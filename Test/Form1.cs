using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ZD();
        }


        int n = 1;
        void ZD()
        {
            for (int i = 0; i < 20; i++)
            {
                this.Location = new Point(this.Location.X - 10*n,this.Location.Y - 10*n);
                n = n * -1;
                System.Threading.Thread.Sleep(40);
            }
        }
    }
}
