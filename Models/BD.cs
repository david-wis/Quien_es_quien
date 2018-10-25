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

namespace Quien_es_Quien
{
    static public class BD
    {
        public static string connectionString = "Server=10.128.8.16;User=QEQA05;Password=QEQA05;Database=QEQA05";
        //public static string connectionString = "Server=.;Database=QEQA05;Trusted_connection=True";
        public static Dictionary<int, string> dicCategorias = new Dictionary<int, string>();
        public static List<Personaje> listaPersonajes = new List<Personaje>();

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

        public static void ObtenerCategoriasPersonaje()
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

        public static string Registrar(string Nombre, string Contra, int Dni)
        {
            string sRespuesta = null;
            SqlConnection conexion = Conectar();
            SqlCommand query = conexion.CreateCommand();
            query.CommandType = System.Data.CommandType.StoredProcedure;
            query.CommandText = "sp_AgregarUsuario";
            query.Parameters.AddWithValue("@Nombre", Nombre);
            query.Parameters.AddWithValue("@Contrasenia", Contra);
            query.Parameters.AddWithValue("@Administrador", 0);
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
            ObtenerCategoriasPersonaje();
            List<Personaje> listaPersonajes = new List<Personaje>();
            SqlConnection conexion = Conectar();
            SqlCommand query = conexion.CreateCommand();
            query.CommandType = System.Data.CommandType.StoredProcedure;
            query.CommandText = "sp_ListarPersonajes";
            if (Categoria != null) { 
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
                    byte[] Foto = (byte[]) lector["Foto"];
                    int IDCat = Convert.ToInt32(lector["Categoria"]);
                    string Cat = dicCategorias[IDCat];
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
                    lector["Error"].ToString();//Idem
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

        public static bool AsociarPregPers(int IDPers, int [] VecPregs)
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
    }
}