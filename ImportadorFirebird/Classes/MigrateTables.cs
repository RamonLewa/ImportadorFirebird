using FirebirdSql.Data.FirebirdClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImportadorFirebird.Classes
{
    public class MigrateTables
    {
        private readonly object _fileLock = new object();

        public async Task<(string createTableScript, string alterTableScript, FbDataReader dataReader, FbCommand command1)> GenerateCreateTableScript(FbConnection connection, string tableName)
        {
            if (tableName.StartsWith("RDB$") || tableName.StartsWith("MON$"))
            {
                return (null, null, null, null);
            }

            StringBuilder sbCreate = new StringBuilder();
            sbCreate.AppendLine($"-- Script para criar a tabela {tableName}");
            sbCreate.AppendLine($"CREATE TABLE {tableName} (");

            string query = @"
                SELECT
                    rf.RDB$FIELD_NAME AS FieldName,
                    rf.RDB$NULL_FLAG AS IsNotNull,
                    f.RDB$FIELD_LENGTH AS FieldLength,
                    f.RDB$FIELD_PRECISION AS FieldPrecision,
                    f.RDB$FIELD_SCALE AS FieldScale,
                    f.RDB$FIELD_TYPE AS FieldType,
                    CASE
                        WHEN f.RDB$FIELD_TYPE = 7 THEN 'SMALLINT'
                        WHEN f.RDB$FIELD_TYPE = 8 THEN 'INTEGER'
                        WHEN f.RDB$FIELD_TYPE = 9 THEN 'QUAD'
                        WHEN f.RDB$FIELD_TYPE = 10 THEN 'FLOAT'
                        WHEN f.RDB$FIELD_TYPE = 12 THEN 'DATE'
                        WHEN f.RDB$FIELD_TYPE = 13 THEN 'TIME'
                        WHEN f.RDB$FIELD_TYPE = 14 THEN 'CHAR'
                        WHEN f.RDB$FIELD_TYPE = 16 THEN 'BIGINT'
                        WHEN f.RDB$FIELD_TYPE = 27 THEN 'DOUBLE PRECISION'
                        WHEN f.RDB$FIELD_TYPE = 35 THEN 'TIMESTAMP'
                        WHEN f.RDB$FIELD_TYPE = 37 THEN 'VARCHAR'
                        WHEN f.RDB$FIELD_TYPE IN (40, 11, 261) THEN 'BLOB'
                        ELSE 'UNKNOWN'
                    END AS FieldTypeString
                FROM
                    RDB$RELATION_FIELDS rf
                    JOIN RDB$FIELDS f ON rf.RDB$FIELD_SOURCE = f.RDB$FIELD_NAME
                WHERE
                    rf.RDB$RELATION_NAME = @tableName
                ORDER BY
                    rf.RDB$FIELD_POSITION;
                ";

            List<string> primaryKeyColumns = new List<string>();
            string primaryKeyQuery = @"
                SELECT
                    sg.RDB$FIELD_NAME AS FieldName
                FROM
                    RDB$INDEX_SEGMENTS sg
                    JOIN RDB$INDICES i ON sg.RDB$INDEX_NAME = i.RDB$INDEX_NAME
                    JOIN RDB$RELATION_CONSTRAINTS rc ON i.RDB$INDEX_NAME = rc.RDB$INDEX_NAME
                WHERE
                    rc.RDB$RELATION_NAME = @tableName
                    AND rc.RDB$CONSTRAINT_TYPE = 'PRIMARY KEY'
                ";

            using (FbCommand command = new FbCommand(query, connection))
            {
                command.Parameters.AddWithValue("@tableName", tableName);

                using (FbDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        string fieldName = reader["FieldName"].ToString().Trim();
                        string fieldType = reader["FieldTypeString"].ToString().Trim();
                        int fieldLength = reader["FieldLength"] != DBNull.Value ? Convert.ToInt32(reader["FieldLength"]) : 0;
                        int fieldPrecision = reader["FieldPrecision"] != DBNull.Value ? Convert.ToInt32(reader["FieldPrecision"]) : 0;
                        int fieldScale = reader["FieldScale"] != DBNull.Value ? Convert.ToInt32(reader["FieldScale"]) : 0;
                        bool isNotNull = reader["IsNotNull"] != DBNull.Value && Convert.ToInt32(reader["IsNotNull"]) == 1;

                        if (fieldType == "UNKNOWN")
                        {
                            throw new Exception($"Tipo de dado desconhecido encontrado na tabela {tableName} para o campo {fieldName}");
                        }

                        string fieldDefinition = $"{fieldName} {fieldType}";

                        if (fieldType == "VARCHAR" || fieldType == "CHAR")
                        {
                            fieldDefinition += $"({fieldLength})";
                        }
                        else if (fieldType == "NUMERIC" || fieldType == "DECIMAL")
                        {
                            fieldDefinition += $"({fieldPrecision}, {Math.Abs(fieldScale)})";
                        }

                        if (isNotNull)
                        {
                            fieldDefinition += " NOT NULL";
                        }

                        sbCreate.AppendLine($"    {fieldDefinition},");

                        using (FbCommand pkCommand = new FbCommand(primaryKeyQuery, connection))

                            if (reader["FieldName"].ToString().Trim() == "CONTROLE")
                                primaryKeyColumns.Add(reader["FieldName"].ToString().Trim());

                    }

                    if (sbCreate[sbCreate.Length - 3] == ',')
                    {
                        sbCreate.Length -= 3;
                    }

                    sbCreate.AppendLine();
                    sbCreate.AppendLine(");");

                    StringBuilder sbAlter = new StringBuilder();
                    if (primaryKeyColumns.Count > 0)
                    {
                        sbAlter.AppendLine($"ALTER TABLE {tableName} ADD CONSTRAINT PK_{tableName} PRIMARY KEY ({string.Join(", ", primaryKeyColumns)});");
                    }

                    return (sbCreate.ToString(), sbAlter.ToString(), reader, command);
                }
            }
        }

        public async Task<List<string>> GetTableNames(FbConnection connection)
        {
            var tableNames = new List<string>();

            string query = @"
                    SELECT RDB$RELATION_NAME 
                    FROM RDB$RELATIONS 
                    WHERE RDB$RELATION_TYPE = 0
                    AND RDB$RELATION_NAME NOT STARTING WITH 'RDB$'
                    AND RDB$RELATION_NAME NOT STARTING WITH 'MON$'
                    ORDER BY RDB$RELATION_NAME;
                ";

            using (var command = new FbCommand(query, connection))
            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    tableNames.Add(reader["RDB$RELATION_NAME"].ToString().Trim());
                }
            }

            return tableNames;
        }

        public async Task MigrateTableData(FbConnection sourceConnection, FbConnection destinationConnection, string tableName)
        {
            if (tableName.StartsWith("RDB$") || tableName.StartsWith("MON$"))
            {
                return;
            }

            string getDataQuery = $"SELECT * FROM {tableName}";
            using (FbCommand sourceCommand = new FbCommand(getDataQuery, sourceConnection))
            using (FbDataReader reader = await sourceCommand.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    StringBuilder insertQuery = new StringBuilder($"INSERT INTO {tableName} (");
                    StringBuilder valuesPart = new StringBuilder(" VALUES (");

                    List<FbParameter> parameters = new List<FbParameter>();

                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        string columnName = reader.GetName(i);
                        object value = reader.GetValue(i);

                        insertQuery.Append($"{columnName}, ");
                        valuesPart.Append($"@{columnName}, ");

                        FbParameter parameter = new FbParameter($"@{columnName}", value);

                        if (reader.GetFieldType(i) == typeof(byte[]))
                        {
                            if (value == DBNull.Value)
                            {
                                parameter.Value = DBNull.Value;
                            }
                            else
                            {
                                parameter.Value = (byte[])value;
                            }
                        }
                        else
                        {
                            if (value is string && string.IsNullOrEmpty(value as string))
                            {
                                parameter.Value = DBNull.Value;
                            }
                            else if (value == DBNull.Value)
                            {
                                parameter.Value = DBNull.Value;
                            }
                        }

                        parameters.Add(parameter);
                    }

                    insertQuery.Length -= 2;
                    valuesPart.Length -= 2;
                    insertQuery.Append(")");
                    valuesPart.Append(")");

                    string finalQuery = insertQuery.ToString() + valuesPart.ToString();
                    using (FbCommand destinationCommand = new FbCommand(finalQuery, destinationConnection))
                    {
                        destinationCommand.Parameters.AddRange(parameters.ToArray());

                        await destinationCommand.ExecuteNonQueryAsync();
                    }
                }
            }
        }

        public async Task AddPrimaryKeys(FbConnection connection)
        {
            string primaryKeyQuery = @"
                    SELECT
                        rc.RDB$RELATION_NAME AS TableName,
                        sg.RDB$FIELD_NAME AS FieldName,
                        rc.RDB$CONSTRAINT_NAME AS ConstraintName
                    FROM
                        RDB$RELATION_CONSTRAINTS rc
                        JOIN RDB$INDEX_SEGMENTS sg ON rc.RDB$INDEX_NAME = sg.RDB$INDEX_NAME
                    WHERE
                        rc.RDB$CONSTRAINT_TYPE = 'PRIMARY KEY'
                    ORDER BY
                        rc.RDB$RELATION_NAME, sg.RDB$FIELD_POSITION;
                ";

            using (FbCommand command = new FbCommand(primaryKeyQuery, connection))
            using (FbDataReader reader = await command.ExecuteReaderAsync())
            {
                string lastTableName = null;
                StringBuilder pkScript = new StringBuilder();

                while (await reader.ReadAsync())
                {
                    string tableName = reader["TableName"].ToString().Trim();
                    string fieldName = reader["FieldName"].ToString().Trim();
                    string constraintName = reader["ConstraintName"].ToString().Trim();

                    if (await TableHasPrimaryKey(connection, tableName))
                    {
                        MessageBox.Show($"Tabela {tableName} já possui uma chave primária. Ignorando adição.");
                        continue;
                    }

                    if (lastTableName != tableName)
                    {
                        if (pkScript.Length > 0)
                        {
                            pkScript.Append(");");
                            await ExecuteSqlWithLogging(connection, pkScript.ToString(), tableName);
                        }
                        pkScript.Clear();
                        pkScript.AppendLine($"ALTER TABLE {tableName} ADD CONSTRAINT {constraintName} PRIMARY KEY ({fieldName}");
                        lastTableName = tableName;
                    }
                    else
                    {
                        pkScript.Append($", {fieldName}");
                    }
                }

                if (pkScript.Length > 0)
                {
                    pkScript.Append(");");
                    await ExecuteSqlWithLogging(connection, pkScript.ToString(), lastTableName);
                }
            }
        }

        public async Task<bool> TableHasPrimaryKey(FbConnection connection, string tableName)
        {
            string query = @"
                    SELECT COUNT(*)
                    FROM RDB$RELATION_CONSTRAINTS
                    WHERE RDB$RELATION_NAME = @tableName
                    AND RDB$CONSTRAINT_TYPE = 'PRIMARY KEY';
                ";

            using (FbCommand command = new FbCommand(query, connection))
            {
                command.Parameters.AddWithValue("@tableName", tableName);

                int count = Convert.ToInt32(await command.ExecuteScalarAsync());
                return count > 0;
            }
        }
        private async Task ExecuteSqlWithLogging(FbConnection connection, string sql, string tableName)
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
