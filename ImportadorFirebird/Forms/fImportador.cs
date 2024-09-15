using FirebirdSql.Data.FirebirdClient;
using ImportadorFirebird.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
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

        private async void btnCriarBanco_Click(object sender, EventArgs e)
        {
            CriarBanco criarBanco = new CriarBanco();

            pgbImportando.Minimum = 0;
            pgbImportando.Maximum = 100;
            pgbImportando.Value = 0;
            pgbImportando.Visible = true;

            await criarBanco.CreateDatabaseAsync(pgbImportando);

            pgbImportando.Visible = false;
        }


        private void btnSelecionarBancoOrigem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Firebird Database Files (*.fdb)|*.fdb|All Files (*.*)|*.*";
            openFileDialog.Title = "Select Firebird Database";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                txtBancoOrigem.Text = openFileDialog.FileName;
            }
        }

        private void btnSelecionarBancoDestino_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Firebird Database Files (*.fdb)|*.fdb|All Files (*.*)|*.*";
            openFileDialog.Title = "Select Firebird Database";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                txtBancoDestino.Text = openFileDialog.FileName;
            }
        }

        private async void btnImportar_Click(object sender, EventArgs e)
        {
            string sourceConnectionString = $"DataSource=localhost;Database={txtBancoOrigem.Text};Port=3050;User=SYSDBA;Password=masterkey;Charset=UTF8;Dialect=3;Connection lifetime=60;Connection Timeout=60;PacketSize=32767;ServerType=0;Unicode=false;Max Pool Size=1000";
            string destinationConnectionString = $"DataSource=localhost;Database={txtBancoDestino.Text};Port=3051;User=SYSDBA;Password=masterkey;Charset=UTF8;Dialect=3;Connection lifetime=60;Connection Timeout=60;PacketSize=32767;ServerType=0;Unicode=false;Max Pool Size=1000";

            MigrateTables migrate = new MigrateTables();
            using (FbConnection sourceConnection = new FbConnection(sourceConnectionString))
            using (FbConnection destinationConnection = new FbConnection(destinationConnectionString))
            {
                await sourceConnection.OpenAsync();
                await destinationConnection.OpenAsync();

                List<string> tableNames = await migrate.GetTableNames(sourceConnection);

                pgbImportando.Minimum = 0;
                pgbImportando.Maximum = tableNames.Count * 2 + 1;
                pgbImportando.Value = 0;
                pgbImportando.Visible = true;

                foreach (string tableName in tableNames)
                {
                    try
                    {
                        var (createTableScript, alterTableScript, dataReader, fbCommand) = await migrate.GenerateCreateTableScript(sourceConnection, tableName)

                        if (createTableScript != null)
                        {
                            using (FbCommand createCommand = new FbCommand(createTableScript, destinationConnection))
                            {
                                await createCommand.ExecuteNonQueryAsync();
                                pgbImportando.Value++;
                            }
                        }

                        // PK
                        if (!string.IsNullOrEmpty(alterTableScript))
                        {
                            using (FbCommand alterCommand = new FbCommand(alterTableScript, destinationConnection))
                            {
                                await alterCommand.ExecuteNonQueryAsync();
                                pgbImportando.Value++;
                            }
                        }

                        await migrate.MigrateTableData(sourceConnection, destinationConnection, tableName);
                        pgbImportando.Value++;

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Erro ao importar a tabela {tableName}: {ex.Message}");
                    }
                }

                // Views
                foreach (string tableName in tableNames)
                {
                    List<string> viewScripts = await ViewsImport.GenerateViewScripts(sourceConnection);
                    foreach (string viewScript in viewScripts)
                    {
                        try
                        {
                            using (FbCommand viewCommand = new FbCommand(viewScript, destinationConnection))
                            {
                                await viewCommand.ExecuteNonQueryAsync();
                                pgbImportando.Value++;
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Erro ao criar a view: {ex.Message}");
                        }
                    }
                }

                // Generators
                try
                {
                    await GeneratorsImport.MigrateGenerators(sourceConnection, destinationConnection);
                    pgbImportando.Value++;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao criar o generator: {ex.Message}");
                }

                pgbImportando.Visible = false;
                MessageBox.Show("Dados importados com sucesso!");
            }
        }
    }
}
