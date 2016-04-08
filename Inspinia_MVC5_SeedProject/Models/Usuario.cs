using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SystemFarma.Models
{
    public class Usuario
    {
        public int Cartao { get; set; }
        public string Senha { get; set; }
        public string Nome { get; set; }
        public string CPF { get; set; }
        public string Email { get; set; }
        public string Mensagem { get; set; }
        public int iCartao { get; set; }
        public string sPrimNome { get; set; }
        public double dCpf { get; set; }
        public string cEmail { get; set; }
        public string Data { get; set; }
        public string Obs { get; set; }
        public string DDLSitua { get; set; }
        public string Dado1 { get; set; }
        public string Dado2 { get; set; }
        public string lblErro { get; set; }
        public string cmdCancelar { get; set; }
        public string cmdConfirmar { get; set; }
        public string NovaSenha { get; set; }
        public string lblMens { get; set; }
        public string Motivo { get; set; }
        public string lblPeriodo { get; set; }
        public string lblExtrato { get; set; }
        public string lblColaborador { get; set; }
        public string lblAviso { get; set; }
        public string Endereco { get; set; }
        public string Numero { get; set; }
        public string Bairro { get; set; }
        public string Complemento { get; set; }
        public string CEP { get; set; }
        public string RG { get; set; }
        public string Cel { get; set; }
        public string Tel { get; set; }
        public bool cbSMS { get; set; }
        public bool cbNew { get; set; }
        public bool ddlSexo { get; set; }



    }
}