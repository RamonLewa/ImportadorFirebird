using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FirebirdSql.Data.FirebirdClient;
using System.IO;
using System.Threading.Tasks;
using FirebirdSql.Data.FirebirdClient;
using System.Windows.Forms;

namespace ImportadorFirebird.Classes
{
    public class CriarBanco
    {
        public async Task CreateDatabaseAsync()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Banco de dados (*.FDB)|*.FDB";
            saveFileDialog.Title = "Caminho banco de dados";
            saveFileDialog.FileName = "BASESGMASTER50.FDB";
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string caminhoArquivo = saveFileDialog.FileName;

                try
                {
                    if (File.Exists(caminhoArquivo))
                    {
                        DialogResult dialogResult = MessageBox.Show("O arquivo já existe. Deseja sobrescrevê-lo?", "Sobrescrever", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                        if (dialogResult == DialogResult.No)
                        {
                            return;
                        }
                        else
                        {
                            File.Delete(caminhoArquivo);
                        }
                    }

                    await Task.Run(() =>
                    {
                        FbConnectionStringBuilder builder = new FbConnectionStringBuilder();
                        builder.DataSource = "localhost";
                        builder.Database = caminhoArquivo;
                        builder.Port = 3051;
                        builder.UserID = "SYSDBA";
                        builder.Password = "masterkey";
                        builder.Charset = "UTF8";
                        builder.Dialect = 3;
                        builder.ConnectionLifeTime = 15;
                        builder.PacketSize = 8192;
                        builder.ServerType = 0;
                        builder.MaxPoolSize = 1000;

                        FbConnection.CreateDatabase(builder.ConnectionString);
                    });

                    MessageBox.Show("Banco de dados gerado com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ocorreu um erro ao gerar o banco de dados: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}