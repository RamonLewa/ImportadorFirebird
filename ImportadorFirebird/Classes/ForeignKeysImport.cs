using FirebirdSql.Data.FirebirdClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImportadorFirebird.Classes
{
    internal class ForeignKeysImport
    {
        public static async Task<string> GenerateForeignKeyScript(FbConnection connection, string tableName)
        {
            StringBuilder sb = new StringBuilder();

            string query = $@"
                    SELECT
                        rc.RDB$CONSTRAINT_NAME AS ConstraintName,
                        rc.RDB$RELATION_NAME AS TableName,       -- Nome da tabela de origem (onde a FK está aplicada)
                        sg.RDB$FIELD_NAME AS FieldName,
                        refc.RDB$UPDATE_RULE AS UpdateRule,
                        refc.RDB$DELETE_RULE AS DeleteRule,
                        relc.RDB$RELATION_NAME AS RefTableName,  -- Nome da tabela de referência (onde a FK aponta)
                        refsg.RDB$FIELD_NAME AS RefFieldName
                    FROM
                        RDB$RELATION_CONSTRAINTS rc
                        JOIN RDB$INDEX_SEGMENTS sg ON rc.RDB$INDEX_NAME = sg.RDB$INDEX_NAME
                        JOIN RDB$REF_CONSTRAINTS refc ON rc.RDB$CONSTRAINT_NAME = refc.RDB$CONSTRAINT_NAME
                        JOIN RDB$RELATION_CONSTRAINTS relc ON refc.RDB$CONST_NAME_UQ = relc.RDB$CONSTRAINT_NAME
                        JOIN RDB$INDEX_SEGMENTS refsg ON relc.RDB$INDEX_NAME = refsg.RDB$INDEX_NAME
                    WHERE
                        relc.RDB$RELATION_NAME = '{tableName}'
                        and rc.RDB$CONSTRAINT_TYPE = 'FOREIGN KEY';
                ";


            using (FbCommand command = new FbCommand(query, connection))
            {
                using (FbDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        string constraintName = reader["ConstraintName"].ToString().Trim();
                        string fieldName = reader["FieldName"].ToString().Trim();
                        string originTableName = reader["TableName"].ToString().Trim();
                        string refTableName = reader["RefTableName"].ToString().Trim();
                        string refFieldName = reader["RefFieldName"].ToString().Trim();

                        string updateRule = reader["UpdateRule"].ToString().Trim().Replace("RESTRICT", "NO ACTION");
                        string deleteRule = reader["DeleteRule"].ToString().Trim().Replace("RESTRICT", "NO ACTION");

                        sb.AppendLine($"CREATE INDEX {constraintName} ON {originTableName} ({fieldName});");
                    }
                }
            }

            string foreignKeyScript = sb.ToString();

            return foreignKeyScript;
        }

        public static async Task ExecuteForeignKeyScript(string connection, string conecctionDest, string tableName)
        {
            var sourceConnection = new FbConnection(connection);
            var DestConnection = new FbConnection(conecctionDest);
            sourceConnection.Open();
            DestConnection.Open();

            string script = await GenerateForeignKeyScript(sourceConnection, tableName);

            if (!string.IsNullOrWhiteSpace(script))
            {
                string[] commands = script.Split(';');

                foreach (var commandText in commands)
                {
                    if (!string.IsNullOrWhiteSpace(commandText))
                    {
                        using (FbCommand command = new FbCommand(commandText.Trim(), DestConnection))
                        {
                            try
                            {
                                await command.ExecuteNonQueryAsync();
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"Erro ao importar as FK para a tabela {tableName}: {ex.Message}");
                            }
                        }
                    }
                }
            }
        }
    }
}
