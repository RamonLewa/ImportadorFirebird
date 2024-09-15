using FirebirdSql.Data.FirebirdClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImportadorFirebird.Classes
{
    public class TriggersImport
    {
        public static async Task<List<string>> GenerateTriggerScripts(FbConnection connection)
        {
            List<string> triggerScripts = new List<string>();

            string query = @"
                    SELECT 
                        TRIM(RDB$TRIGGER_NAME) AS TriggerName,
                        TRIM(RDB$RELATION_NAME) AS TableName,
                        RDB$TRIGGER_TYPE AS TriggerType,
                        RDB$TRIGGER_SOURCE AS TriggerSource,
                        RDB$TRIGGER_INACTIVE AS TriggerInactive
                    FROM 
                        RDB$TRIGGERS
                    WHERE 
                        RDB$SYSTEM_FLAG = 0; -- Excluir triggers do sistema
                ";

            using (FbCommand command = new FbCommand(query, connection))
            using (FbDataReader reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    string triggerName = reader["TriggerName"].ToString();
                    string tableName = reader["TableName"].ToString();
                    string triggerSource = reader["TriggerSource"].ToString();
                    int triggerType = Convert.ToInt32(reader["TriggerType"]);
                    bool isInactive = Convert.ToInt32(reader["TriggerInactive"]) == 1;

                    if (isInactive)
                    {
                        continue;
                    }

                    if (triggerName == "TITEMOS_AD32000" || triggerName == "TOS_AD32000")
                        continue;

                    string triggerTypeDescription = GetTriggerTypeDescription(triggerType);

                    StringBuilder triggerScript = new StringBuilder();
                    triggerScript.AppendLine($"-- Criar trigger {triggerName} para a tabela {tableName}");
                    triggerScript.AppendLine($"CREATE OR ALTER TRIGGER {triggerName} FOR {tableName}");
                    triggerScript.AppendLine($"{triggerTypeDescription}");

                    if (!triggerSource.TrimStart().StartsWith("AS", StringComparison.OrdinalIgnoreCase))
                    {
                        triggerScript.AppendLine("AS");
                    }

                    triggerScript.AppendLine(triggerSource.Replace("collate PT_BR", null).Trim());

                    triggerScript.AppendLine();

                    triggerScripts.Add(triggerScript.ToString());
                }
            }

            return triggerScripts;
        }

        private static string GetTriggerTypeDescription(int triggerType)
        {
            switch (triggerType)
            {
                case 1: return "BEFORE INSERT";
                case 2: return "ACTIVE AFTER INSERT POSITION 0";
                case 3: return "BEFORE UPDATE";
                case 4: return "AFTER UPDATE";
                case 5: return "BEFORE DELETE";
                case 6: return "ACTIVE AFTER DELETE POSITION 32000";
                case 17: return "ACTIVE BEFORE INSERT OR UPDATE POSITION 0";
                case 18: return "ACTIVE AFTER INSERT OR UPDATE POSITION 0";
                case 114: return "ACTIVE AFTER INSERT OR UPDATE OR DELETE POSITION 0";
                default: return "";
            }
        }

        public static async Task MigrateTriggers(FbConnection sourceConnection, FbConnection destinationConnection)
        {
            List<string> triggerScripts = await GenerateTriggerScripts(sourceConnection);

            foreach (var script in triggerScripts)
            {
                await ExecuteSqlWithLogging(destinationConnection, script, "Trigger");
            }
        }

        private static async Task ExecuteSqlWithLogging(FbConnection connection, string sql, string tableName)
        {
            try
            {
                using (FbCommand command = new FbCommand(sql, connection))
                {
                    await command.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao executar SQL para a tabela {tableName}: {sql}");
                MessageBox.Show($"Erro: {ex.Message}");
                throw;
            }
        }
    }
}
