using FirebirdSql.Data.FirebirdClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportadorFirebird.Classes
{
    public class GeneratorsImport
    {
        public static async Task MigrateGenerators(FbConnection sourceConnection, FbConnection destinationConnection)
        {
            string getGeneratorsQuery = @"
                    SELECT RDB$GENERATOR_NAME 
                    FROM RDB$GENERATORS 
                    WHERE RDB$SYSTEM_FLAG = 0;
                ";

            using (FbCommand sourceCommand = new FbCommand(getGeneratorsQuery, sourceConnection))
            using (FbDataReader reader = await sourceCommand.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    string generatorName = reader["RDB$GENERATOR_NAME"].ToString().Trim();

                    string getGeneratorValueQuery = $"SELECT GEN_ID({generatorName}, 0) FROM RDB$DATABASE";
                    using (FbCommand valueCommand = new FbCommand(getGeneratorValueQuery, sourceConnection))
                    {
                        object generatorValue = await valueCommand.ExecuteScalarAsync();

                        long newGeneratorValue = Convert.ToInt64(generatorValue) + 1;

                        string createGeneratorQuery = $"CREATE SEQUENCE {generatorName};";
                        using (FbCommand createCommand = new FbCommand(createGeneratorQuery, destinationConnection))
                        {
                            await createCommand.ExecuteNonQueryAsync();
                        }

                        string setGeneratorValueQuery = $"ALTER SEQUENCE {generatorName} RESTART WITH {newGeneratorValue};";
                        using (FbCommand setCommand = new FbCommand(setGeneratorValueQuery, destinationConnection))
                        {
                            await setCommand.ExecuteNonQueryAsync();
                        }
                    }
                }
            }
        }
    }
}
