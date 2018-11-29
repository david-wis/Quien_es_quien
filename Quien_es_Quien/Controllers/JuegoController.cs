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
            if (BD.listaPersonajes.Count > 1)
            {
                foreach (Personaje p in BD.listaPersonajes) //Cargar fotos en la carpetita
                {
                    string imgSrc = "data:Image/png;base64," + Convert.ToBase64String(p.Foto);
                    p.RutaFoto = imgSrc;
                }

                Random rnd = new Random();
                int persElegido = BD.listaPersonajes[rnd.Next(BD.listaPersonajes.Count)].IDPersonaje;
                ViewBag.persElegido = persElegido;

                ViewBag.listaPreguntas = BD.ObtenerPreguntas(null);

                foreach (Personaje p in BD.listaPersonajes)
                {
                    p.ListaPregs = BD.ObtenerPreguntasPersonaje(p.IDPersonaje);
                }
            }
            else
            {
                //ViewBag.Error = "404";
                return View("AgregarPersonaje", "BackOffice");//Por ahora hagamos que te mande a agregar personaje
            }
            return View();
        }

        public ActionResult ElegirModoJuego(bool? invitado = null)
        {
            if (invitado == true)
            {
                Session["nombre"] = "Anonimo";
                Session["administrador"] = false;
            }
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

        public ActionResult MenuMultiplayer()
        {
            Dictionary<int, string> dicCurrentGames = BD.GetCurrentGames();
            ViewBag.dicCurrentGames = dicCurrentGames;
            return View();
        }

        [HttpPost]
        public void CargarPuntaje(int score)
        {
            Session["puntaje"] = score;
        }

        public ActionResult Ganaste(bool? Multiplayer = null)
        {
            string Nombre;
            string sPuntaje;
            int puntaje = 0;
            try
            {
                sPuntaje = Session["puntaje"].ToString();
                puntaje = Convert.ToInt32(sPuntaje);
            } 
            catch (NullReferenceException)
            {
                ViewBag.puntaje = -1;
            }

            try {
                Nombre = Session["nombre"].ToString();
            } catch (NullReferenceException) {
                Nombre = "Anonimo";
            }
            if (Multiplayer == null)
            {
                BD.AgregarPartida(Nombre, null, BD.TipoPartida, puntaje);
            }
            return View();
        }

        public ActionResult Perdiste(bool? Multiplayer = null)
        {
            if (Multiplayer == null) { 
                BD.AgregarPartida(null, Session["nombre"].ToString(), BD.TipoPartida, 0);
            }
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
            Response.Cookies["MiJugador"].Value = "1";
            Response.Cookies["MiJugador"].Expires = DateTime.Now.AddHours(1);
            return RedirectToAction("SalaEspera", "Juego");
        }

        public ActionResult ElegirPersOtro()
        {
            BD.ObtenerCategoriasPersonajes();//En un futuro elegir categoria desde esta view
            BD.listaPersonajes = BD.ListarPersonajes(null);
            BD.ObtenerCategoriasPersonajes();
            foreach (Personaje p in BD.listaPersonajes)
            {
                string imgSrc = "data:Image/png;base64," + Convert.ToBase64String(p.Foto);
                p.RutaFoto = imgSrc;
            }
            List<Personaje> listaOrdenadaPers = BD.listaPersonajes.OrderBy(x => x.Nombre).ToList(); //Ordena alfabeticamente la lista y la guarda en una temporal
            BD.listaPersonajes = listaOrdenadaPers; //Asigno a la lista de la BD la lista ordenada alfabeticamente
            ViewBag.ListaCategorias = BD.dicCategorias;
            ViewBag.ListaPersonajes = BD.listaPersonajes;
            return View();
        }

        public ActionResult ElegirPO(int id)
        {
            int IDPartida = Convert.ToInt32(Session["IDPartida"]);
            int ValorJugador = Convert.ToInt32(Server.HtmlEncode(Request.Cookies["MiJugador"].Value));
            int idCat = Convert.ToInt32(Server.HtmlEncode(Request.Cookies["CategoriaPersonaje"].Value));
            BD.SeleccionarPersonajeMP(IDPartida, ValorJugador, id, idCat);
            return RedirectToAction("SalaEsperaSeleccionPersonajes", "Juego");
        }

        public ActionResult SalaEspera()
        {
            ViewBag.jugador2Listo = BD.VerificarConexionJugador2(Convert.ToInt32(Session["IDPartida"]));
            return View();
        }

        public ActionResult SalaEsperaSeleccionPersonajes()
        {
            ViewBag.otroJugadorListo = BD.VerificarPersElegidos(Convert.ToInt32(Session["IDPartida"]));
            return View();
        }

        public ActionResult ConectarseAPartida(int IDPartida)
        {
            BD.ConnectToGame(IDPartida, Session["Nombre"].ToString());
            Session["IDPartida"] = IDPartida;
            Response.Cookies["Turno"].Expires = DateTime.Now.AddDays(-1d);
            Response.Cookies["MiJugador"].Value = "0";//Queria probar las cookies comunes sin viewbag
            Response.Cookies["MiJugador"].Expires = DateTime.Now.AddHours(1);
            return RedirectToAction("ElegirPersOtro", "Juego");
        }

        public ActionResult MultiplayerJuego()
        {
            int IDPartida = Convert.ToInt32(Session["IDPartida"]);
            int MiJugador = Convert.ToInt32(Server.HtmlEncode(Request.Cookies["MiJugador"].Value));
            string catMP = BD.ObtenerCategoriaMP(IDPartida, MiJugador);
            if (catMP == "")
            {
                catMP = null;
            }
            BD.listaPersonajes = BD.ListarPersonajes(catMP);
            if (BD.listaPersonajes.Count > 1)
            {
                foreach (Personaje p in BD.listaPersonajes) //Cargar fotos en la carpetita
                {
                    string imgSrc = "data:Image/png;base64," + Convert.ToBase64String(p.Foto);
                    p.RutaFoto = imgSrc;
                }

                //Random rnd = new Random();
                int persElegido = BD.ObtenerPersonajeMP(Convert.ToInt32(Session["IDPartida"]), Convert.ToInt32(Server.HtmlEncode(Request.Cookies["MiJugador"].Value)));
                ViewBag.persElegido = persElegido;

                ViewBag.listaPreguntas = BD.ObtenerPreguntas(null);

                foreach (Personaje p in BD.listaPersonajes)
                {
                    p.ListaPregs = BD.ObtenerPreguntasPersonaje(p.IDPersonaje);
                }
            }
            else
            {
                //ViewBag.Error = "404";
                return RedirectToAction("AgregarPersonaje", "BackOffice", new { area = "" });//Por ahora hagamos que te mande a agregar personaje
            }
            return View();
        }

        public void VerificarTurno()
        {
            int turno = BD.VerificarTurno(Convert.ToInt32(Session["IDPartida"]));
            Response.Cookies["Turno"].Value = turno.ToString();
            Response.Cookies["Turno"].Expires = DateTime.Now.AddHours(1);
        }

        [HttpPost]
        public void CambiarTurno(bool Victoria)
        {
            BD.CambiarTurno(Convert.ToInt32(Session["IDPartida"]), Victoria);
            int MiJugador = Convert.ToInt32(Server.HtmlEncode(Request.Cookies["MiJugador"].Value));
            if (MiJugador == 0)
            {
                Response.Cookies["Turno"].Value = "1";
            }
            else
            {
                Response.Cookies["Turno"].Value = "0";
            }

        }

        public ActionResult GanasteMultiplayer()
        {
            int IDPartida = Convert.ToInt32(Session["IDPartida"]);
            int MiJugador = Convert.ToInt32(Server.HtmlEncode(Request.Cookies["MiJugador"].Value));
            BD.CargarPartidaMultiplayer(IDPartida, MiJugador, -1);
            return RedirectToAction("Ganaste", "Juego");
        }

        public ActionResult PerdisteMultiplayer()
        {
            return RedirectToAction("Perdiste", "Juego");
        }

        public void LimpiarBasura()
        {
            BD.LimpiarPartidas();
        }
    }
}