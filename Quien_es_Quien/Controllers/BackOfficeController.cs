using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Drawing;
using System.IO;

namespace Quien_es_Quien.Controllers
{
    public class BackOfficeController : Controller
    {
        // GET: BackOffice
        public ActionResult IndexAdmins()
        {
            return View();
        }

        public ActionResult Login()
        {
            if(Session["nombre"] != null)
            {
                if ((bool)Session["administrador"] == true)
                {
                    return RedirectToAction("IndexAdmins", "BackOffice");
                }
                else
                {
                    return RedirectToAction("Index", "Juego");
                }
            }
            return View();
        }

        [HttpPost]

        public ActionResult Login(string nombre, string contrasenia)
        {
            //0 = login incorrecto
            //1 = login correcto y no es administrador 
            //2 = login correcto y es administrador
            if (Convert.ToInt32(BD.Login(nombre, contrasenia)) == 0)
            {
                ViewBag.Mensaje = "Error, login incorrecto";
                return View();
            }
            else if (Convert.ToInt32(BD.Login(nombre, contrasenia)) == 1)
            {
                Session["nombre"] = nombre;
                Session["administrador"] = false;
                return RedirectToAction("ElegirModoJuego", "Juego");
            }
            else
            {
                Session["nombre"] = nombre;
                Session["administrador"] = true;
                return RedirectToAction("IndexAdmins", "BackOffice");
            }
        }

        public ActionResult CambiarContrasenia()
        {
            return View();
        }

        [HttpPost]

        public ActionResult CambiarContrasenia(string nombre, string dni, string password)
        {
            int DNIPosta;
            bool exito = int.TryParse(dni, out DNIPosta);
            if (exito)
            {
                string retorno = BD.CambiarContrasenia(nombre, DNIPosta, password);
                if (retorno == "1")
                {
                    return RedirectToAction("Login", "BackOffice");
                }
                else
                {
                    ViewBag.Mensaje = retorno;
                }
            }
            else
            {
                ViewBag.Mensaje = "El DNI ingresado es invalido";
            }
            return View();
        }

        public ActionResult Registro()
        {
            return View();
        }

        [HttpPost]

        public ActionResult Registro(string nombre, string contrasenia, string dni, string pin, bool admin = false)
        {
            int DNIPosta;
            bool exitoDNI = int.TryParse(dni, out DNIPosta);
            int PinPosta = 0; //Si no quiere ser admin, se manda cero
            bool exitoPin = false;
            if (admin)
            {
                exitoPin = int.TryParse(pin, out PinPosta);
            }
            if (exitoDNI && DNIPosta >= 10000000 && DNIPosta <= 99999999)
            {
                if ((exitoPin == true) || (admin == false)) //Si selecciono admin pero no se pudo convertir hay ERROR
                {
                    string retorno = BD.Registrar(nombre, contrasenia, DNIPosta, PinPosta);
                    if (retorno == "1")
                    {
                        return RedirectToAction("Login", "BackOffice");
                    }
                    else
                    {
                        ViewBag.Mensaje = retorno;
                    }
                }
                else
                {
                    ViewBag.Mensaje = "El PIN ingresado es inválido";
                }
            }
            else
            {
                ViewBag.Mensaje = "El DNI ingresado es inválido";
            }
            return View();
        }

        public ActionResult AgregarPersonaje()
        {
            return View();
        }

        [HttpPost]

        public ActionResult AgregarPersonaje(Personaje unPersonaje)
        {
            if (unPersonaje.Img != null)
            {
                string NuevaUbicacion = Server.MapPath("~/Content/Fotos/") + unPersonaje.Img.FileName;
                unPersonaje.Img.SaveAs(NuevaUbicacion);
                unPersonaje.RutaFoto = NuevaUbicacion;
                unPersonaje.Foto = ImageToByteArray(Image.FromFile(unPersonaje.RutaFoto));
                string sRespuesta = BD.AgregarPersonaje(unPersonaje);
                if (sRespuesta == "1")
                {
                    Session["Mensaje"] = "El personaje se agrego correctamente";
                    ViewBag.listaPreguntas = BD.ObtenerPreguntas(null);
                }
                else
                {
                    Session["Mensaje"] = sRespuesta;
                }
            }
            else
            {
                Session["Error"] = 404;
            }
            return RedirectToAction("ListarPersonajes", "BackOffice");
        }

        public ActionResult ModificarPersonaje(int id)
        {
            int iCantLista = BD.listaPersonajes.Count();
            int iPosLista = 0;
            bool encontrado = false;
            Personaje PersModificar = new Personaje();

            while (!encontrado && iPosLista < iCantLista)
            {
                if (BD.listaPersonajes[iPosLista].IDPersonaje == id)
                {
                    encontrado = true;
                    PersModificar = BD.listaPersonajes[iPosLista];

                }
                else
                {
                    iPosLista++;
                }
            }
            ViewBag.NomPersModificar = PersModificar.Nombre; //Se muestra despues en la View
            Session["IDPersonaje"] = id;
            return View("ModificarPersonaje", PersModificar);
        }

        [HttpPost]

        public ActionResult ModificarPersonaje(Personaje unPers)
        {
            unPers.IDPersonaje = (int) Session["IDPersonaje"];
            if (unPers.Img != null)
            {
                string NuevaUbicacion = Server.MapPath("~/Content/Fotos/") + unPers.Img.FileName;
                unPers.Img.SaveAs(NuevaUbicacion);
                unPers.RutaFoto = NuevaUbicacion;
                unPers.Foto = ImageToByteArray(Image.FromFile(unPers.RutaFoto));
            }

            unPers.Categoria = BD.dicCategorias[unPers.IDCategoria];
            string sRespuesta = BD.ModificarPersonaje(unPers);
            if (sRespuesta == "1")
            {
                Session["Mensaje"] = "El personaje se modifico correctamente";
            }
            else
            {
                Session["Mensaje"] = sRespuesta;
            }
            return RedirectToAction("ListarPersonajes", "BackOffice");
        }

        public ActionResult ListarPersonajes()
        {
            BD.listaPersonajes = BD.ListarPersonajes(null);
            BD.ObtenerCategoriasPersonajes();
            foreach (Personaje p in BD.listaPersonajes)
            {
                /*MemoryStream imgStream = new MemoryStream(p.Foto);
                Image img = Image.FromStream(imgStream);
                img.Save(Server.MapPath("~/Content/Fotos/" + p.IDPersonaje + ".jpg"), System.Drawing.Imaging.ImageFormat.Jpeg);*/
                string imgSrc = "data:Image/png;base64," + Convert.ToBase64String(p.Foto);
                p.RutaFoto = imgSrc;
            }
            List<Personaje> listaOrdenadaPers = BD.listaPersonajes.OrderBy(x => x.Nombre).ToList(); //Ordena alfabeticamente la lista y la guarda en una temporal
            BD.listaPersonajes = listaOrdenadaPers; //Asigno a la lista de la BD la lista ordenada alfabeticamente
            ViewBag.ListaCategorias = BD.dicCategorias;
            ViewBag.ListaPersonajes = BD.listaPersonajes;
            return View();
        }

        [HttpPost]

        public ActionResult ListarPersonajes(string Categoria)
        {
            BD.listaPersonajes = BD.ListarPersonajes(Categoria);
            List<Personaje> listaOrdenadaPers = BD.listaPersonajes.OrderBy(x => x.Nombre).ToList(); //Ordena alfabeticamente la lista y la guarda en una temporal
            foreach (Personaje p in listaOrdenadaPers)
            {
                string imgSrc = "data:Image/png;base64," + Convert.ToBase64String(p.Foto);
                p.RutaFoto = imgSrc;
            }
            BD.listaPersonajes = listaOrdenadaPers; //Asigno a la lista de la BD la lista ordenada alfabeticamente
            ViewBag.ListaCategorias = BD.dicCategorias;
            ViewBag.listaPersonajes = BD.listaPersonajes;
            return View();
        }

        public byte[] ImageToByteArray(Image imageIn)
        {
            using (var ms = new MemoryStream())
            {
                imageIn.Save(ms, imageIn.RawFormat);
                return ms.ToArray();
            }
        }

        public Image ByteArrayToImage(byte[] imageIn)
        {
            using (var ms = new MemoryStream(imageIn))
            {
                return Image.FromStream(ms);
            }
        }

        public ActionResult ListarPreguntas()
        {
            BD.ObtenerCategoriaPreguntas();
            List<Pregunta> listaPreguntas = BD.ObtenerPreguntas(null);
            ViewBag.listaPreguntas = listaPreguntas;
            return View();
        }

        //El ActionResult por GET muestra todas las preguntas
        //El ActionResult por POST muestra las preguntas de una categoria (la categoria se elije en la view ListarPreguntas)

        [HttpPost]

        public ActionResult ListarPreguntas(string Categoria)
        {
            BD.ObtenerCategoriaPreguntas();
            List<Pregunta> listaPreguntas = BD.ObtenerPreguntas(Categoria);
            ViewBag.listaPreguntas = listaPreguntas;
            return View();
        }

        public ActionResult BorrarPregunta(int id)
        {
            string respuesta = BD.BorrarPregunta(id);
            if (respuesta != "1")
            {
                Session["Mensaje"] = respuesta;
            }
            else
            {
                Session["Mensaje"] = "La pregunta se elimino correctamente";
            }

            return RedirectToAction("ListarPreguntas", "BackOffice");
        }

        public ActionResult BorrarPersonaje(int id)
        {
            string respuesta = BD.BorrarPersonaje(id);
            if (respuesta != "1")
            {
                Session["Mensaje"] = respuesta;
            }
            else
            {
                Session["Mensaje"] = "El personaje se elimino correctamente";
            }
            return RedirectToAction("ListarPersonajes", "BackOffice");
        }

        public ActionResult CaracteristicasPersonaje(int id)
        {
            int iCantPersonajes = BD.listaPersonajes.Count();
            List<int> listaPregs = BD.ObtenerPreguntasPersonaje(id);
            int i = 0;
            string nombre;
            bool encontrado = false;
            List<Pregunta> listaPreguntas = BD.ObtenerPreguntas(null);
            ViewBag.listaPreguntas = listaPreguntas;
            Session["IDPersonaje"] = id;
            while (encontrado == false && i < iCantPersonajes)
            {
                if (BD.listaPersonajes[i].IDPersonaje == id)
                {
                    encontrado = true;
                    nombre = BD.listaPersonajes[i].Nombre;
                    ViewBag.Personaje = nombre;
                }
                else
                {
                    i++;
                }
            }
            ViewBag.listaPregsPers = listaPregs; 
            return View();
        }

        [HttpPost]

        public ActionResult CaracteristicasPersonaje(int id, int[] vecPregs)
        {
            int ID = (int)Session["IDPersonaje"];
            if (BD.AsociarPregPers(id, vecPregs))
            {
                return RedirectToAction("ListarPersonajes", "BackOffice");
            }
            return RedirectToAction("CaracteristicasPersonaje", "BackOffice");
        }

        public ActionResult LogOut()
        {
            Session["nombre"] = null;
            return RedirectToAction("Index", "Home");
        }

        public ActionResult AgregarCategoriaPersonaje()
        {
            return View();
        }

        [HttpPost]

        public ActionResult AgregarCategoriaPersonaje(string nombre)
        {
            string sRespuesta = BD.AgregarCategoriaPersonaje(nombre);
            if (sRespuesta == "1")
            {
                Session["Mensaje"] = "La categoría de personaje se agrego correctamente";
            }
            else
            {
                Session["Error"] = sRespuesta;
            }
            return RedirectToAction("ListarCategoriasPersonajes", "BackOffice");
        }

        public ActionResult AgregarCategoriaPregunta()
        {
            return View();
        }

        [HttpPost]

        public ActionResult AgregarCategoriaPregunta(string nombre)
        {
            string sRespuesta = BD.AgregarCategoriaPregunta(nombre);
            if (sRespuesta == "1")
            {
                Session["Mensaje"] = "La categoría de pregunta se agrego correctamente";
            }
            else
            {
                Session["Error"] = sRespuesta;
            }
            return RedirectToAction("ListarCategoriasPreguntas", "BackOffice");
        }

        public ActionResult AgregarPregunta()
        {
            return View();
        }

        [HttpPost]

        public ActionResult AgregarPregunta(Pregunta p)
        {
            if (p.Valor > 0)
            {
                BD.AgregarPregunta(p);
                return RedirectToAction("ListarPreguntas", "BackOffice");
            }
            ViewBag.Error = "No se puede ingresar un valor negativo";
            return View();
        }

        public ActionResult ListarCategoriasPreguntas()
        {
            BD.ObtenerCategoriaPreguntas();
            return View();
        }

        public ActionResult ListarCategoriasPersonajes()
        {
            BD.ObtenerCategoriasPersonajes();
            return View();
        }
        
        public ActionResult BorrarCategoriaPreguntas(string sCatPregunta)
        {
            string sRespuesta = BD.BorrarCategoriaPregunta(sCatPregunta);
            if (sRespuesta != "1")
            {
                Session["Mensaje"] = sRespuesta;
            }
            else
            {
                Session["Mensaje"] = "La categoria de preguntas se elimino correctamente";
            }
            return RedirectToAction("ListarCategoriasPreguntas", "BackOffice");
        }

        public ActionResult BorrarCategoriaPersonajes(string sCatPersonaje)
        {
            string sRespuesta = BD.BorrarCategoriaPersonajes(sCatPersonaje);
            if (sRespuesta != "1")
            {
                Session["Mensaje"] = sRespuesta;
            }
            else
            {
                Session["Mensaje"] = "La categoria de personajes se elimino correctamente";
            }
            return RedirectToAction("ListarCategoriasPersonajes", "BackOffice");
        }
    }
}