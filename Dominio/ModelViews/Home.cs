using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace minimal_api.Dominio.ModelViews
{
    public struct Home
    {
        public string Mensagem => "Bem-vindo à API de veículos - Minimal API";
        public string Documentacao => "/swagger";
        public string Versao => "v1.0";
        public string Contato => "suporte@seudominio.com";
    }
}