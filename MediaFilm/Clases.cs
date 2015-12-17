using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaFilm
{
    class Config
    {
        public string dirTorrent { get; set; }
        public string mediaLog { get; set; }
        public string errorLog { get; set; }
        public string datosLog { get; set; }
        public string dirTrabajo { get; set; }     
        public string ficheroSeries { get; set; }
        public string ficheroPatrones { get; set; }
    }

    class Log
    {
        public DateTime fecha { set; get; }
        public string tipo { set; get; }
        public string mensaje { set; get; }

        public Log(string tipo, string mensaje)
        {
            fecha = DateTime.Now;
            this.tipo = tipo;
            this.mensaje = mensaje;
        }
    }

    class Serie : IComparable
    {
        public string titulo { get; set; }
        public int temporadaActual { get; set; }
        public int numeroTemporadas { get; set; }
        public int capitulosPorTemporada { get; set; }
        public string estado { get; set; }
        public List<Patron> patrones { get; set; }
        public string extension { get; set; }

        public void addPatron(Patron pat)
        {
            this.patrones.Add(pat);
        }
        public void obtenerPatrones()
        {
  //          FicheroPatronesXML xmlPat = new FicheroPatronesXML("Patrones.xml");
    //        patrones = xmlPat.leerPatrones(titulo);

        }
        public void limpiarPatrones()
        {
            patrones.Clear();
        }
        public int CompareTo(Serie obj)
        {
            return String.Compare(this.titulo, obj.titulo);
        }
        public int CompareTo(object obj)
        {
            Serie tmp = (Serie)obj;
            return String.Compare(this.titulo, tmp.titulo);
        }
    }

    class Patron
    {
        public string nombreSerie { get; set; }
        public string textoPatron { get; set; }
    }
}
