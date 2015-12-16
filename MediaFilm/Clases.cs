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
        public string dirTrabajo { get; set; }

      
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
}
