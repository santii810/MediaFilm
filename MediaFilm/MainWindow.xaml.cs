﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MediaFilm
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //acceso a datos
        private ConfigXML xmlConfig = new ConfigXML();
        private LoggerXML xmlMediaLog;
        private LoggerXML xmlErrorLog;

        //clases
        private Config config;




        public MainWindow()
        {
            InitializeComponent();
            config = xmlConfig.leerConfig();
            xmlMediaLog = new LoggerXML(config.mediaLog);
            xmlErrorLog = new LoggerXML(config.errorLog);
        }

        //funcion recursiva que devuelve todos los ficheros dentro de la carpeta y subcarpeta enviada como parametro
        public List<FileInfo> listarFicheros(FileSystemInfo[] filesInfo)
        {
            List<FileInfo> retorno = new List<FileInfo>();

            foreach (FileSystemInfo item in filesInfo)
            {
                if (item is DirectoryInfo)
                {
                    DirectoryInfo dInfo = (DirectoryInfo)item;
                    retorno.AddRange(listarFicheros(dInfo.GetFileSystemInfos()));
                }
                else if (item is FileInfo)
                {
                    retorno.Add((FileInfo)item);
                }
            }
            return retorno;
        }
        //Recorre la carpeta torrent borrando los elementos no necesarios y llevando los ficheros a la carpeta de trabajo
        public void recorrerTorrent()
        {
            int videosMovidos = 0;
            int ficherosBorrados = 0;

            DirectoryInfo dir = new DirectoryInfo(config.dirTorrent);
            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException("Directorio de descargas no encontrado");
            }
            FileSystemInfo[] filesInfo = dir.GetFileSystemInfos();
            List<FileInfo> ficherosTorrent = new List<FileInfo>();
            ficherosTorrent.AddRange(listarFicheros(filesInfo));

            foreach (FileInfo item in ficherosTorrent)
            {
                switch (item.Extension)
                {
                    //borrar
                    case ".txt":
                        borrarFichero(item);
                        ficherosBorrados++;
                        break;
                    case ".!ut":
                        borrarFichero(item);
                        ficherosBorrados++;
                        break;
                    case ".url":
                        borrarFichero(item);
                        ficherosBorrados++;
                        break;
                    //mover
                    case ".avi":
                        moverFichero(item);
                        videosMovidos++;
                        break;
                    case ".mkv":
                        moverFichero(item);
                        videosMovidos++;
                        break;
                    case ".mp4":
                        moverFichero(item);
                        videosMovidos++;
                        break;
                    default:
                        break;
                }
            }

        }
        private void borrarFichero(FileInfo fichero)
        {
            string nombreFichero = fichero.Name;
            try
            {
                fichero.Delete();
                xmlMediaLog.añadirEntrada(new Log("Borrado", "Fichero '" + nombreFichero + "' borrado correctamente")
            }
            catch (Exception e)
            {
                xmlErrorLog.añadirEntrada(new Log("Error de borrado", "Error borrando '" + nombreFichero + "' \t" + e.ToString()));
            }
        }
        private void moverFichero(FileInfo fichero)
        {
            string nombreFichero = fichero.Name;
            string pathDestino = config.dirTrabajo + @"\" + fichero.Name;
            fichero.MoveTo(pathDestino);
            xmlMediaLog.añadirEntrada(new Log("Movido", "Fichero '" + nombreFichero + "' movido a '" + fichero.FullName + "'"));
        }





        private void buttonOrdenaSeries_Click(object sender, RoutedEventArgs e)
        {
            recorrerTorrent();
        }
    }



}
