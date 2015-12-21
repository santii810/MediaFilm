using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MediaFilm
{
    class ConfigXML
    {
        string nombreFichero = @"config.xml";
        XmlDocument documento;
        XmlNode raiz;

        private bool cargarXML()
        {
            if (File.Exists(nombreFichero))
            {
                documento = new XmlDocument();
                documento.Load(nombreFichero);
                raiz = documento.DocumentElement;
                return true;
            }
            else return false;
        }
        public Config leerConfig()
        {
            Config config = new Config();
            if (cargarXML())
            {
                config.dirTorrent = @documento.GetElementsByTagName("dirTorrent")[0].InnerText;
                config.errorLog = @documento.GetElementsByTagName("errorLog")[0].InnerText;
                config.mediaLog = @documento.GetElementsByTagName("mediaLog")[0].InnerText;
                config.datosLog = @documento.GetElementsByTagName("datosLog")[0].InnerText;
                config.dirTrabajo = @documento.GetElementsByTagName("dirTrabajo")[0].InnerText;
                config.ficheroSeries = @documento.GetElementsByTagName("fichSeries")[0].InnerText;
                config.ficheroPatrones = @documento.GetElementsByTagName("fichPatrones")[0].InnerText;
                config.dirSeries = @documento.GetElementsByTagName("dirSeries")[0].InnerText;
            }
            return config;
        }
    }
    class LoggerXML
    {
        string nombreFichero;
        XmlDocument documento;
        XmlNode raiz;
        public LoggerXML(string rutaFichero)
        {
            this.nombreFichero = rutaFichero;
        }
        public bool cargarXML()
        {
            if (File.Exists(nombreFichero))
            {
                documento = new XmlDocument();
                documento.Load(nombreFichero);
                raiz = documento.DocumentElement;
                return true;
            }
            else return false;
        }

        public void añadirEntrada(Log log)
        {
            documento = new XmlDocument();
            if (!File.Exists(nombreFichero))
            {
                XmlDeclaration declaracion = documento.CreateXmlDeclaration("1.0", "ISO-8859-1", null);
                documento.AppendChild(declaracion);
                raiz = documento.CreateElement("Logs");
                documento.AppendChild(raiz);
            }
            else
            {
                documento.Load(nombreFichero);
                raiz = documento.DocumentElement;
            }

            raiz.AppendChild(crearNodo(log));

            documento.Save(nombreFichero);
        }
        public XmlNode crearNodo(Log log)
        {
            XmlElement nodo = documento.CreateElement("Log");
            nodo.SetAttribute("fecha", log.fecha.ToString());
            nodo.SetAttribute("tipo", log.tipo);
            nodo.SetAttribute("mensaje", log.mensaje);
            return nodo;
        }
    }
    class SeriesXML
    {
        string nombreFichero;
        XmlDocument documento;
        XmlNode raiz;
        LoggerXML xmlDatos;
        LoggerXML xmlError;
        PatronesXML xmlPatrones;


        public SeriesXML(Config config)
        {
            this.nombreFichero = config.ficheroSeries;
            xmlDatos = new LoggerXML(config.datosLog);
            xmlError = new LoggerXML(config.errorLog);
            xmlPatrones = new PatronesXML(config);
        }
        public bool cargarXML()
        {
            if (File.Exists(nombreFichero))
            {
                documento = new XmlDocument();
                documento.Load(nombreFichero);
                raiz = documento.DocumentElement;
                return true;
            }
            else return false;
        }
        public List<Serie> leerSeries()
        {
            List<Serie> series = new List<Serie>();
            if (cargarXML())
            {
                foreach (XmlNode item in documento.GetElementsByTagName("serie"))
                {
                    series.Add(new Serie
                    {
                        titulo = item["titulo"].InnerText.ToString(),
                        temporadaActual = Convert.ToInt32(item["temporadaActual"].InnerText.ToString()),
                        numeroTemporadas = Convert.ToInt32(item["numeroTemporadas"].InnerText.ToString()),
                        capitulosPorTemporada = Convert.ToInt32(item["capitulosPorTemporada"].InnerText.ToString()),
                        estado = item["estado"].InnerText,
                        extension = item["extension"].InnerText
                    });
                }
            }
            return series;
        }
        public void añadirSerie(Serie serie)
        {
            documento = new XmlDocument();

            if (!File.Exists(nombreFichero))
            {
                XmlDeclaration declaracion = documento.CreateXmlDeclaration("1.0", "ISO-8859-1", null);
                documento.AppendChild(declaracion);
                raiz = documento.CreateElement("Series");
                documento.AppendChild(raiz);
            }
            else
            {
                documento.Load(nombreFichero);
                raiz = documento.DocumentElement;
            }
            if (!existe(serie.titulo))
            {
                raiz.AppendChild(crearNodo(serie));
                documento.Save(nombreFichero);

                xmlDatos.añadirEntrada(new Log("AñadirSerie", "Serie '" + serie.titulo + "' añadida correctamente"));
                //Añado 2 patrones por defecto a todas las series nada mas ser añadidas
                xmlPatrones.añadirPatron(new Patron { nombreSerie = serie.titulo, textoPatron = serie.titulo });
                xmlPatrones.añadirPatron(new Patron { nombreSerie = serie.titulo, textoPatron = serie.titulo.Replace(' ', '.') });
            }
            else
            {
                xmlError.añadirEntrada(new Log("Error añadiendo datos", "Serie '" + serie.titulo + "' ya existe"));
            }
        }
        public XmlNode crearNodo(Serie serie)
        {
            XmlElement nodoSerie = documento.CreateElement("serie");
            nodoSerie.SetAttribute("titulo", serie.titulo);

            XmlElement titulo = documento.CreateElement("titulo");
            titulo.InnerText = serie.titulo;
            nodoSerie.AppendChild(titulo);

            XmlElement temporadaActual = documento.CreateElement("temporadaActual");
            temporadaActual.InnerText = serie.temporadaActual.ToString();
            nodoSerie.AppendChild(temporadaActual);

            XmlElement numeroTemporadas = documento.CreateElement("numeroTemporadas");
            numeroTemporadas.InnerText = serie.numeroTemporadas.ToString();
            nodoSerie.AppendChild(numeroTemporadas);

            XmlElement capitulosPorTemporada = documento.CreateElement("capitulosPorTemporada");
            capitulosPorTemporada.InnerText = serie.capitulosPorTemporada.ToString();
            nodoSerie.AppendChild(capitulosPorTemporada);


            XmlElement estado = documento.CreateElement("estado");
            estado.InnerText = serie.estado;
            nodoSerie.AppendChild(estado);

            XmlElement extension = documento.CreateElement("extension");
            extension.InnerText = serie.extension;
            nodoSerie.AppendChild(extension);

            return nodoSerie;
        }
        public bool existe(string nombreSerie)
        {
            foreach (XmlNode item in documento.GetElementsByTagName("serie"))
                if (item.Attributes["titulo"].Value.Equals(nombreSerie))
                    return true;
            return false;
        }
    }
    class PatronesXML
    {
        string nombreFichero;
        XmlDocument documento;
        XmlNode raiz;
        LoggerXML xmlDatos;
        LoggerXML xmlError;


        public PatronesXML(Config config)
        {
            this.nombreFichero = config.ficheroPatrones;
            xmlDatos = new LoggerXML(config.datosLog);
            xmlError = new LoggerXML(config.errorLog);
        }
        public bool cargarXML()
        {
            if (File.Exists(nombreFichero))
            {
                documento = new XmlDocument();
                documento.Load(nombreFichero);
                raiz = documento.DocumentElement;
                return true;
            }
            else return false;
        }
        public void añadirPatron(Patron patron)
        {
            documento = new XmlDocument();
            if (!File.Exists(this.nombreFichero))
            {
                XmlDeclaration declaracion = documento.CreateXmlDeclaration("1.0", "ISO-8859-1", null);
                documento.AppendChild(declaracion);
                raiz = documento.CreateElement("raiz");
                documento.AppendChild(raiz);
            }
            else
            {
                documento.Load(this.nombreFichero);
                raiz = documento.DocumentElement;
            }
            if (!existe(patron.textoPatron))
            {
                raiz.AppendChild(crearNodo(patron));
                documento.Save(this.nombreFichero);
                xmlDatos.añadirEntrada(new Log("AñadirPatron", "patron '" + patron.nombreSerie + "-" + patron.textoPatron + "' añadido correctamente a serie "));
            }
            else
            {
                xmlError.añadirEntrada(new Log ("Error","patron '" + patron.nombreSerie + "-" + patron.textoPatron + "' Ya existe " ));
            }
        }
        public XmlNode crearNodo(Patron patron)
        {
            XmlElement serie = documento.CreateElement("serie");
            serie.SetAttribute("titulo", patron.nombreSerie);
            serie.InnerText = patron.textoPatron;

            return serie;
        }
        public List<Patron> leerPatrones(string serie)
        {
            List<Patron> patrones = new List<Patron>();
            if (cargarXML())
            {
                foreach (XmlNode item in documento.GetElementsByTagName("serie"))
                {
                    if (item.Attributes["titulo"].Value.Equals(serie))
                    {
                        patrones.Add(new Patron
                        {
                            nombreSerie = serie,
                            textoPatron = item.InnerText.ToString()
                        });
                    }
                }
            }
            return patrones;
        }
        public bool existe(string textoPatron)
        {
            foreach (XmlNode item in documento.GetElementsByTagName("serie"))
            {
                if (item.InnerText.Equals(textoPatron))
                {
                    return true;
                }
            }
            return false;
        }
    }


}
