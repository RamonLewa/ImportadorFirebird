using FirebirdSql.Data.FirebirdClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImportadorFirebird.Classes
{
    public class ProceduresImport
    {
        private static Dictionary<string, string> ParametrosProcedures()
        {
            var Dictonary = new Dictionary<string, string>();

            Dictonary.Add("SPAJUSTAQTDEDESMEMBRAMENTO", "CODDESMEMBRAMENTO integer");
            Dictonary.Add("SPAJUSTAQTDELOTE", "CODMODULO integer, ORIGEM varchar(10), OPERACAO char(1)");
            Dictonary.Add("SPALIMENTACONTROLE", "TABELA varchar(50) not null, OLDCONTROLE integer not null");
            Dictonary.Add("SPALIMENTAITEMDESMEMBRAMENTO", "CODPRODUCAO integer, CODDESMEMBRAMENTO integer");
            Dictonary.Add("SPALIMENTAITEMOP", "CODPRODUCAO integer, CODMODULO integer, ORIGEM varchar(10)");
            Dictonary.Add("SPALIMENTAITEMOPCOMPOSICAO", "CONTROLEITEMOP integer, CODOP integer,   CODMODULO integer, ORIGEM varchar(10)");
            Dictonary.Add("SPATUALIZADATAESTOQUE", "SPPECODPRODUTO integer");
            Dictonary.Add("SPATUALIZADATAVENDACOMPRA", "SPPECODFORNECEDOR integer, SPPECODCLIENTE integer");
            Dictonary.Add("SPATUALIZAEXCLUSAODAVD2", "CODORCAMENTO integer, CODCONDICIONAL integer, CODPEDIDOVENDA integer, CODOS integer");
            Dictonary.Add("SPATUALIZAEXCLUSAODAVD3", "CODITEMCONDICIONAL integer, CODITEMORCAMENTO integer, CODITEMOS integer, CODITEMPEDIDOVENDA integer");
            Dictonary.Add("SPATUALIZAQTDE", "SPPECODPRODUTO integer not null");
            Dictonary.Add("SPATUALIZAQTDEGRADE", "CODPRODUTO integer not null, CODGRADE integer");
            Dictonary.Add("SPATUALIZAQTDEINICIALESTOQUE", "SPPECODPRODUTO integer");
            Dictonary.Add("SPATUALIZASALDOBANCO", "CODBANCO integer not null, VALENTRADA decimal(15,2), VALSAIDA decimal(15,2), CODCONTA integer not null");
            Dictonary.Add("SPATUALIZASALDOCAIXA", "SPCODPDV integer, SPVALORENTRADA decimal(15,2), SPVALORSAIDA decimal(15,2)");
            Dictonary.Add("SPATUALIZASTATUSCOMANDA", "CODCOMANDA integer");
            Dictonary.Add("SPATUALIZASTATUSITEMOS", "SPPECODOS integer not null, SPPESTATUS varchar(50) not null");
            Dictonary.Add("SPATUALIZASTATUSITEMPEDCOMPRA", "SPPECODPEDIDOCOMPRA integer not null, SPPESTATUS varchar(50) not null");
            Dictonary.Add("SPATUALIZASTATUSITEMPEDVENDA", "SPPECODPEDIDOVENDA integer not null");
            Dictonary.Add("SPATUALIZASTATUSORDEMPRODUCAO", "SPPECODORDEMPRODUCAO integer not null, SPPESTATUS varchar(50) not null");
            Dictonary.Add("SPAUDITORIA", "CODMODULO integer not null, TABELA varchar(20) not null, DATAHORACADMOD timestamp not null, DESCRICAOEXCLUIDO varchar(2000) not null");
            Dictonary.Add("SPCANCELAOP", "CODMODULO integer, ORIGEM varchar(10)");
            Dictonary.Add("SPCRIAOP", "CODMODULO integer, ORIGEM varchar(10)");
            Dictonary.Add("SPCRIAOPDESMEMBRAMENTO", "CODDESMEMBRAMENTO integer");

            // Procedures comentadas
            Dictonary.Add("SPINSEREDAVD2", "CONTROLE integer, DATACADASTRO timestamp, CNPJ varchar(20), SERIEECF varchar(21), TIPOECF varchar(7), MARCAECF varchar(20), MODELOECF varchar(20), COO integer, NUMERODAV varchar(13), DATADAV date, TITULODAV varchar(30), VALORTOTAL decimal(15,2),  COOVINCULADO integer, CLIENTE varchar(100), NUMEROECF integer, CPFCLIENTE varchar(20), CNPJCLIENTE varchar(20), MD5DAV varchar(100), CODORCAMENTO integer, CODPEDIDOVENDA integer, CODOS integer, CODCONDICIONAL integer");
            Dictonary.Add("SPINSEREDAVD3", "CONTROLE integer, NUMERODAV varchar(13), DATA date, CODITEM integer, CODPRODUTO integer, PRODUTO varchar(100), QTDE decimal(15,2), UN varchar(10), VALORUNITARIO decimal(15,2), VALORDESCONTOITEM numeric(15,2), VALORACRESCIMOITEM numeric(15,2), TOTALLIQUIDO decimal(15,2), SITUACAOTRIBUTARIA varchar(1), ALIQUOTA numeric(15,2), CANCELADO varchar(1), DECIMAISQTDE integer, DECIMAISVALOR integer, CODITEMORCAMENTO integer, CODITEMPEDIDOVENDA integer, CODITEMCONDICIONAL integer, CODITEMOS integer, MD5ITEMDAV varchar(100)");

            Dictonary.Add("SPQTDEITEMPDV", "SPCOO integer, SPCODVENDAECF integer");
            Dictonary.Add("SPTOTALIZACONDICIONAL", "SPCONTROLECONDICIONAL integer");
            Dictonary.Add("SPTOTALIZANOTAMANUAL", "SPCONTROLENOTAMANUAL integer");
            Dictonary.Add("SPTOTALIZAORCAMENTO", "SPCONTROLETORCAMENTO integer");
            Dictonary.Add("SPTOTALIZAOS", "SPCONTROLEOS integer");
            Dictonary.Add("SPTOTALIZAPEDIDOVENDA", "SPCONTROLEPEDIDOVENDA integer");
            Dictonary.Add("SPTOTALIZAVENDAECF", "SPCOOECF integer not null, SPCONTROLETVENDAECF integer not null, SPNUMEROSERIEECF varchar(100) not null");
            Dictonary.Add("SPTOTALIZAVENDANFCE", "SPCONTROLETVENDANFCE integer");
            Dictonary.Add("SPVALIDAEXCLUSAO", "TABELA varchar(50) not null, OLDCONTROLE integer not null");
            Dictonary.Add("SPVERIFICAOP", "CODMODULO integer, ORIGEM varchar(10)");
            Dictonary.Add("SPVERIFICAQTDELOTE", "CODMODULO integer, ORIGEM varchar(10), OPERACAO char(1)");


            return Dictonary;
        }
        public static async Task<List<string>> GetStoredProceduresScripts(FbConnection connection)
        {
            var procedures = new List<string>();
            var Dictonary = new Dictionary<string, string>();

            Dictonary = ParametrosProcedures();

            string query = @"
                    SELECT 
                        RDB$PROCEDURE_NAME AS ProcedureName,
                        RDB$PROCEDURE_SOURCE AS ProcedureSource
                    FROM RDB$PROCEDURES
                    WHERE RDB$PROCEDURE_SOURCE IS NOT NULL
                    ORDER BY RDB$PROCEDURE_NAME;
                ";

            using (var command = new FbCommand(query, connection))
            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    var gambiarra = Dictonary.Where(c => c.Key == reader["ProcedureName"].ToString().Trim()).First();

                    string procedureName = reader["ProcedureName"].ToString().Trim();
                    string procedureSource = reader["ProcedureSource"].ToString();

                    string formattedScript = $"CREATE OR ALTER PROCEDURE {procedureName}\n" +
                                             $"(\n" +
                                             $"{gambiarra.Value}" +
                                             $")\n" +
                                             $"AS\n" +
                                             $"    {procedureSource.Replace("collate PT_BR", null).Trim()}\n";

                    procedures.Add(formattedScript);
                }
            }

            return procedures.OrderByDescending(c => c.Contains("SPATUALIZAQTDE")).ToList();
        }

        public static async Task ExecuteProcedureScripts(FbConnection connection, List<string> scripts)
        {
            foreach (var script in scripts)
            {
                if (string.IsNullOrWhiteSpace(script))
                    continue;

                string delimitedScript = script.Replace("suspend;", null);

                using (var command = new FbCommand(delimitedScript, connection))
                {
                    try
                    {
                        await command.ExecuteNonQueryAsync();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Erro ao executar o script: {ex.Message}\n", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}
