using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Text;
using System.Net;
using System.IO;
using System.Drawing;
using System.Runtime.Serialization.Formatters.Binary;
using System.Timers;

namespace Quien_es_Quien
{
    static public class BD
    {
        //public static string connectionString = "Server=10.128.8.16;User=QEQA05;Password=QEQA05;Database=QEQA05";
        public static string connectionString = "Server=.;Database=QEQA05;Trusted_connection=True";
        public static Dictionary<int, string> dicCategorias = new Dictionary<int, string>();
        public static Dictionary<int, string> dicCategoriasPreguntas = new Dictionary<int, string>();
        public static List<Personaje> listaPersonajes = new List<Personaje>();
        public static string TipoPartida;
        public static string CategoriaJuego;
        
        public static SqlConnection Conectar()
        {
            SqlConnection conexion = new SqlConnection(connectionString);
            conexion.Open();
            return conexion;
        }

        public static void Desconectar(SqlConnection conexion)
        {
            conexion.Close();
        }

        public static void ObtenerCategoriasPersonajes()
        {
            dicCategorias.Clear();
            SqlConnection conexion = Conectar();
            SqlCommand query = conexion.CreateCommand();
            query.CommandType = System.Data.CommandType.StoredProcedure;
            query.CommandText = "sp_ListarCategoriasPersonaje";
            SqlDataReader lector = query.ExecuteReader();
            while (lector.Read())
            {
                int ID = Convert.ToInt32(lector["IDCategoria"]);
                string Nombre = lector["Nombre"].ToString();
                dicCategorias.Add(ID, Nombre);
            }
            Desconectar(conexion);
        }

        public static string Login(string Nombre, string Contra)
        {
            string sRespuesta = null;
            SqlConnection conexion = Conectar();
            SqlCommand query = conexion.CreateCommand();
            query.CommandType = System.Data.CommandType.StoredProcedure;
            query.CommandText = "sp_Login";
            query.Parameters.AddWithValue("@Nombre", Nombre);
            query.Parameters.AddWithValue("@Contrasenia", Contra);
            query.Parameters.AddWithValue("@IP", Dns.GetHostEntry(string.Empty).ToString());
            SqlDataReader lector = query.ExecuteReader();
            if (lector.Read())
            {
                try
                {
                    sRespuesta = lector["Error"].ToString();
                }
                catch (IndexOutOfRangeException e)
                {
                    sRespuesta = lector["AdminLvl"].ToString();
                }
            }
            Desconectar(conexion);
            return sRespuesta;
        }

        public static string Registrar(string Nombre, string Contra, int Dni, int Pin)
        {
            string sRespuesta = null;
            SqlConnection conexion = Conectar();
            SqlCommand query = conexion.CreateCommand();
            query.CommandType = System.Data.CommandType.StoredProcedure;
            query.CommandText = "sp_AgregarUsuario";
            query.Parameters.AddWithValue("@Nombre", Nombre);
            query.Parameters.AddWithValue("@Contrasenia", Contra);
            query.Parameters.AddWithValue("@Pin", Pin);
            query.Parameters.AddWithValue("@DNI", Dni);
            SqlDataReader lector = query.ExecuteReader();
            if (lector.Read())
            {
                try
                {
                    sRespuesta = lector["Error"].ToString();
                }
                catch (IndexOutOfRangeException e)
                {
                    if (lector["Success"] != null)
                    {
                        sRespuesta = "1";
                    }
                }
            }
            conexion.Close();
            return sRespuesta;
        }

        public static List<Personaje> ListarPersonajes(string Categoria)
        {
            ObtenerCategoriasPersonajes();
            List<Personaje> listaPersonajes = new List<Personaje>();
            SqlConnection conexion = Conectar();
            SqlCommand query = conexion.CreateCommand();
            query.CommandType = System.Data.CommandType.StoredProcedure;
            query.CommandText = "sp_ListarPersonajes";
            if (Categoria != null)
            {
                query.Parameters.AddWithValue("@Categoria", Categoria);
            }
            SqlDataReader lector = query.ExecuteReader();
            while (lector.Read())
            {
                try
                {
                    lector["Error"].ToString();//En realidad no hace nada pero se fija si existe 
                }
                catch (IndexOutOfRangeException e)
                {
                    int ID = Convert.ToInt32(lector["IDPersonaje"]);
                    string Nombre = lector["Nombre"].ToString();
                    byte[] Foto = (byte[])lector["Foto"];
                    int IDCat = Convert.ToInt32(lector["Categoria"]);
                    string Cat;
                    if (Categoria == null) { 
                        Cat = dicCategorias[IDCat];
                    } else
                    {
                        Cat = Categoria;
                    }
                    listaPersonajes.Add(new Personaje(ID, Nombre, Foto, IDCat, Cat));
                }
            }
            Desconectar(conexion);
            return listaPersonajes;
        }

        public static string BorrarPersonaje(int IDPersonaje)
        {
            string sRespuesta = null;
            SqlConnection conexion = Conectar();
            SqlCommand query = conexion.CreateCommand();
            query.CommandType = System.Data.CommandType.StoredProcedure;
            query.CommandText = "sp_BorrarPersonaje";
            query.Parameters.AddWithValue("@IDPersonaje", IDPersonaje);
            SqlDataReader lector = query.ExecuteReader();
            if (lector.Read())
            {
                try
                {
                    sRespuesta = lector["Error"].ToString();
                }
                catch (IndexOutOfRangeException e)
                {
                    if (lector["Success"] != null)
                    {
                        sRespuesta = "1";
                    }
                }
            }
            Desconectar(conexion);
            return sRespuesta;
        }

        public static string AgregarPersonaje(Personaje p)
        {
            string sRespuesta = null;
            SqlConnection conexion = Conectar();
            SqlCommand query = conexion.CreateCommand();
            query.CommandType = System.Data.CommandType.StoredProcedure;
            query.CommandText = "sp_AgregarPersonaje";
            query.Parameters.AddWithValue("@Nombre", p.Nombre);
            query.Parameters.AddWithValue("@Foto", p.Foto);
            query.Parameters.AddWithValue("@Categoria", p.Categoria);
            SqlDataReader lector = query.ExecuteReader();
            if (lector.Read())
            {
                try
                {
                    sRespuesta = lector["Error"].ToString();
                }
                catch (IndexOutOfRangeException e)
                {
                    if (lector["Success"] != null)
                    {
                        sRespuesta = "1";
                    }
                }
            }
            Desconectar(conexion);
            return sRespuesta;
        }

        public static string ModificarPersonaje(Personaje p)
        {
            string sRespuesta = null;
            SqlConnection conexion = Conectar();
            SqlCommand query = conexion.CreateCommand();
            query.CommandType = System.Data.CommandType.StoredProcedure;
            query.CommandText = "sp_ModificarPersonaje";
            query.Parameters.AddWithValue("@IDPersonaje", p.IDPersonaje);
            query.Parameters.AddWithValue("@Nombre", p.Nombre);
            query.Parameters.AddWithValue("@Foto", p.Foto);
            query.Parameters.AddWithValue("@Categoria", p.Categoria);
            SqlDataReader lector = query.ExecuteReader();
            if (lector.Read())
            {
                try
                {
                    sRespuesta = lector["Error"].ToString();
                }
                catch (IndexOutOfRangeException e)
                {
                    if (lector["Success"] != null)
                    {
                        sRespuesta = "1";
                    }
                }
            }
            Desconectar(conexion);
            return sRespuesta;
        }

        public static List<Pregunta> ObtenerPreguntas(string Categoria)
        {
            List<Pregunta> listaPreguntas = new List<Pregunta>();
            SqlConnection conexion = Conectar();
            SqlCommand query = conexion.CreateCommand();
            query.CommandType = System.Data.CommandType.StoredProcedure;
            query.CommandText = "sp_ListarPreguntas";
            query.Parameters.AddWithValue("@Categoria", Categoria);
            SqlDataReader lector = query.ExecuteReader();
            while (lector.Read())
            {
                try
                {
                    string a = lector["Error"].ToString();//Idem
                }
                catch (IndexOutOfRangeException e)
                {
                    int ID = Convert.ToInt32(lector["IDPregunta"]);
                    string Texto = lector["TextoPregunta"].ToString();
                    int Valor = Convert.ToInt32(lector["ValorPregunta"]);
                    int IDCategoria = Convert.ToInt32(lector["Categoria"]);
                    listaPreguntas.Add(new Pregunta(ID, Texto, Valor, IDCategoria));
                }
            }
            Desconectar(conexion);
            return listaPreguntas;
        }

        public static List<int> ObtenerPreguntasPersonaje(int id)
        {
            List<int> listaPreguntas = new List<int>();
            SqlConnection conexion = Conectar();
            SqlCommand query = conexion.CreateCommand();
            query.CommandType = System.Data.CommandType.StoredProcedure;
            query.CommandText = "sp_ObtenerPregPersonaje";
            query.Parameters.AddWithValue("@IDPersonaje", id);
            SqlDataReader lector = query.ExecuteReader();
            while (lector.Read())
            {
                try
                {
                    string a = lector["Error"].ToString();//Idem
                }
                catch (IndexOutOfRangeException e)
                {
                    int ID = Convert.ToInt32(lector["IDPregunta"]);
                    listaPreguntas.Add(ID);
                }
            }
            Desconectar(conexion);
            return listaPreguntas;
        }

        public static string CambiarContrasenia(string Nombre, int DNI, string Contrasenia)
        {
            string sRespuesta = null;
            SqlConnection conexion = Conectar();
            SqlCommand query = conexion.CreateCommand();
            query.CommandType = System.Data.CommandType.StoredProcedure;
            query.CommandText = "sp_CambiarContrasenia";
            query.Parameters.AddWithValue("@Nombre", Nombre);
            query.Parameters.AddWithValue("@DNI", DNI);
            query.Parameters.AddWithValue("@NuevaContra", Contrasenia);
            SqlDataReader lector = query.ExecuteReader();
            if (lector.Read())
            {
                try
                {
                    sRespuesta = lector["Error"].ToString();
                }
                catch (IndexOutOfRangeException e)
                {
                    if (lector["Success"] != null)
                    {
                        sRespuesta = "1";
                    }
                }
            }
            Desconectar(conexion);
            return sRespuesta;
        }

        public static string BorrarPregunta(int IDPregunta)
        {
            string sRespuesta = null;
            SqlConnection conexion = Conectar();
            SqlCommand query = conexion.CreateCommand();
            query.CommandType = System.Data.CommandType.StoredProcedure;
            query.CommandText = "sp_BorrarPregunta";
            query.Parameters.AddWithValue("@IDPregunta", IDPregunta);
            SqlDataReader lector = query.ExecuteReader();
            if (lector.Read())
            {
                try
                {
                    sRespuesta = lector["Error"].ToString();
                }
                catch (IndexOutOfRangeException e)
                {
                    if (lector["Success"] != null)
                    {
                        sRespuesta = "1";
                    }
                }
            }
            Desconectar(conexion);
            return sRespuesta;
        }

        public static bool AsociarPregPers(int IDPers, int[] VecPregs)
        {
            bool exito = true;
            int regs;
            SqlConnection conexion;
            SqlCommand query;
            for (int i = 0; i < VecPregs.Length; i++)
            {
                conexion = Conectar();
                query = conexion.CreateCommand();
                query.CommandType = System.Data.CommandType.StoredProcedure;
                query.CommandText = "sp_AsociarRespPers";
                query.Parameters.AddWithValue("@IDPersonaje", IDPers);
                query.Parameters.AddWithValue("@IDPregunta", VecPregs[i]);
                regs = query.ExecuteNonQuery();
                if (regs == 0)
                {
                    exito = false;
                }
                Desconectar(conexion);
            }
            return exito;
        }

        public static string AgregarCategoriaPersonaje(string Categoria)
        {
            string sRespuesta = null;
            SqlConnection conexion = Conectar();
            SqlCommand query = conexion.CreateCommand();
            query.CommandType = System.Data.CommandType.StoredProcedure;
            query.CommandText = "sp_AgregarCategoriaPersonaje";
            query.Parameters.AddWithValue("@Nombre", Categoria);
            SqlDataReader lector = query.ExecuteReader();
            if (lector.Read())
            {
                try
                {
                    sRespuesta = lector["Error"].ToString();
                }
                catch (IndexOutOfRangeException e)
                {
                    if (lector["Success"] != null)
                    {
                        sRespuesta = "1";
                    }
                }
            }
            Desconectar(conexion);
            return sRespuesta;
        }

        public static string AgregarCategoriaPregunta(string Categoria)
        {
            string sRespuesta = null;
            SqlConnection conexion = Conectar();
            SqlCommand query = conexion.CreateCommand();
            query.CommandType = System.Data.CommandType.StoredProcedure;
            query.CommandText = "sp_AgregarCategoriaPreguntas";
            query.Parameters.AddWithValue("@Nombre", Categoria);
            SqlDataReader lector = query.ExecuteReader();
            if (lector.Read())
            {
                try
                {
                    sRespuesta = lector["Error"].ToString();
                }
                catch (IndexOutOfRangeException e)
                {
                    if (lector["Success"] != null)
                    {
                        sRespuesta = "1";
                    }
                }
            }
            Desconectar(conexion);
            return sRespuesta;
        }

        public static string BorrarCategoriaPregunta(string Categoria)
        {
            string sRespuesta = null;
            SqlConnection conexion = Conectar();
            SqlCommand query = conexion.CreateCommand();
            query.CommandType = System.Data.CommandType.StoredProcedure;
            query.CommandText = "sp_BorrarCategoriaPreguntas";
            query.Parameters.AddWithValue("@Nombre", Categoria);
            SqlDataReader lector = query.ExecuteReader();
            if (lector.Read())
            {
                try
                {
                    sRespuesta = lector["Error"].ToString();
                }
                catch (IndexOutOfRangeException e)
                {
                    if (lector["Success"] != null)
                    {
                        sRespuesta = "1";
                    }
                }
            }
            Desconectar(conexion);
            return sRespuesta;
        }

        public static string BorrarCategoriaPersonajes(string Categoria)
        {
            string sRespuesta = null;
            SqlConnection conexion = Conectar();
            SqlCommand query = conexion.CreateCommand();
            query.CommandType = System.Data.CommandType.StoredProcedure;
            query.CommandText = "sp_BorrarCategoriaPersonaje";
            query.Parameters.AddWithValue("@Nombre", Categoria);
            SqlDataReader lector = query.ExecuteReader();
            if (lector.Read())
            {
                try
                {
                    sRespuesta = lector["Error"].ToString();
                }
                catch (IndexOutOfRangeException e)
                {
                    if (lector["Success"] != null)
                    {
                        sRespuesta = "1";
                    }
                }
            }
            Desconectar(conexion);
            return sRespuesta;
        }

        public static void AgregarPregunta(Pregunta p)
        {
            string sRespuesta = null;
            SqlConnection conexion = Conectar();
            SqlCommand query = conexion.CreateCommand();
            query.CommandType = System.Data.CommandType.StoredProcedure;
            query.CommandText = "sp_AgregarPregunta";
            query.Parameters.AddWithValue("@TextoPregunta", p.TextoPregunta);
            query.Parameters.AddWithValue("@Valor", p.Valor);
            query.Parameters.AddWithValue("@Categoria", p.IDCategoria);
            SqlDataReader lector = query.ExecuteReader();
            if (lector.Read())
            {
                try
                {
                    sRespuesta = lector["Error"].ToString();
                }
                catch (IndexOutOfRangeException)
                {
                    if (lector["Success"] != null)
                    {
                        sRespuesta = "1";
                    }
                }
            }
            Desconectar(conexion);
        }

        public static void ObtenerCategoriaPreguntas()
        {
            dicCategoriasPreguntas.Clear();
            SqlConnection conexion = Conectar();
            SqlCommand comando = conexion.CreateCommand();
            comando.CommandType = System.Data.CommandType.StoredProcedure;
            comando.CommandText = "sp_ListarCategoriasPreguntas";
            SqlDataReader lector = comando.ExecuteReader();
            while (lector.Read())
            {
                int ID = Convert.ToInt32(lector["IDCategoria"]);
                string Nombre = lector["Nombre"].ToString();
                dicCategoriasPreguntas.Add(ID, Nombre);
            }
            Desconectar(conexion);
        }

        public static List<string> ObtenerTiposPartida()
        {
            List<string> listaTiposPartida = new List<string>();
            SqlConnection conexion = Conectar();
            SqlCommand query = conexion.CreateCommand();
            query.CommandType = System.Data.CommandType.StoredProcedure;
            query.CommandText = "sp_ListarTiposDePartida";
            SqlDataReader lector = query.ExecuteReader();
            while (lector.Read())
            {
                string Nombre = lector["Nombre"].ToString();
                listaTiposPartida.Add(Nombre);
            }
            Desconectar(conexion);
            return listaTiposPartida;
        }

        public static string AgregarPartida(string Ganador, string Perdedor, string Tipo, int PuntosGanador)
        {
            string sRespuesta = null;
            SqlConnection conexion = Conectar();
            SqlCommand query = conexion.CreateCommand();
            query.CommandType = System.Data.CommandType.StoredProcedure;
            query.CommandText = "sp_AgregarPartida";
            query.Parameters.AddWithValue("@Ganador", Ganador);
            query.Parameters.AddWithValue("@Perdedor", Perdedor);
            query.Parameters.AddWithValue("@TipoPartida", Tipo);
            query.Parameters.AddWithValue("@PuntosGanador", PuntosGanador);
            SqlDataReader lector = query.ExecuteReader();
            if (lector.Read())
            {
                try
                {
                    sRespuesta = lector["idPartida"].ToString();
                    
                }
                catch (IndexOutOfRangeException)
                {
                    try
                    {
                        sRespuesta = lector["Error"].ToString();
                    }
                    catch (IndexOutOfRangeException)
                    {
                        sRespuesta = "-1";
                    } 
                }
            }
            Desconectar(conexion);
            return sRespuesta;
        }

        public static Dictionary<string, int> RankingTop10()
        {
            Dictionary<string, int> dicTop10 = new Dictionary<string, int>();
            SqlConnection conexion = Conectar();
            SqlCommand comando = conexion.CreateCommand();
            comando.CommandType = System.Data.CommandType.StoredProcedure;
            comando.CommandText = "sp_ObtenerTop10";
            SqlDataReader lector = comando.ExecuteReader();
            while (lector.Read())
            {
                string nombreUsu = lector["Nombre"].ToString();
                int puntajeUsu = Convert.ToInt32(lector["PuntajeTotal"]);
                dicTop10.Add(nombreUsu, puntajeUsu);
            }
            return dicTop10;
        }

        public static int CantidadJugadas(int Player)
        {
            int CantJugo = -1;
            SqlConnection conexion = Conectar();
            SqlCommand comando = conexion.CreateCommand();
            comando.CommandType = System.Data.CommandType.StoredProcedure;
            comando.CommandText = "sp_CantJugo";
            comando.Parameters.AddWithValue("@IDUsuario", Player);
            SqlDataReader lector = comando.ExecuteReader();
            if (lector.Read())
            {
                try
                {
                    CantJugo = Convert.ToInt32(lector["CantJugo"]);
                }
                catch (IndexOutOfRangeException e)
                {
                    CantJugo = -1;
                }
            }
            Desconectar(conexion);
            return CantJugo;
        }

        public static int CantidadGanadas(int Player)
        {
            int CantGano = -1;
            SqlConnection conexion = Conectar();
            SqlCommand comando = conexion.CreateCommand();
            comando.CommandType = System.Data.CommandType.StoredProcedure;
            comando.CommandText = "sp_CantGano";
            comando.Parameters.AddWithValue("@IDUsuario", Player);
            SqlDataReader lector = comando.ExecuteReader();
            if (lector.Read())
            {
                try
                {
                    CantGano = Convert.ToInt32(lector["CantGano"]);
                }
                catch (IndexOutOfRangeException e)
                {
                    CantGano = -1;
                }
            }
            Desconectar(conexion);
            return CantGano;
        }

        public static int ObtenerIDUsuario(string Player)
        {
            int ID = -1;
            SqlConnection conexion = Conectar();
            SqlCommand comando = conexion.CreateCommand();
            comando.CommandType = System.Data.CommandType.StoredProcedure;
            comando.CommandText = "sp_ObtenerID";
            comando.Parameters.AddWithValue("@Usuario", Player);
            SqlDataReader lector = comando.ExecuteReader();
            if (lector.Read())
            {
                try
                {
                    ID = Convert.ToInt32(lector["IDUsuario"]);
                }
                catch (IndexOutOfRangeException e)
                {
                    ID = -1;
                }
            }
            Desconectar(conexion);
            return ID;
        }

        public static Dictionary<int, string> GetCurrentGames() //Devuelve IDPartida - Usuario
        {
            Dictionary<int, string> dicCurrentGames = new Dictionary<int, string>();
            SqlConnection conexion = Conectar();
            SqlCommand comando = conexion.CreateCommand();
            comando.CommandType = System.Data.CommandType.StoredProcedure;
            comando.CommandText = "sp_GetCurrentGames";
            SqlDataReader lector = comando.ExecuteReader();
            while (lector.Read())
            {
                int IDPartida = Convert.ToInt32(lector["IDPartida"]);
                string sUsuario = lector["Usuario"].ToString();
                dicCurrentGames.Add(IDPartida, sUsuario);
            }
            Desconectar(conexion);
            return dicCurrentGames;
        }

        public static void SeleccionarPersonajeMP(int IDPartida, int NJugador, int Personaje)
        {
            SqlConnection conexion = Conectar();
            SqlCommand query = conexion.CreateCommand();
            query.CommandType = System.Data.CommandType.StoredProcedure;
            query.CommandText = "sp_SeleccionarPersonajePartidaMP";
            query.Parameters.AddWithValue("@IDPartida", IDPartida);
            query.Parameters.AddWithValue("@Jugador", NJugador);
            query.Parameters.AddWithValue("@Personaje", Personaje);
            query.ExecuteNonQuery();
            Desconectar(conexion);
        }

        public static int VerificarConexionJugador2(int IDPartida)
        {
            int response = -1;
            SqlConnection conexion = Conectar();
            SqlCommand query = conexion.CreateCommand();
            query.CommandType = System.Data.CommandType.StoredProcedure;
            query.CommandText = "sp_VerificarConexion2";
            query.Parameters.AddWithValue("@IDPartida", IDPartida);
            SqlDataReader lector = query.ExecuteReader();
            if (lector.Read())
            {
                response = Convert.ToInt32(lector["Response"]);
            }
            Desconectar(conexion);
            return response;
        }

        public static int VerificarPersElegidos(int IDPartida)
        {
            int Result = -1;
            SqlConnection conexion = Conectar();
            SqlCommand query = conexion.CreateCommand();
            query.CommandType = System.Data.CommandType.StoredProcedure;
            query.CommandText = "sp_VerificarPersonajeElegido";
            query.Parameters.AddWithValue("@IDPartida", IDPartida);
            SqlDataReader lector = query.ExecuteReader();
            if (lector.Read())
            {
                Result = Convert.ToInt32(lector["Result"]);
            }
            Desconectar(conexion);
            return Result;
        }

        public static void ConnectToGame(int IDPartida, string Usuario)
        {
            SqlConnection conexion = Conectar();
            SqlCommand query = conexion.CreateCommand();
            query.CommandType = System.Data.CommandType.StoredProcedure;
            query.CommandText = "sp_ConnectToGame";
            query.Parameters.AddWithValue("@IDPartida", IDPartida);
            query.Parameters.AddWithValue("@Usuario", Usuario);
            query.ExecuteNonQuery();
            Desconectar(conexion);
        }

        public static int ObtenerPersonajeMP(int IDPartida, int NJugador)
        {
            int IDPers = -1;
            SqlConnection conexion = Conectar();
            SqlCommand query = conexion.CreateCommand();
            query.CommandType = System.Data.CommandType.StoredProcedure;
            query.CommandText = "sp_ObtenerPersonajeMP";
            query.Parameters.AddWithValue("@IDPartida", IDPartida);
            query.Parameters.AddWithValue("@NJug", NJugador);
            SqlDataReader lector = query.ExecuteReader();
            if (lector.Read())
            {
                IDPers = Convert.ToInt32(lector["Personaje"]);
            }
            Desconectar(conexion);
            return IDPers;
        }

        public static void CambiarTurno(int IDPartida, bool Victoria)
        {
            SqlConnection conexion = Conectar();
            SqlCommand query = conexion.CreateCommand();
            query.CommandType = System.Data.CommandType.StoredProcedure;
            query.CommandText = "sp_CambiarTurno";
            query.Parameters.AddWithValue("@IDPartida", IDPartida);
            query.Parameters.AddWithValue("@Victory", Victoria);
            query.ExecuteNonQuery();
            Desconectar(conexion);
        }

        public static int VerificarTurno(int IDPartida)
        {
            int turno = -1;
            SqlConnection conexion = Conectar();
            SqlCommand query = conexion.CreateCommand();
            query.CommandType = System.Data.CommandType.StoredProcedure;
            query.CommandText = "sp_VerificarTurno";
            query.Parameters.AddWithValue("@IDPartida", IDPartida);
            SqlDataReader lector = query.ExecuteReader();
            if (lector.Read())
            {
                turno = Convert.ToInt32(lector["Turno"]);
            }
            Desconectar(conexion);
            return turno;
        }

        public static void CargarPartidaMultiplayer(int IDPartida, int NJugador, int Puntos)
        {
            SqlConnection conexion = Conectar();
            SqlCommand query = conexion.CreateCommand();
            query.CommandType = System.Data.CommandType.StoredProcedure;
            query.CommandText = "sp_LoadPartidaMP";
            query.Parameters.AddWithValue("@IDPartida", IDPartida);
            query.Parameters.AddWithValue("@NJug", NJugador);
            if (Puntos != -1)//-1 = no hay puntos
            { 
                query.Parameters.AddWithValue("@PuntosGanador", Puntos);
            }
            query.ExecuteNonQuery();
            Desconectar(conexion);
        }
    }
}