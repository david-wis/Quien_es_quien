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
            if (BD.listaPersonajes.Count > 1) { 
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
            } else
            {
                //ViewBag.Error = "404";
                return View("AgregarPersonaje", "BackOffice");//Por ahora hagamos que te mande a agregar personaje
            }
            return View();
        }

        public ActionResult ElegirModoJuego()
        {
            List<string> listaTipos = new List<string>();
            listaTipos = BD.ObtenerTiposPartida();
            ViewBag.ListaTipos = listaTipos;
            return View();
        }

        public ActionResult ElegirModo(string nombre)
        {
            string Action = "";
            BD.TipoPartida = nombre;
            switch (BD.TipoPartida)
            {
                case "SinglePlayer":
                    Action = "ElegirCategoria";
                    break;
                case "MultiPlayer":
                    Action = "MenuMultiplayer";
                    break;
                case "Aprendiz":
                    break;
                case "Profesional":
                    break;
                case "Supremo":
                    break;
            }
            return RedirectToAction(Action, "Juego");
        }
        
        /*[HttpPost]

        public ActionResult ElegirModoJuego(string Tipo)
        {
            BD.TipoPartida = Tipo;
            return RedirectToAction("ElegirCategoria", "Juego");
        }*/

        public ActionResult ElegirCategoria()
        {
            BD.ObtenerCategoriasPersonajes();
            return View();
        }

        [HttpPost]
        public ActionResult ElegirCategoria(string Categoria)
        {
            BD.CategoriaJuego = Categoria;
            return RedirectToAction("Juego", "Juego");
        }

        [HttpPost]

        public ActionResult AgregarPartida(string Ganador, string Perdedor, string TipoPartida, int PuntosGanador)
        {
            BD.AgregarPartida(Ganador, Perdedor, TipoPartida, PuntosGanador);
            return RedirectToAction("holis"); //aca hay que cambiarlo porque no tengo idea a donde carajo va
        }

        public ActionResult MenuMultiplayer()
        {
            Dictionary<int, string> dicCurrentGames = BD.GetCurrentGames();
            ViewBag.dicCurrentGames = dicCurrentGames;
            return View();
        }

        public ActionResult Ganaste(int score)
        {
            Session["puntaje"] = score;
            BD.AgregarPartida(Session["nombre"].ToString(), null, BD.TipoPartida, score);
            return View();
        }

        public ActionResult Perdiste()
        {
            BD.AgregarPartida(null, Session["nombre"].ToString(), BD.TipoPartida, 0);
            return View();
        }

        public ActionResult Estadisticas()
        {
            Dictionary<string, int> dicTop10 = BD.RankingTop10();
            ViewBag.dicTop10 = dicTop10;
            int ID = BD.ObtenerIDUsuario(Session["nombre"].ToString());
            ViewBag.Jugadas = BD.CantidadJugadas(ID);
            ViewBag.Ganadas = BD.CantidadGanadas(ID);
            return View();
        }
        public ActionResult CrearPartida(string UsuarioCreador)
        {
            int idPartida = Convert.ToInt32(BD.AgregarPartida(UsuarioCreador, null, BD.TipoPartida, 0));
            Session["IDPartida"] = idPartida;
            return RedirectToAction("ElegirPersOtro", "Juego");
        }

        public ActionResult ElegirPersOtro()
        {
            BD.listaPersonajes = BD.ListarPersonajes(null);
            BD.ObtenerCategoriasPersonajes();
            foreach (Personaje p in BD.listaPersonajes)
            {
                MemoryStream imgStream = new MemoryStream(p.Foto);
                Image img = Image.FromStream(imgStream);
                img.Save(Server.MapPath("~/Content/Fotos/" + p.IDPersonaje + ".jpg"), System.Drawing.Imaging.ImageFormat.Jpeg);
            }
            List<Personaje> listaOrdenadaPers = BD.listaPersonajes.OrderBy(x => x.Nombre).ToList(); //Ordena alfabeticamente la lista y la guarda en una temporal
            BD.listaPersonajes = listaOrdenadaPers; //Asigno a la lista de la BD la lista ordenada alfabeticamente
            ViewBag.ListaCategorias = BD.dicCategorias;
            ViewBag.ListaPersonajes = BD.listaPersonajes;
            return View();
        }

        public ActionResult ElegirPO(int id)
        {
            BD.SeleccionarPersonajeMP(Convert.ToInt32(Session["IDPartida"]), 1, id);
            return RedirectToAction("SalaEspera", "Juego");
        }

        public ActionResult SalaEspera()
        {
            return View();
        }
    }
}