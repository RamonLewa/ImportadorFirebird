using FirebirdSql.Data.FirebirdClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImportadorFirebird.Classes
{
    public class ExceptionsImport
    {
        private List<string> ParametrosExceptions()
        {
            var parametros = new List<string>();

            parametros.Add("CREATE OR ALTER  EXCEPTION AVISO 'Aviso descrição';");
            parametros.Add("CREATE OR ALTER EXCEPTION CANCELAEXCLUSAO 'Aviso: Impossível excluir registro. SGBr Sistemas.';");
            parametros.Add("CREATE OR ALTER EXCEPTION INTERNALFBERROR 'ERROR';");

            return parametros;
        }

        public async Task ExceptionScripts(FbConnection connection)
        {
            foreach (var parametros in ParametrosExceptions())
            {
                // Verifica se o script não está vazio
                if (string.IsNullOrWhiteSpace(parametros))
                    continue;

                using (var command = new FbCommand(parametros, connection))
                {
                    try
                    {
                        await command.ExecuteNonQueryAsync();
                    }
                    catch (Exception ex)
                    {
                        // Log ou exibe o erro
                        MessageBox.Show($"Erro ao executar o script de criação das exceptions: {ex.Message}\n", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}
