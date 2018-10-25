using System;
using System.Web.DynamicData;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Web;

namespace Quien_es_Quien
{
    public class Personaje
    {
        private int _IDPersonaje;
        private string _Nombre;
        private string _RutaFoto;//Ver si quedo en algun lado
        private byte[] _foto;//Segun los foros de asp.net asi se guarda un varbinary
        private int _IDCategoria;
        private string _Categoria;
        private Image _ImagenPosta;
        private HttpPostedFileBase _Img;

        public int IDPersonaje { get => _IDPersonaje; set => _IDPersonaje = value; }
        [Required(ErrorMessage = "Es necesario ingresar el nombre del personaje")]
        public string Nombre { get => _Nombre; set => _Nombre = value; }

        public string RutaFoto { get => _RutaFoto; set => _RutaFoto = value; }
        public byte[] Foto { get => _foto; set => _foto = value; }
        [Required(ErrorMessage = "Es necesario ingresar la categoria del personaje")]
        public int IDCategoria { get => _IDCategoria; set => _IDCategoria = value; }

        public string Categoria { get => _Categoria; set => _Categoria = value; }

        [Required(ErrorMessage = "Es necesario elegir una foto para el personaje")]
        public HttpPostedFileBase Img { get => _Img; set => _Img = value; }

        /*public int IDPersonaje
        {
            get
            {
                return _IDPersonaje;
            }

            set
            {
                _IDPersonaje = value;
            }
        }

        public string Nombre
        {
            get
            {
                return _Nombre;
            }

            set
            {
                _Nombre = value;
            }
        }

        public string RutaFoto
        {
            get
            {
                return _RutaFoto;
            }

            set
            {
                _RutaFoto = value;
            }
        }

        public byte[] Foto
        {
            get
            {
                return _foto;
            }

            set
            {
                _foto = value;
            }
        }

        public int IDCategoria
        {
            get
            {
                return _IDCategoria;
            }

            set
            {
                _IDCategoria = value;
            }
        }

        public string Categoria
        {
            get
            {
                return _Categoria;
            }

            set
            {
                _Categoria = value;
            }
        }

        public Image ImagenPosta
        {
            get
            {
                return ImagenPosta;
            }

            set
            {
                ImagenPosta = value;
            }
        }*/

        public Personaje()
        {
        }

        public Personaje(string Nombre, byte[] foto, int IDCategoria, string Categoria)
        {
            _IDPersonaje = -1; //En el caso de que el ID del personaje sea desconocido se inicializará en -1
            _Nombre = Nombre;
            _foto = foto;
            _IDCategoria = IDCategoria;
            _Categoria = Categoria;
        }

        public Personaje(int IDPersonaje, string Nombre, byte[] foto, int IDCategoria, string Categoria)
        {
            _IDPersonaje = IDPersonaje;
            _Nombre = Nombre;
            _foto = foto;
            _IDCategoria = IDCategoria;
            _Categoria = Categoria;
        }

        public Personaje(int IDPersonaje, string Nombre, byte[] foto, Image imagenPosta, int IDCategoria, string Categoria)
        {
            _IDPersonaje = IDPersonaje;
            _Nombre = Nombre;
            _foto = foto;
            _ImagenPosta = imagenPosta;
            _IDCategoria = IDCategoria;
            _Categoria = Categoria;
        }

        //Este se va a usar unicamente para agregar un personaje
        public Personaje(string Nombre, string RutaFoto, int IDCategoria)
        {
            _Nombre = Nombre;
            _RutaFoto = RutaFoto;
            _IDCategoria = IDCategoria;
        }

        public Personaje(string Nombre, HttpPostedFileBase Foto, int IDCategoria)
        {
            _Nombre = Nombre;
            _Img = Foto;
            _IDCategoria = IDCategoria;
        }
    }
}