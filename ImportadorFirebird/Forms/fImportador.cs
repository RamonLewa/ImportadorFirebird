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
using System.Xml.Linq;

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
            btnCriarBanco.Enabled = false;
            CriarBanco criarBanco = new CriarBanco();

            pgbImportando.Minimum = 0;
            pgbImportando.Maximum = 100;
            pgbImportando.Value = 0;
            pgbImportando.Visible = true;

            var progress = new Progress<int>(value =>
            {
                pgbImportando.Value = value;
            });

            await criarBanco.CreateDatabaseAsync(progress);

            pgbImportando.Value = 0;
            btnCriarBanco.Enabled = true;
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
            if (string.IsNullOrWhiteSpace(txtBancoOrigem.Text) || string.IsNullOrWhiteSpace(txtBancoDestino.Text))
            {
                MessageBox.Show("Por favor, preencha ambos os campos: Banco de Origem e Banco de Destino.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; 
            }
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
                pgbImportando.Maximum = tableNames.Count * 3 + 10;
                pgbImportando.Value = 0;
                pgbImportando.Visible = true;
                btnImportar.Enabled = false;

                foreach (string tableName in tableNames)
                {
                    try
                    {
                        var (createTableScript, alterTableScript, dataReader, fbCommand) = await migrate.GenerateCreateTableScript(sourceConnection, tableName);

                        if (createTableScript != null)
                        {
                            using (FbCommand createCommand = new FbCommand(createTableScript, destinationConnection))
                            {
                                try
                                {
                                    await createCommand.ExecuteNonQueryAsync();
                                    await Task.Run(() => UpdateProgressBar());
                                }
                                catch (FbException fbEx)
                                {
                                    if (fbEx.ErrorCode == 335544351) 
                                    {
                                        MessageBox.Show($"A tabela {tableName} já existe no banco de destino.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    }
                                    else
                                    {
                                        throw;
                                    }
                                }

                                }
                        }

                        // PK
                        if (!string.IsNullOrEmpty(alterTableScript))
                        {
                            using (FbCommand alterCommand = new FbCommand(alterTableScript, destinationConnection))
                            {
                                try
                                {
                                    await alterCommand.ExecuteNonQueryAsync();
                                    await Task.Run(() => UpdateProgressBar());
                                }
                                catch (FbException fbEx)
                                {
                                    if (fbEx.ErrorCode == 335544351)
                                    {
                                        MessageBox.Show($"A tabela {tableName} já existe no banco de destino.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    }
                                    else
                                    {
                                        throw;
                                    }
                                }
                            }
                        }

                        await migrate.MigrateTableData(sourceConnection, destinationConnection, tableName);
                        await Task.Run(() => UpdateProgressBar());


                    }
                    catch {}
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
                                await Task.Run(() => UpdateProgressBar());
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
                    await Task.Run(() => UpdateProgressBar());
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao criar o generator: {ex.Message}");
                }

                // Exceptions
                try
                {
                    ExceptionsImport exceptions = new ExceptionsImport();
                    await exceptions.ExceptionScripts(destinationConnection);
                    await Task.Run(() => UpdateProgressBar());
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao importar a exception: {ex.Message}");
                }

                // Procedures
                try
                {
                    List<string> procedureScripts = await ProceduresImport.GetStoredProceduresScripts(sourceConnection);
                    await ProceduresImport.ExecuteProcedureScripts(destinationConnection, procedureScripts);
                    await Task.Run(() => UpdateProgressBar());
                }
                catch (Exception ex) 
                {
                    MessageBox.Show($"Erro ao importar a procedure: {ex.Message}");
                }

                // Triggers
                try
                {
                    await TriggersImport.MigrateTriggers(sourceConnection, destinationConnection);
                    await Task.Run(() => UpdateProgressBar());
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao importar a trigger: {ex.Message}");
                }

                // FKs
                foreach (string tableName in tableNames)
                {
                    try
                    {
                        await ForeignKeysImport.ExecuteForeignKeyScript(sourceConnectionString, destinationConnectionString, tableName);
                        await Task.Run(() => UpdateProgressBar());

                    } catch (Exception ex)
                    {
                        MessageBox.Show($"Erro ao importar FK: {ex.Message}");
                    }
                }

                pgbImportando.Value = 0;
                MessageBox.Show("Dados importados com sucesso!", "Importador", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            btnImportar.Enabled = true;
        }
        private async void UpdateProgressBar()
        {
            if (pgbImportando.InvokeRequired)
            {
                pgbImportando.Invoke(new Action(UpdateProgressBar));
            }
            else
            {
                if (pgbImportando.Value < pgbImportando.Maximum)
                {
                    pgbImportando.Value = Math.Min(pgbImportando.Value + 1, pgbImportando.Maximum);
                    await Task.Delay(5);
                }
            }
        }

    }
}
