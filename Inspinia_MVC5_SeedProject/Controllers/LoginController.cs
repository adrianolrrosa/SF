using System;
using System.Web.Mvc;
using SystemFarma.Models;
using System.IO;
using System.Xml;
using BotDetect.Web.Mvc;

namespace SystemFarma.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [CaptchaValidation("CaptchaCode", "ExampleCaptcha", "Incorrect CAPTCHA code!")]
        public ActionResult AutenticaUsuario(Usuario usuario)
        {

            if (ModelState.IsValid)
            {
                string teste = "";
            }

            int iCartao = 0;
            string cPassword;
            string cCartao;

            cCartao = usuario.Cartao.ToString().Trim().PadLeft(16, '0');
            if ((cCartao.Substring(0, 8) != "00000000") && (cCartao.Substring(0, 8) != "61033770"))
            {
                ModelState.AddModelError("CartaoInvalido", "Número do cartão inválido!");
                return View("Index");
            }
            else
            {
                try
                {
                    iCartao = Convert.ToInt32(cCartao.Substring(8, 8));
                }
                catch
                {
                    ModelState.AddModelError("CartaoInvalido", "Número do cartão inválido!");
                    return View("Index");
                }
            }

            cPassword = usuario.Senha.Trim().ToUpper();
            Session["cPasswordFun"] = cPassword;
            Session["cCartao"] = usuario.Cartao;

            systemweb.systemwebClass oVFP = new systemweb.systemwebClass();
            string strXML;
            strXML = oVFP.ValiFunci1(iCartao, cPassword);

            if ((strXML.Substring(0, 4) == "0000") || (strXML.Substring(0, 4) == "9999"))
            {

                int nCodFun = Convert.ToInt32(strXML.Substring(strXML.IndexOf(" "), strXML.Length - strXML.IndexOf(" ")).Replace(" ", ""));
                Session["uCodFun"] = nCodFun.ToString();

                //Validar o Funcionário
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
                        if (myXML.Name == "nome")
                        {
                            myXML.Read();
                            Session["uFunNome"] = myXML.ReadString();

                        }
                        if (myXML.Name == "cod_emp")
                        {
                            myXML.Read();
                            Session["uCodEmp"] = myXML.ReadString();
                        }
                        if (myXML.Name == "matricula")
                        {
                            myXML.Read();
                            Session["uMatricula"] = myXML.ReadString();
                        }

                    }
                }
                myXML.Close();


                // Criar variável de sessão
                Session["uColaborador"] = Session["uMatricula"].ToString() +
                    " - " + Session["uFunNome"].ToString();

                string cRetorno1;
                cRetorno1 = oVFP.GetEmpresa(Convert.ToInt32(Session["uCodEmp"]));

                XmlTextReader myXML1 = null;
                StringReader stream1;
                stream1 = new StringReader(cRetorno1);
                myXML1 = new XmlTextReader(stream1);

                Session["uUSA_SENHA"] = null;

                while (myXML1.Read())
                {
                    if (myXML1.NodeType == XmlNodeType.Element)
                    {
                        if (myXML1.Name == "ver_prod")
                        {
                            myXML1.Read();
                            Session["uVerProd"] = myXML1.ReadString();
                        }

                        //Mudado dia 28/01/2014 - senha (FLEXTRONICS)
                        if (myXML1.Name == "usa_senha")
                        {
                            myXML1.Read();
                            Session["uUSA_SENHA"] = myXML1.ReadString();
                        }
                    }
                }
                myXML1.Close();

                //Autenticado
                string strXML_Autenticado;
                strXML_Autenticado = oVFP.LerCadFun(nCodFun, cPassword);

                myXML = null;
                stream = null;
                stream = new StringReader(strXML_Autenticado);
                myXML = new XmlTextReader(stream);


                while (myXML.Read())
                {
                    if (myXML.NodeType == XmlNodeType.Element)
                    {
                        if (myXML.Name == "email")
                        {
                            myXML.Read();
                            Session["email"] = myXML.ReadString();
                        }
                    }
                }

            }

            //Usuário com o cadastro OK = "0000"
            //Usuário sem estar cadastrado = "9999"
            if (strXML.Substring(0, 4) == "0000")
            {
                Session["uValidar"] = "true";
                Session["autentica"] = "1";
                Session["uValidar_Cadastro"] = "true";
                Response.Redirect("~/Usuario/Padrao.aspx", false);

            }
            else if (strXML.Substring(0, 4) == "9999")
            {

                Session["uValidar"] = "true";
                Session["autentica"] = "1";
                Session["uValidar_Cadastro"] = "false";
                Response.Redirect("~/Usuario/Cadastro.aspx", false);

            }
            else
            {
                ModelState.AddModelError("Erro", strXML);
                return View("Index");
            }

            return View();
        }
    }
}