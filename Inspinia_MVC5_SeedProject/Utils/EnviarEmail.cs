using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace SystemFarma.Utils
{
    public class EnviarEmail
    {

        public string ErroMens = "";
        //private bool Retorno = false; 

        public void EnviaEmail(string De, string Para, string Assunto, string TituloPagina, string Mensagem, ref bool bRetorno)
        {

            System.Text.StringBuilder Corpo = new System.Text.StringBuilder();

            //*** Monta a mensagem 
            {
                Corpo.Append("<!DOCTYPE html PUBLIC '-//W3C//DTD XHTML 1.0 Transitional//EN' 'http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd'>" + (char)13 + (char)10);
                Corpo.Append("<html xmlns='http://www.w3.org/1999/xhtml' lang='pt-BR'>" + (char)13 + (char)10);
                Corpo.Append("<head>" + (char)13 + (char)10);
                Corpo.Append("<title>" + TituloPagina + " </title>" + (char)13 + (char)10);
                Corpo.Append("<style type='text/css'>td img {display: block;}</style>" + (char)13 + (char)10);
                Corpo.Append("</head>" + (char)13 + (char)10);
                Corpo.Append("<body bgcolor='#ffffff'>" + (char)13 + (char)10);
                //Corpo.Append("<img border='0' alt='' src='" + ConfigurationManager.AppSettings("EnderecoSite") + "/Institucional/Mailing/CorpoMailing.aspx?IdEmail=" + IdEmail + "&IdEnvio=" + IdEnvio + "'></a>" + (char)13 + (char)10); 
                //.Append("<div style='text-align: center; font-family: verdana; font-size: 10px; padding-top: 10px'>" & vbCrLf) 
                //.Append("Problemas para visualizar este e-mail adequadamente? Acesse o link: <a href='" & ConfigurationManager.AppSettings("EnderecoSite") & "/SystemFarma/Mailing/Default.aspx'>http://www.systemfarma.com.br/mailing/</a>" & vbCrLf) 
                //.Append("</div>" & vbCrLf) 
                Corpo.Append("<div style='text-align: left; font-family: verdana; font-size: 12px; padding-top: 10px'>" + (char)13 + (char)10);
                Corpo.Append(Mensagem);
                //Corpo.Append("Para deixar de rebecer os e-mails, <a href='www.systemfarma.com.br/Institucional/'>clique Aqui!</a>" ); 

                //Corpo.Append("Para deixar de rebecer os e-mails, <a href='" + ConfigurationManager.AppSettings("EnderecoSite") + "/Institucional/Mailing/RemoveMailing.aspx'>clique Aqui!</a>" + Constants.vbCrLf); 
                Corpo.Append("</div>" + (char)13 + (char)10);
                Corpo.Append("</body>" + (char)13 + (char)10);
                Corpo.Append("</html>" + (char)13 + (char)10);
            }
            try
            {

                DataSet ds = new DataSet();
                ds.ReadXml(HttpContext.Current.Server.MapPath("~/Usuario/XML/Conta_Email.xml"));
                DataView dsContaEmail = new DataView(ds.Tables[0]);

                string strEmail = dsContaEmail[0]["Email"].ToString();
                string strSenha = dsContaEmail[0]["Senha"].ToString();
                string strServidorEntradaPOP = dsContaEmail[0]["ServidorEntradaPOP"].ToString();
                string strServidorSaidaSMTP = dsContaEmail[0]["ServidorSaidaSMTP"].ToString();
                int strServidorSaidaPort = Convert.ToInt16(dsContaEmail[0]["ServidorSaidaPort"].ToString());
                bool bAutenticacao = bool.Parse(dsContaEmail[0]["Autenticacao"].ToString());

                ds.Clear();
                ds.Dispose();
                dsContaEmail.Dispose();

                ds.Clear();
                ds.Dispose();
                dsContaEmail.Dispose();

                MailAddress FromMail = new MailAddress(strEmail, "SystemFarma");
                MailAddress ToMail = new MailAddress(Para);

                MailMessage Mail = new MailMessage(FromMail, ToMail);
                {
                    Mail.Subject = Assunto;
                    Mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
                    Mail.IsBodyHtml = true;
                    Mail.Body = Corpo.ToString();
                    Mail.BodyEncoding = System.Text.Encoding.GetEncoding("iso-8859-1");
                }

                SmtpClient mailClient = new SmtpClient();
                {
                    mailClient.Host = strServidorSaidaSMTP;
                    mailClient.Port = strServidorSaidaPort;
                    mailClient.UseDefaultCredentials = bAutenticacao;
                    mailClient.Credentials = new System.Net.NetworkCredential(strEmail, strSenha);
                    mailClient.Send(Mail);
                }
            }
            catch (Exception ex)
            {
                ErroMens = ex.Message.ToString();
                bRetorno = false;
                return;
            }

            bRetorno = true;
            return;
        }

        /// <summary> 
        /// </summary> 
        /// <param name="De">E-mail do remetente</param> 
        /// <param name="Para">E-mail do destinatário</param> 
        /// <param name="Assunto">Assunto do e-mail</param> 
        /// <param name="TituloPagina">Título escrito na página</param> 
        /// <param name="Mensagem">Mensagem a se enviada</param> 
        /// <param name="ExibeCabecalho">Se será exibido o cabeçalho de assinatura padrão</param> 
        /// <param name="ExibeRemover">Se será exibido o rodapé de assinatura padrão</param> 
        /// <param name="IdMailing">Usado para remover e-mail da listagem de newsletter</param> 
        /// <returns>True se enviado com sucesso</returns> 
        /// <remarks></remarks> 

    }

}