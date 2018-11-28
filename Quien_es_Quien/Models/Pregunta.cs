using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Quien_es_Quien
{
    public class Pregunta
    {
        private int _IDPregunta;
        [Required]
        private string _TextoPregunta;
        [Required]
        private int _Valor;
        [Required]
        private int _IDCategoria;

        public int IDPregunta
        {
            get
            {
                return _IDPregunta;
            }

            set
            {
                _IDPregunta = value;
            }
        }

        public string TextoPregunta
        {
            get
            {
                return _TextoPregunta;
            }

            set
            {
                _TextoPregunta = value;
            }
        }

        public int Valor
        {
            get
            {
                return _Valor;
            }

            set
            {
                _Valor = value;
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

        public Pregunta()
        {

        }

        public Pregunta(string TextoPregunta, int Valor)
        {
            _IDPregunta = -1; //En el caso de que el ID del personaje sea desconocido se inicializará en -1
            _TextoPregunta = TextoPregunta;
            _Valor = Valor;
        }

        public Pregunta(int IDPregunta, string TextoPregunta, int Valor)
        {
            _IDPregunta = IDPregunta;
            _TextoPregunta = TextoPregunta;
            _Valor = Valor;
        }

        public Pregunta(int IDPregunta, string TextoPregunta, int Valor, int IDCategoria)
        {
            _IDPregunta = IDPregunta;
            _TextoPregunta = TextoPregunta;
            _Valor = Valor;
            _IDCategoria = IDCategoria;
        }

        /*public int IDPregunta { get => _IDPregunta; set => _IDPregunta = value; }
        public string TextoPregunta { get => _TextoPregunta; set => _TextoPregunta = value; }
        public int Valor { get => _Valor; set => _Valor = value; }
        public int IDCategoria { get => _IDCategoria; set => _IDCategoria = value; }*/
    }
}