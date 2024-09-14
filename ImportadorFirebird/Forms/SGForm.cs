using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImportadorFirebird.Forms
{
    public partial class SGForm : Form
    {
        [DllImport("DwmApi")]


        private static extern int DwmSetWindowAttribute(IntPtr hwn, int attr, int[] attrValue, int attrSize);

        protected override void OnHandleCreated(EventArgs e)
        {
            if (DwmSetWindowAttribute(Handle, 19, new[] { 1 }, 4) != 0)
            {
                DwmSetWindowAttribute(Handle, 20, new[] { 1 }, 4);
            }
        }
        public SGForm()
        {
            InitializeComponent();
        }
    }
}
