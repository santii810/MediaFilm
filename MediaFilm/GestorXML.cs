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
                config.dirTrabajo = @documento.GetElementsByTagName("dirTrabajo")[0].InnerText;
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



}
