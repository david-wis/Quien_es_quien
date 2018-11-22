using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Quien_es_Quien.Controllers
{
    public class JuegoController : Controller
    {
        public ActionResult Juego(string nombre = null)
        {
            BD.listaPersonajes = BD.ListarPersonajes(nombre);//Agregar para traer por categoria
            foreach (Personaje p in BD.listaPersonajes) //Cargar fotos en la carpetita
            {
                MemoryStream imgStream = new MemoryStream(p.Foto);
                Image img = Image.FromStream(imgStream);
                img.Save(Server.MapPath("~/Content/Fotos/" + p.IDPersonaje + ".jpg"), System.Drawing.Imaging.ImageFormat.Jpeg);
            }

            Random rnd = new Random();
            int persElegido = BD.listaPersonajes[rnd.Next(BD.listaPersonajes.Count)].IDPersonaje;
            ViewBag.persElegido = persElegido;

            ViewBag.listaPreguntas = BD.ObtenerPreguntas(null);

            foreach (Personaje p in BD.listaPersonajes)
            {
                p.ListaPregs = BD.ObtenerPreguntasPersonaje(p.IDPersonaje);
            }

            return View();
        }


        [HttpPost]
        public ActionResult Juego(bool Victoria)
        {
            return View();
        }

        public ActionResult ElegirModoJuego()
        {
            List<string> listaTipos = new List<string>();
            listaTipos = BD.ObtenerTiposPartida();
            ViewBag.ListaTipos = listaTipos;
            return View();
        }
        
        [HttpPost]

        public ActionResult ElegirModoJuego(string Tipo)
        {
            BD.TipoPartida = Tipo;
            return RedirectToAction("ElegirCategoria", "Juego");
        }

        public ActionResult ElegirCategoria()
        {
            BD.ObtenerCategoriasPersonajes();
            return View();
        }

        [HttpPost]

        public ActionResult ElegirCategoria(string Categoria)
        {
            BD.CategoriaJuego = Categoria;
            return RedirectToAction("Index", "Juego");
        }

        [HttpPost]

        public ActionResult AgregarPartida(string Ganador, string Perdedor, int PuntosGanador, string TipoPartida)
        {
            BD.AgregarPartida(Ganador, Perdedor, PuntosGanador, TipoPartida);
            return RedirectToAction("holis"); //aca hay que cambiarlo porque no tengo idea a donde carajo va
        }

        public ActionResult Ganaste()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Ganaste(int score)
        {
            Session["puntaje"] = score;
            return View("Ganaste");
        }

        public ActionResult Perdiste()
        {
            return View();
        }

        public ActionResult RankingTop10()
        {
            Dictionary<string, int> dicTop10 = BD.RankingTop10();
            ViewBag.dicTop10 = dicTop10;
            return View();
        }
    }
}