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
        public ActionResult Juego()
        {
            BD.listaPersonajes = BD.ListarPersonajes(null);//Agregar para traer por categoria
            foreach (Personaje p in BD.listaPersonajes) //Cargar fotos en la carpetita
            {
                MemoryStream imgStream = new MemoryStream(p.Foto);
                Image img = Image.FromStream(imgStream);
                img.Save(Server.MapPath("~/Content/Fotos/" + p.IDPersonaje + ".jpg"), System.Drawing.Imaging.ImageFormat.Jpeg);
            }

            ViewBag.listaPreguntas = BD.ObtenerPreguntas(null);

            foreach (Personaje p in BD.listaPersonajes)
            {
                p.ListaPregs = BD.ObtenerPreguntasPersonaje(p.IDPersonaje);
            }

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