using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Quien_es_Quien.Controllers
{
    public class JuegoController : Controller
    {
        // GET: Juego
        public ActionResult Juego()
        {
            BD.ListarPersonajes(null);//Agregar para traer por categoria
            ViewBag.listaPreguntas = BD.ObtenerPreguntas(null);
            ViewBag.listaIDPreguntas = BD.ObtenerPreguntasPersonaje();
            return View();
        }

        [HttpPost]
        public ActionResult Juego(int idPregunta)
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
    }
}