using ImportadorFirebird.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImportadorFirebird
{
    public partial class fImportador : Forms.SGForm
    {
        public fImportador()
        {
            InitializeComponent();
        }

        private void btnCriarBanco_Click(object sender, EventArgs e)
        {
            CriarBanco criarBanco = new CriarBanco();
            criarBanco.CreateDatabase();
        }
    }
}
