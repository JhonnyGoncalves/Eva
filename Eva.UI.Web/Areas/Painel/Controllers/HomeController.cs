﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Eva.Aplicacao;
using Eva.Dominio;
using Eva.UI.Web.Helpers;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

namespace Eva.UI.Web.Areas.Painel.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly UsuarioAplicacao usuarioApp;

        public HomeController()
        {
            usuarioApp = Fabrica.UsuarioAplicacaoMongo();
        }
        public ActionResult Index()
        {
            this.Flash("Aqui vai as mensagem de usuario sem permissoa", FlashEnum.Info);
            return View();
        }

        [AllowAnonymous]
        public ActionResult Login(LoginViewModel usuario)
        {
            if (Request.HttpMethod == "POST")
            {
                var usuarioLogado = usuarioApp.Login(usuario.Login, usuario.Senha);
                if (usuarioLogado != null)
                {
                    Seguranca.GerearSessaoDeUsuario(usuarioLogado);
                    return RedirectToAction("Index");
                }

                this.Flash("Dados de acesso não são válidos!", FlashEnum.Error);
            }

            return View(usuario);
        }

        public ActionResult Sair()
        {
            Seguranca.DestruirSessaoDeUsuario();
            return RedirectToAction("Login");
        }

        //Todo: Instalacao inicial pros Devs
        public string Instalar()
        {
            if (Fabrica.UsuarioAplicacaoMongo().Login("eva@eva.com.br", "eva") == null)
            {
                var grupo = new GrupoDeUsuario()
                {
                    Nome = "Admin",
                    Permissoes = new[] {"usuarios,noticias"} //nao ta funcionando ainda
                };
                Fabrica.GrupoDeUsuarioAplicacaoMongo().Salvar(grupo);
                var usuario = new Usuario()
                {
                    Nome = "Eva",
                    Email = "eva@eva.com.br",
                    Grupo = grupo,
                    Senha = "eva"
                };
                Fabrica.UsuarioAplicacaoMongo().Salvar(usuario);

                return "Usuario: eva@eva.com.br --- Senha: eva";
            }
            return "O sistema já foi instalado";
        }

    }

    public class LoginViewModel
    {
        public string Login { get; set; }
        public string Senha { get; set; }
        public bool Lembrar { get; set; }
    }
}