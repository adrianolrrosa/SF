using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using SystemFarma.Models;
using SystemFarma.Utils;
using systemweb;

namespace SystemFarma.Controllers
{
    public class UsuarioController : Controller
    {
        Usuario usuario = new Usuario();

        systemwebClass oVFP = new systemwebClass();

        protected string cSoma;
        string cXML1;

        DataSet myDataSet = new DataSet();
        DataSet myDataSet2 = new DataSet();

        // GET: Usuario
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Cadastro()
        {
            return View();
        }

        public ActionResult RedeDeEstabelecimentos()
        {
            systemweb.systemwebClass oVFP = new systemweb.systemwebClass();

            // Buscar estados
            string cEstados = oVFP.BuscaEstado();

            DataSet dsE = new DataSet();
            dsE.ReadXml(new StringReader(cEstados));
            ViewBag.Estados = dsE.Tables[0].AsEnumerable();
            //this.ddlEstados.DataTextField = "estado";
            //this.ddlEstados.DataValueField = "estado";
            //this.ddlEstados.DataBind();
            //ddlEstados.SelectedIndex = ddlEstados.Items.IndexOf(ddlEstados.Items.FindByText("SP"));
            //string cEstado = ddlEstados.SelectedValue.ToString();

            // Buscar cidades
            string cCidades = oVFP.BuscaCidade(dsE.Tables[0].Rows[0].ToString());

            DataSet dsC = new DataSet();
            dsC.ReadXml(new StringReader(cCidades));
            //DataView dvC = new DataView();
            //dvC = dsC.Tables[0].DefaultView;
            ViewBag.Cidades = dsC.Tables[0].AsEnumerable();

            //string cDefault = ddlEstados.SelectedValue.ToString();
            //dvC.RowFilter = "estado='" + cDefault + "'";

            //this.ddlCidades.DataTextField = "cidade";
            //this.ddlCidades.DataValueField = "id_cidade";
            //this.ddlCidades.DataBind();

            //ddlCidades.SelectedIndex = ddlCidades.Items.IndexOf(ddlCidades.Items.FindByText("SOROCABA"));

            // Obter empresa Alterado 03/10/11
            int nCodEmp = 276;

            //Buscar Ramos
            string cRamos = oVFP.BuscaRamo1(nCodEmp);

            DataSet dsR = new DataSet();
            dsR.ReadXml(new StringReader(cRamos));
            ViewBag.Ramos = dsR.Tables[0].AsEnumerable();
            //this.ddlRamos.DataTextField = "ramo";
            //this.ddlRamos.DataValueField = "id_ramo";
            //this.ddlRamos.DataBind();

            return View();
        }

        public ActionResult ConsultarGastos()
        {
            return View();
        }

        public ActionResult ExtratoFunciona(Usuario usuario)
        {
            if (Session["uVerProd"].ToString() != "")
            {
        
                // Se pode mostrar produtos
                string cPode = Session["uVerProd"].ToString();

                // Obter empresa
                int nCodEmp = Convert.ToInt32(Session["uCodEmp"].ToString());

                // Obter fechamento
                int nFecha = Convert.ToInt32(Session["uFecha"]);

                // Obter Período
                string cPeriodo = Session["uPeriodo"].ToString();
                this.usuario.lblPeriodo = cPeriodo;


                if (Session["uExtrato"].ToString() != "")
                {
                    //Obter Extrato Referente
                    string cExtrato = Session["uExtrato"].ToString();
                    this.usuario.lblExtrato = "Extrato Referente: " + cExtrato;
                }
                if (Session["uAviso"].ToString() != "")
                {
                    string cAviso = Session["uAviso"].ToString();
                    this.usuario.lblExtrato = cAviso;
                }

                // Obter funcionário
                int nCodFun = Convert.ToInt32(Session["uCodFun"]);
                string cCola = Session["uColaborador"].ToString();
                this.usuario.lblColaborador = cCola;

                // Criar DataSet
                DataSet myDataSet = new DataSet();
                myDataSet.EnforceConstraints = false;

                // Ler liberações
                string cXML;
                cXML = oVFP.BuscaFunLibe(nCodFun, nFecha);
                myDataSet.ReadXml(new StringReader(cXML));

                // Ler detalhes das liberações
                // Todo: Mudar para BuscaFunNotas1
                cXML1 = oVFP.BuscaFunNotas1(nCodFun, nCodEmp, nFecha);

                if (cXML1 != "")
                {
                    myDataSet.ReadXml(new StringReader(cXML1));

                    // Relacionamento
                    myDataSet.Relations.Add("Notas",
                    myDataSet.Tables[0].Columns["id_mov"],
                    myDataSet.Tables[1].Columns["id_mov"]);
                }


                // Somar resultado
                DataTable DBTable = new DataTable();
                DBTable = myDataSet.Tables[0];
                string cTemp = DBTable.Compute("Sum(valor_nf)", "").ToString();
                if (cTemp != "")
                {
                    cSoma = "R$ " + DBTable.Compute("Sum(valor_nf)", "").ToString();
                }
                else
                    cSoma = "R$ 0.00";

                // DataBind
                DataView source = new DataView(myDataSet.Tables[0]);
                //notas.DataSource = source;
                //notas.DataBind();
            }

            return View();
        }

        public ActionResult ConsultarSaldo()
        {
            return View();
        }

        public ActionResult SaldoFuncion(Usuario usuario)
        {
            if (Session["uExtrato"].ToString() == "")
            {
                string cPeriodo = Session["uPeriodo"].ToString();
                this.usuario.lblPeriodo = cPeriodo;
                //ASPxGridView3.Visible = true;
                //GridView1.Visible = true;

            }
            else
            {
                string cExtrato = Session["uExtrato"].ToString();
                this.usuario.lblPeriodo = cExtrato;

                //ASPxRoundPanel ASPxRoundPanel2 = (ASPxRoundPanel)Master.FindControl("ASPxRoundPanel1") as ASPxRoundPanel;
                //ASPxRoundPanel2.HeaderText = "Gastos Anteriores ";

                //Desabilitar Saldo
                //ASPxGridView3.Visible = false;
                //GridView1.Visible = false;

            }

            string cAviso = Session["uAviso"].ToString();
            this.usuario.lblAviso = cAviso;

            systemweb.systemwebClass oVFP = new systemweb.systemwebClass();
            // Consultar Limites
            try
            {
                string cXML2;
                //string cXML3;

                // Obter funcionário
                int nCodFun = Convert.ToInt32(Session["uCodFun"].ToString());

                cXML2 = oVFP.ConsultaLimite1(nCodFun, 0, 0);
                myDataSet2.ReadXml(new StringReader(cXML2));
                //cXML3 = oVFP.ConsultaLimite1(nCodFun, 0);
                //myDataSet3.ReadXml(Server.MapPath(cXML3));

                //Consultar Limite
                try
                {
                    //ASPxGridView3.DataSource = myDataSet2.Tables[0];
                    //ASPxGridView3.KeyFieldName = "codigo";
                    //ASPxGridView3.DataBind();

                    //GridView1.DataSource = myDataSet2.Tables[0].DefaultView;
                    //GridView1.DataBind();
                }

                catch (Exception ex)
                {
                    usuario.lblAviso = "Erro " + ex.ToString();
                }

                //DataGridBind();

                //ASPxGridView3.DataSource = Session["DsTeste"];
                //ASPxGridView3.KeyFieldName = "codigo";
                //ASPxGridView3.DataBind();


                //Se a sessão for nula voltar ao início
                if (Session["uFecha"] == null)
                {
                    Response.Redirect("~/LoginUsuario.aspx", false);
                    return View();
                }


                //ASPxGridView1.DataSource = Session["DsPrincipal"];
                //ASPxGridView1.DataBind();

                //hlkRelatorio.Attributes.Add("onClick", "window.open('ExtratoColaborador.aspx','_blank')");

                }
            catch
                {
                    //TODO: Tratar exceção
                }
                 return View();
            }

        public ActionResult AlterarCadastro()
        {
            // Buscar estados
            string cEstados = oVFP.BuscaEstadoT();

            DataSet dsE = new DataSet();
            dsE.ReadXml(new StringReader(cEstados));
            //this.ddlEstados.DataSource = dsE.Tables[0];
            //this.ddlEstados.ValueField = "estado";
            //this.ddlEstados.TextField = "estado";
            //this.ddlEstados.DataBind();
            //ddlEstados.SelectedIndex = ddlEstados.Items.IndexOf(ddlEstados.Items.FindByText("SP"));
            //PreencheCidades("SP");
            //ddlCidades.SelectedIndex = 0;

            //lblNumCartao.Text = Session["cCartao"].ToString();
            int nCodFun = Convert.ToInt32(Session["uCodFun"].ToString());

            string cRetorno;
            cRetorno = oVFP.GetFunciona(nCodFun);

            XmlTextReader myXML = null;
            StringReader stream;
            stream = new StringReader(cRetorno);
            myXML = new XmlTextReader(stream);

            while (myXML.Read())
            {
                if (myXML.NodeType == XmlNodeType.Element)
                {
                    if (myXML.Name == "cpf")
                    {
                        myXML.Read();
                        usuario.CPF = myXML.ReadString();

                    }
                    if (myXML.Name == "nome")
                    {
                        myXML.Read();
                        //lblUsuario.Text = myXML.ReadString();
                    }
                }
            }
            myXML.Close();

            //Verificar se o funcionário já possui dados de cadastro
            string cPassword = Session["cPasswordFun"].ToString();
            string strXML;

            strXML = oVFP.LerCadFun(nCodFun, cPassword);

            myXML = null;
            stream = null;
            stream = new StringReader(strXML);
            myXML = new XmlTextReader(stream);

            string strDDD = "";
            string strTelefone = "";
            string strDDD1 = "";
            string strCelular = "";
            string iCidade = "";
            string strEstado = "";

            while (myXML.Read())
            {
                if (myXML.NodeType == XmlNodeType.Element)
                {
                    if (myXML.Name == "endereco")
                    {
                        myXML.Read();
                        string endereco = myXML.ReadString();
                        int startIndex = endereco.IndexOf(", ") + 2;
                        int intervalo = endereco.Length - startIndex;

                        try
                        {
                            usuario.Endereco = endereco.Substring(0, startIndex - 2);
                            usuario.Numero = endereco.Substring(startIndex, intervalo);
                        }
                        catch (Exception)
                        { }
                    }
                    if (myXML.Name == "bairro")
                    {
                        myXML.Read();
                        usuario.Bairro = myXML.ReadString();
                    }
                    if (myXML.Name == "complemen")
                    {
                        myXML.Read();
                        usuario.Complemento = myXML.ReadString();
                    }
                    if (myXML.Name == "id_cidade")
                    {
                        myXML.Read();
                        //Verificar Cidade
                        iCidade = myXML.ReadString();
                    }
                    if (myXML.Name == "estado")
                    {
                        myXML.Read();
                        strEstado = myXML.ReadString();
                        //ddlEstados.Value = strEstado;
                    }
                    if (myXML.Name == "cep")
                    {
                        myXML.Read();
                        usuario.CEP = myXML.ReadString();
                    }
                    if (myXML.Name == "sexo")
                    {
                        myXML.Read();
                        //ddlSexo.Value = myXML.ReadString();
                    }
                    if (myXML.Name == "rg")
                    {
                        myXML.Read();
                        usuario.RG = myXML.ReadString();
                    }
                    if (myXML.Name == "ddd1")
                    {
                        myXML.Read();
                        strDDD1 = myXML.ReadString();
                    }
                    if (myXML.Name == "telefone")
                    {
                        myXML.Read();
                        strTelefone = myXML.ReadString();
                    }
                    if (myXML.Name == "ddd")
                    {
                        myXML.Read();
                        strDDD = myXML.ReadString();
                    }
                    if (myXML.Name == "celular")
                    {
                        myXML.Read();
                        strCelular = myXML.ReadString();
                    }
                    if (myXML.Name == "nascimento")
                    {
                        myXML.Read();
                        string dNasciemnto = myXML.ReadString();
                        //ddlNascimento.Value = Convert.ToDateTime(dNasciemnto).ToString("dd/MM/yyyy");
                        //ddlNascimento.Text = Convert.ToDateTime(dNasciemnto).ToString("dd/MM/yyyy");
                    }
                    if (myXML.Name == "permite_email")
                    {
                        myXML.Read();
                        string sNew;
                        if (myXML.ReadString() == "1")
                            sNew = "true";
                        else sNew = "false";
                        //cbNew.Checked = (Convert.ToBoolean(sNew) ? true : false);
                    }
                    if (myXML.Name == "permite_sms")
                    {
                        myXML.Read();
                        string sSMS;
                        if (myXML.ReadString() == "1")
                            sSMS = "true";
                        else sSMS = "false";
                        //cbSMS.Checked = (Convert.ToBoolean(sSMS) ? true : false);
                    }
                }

                //PreencheCidades(strEstado);
                //ddlCidades.SelectedIndex = ddlCidades.Items.IndexOf(ddlCidades.Items.FindByText(strCidade));
                //ddlCidades.Value = iCidade;

                usuario.Tel = strDDD1 + strTelefone;
                usuario.Cel = strDDD + strCelular;
            }

            myXML.Close();

            return View();
        }

        [HttpPost]
        public ActionResult AlterarCadastro(Usuario usuario)
        {
            ////Montar XML
            string strEndereco;
            string strComplemento;
            string strBairro;
            string strCidade;
            int iCidade;

            string strEstado;
            string strRG;
            string strperEmail;
            string strSMS;

            string strCEP;
            string strSexo;

            string strDDD1;
            string strTelefone;

            string strDDD2;
            string strCelular;

            string strDataNascimento;

            //Validar Mask
            if (usuario.Cel.Replace("(  )     -", "").Length > 0 && usuario.Cel.Replace(" ", "").Length < 13)
            {
                usuario.lblMens = "* Favor verificar o número do celular!";
                return View();
            }
            if (usuario.Cel.Length < 14 && usuario.cbSMS == true)
            {
                usuario.lblMens = "* Favor preencher o número do Celular para receber SMS.";
                return View();
            }

            //strEndereco = RemoverAcentos(usuario.Endereco.Replace(",", "").ToUpper() + ", " + usuario.Numero);
            //strComplemento = RemoverAcentos(usuario.Complemento.ToUpper());
            //strBairro = RemoverAcentos(usuario.Bairro.ToUpper());

            //strCidade = ddlCidades.SelectedItem.Text;
            //iCidade = Convert.ToInt32(ddlCidades.Value.ToString());
            //strEstado = ddlEstados.Value.ToString();
            //strRG = RemoverAcentos(usuario.RG.Replace(".", "").Replace("-", ""));
            strCEP = usuario.CEP;
            strSexo = usuario.ddlSexo.ToString();
            strDDD1 = usuario.Tel.Substring(1, 2);
            strTelefone = usuario.Tel.Substring(5, 9);

            if (usuario.Cel.Length > 10)
            {
                strDDD2 = usuario.Cel.Substring(1, 2);
                strCelular = usuario.Cel.Substring(5, 9);
            }
            else
            {
                strDDD2 = "";
                strCelular = "";
            }

            //strDataNascimento = Convert.ToDateTime(ddlNascimento.Value).ToString("dd/MM/yyyy");

            //strperEmail = (usuario.cbNew.Checked ? "1" : "0");
            //strSMS = (usuario.cbSMS.Checked ? "1" : "0");

            StringBuilder sbDados = new StringBuilder();

            sbDados.Append("<endereco>");
            //sbDados.Append(strEndereco);
            sbDados.Append("</endereco>");

            sbDados.Append("<complemen>");
            //sbDados.Append(strComplemento);
            sbDados.Append("</complemen>");

            sbDados.Append("<bairro>");
            //sbDados.Append(strBairro);
            sbDados.Append("</bairro>");

            sbDados.Append("<cidade>");
            //sbDados.Append(strCidade);
            sbDados.Append("</cidade>");

            sbDados.Append("<id_cidade>");
            //sbDados.Append(iCidade);
            sbDados.Append("</id_cidade>");

            sbDados.Append("<estado>");
            //sbDados.Append(strEstado);
            sbDados.Append("</estado>");

            //Falta Colocar RG
            sbDados.Append("<rg>");
            //sbDados.Append(strRG);
            sbDados.Append("</rg>");

            sbDados.Append("<per_email>");
            //sbDados.Append(strperEmail);
            sbDados.Append("</per_email>");

            sbDados.Append("<per_sms>");
            //sbDados.Append(strSMS);
            sbDados.Append("</per_sms>");

            sbDados.Append("<cep>");
            sbDados.Append(strCEP);
            sbDados.Append("</cep>");

            sbDados.Append("<sexo>");
            sbDados.Append(strSexo);
            sbDados.Append("</sexo>");

            sbDados.Append("<ddd1>");
            sbDados.Append(strDDD1);
            sbDados.Append("</ddd1>");

            sbDados.Append("<telefone>");
            sbDados.Append(strTelefone);
            sbDados.Append("</telefone>");

            sbDados.Append("<ddd>");
            sbDados.Append(strDDD2);
            sbDados.Append("</ddd>");

            sbDados.Append("<celular>");
            sbDados.Append(strCelular);
            sbDados.Append("</celular>");

            sbDados.Append("<nascimento>");
            //sbDados.Append(strDataNascimento);
            sbDados.Append("</nascimento>");

            int nCodFun = Convert.ToInt32(Session["uCodFun"].ToString());
            string cPassword = Session["cPasswordFun"].ToString();

            systemweb.systemwebClass oVFP = new systemweb.systemwebClass();
            string strXML;
            strXML = oVFP.GravarCadFun(nCodFun, cPassword, sbDados.ToString());

            if (strXML.Substring(0, 4) == "0000")
            {
                Session["uValidar_Cadastro"] = "true";
                Session["uValidar"] = "true";
                Session["autentica"] = "1";
                ViewBag.Mensagem = "\\n Cadastro atualizado com sucesso!";
                //string strScript = "<script>alert('\\n Cadastro atualizado com sucesso!');window.location.href='padrao.aspx';</script>";
                //ClientScript.RegisterClientScriptBlock(typeof(string), string.Empty, strScript);

                //Response.Redirect("~/usuario/padrao.aspx");
            }
            else
            {
                usuario.lblMens = "* " + strXML.ToString();
            }
            return View();
        }
        
        public ActionResult TrocarSenha()
        {
            return View();
        }

        [HttpPost]
        public ActionResult TrocaSenhaSite(Usuario usuario)
        {
            if (usuario.NovaSenha.Length < 5)
            {
                usuario.lblMens = "A Nova Senha Precisa ser igual ou maior que 5 caracteres!";
                return View();
            }
            else
            {
                int nCodFun = Convert.ToInt32(Session["uCodFun"]);
                string cSenha = usuario.Senha.ToUpper();
                string cEmail;
                try
                {
                    cEmail = Session["email"].ToString();
                }
                catch
                {
                    cEmail = "";
                }

                string cCartao = Session["cCartao"].ToString();
                cCartao = cCartao.Trim().PadLeft(16, '0');
                int iCartao;
                if ((cCartao.Substring(0, 8) != "00000000") && (cCartao.Substring(0, 8) != "61033770"))
                {
                    usuario.lblMens = " * Número do Cartão Inválido!";
                    return View();
                }
                else
                {
                    try
                    {
                        iCartao = Convert.ToInt32(cCartao.Substring(8, 8));
                    }
                    catch
                    {
                        usuario.lblMens = " * Número do Cartão Inválido!";
                        return View();
                    }
                }

                systemweb.systemwebClass oVFP = new systemweb.systemwebClass();
                string strXML;
                strXML = oVFP.ValiFunci1(iCartao, cSenha);

                if (strXML.Substring(0, 4) == "0000")
                {
                    //Gravar Nova Senha
                    string strXMLGravarSenha;
                    string Mensagem;
                    cSenha = usuario.NovaSenha.ToUpper();

                    strXMLGravarSenha = oVFP.GravarSenha(nCodFun, cSenha, cEmail);

                    if (strXMLGravarSenha.Substring(0, 4) == "0000")
                    {

                        //Nova Senha
                        Session["cPasswordFun"] = cSenha;
                        string url = (Request.Url.Scheme + "://" + HttpContext.Request.Url.Authority);

                        Mensagem = "<br> Parabéns!!! Você alterou com sucesso sua senha para acesso ao SystemFarma. Data " + 
                            DateTime.Today.ToString("dd/MM/yyyy") + " <br><p>";
                        ////Mensagem = Mensagem + "Para continuar o seu cadastro você está recebendo sua senha. <br><p><p>";
                        Mensagem = Mensagem + "Sua Senha: " + usuario.NovaSenha + " <br><p><p>";

                        Mensagem = Mensagem + "Favor acessar o site do SystemFarma para poder consultar suas compras. <br><p>";
                        //Mensagem = Mensagem + "<a href ='" + url + "/System/login.aspx?RedirectUser=Usuario' alt='SystemFarma' target='_blank'>www.systemfarma.com.br</a> <br><p>";
                        Mensagem = Mensagem + "Qualquer dúvida entre em contato conosco ou informe-se pelo nosso site.";

                        if (cEmail != "")
                        {
                            //Usando a Classe enviar email
                            bool bRetorno = true;
                            string strDe = "";
                            EnviarEmail Envia = new EnviarEmail();
                            Envia.EnviaEmail(strDe, cEmail.ToLower(), "Confirmação de Alteração de Senha (SystemFarma)", "Envio de Email", Mensagem, ref bRetorno);
                        }

                        ViewBag.Mensagem = "\\n Você alterou com sucesso a senha de acesso ao SystemFarma.\\n *** SystemFarma Adm de Serviços Ltda ***')";
                       
                        //string strScript = "<script>alert('\\n Você alterou com sucesso a senha de acesso ao SystemFarma.\\n *** SystemFarma Adm de Serviços Ltda ***');window.location.href='padrao.aspx';</script>";
                        //ClientScript.RegisterClientScriptBlock(typeof(string), string.Empty, strScript);

                    }
                    else
                    {
                        usuario.lblMens = strXMLGravarSenha.ToString();
                    }
                }
                else
                {
                    usuario.lblMens = "Senha informada está incorreta!";
                }
            }
                return View();
        }

        [HttpPost]
        public ActionResult TrocaSenhaCartao(Usuario usuario)
        {
            if (usuario.NovaSenha.Length < 4)
            {
                usuario.lblMens = "A Nova Senha Precisa ter 4 caracteres numéricos!";
                return View();
            }
            else
            {
                int nCodFun = Convert.ToInt32(Session["uCodFun"].ToString());

                //Se precisar enviar senha para o email
                //string cEmail = Session["email"].ToString();

                string cCartao = Session["cCartao"].ToString();
                cCartao = cCartao.Trim().PadLeft(16, '0');
                int iCartao = Convert.ToInt32(cCartao.Substring(8, 8));
                string cChave = "s1#4@p$a";

                ////MD5 Senha Antiga
                string cSenha = usuario.Senha;
                System.Security.Cryptography.MD5 md5Senha = System.Security.Cryptography.MD5.Create();
                byte[] data = md5Senha.ComputeHash(System.Text.Encoding.Default.GetBytes(cSenha + cChave));
                System.Text.StringBuilder sbString = new System.Text.StringBuilder();
                for (int i = 0; i < data.Length; i++)
                    sbString.Append(data[i].ToString("x2"));

                string cHashA = sbString.ToString();

                ////MD5 Senha Nova
                string cSenhaNovaSenha = usuario.NovaSenha;
                System.Security.Cryptography.MD5 md5SenhaNova = System.Security.Cryptography.MD5.Create();
                byte[] data1 = md5SenhaNova.ComputeHash(System.Text.Encoding.Default.GetBytes(cSenhaNovaSenha + cChave));
                System.Text.StringBuilder sbStringNova = new System.Text.StringBuilder();
                for (int i = 0; i < data1.Length; i++)
                    sbStringNova.Append(data1[i].ToString("x2"));

                string cHashN = sbStringNova.ToString();

                systemweb.systemwebClass oVFP = new systemweb.systemwebClass();

                int nRetorno = oVFP.TrocaSenU(nCodFun, cHashA, cHashN);

                if (nRetorno > 0)
                    this.usuario.lblMens = "Senha trocada com sucesso!!!";
                else
                    this.usuario.lblMens = "Senha não foi trocada. " + oVFP.CMENSERRO;
            }
                return View();
        }

        [HttpPost]
        public ActionResult TrocarSenhaCartaoAdicional(Usuario usuario)
        {
            usuario.lblMens = "";

            int nCodAdic = Convert.ToInt32(Session["nCodAdic"]);
            string cUserID = Convert.ToString(Session["UserID"]);

            systemweb.systemwebClass oVFP = new systemweb.systemwebClass();
            string cRetorno = oVFP.GetAdicional(nCodAdic);
            DataSet myDataSet = new DataSet();
            myDataSet.ReadXml(new StringReader(cRetorno));
            DataView source = new DataView(myDataSet.Tables[0]);

            this.usuario.Nome = source[0]["nome"].ToString();

            if (usuario.NovaSenha.Length < 4)
            {
                usuario.lblMens = "A Nova Senha Precisa ter 4 caracteres numéricos!";
                return View();
            }
            else
            {
                int nCodFun = Convert.ToInt32(Session["uCodFun"].ToString());
                //int cUserID = Convert.ToInt32(Session["UserID"].ToString());

                //Se precisar enviar senha para o email
                string cEmail = Session["email"].ToString();
                int iCartao = Convert.ToInt32(Session["cCartao"]);

                ////MD5 Senha Antiga
                string cSenha = usuario.Senha;
                System.Security.Cryptography.MD5 md5Senha = System.Security.Cryptography.MD5.Create();
                byte[] data = md5Senha.ComputeHash(System.Text.Encoding.Default.GetBytes(cSenha));
                System.Text.StringBuilder sbString = new System.Text.StringBuilder();
                for (int i = 0; i < data.Length; i++)
                    sbString.Append(data[i].ToString("x2"));

                string cHashA = sbString.ToString();

                ////MD5 Senha Nova
                string cSenhaNovaSenha = usuario.NovaSenha;
                System.Security.Cryptography.MD5 md5SenhaNova = System.Security.Cryptography.MD5.Create();
                byte[] data1 = md5SenhaNova.ComputeHash(System.Text.Encoding.Default.GetBytes(cSenhaNovaSenha));
                System.Text.StringBuilder sbStringNova = new System.Text.StringBuilder();
                for (int i = 0; i < data1.Length; i++)
                    sbStringNova.Append(data1[i].ToString("x2"));

                string cHashN = sbStringNova.ToString();

                //int nCodAdic = Convert.ToInt32(Session["nCodAdic"]);

                int nRetorno = oVFP.TrocaSenAU(nCodFun, nCodAdic, cHashA, cHashN);

                if (nRetorno > 0)
                    this.usuario.lblMens = "Senha trocada com sucesso!!!";
                else
                    this.usuario.lblMens = "Senha não foi trocada. " + oVFP.CMENSERRO;
            }
                return View();
        }

        [HttpPost]
        public ActionResult TrocarCartaoNEstaUsando(Usuario usuario)
        {
            // Obter funcionário
            int nCodFun = Convert.ToInt32(Session["uCodFun"].ToString());

            string cUserID = Convert.ToString(Session["uMatricula"]);

            systemweb.systemwebClass oVFP = new systemweb.systemwebClass();
            string cRetorno = oVFP.GetFunciona(nCodFun);
            DataSet myDataSet = new DataSet();
            myDataSet.ReadXml(new StringReader(cRetorno));
            DataView source = new DataView(myDataSet.Tables[0]);

            string cSitua = source[0]["situacao"].ToString();
            if (cSitua.Length == 0)
                cSitua = " ";
            string cImpresso = source[0]["impresso"].ToString();
            if (cImpresso.Length == 0)
                cImpresso = " ";

            if ((cSitua != " ") | (cImpresso == "0"))
            {
                if (cSitua != " ")
                    this.usuario.Obs = "Troca não permitida: Situação do colaborador não está liberada.";
                else
                    this.usuario.Obs = "Troca não permitida: Cartão ainda não foi impresso.";
            }
            else
            {
                int nCartao = Convert.ToInt32(source[0]["cartao"]);
                int nRetorno = oVFP.TrocaCar(nCodFun, this.usuario.Motivo, nCartao, cUserID);
                if (nRetorno > 0)
                    this.usuario.Obs = "Cartão trocado. Novo número: " + nRetorno.ToString().PadLeft(8, '0');
                else
                    this.usuario.Obs = "ERRO: Cartão não foi trocado.";
            }
            //this.txtMotivo.ReadOnly = true;
            //this.cmdCancelar.Visible = false;
            //this.cmdConfirmar.Visible = false;
        
            return View();
        }

        public ActionResult CartaoAdicional()
        {          
            return View();
        }

        public ActionResult CartaoAdicionalRecebe(Usuario usuario)
        {
            int nCodFun = Convert.ToInt32(Session["uCodFun"]);

            systemweb.systemwebClass oVFP = new systemweb.systemwebClass();
            string cXML;

            cXML = oVFP.GetFuncAdic(nCodFun);

            DataSet myDataSet = new DataSet();
            myDataSet.ReadXml(new StringReader(cXML));
            DataView source = new DataView(myDataSet.Tables[0]);

           // this.ASPxGridView1.DataSource = source;
            //this.ASPxGridView1.DataBind();

            //Mudado dia 28/12/2014 (Senha - FLEXTRONICS)
            if (Session["uUSA_SENHA"].ToString() != "1")
            {
                //ASPxGridView1.Columns[6].Visible = false;
            }

            return View();
        }

        public ActionResult Cadastrar(Usuario usuario)
        {
            //Enviar para o Usuário
            string Mensagem;

            int iCartao;
            string sPrimNome;
            double dCpf;
            string cEmail;

            try
            {
                iCartao = Convert.ToInt32(usuario.Cartao.ToString().Trim());
            }
            catch
            {
                ModelState.AddModelError("CartaoInvalido", "Número do cartão inválido!");
                return View("Cadastro");
            }

            sPrimNome = usuario.Nome.Trim();
            iCartao = Convert.ToInt32(usuario.Cartao.ToString().Trim());
            dCpf = Convert.ToDouble(usuario.CPF.Replace(",", "").Replace(".", ""));
            cEmail = usuario.Email;

            systemweb.systemwebClass oVFP = new systemweb.systemwebClass();
            string strXML;

            strXML = oVFP.CheckFunci(iCartao, sPrimNome, dCpf, cEmail);

            //Se for retorno 0000 ---> Cadastrar Usuário
            //Se for retorno 9999 ---> Enviar Senha Novamente e Gravar Senha
            if (strXML.Substring(0, 4) == "0000")
            {
                Mensagem = "<br> Parabéns!!! Você realizou com sucesso o Pré-cadastro no SystemFarma. Data " + System.DateTime.Today.ToString("dd/MM/yyyy") + " <br><p>";
                Mensagem = Mensagem + "Para continuar o seu cadastro você está recebendo sua senha. <br>";
            }
            else if (strXML.Substring(0, 4) == "9999")
            {
                Mensagem = "<br>  Verificamos o seu cadastro no SystemFarma  . <br><p>";
                Mensagem = Mensagem + "Sua nova Senha foi gerada. <br><p>";
            }
            else
            {
                ModelState.AddModelError("Erro", strXML);
                return View("Cadastro");
            }

            try
            {
                //Codigo do Funcionario
                int nCodFun = Convert.ToInt32(strXML.Substring(strXML.IndexOf(" "), strXML.Length - strXML.IndexOf(" ")).Replace(" ", ""));

                //Gerar Senha de Acesso Usuários
                string strXMLGerarSenha;
                int iSeq = -1;
                strXMLGerarSenha = oVFP.GerarChaveAle(iSeq);
                string cSenha = "";

                if (strXMLGerarSenha.Substring(0, 4) == "0000")
                {
                    cSenha = strXMLGerarSenha.Replace("0000 ", "");
                }
                else
                {
                    ModelState.AddModelError("Erro", "* " + "GERARCHAVEALE " + strXMLGerarSenha.ToString());
                    return View("Cadastro");
                }
                //Gravar Senha no Banco
                string strXMLGravarSenha;
                strXMLGravarSenha = oVFP.GravarSenha(nCodFun, cSenha, cEmail);

                if (strXMLGravarSenha.Substring(0, 4) == "0000")
                {
                    string url = (Request.Url.Scheme + "://" + HttpContext.Request.Url.Authority);

                    ////Mensagem = "<br> Parabéns!!! Você realizou com sucesso o Pré-cadastro no SystemFarma. <br><p>";
                    ////Mensagem = Mensagem + "Para continuar o seu cadastro você está recebendo sua senha. <br><p><p>";
                    Mensagem = Mensagem + "Sua Senha: " + cSenha + " <br><p><p>";

                    Mensagem = Mensagem + "Favor acessar o site do SystemFarma e clicar na área Restrita, no selo de Usuários e assim fazer seu login para poder consultar suas compras. <br><p>";
                    //Mensagem = Mensagem + "<a href ='" + url + "/System/login.aspx?RedirectUser=Usuario' alt='SystemFarma' target='_blank'>www.systemfarma.com.br</a> <br><p>";
                    Mensagem = Mensagem + "Qualquer dúvida entre em contato conosco ou informe-se pelo nosso site.";

                    //Usando a Classe enviar email
                    bool bRetorno = true;
                    string strDe = "";
                    EnviarEmail Envia = new EnviarEmail();
                    Envia.EnviaEmail(strDe, usuario.Email.ToLower(), "Confirmação Pré-Cadastro (SystemFarma) " + System.DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"), "Envio de Email", Mensagem, ref bRetorno);

                    if (bRetorno)
                    {

                        ViewBag.MensagemSucesso = "\\n Você receberá instrucões por email para continuar o cadastro.\\n *** SystemFarma Adm. de Serviços Ltda ***";

                        //string strScript = "<script>alert('\\n Você receberá instrucões por email para continuar o cadastro.\\n *** SystemFarma Adm. de Serviços Ltda ***');window.location.href='LoginUser.aspx';</script>";
                        //ClientScript.RegisterClientScriptBlock(typeof(string), string.Empty, strScript);
                    }
                    else
                    {
                        ViewBag.MensagemSucesso = "\\n Problemas no envio de email!!\\n Não foi possível criar sua senha.\\n Favor tentar novamente mais tarde. \\n*** SystemFarma Adm. de Serviços Ltda ***";
                        //string strScript = "<script>alert('\\n Problemas no envio de email!!\\n Não foi possível criar sua senha.\\n Favor tentar novamente mais tarde. \\n*** SystemFarma Adm. de Serviços Ltda ***');window.location.href='LoginUser.aspx';</script>";
                        //ClientScript.RegisterClientScriptBlock(typeof(string), string.Empty, strScript);
                    }
                }
                else
                {
                    ModelState.AddModelError("Erro", "* " + strXMLGravarSenha.ToString());
                    return View("Cadastro");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Erro", "* " + "* " + ex.Message.ToString());
                return View("Cadastro");
            }
            return View();
        }
    }
        
        
    }
