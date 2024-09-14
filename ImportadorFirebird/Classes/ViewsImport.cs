using FirebirdSql.Data.FirebirdClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportadorFirebird.Classes
{
    public class ViewsImport
    {
        public static async Task<List<string>> GenerateViewScripts(FbConnection connection)
        {
            List<string> viewScripts = new List<string>();

            viewScripts.Add(@"CREATE OR ALTER VIEW VACOUGUE(
                    CONTROLE,
                    CODFUNCIONARIO,
                    FUNCIONARIO,
                    CODPRODUTO,
                    PRODUTO,
                    PESOTOTAL,
                    PRECOKG,
                    ANIMAL,
                    GRUPO,
                    STATUS,
                    DATAHORACADASTRO)
                AS
                select
                    taco.controle,
                    taco.codfuncionario,
                    (Select funcionario from tfuncionario where controle = taco.codfuncionario) as Funcionario,
                    taco.codprodutoorigem,
                    (Select produto from testoque where controle = taco.codprodutoorigem),
                    taco.pesototal,
                    taco.precokg,
                    case taco.animal
                        when 0 then 'BOVINO'
                        when 1 then 'SUÃNO'
                    end as animal,
                    case taco.grupo
                        when 0 then 'DIANTEIRO'
                        when 1 then 'TRASEIRO'
                        when 2 then 'CARCAÃ‡A COMPLETA'
                        when 3 then 'TODOS'
                    end as grupo,
                    case taco.status
                        when 0 then 'ABERTO'
                        when 1 then 'EM PRODUÃ‡ÃƒO'
                        when 2 then 'FINALIZADO'
                    end as status,
                    taco.datahoracadastro
                 from tacouguedesmembramento taco
                ;
                ;");
            viewScripts.Add(@"CREATE OR ALTER VIEW VCASHBACK(
                    CODCLIENTE,
                    CLIENTE,
                    SALDO)
                AS
                select
                    thist.CODCLIENTE AS codCliente,
                    (select cliente from tcliente where controle = thist.codcliente ) as Cliente,
                    iif(sum(thist.valorentrada - thist.valorsaida) > 0, sum(thist.valorentrada - thist.valorsaida), 0) as Saldo
                from thistoricocashback thist
                where thist.confirmado = '1'
                group by
                    thist.codcliente;
                ;");
            viewScripts.Add(@"CREATE OR ALTER VIEW VESTOQUEACOUGUE(
                    CONTROLE,
                    CODPRODUTOORIGEM,
                    PRODUTOORIGEM,
                    CODPRODUTO,
                    PRODUTO,
                    CODCORTE,
                    CORTE,
                    ANIMAL,
                    GRUPO,
                    PERCPESOTOTAL,
                    PERCPESOGRUPO,
                    DATAHORACADASTRO)
                AS
                select
                    lpad(est.controle, 6, 0),
                    lpad(est.codprodutoorigem, 6,0) as codprodutoorigem,
                    (select produto from testoque where controle = est.codprodutoorigem) as produtoorigem,
                    lpad(est.codproduto, 6,0) as codproduto,
                    (select produto from testoque where controle = est.codproduto) as produto,
                    lpad(est.codcorte,6,0) as codcorte,
                    (select corte from tacouguecorte where controle = est.codcorte) as corte,
                    case est.animal
                        when 0 then 'BOVINO'
                        when 1 then 'SUï¿½NO'
                    end as animal,
                    case est.grupo
                        when 0 then 'DIANTEIRO'
                        when 1 then 'TRASEIRO'
                        when 2 then 'CARCAï¿½A COMPLETA'
                    end as grupo,
                    est.percpesototal,
                    est.percpesogrupo,
                    est.datahoracadastro
                from testoqueacougue est;
                ;");
            viewScripts.Add(@"CREATE OR ALTER VIEW VGNRE(
                    CONTROLE,
                    NUMEROGUIA,
                    AMBIENTE,
                    DATAHORACADASTRO,
                    UFFAVORECIDA,
                    CODIGORECEITA,
                    CODIGOSTATUS,
                    STATUS,
                    PROTOCOLO,
                    NUMCONTROLE,
                    REPNUMERICA,
                    CODBARRAS,
                    IEEMITUFFAVORECIDA,
                    RAZAOSOCIALEMIT,
                    DOCUMENTOEMIT,
                    IEEMIT,
                    ENDERECOEMIT,
                    UFEMIT,
                    CODMUNICEMIT,
                    MUNICIPIOEMIT,
                    TELEFONEEMIT,
                    CEPEMIT,
                    DETALHAMENTORECEITA,
                    PRODUTORECEITA,
                    CODDOCUMENTORECEITA,
                    NDOCORIGEM,
                    PERIODOREFERENCIA,
                    MESREFERENCIA,
                    ANOREFERENCIA,
                    PARCELA,
                    CONVENIO,
                    DATAVENCIMENTO,
                    DATAPAGAMENTO,
                    VALORPRINCIPALTOTAL,
                    IEDESTUFFAVORECIDA,
                    CODDESTINATARIO,
                    RAZAOSOCIALDEST,
                    DOCUMENTODEST,
                    IEDEST,
                    ENDERECODEST,
                    UFDEST,
                    CODMUNICDEST,
                    MUNICIPIODEST,
                    TELEFONEDEST,
                    CEPDEST,
                    CHAVEACESSONFE,
                    OBS1,
                    OBS2,
                    INFORMACOESCOMPLEMENTARES)
                AS
                select
                    gnre.controle controle,
                    gnre.numeroguia numeroguia,
                    gnre.ambiente ambiente,
                    gnre.datahoracadastro datahoracadastro,
                    uffavorecida uffavorecida,
                    codigoreceita codigoreceita,
                    agn.codigostatus codigostatus,
                    agn.status status,
                    agn.protocolo protocolo,
                    agn.numcontrole numcontrole,
                    agn.repnumerica repnumerica,
                    agn.codbarras codbarras,
                    ieemituffavorecida ieemituffavorecida,
                    emi.razaosocial razaosocial,
                    (iif(emi.cnpj is null,emi.cpf,emi.cnpj)) documentoemit,
                    emi.ie ie,
                    emi.endereco endereco,
                    emi.uf uf,
                    emi.codcidadeibge codcidadeibge,
                    emi.cidade cidade,
                    emi.telefone telefone,
                    emi.cep cep,
                    detalhamentoreceita detalhamentoreceita,
                    produtoreceita produtoreceita,
                    coddocumentoreceita coddocumentoreceita,
                    ndocorigem ndocorigem,
                    periodoreferencia periodoreferencia,
                    mesreferencia mesreferencia,
                    anoreferencia anoreferencia,
                    parcela parcela,
                    convenio convenio,
                    datavencimento datavencimento,
                    datapagamento datapagamento,
                    valorprincipaltotal valorprincipaltotal,
                    iedestuffavorecida iedestuffavorecida,
                    iif(coddestinatario is null,1,coddestinatario) coddestinatario,
                    cli.cliente cliente,
                    (iif(cli.cnpj is null,cli.cpf,cli.cnpj)) documentodest,
                    cli.ie ie,
                    cli.endereco endereco,
                    cli.uf uf,
                    cli.codigocidadeibge codigocidadeibge,
                    cli.cidade cidade,
                    cli.telefone telefone,
                    cli.cep cep,
                    chaveacessonfe chaveacessonfe,
                    obs1 obs1,
                    obs2 obs2,
                    informacoescomplementares informacoescomplementares
                from tgnre gnre
                    left join (select controle,cliente,cnpj,cpf,ie,endereco,uf,codigocidadeibge,cidade,telefone,cep from tcliente) cli on
                        iif(coalesce(gnre.coddestinatario,'') = '',1,gnre.coddestinatario) = cli.controle
                    left join (select codgnre, protocolo, codigostatus,status,numcontrole,repnumerica,codbarras from tautorizacaognre) agn on
                        gnre.controle = agn.codgnre,
                    (select razaosocial,cnpj,cpf,ie,endereco,uf,codcidadeibge,cidade,telefone,cep from temitente) emi
                order by
                    numeroguia,
                    ambiente
                ;");
            viewScripts.Add(@"CREATE OR ALTER VIEW VLIGACAOCRM(
                    CONTROLE,
                    DATAHORACADASTRO,
                    CODFUNCIONARIO,
                    FUNCIONARIO,
                    CODCLIENTE,
                    CLIENTE,
                    MOTIVO,
                    ORIGEM,
                    OBS)
                AS
                select
                    lig.controle,
                    lig.datahoracadastro,
                    fnc.controle,
                    fnc.funcionario,
                    cli.controle,
                    cli.cliente,
                    lim.motivo,
                    lig.origem,
                    lig.obs
                from
                    tligacao lig
                    inner join tligacaomotivo lim
                        on lig.codmotivo = lim.controle
                    inner join tcliente cli
                        on lig.codcliente = cli.controle
                    inner join tfuncionario fnc
                        on lig.codfuncionario = fnc.controle
                ;");

            foreach (string view in viewScripts)
            {
                // Vincular o comando à conexão aberta
                using (FbCommand command = new FbCommand(view, connection))
                {
                    // Necessário para executar o comando SQL
                    command.Connection = connection;
                    await command.ExecuteNonQueryAsync();
                }
            }

            return viewScripts;
        }
    }
}
