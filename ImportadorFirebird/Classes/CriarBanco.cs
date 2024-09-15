using FirebirdSql.Data.FirebirdClient;
using System;
using System.Data.Common;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImportadorFirebird.Classes
{
    public class CriarBanco
    {
        public async Task CreateDatabaseAsync(ProgressBar progressBar)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Banco de dados (*.FDB)|*.FDB",
                Title = "Caminho banco de dados",
                FileName = "BASESGMASTER50.FDB",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string caminhoArquivo = saveFileDialog.FileName;

                try
                {
                    if (File.Exists(caminhoArquivo))
                    {
                        File.Delete(caminhoArquivo);
                    }

                    FbConnectionStringBuilder builder = new FbConnectionStringBuilder
                    {
                        DataSource = "localhost",
                        Database = caminhoArquivo,
                        Port = 3051,
                        UserID = "SYSDBA",
                        Password = "masterkey",
                        Charset = "UTF8",
                        Dialect = 3,
                        ConnectionLifeTime = 15,
                        PacketSize = 8192,
                        ServerType = 0,
                        MaxPoolSize = 1000
                    };

                    await Task.Run(() =>
                    {
                        FbConnection.CreateDatabase(builder.ConnectionString);
                    });

                    if (progressBar != null)
                    {
                        progressBar.Invoke(new Action(() => progressBar.Value = 100));
                    }

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
