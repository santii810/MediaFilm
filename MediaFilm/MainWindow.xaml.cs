using System;
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
        private SeriesXML xmlSeries;
        //clases
        private Config config;

        //listas
        List<Serie> series = new List<Serie>();
        List<String> mensajes = new List<string>();


        public MainWindow()
        {
            InitializeComponent();
            config = xmlConfig.leerConfig();
            xmlMediaLog = new LoggerXML(config.mediaLog);
            xmlErrorLog = new LoggerXML(config.errorLog);
            xmlSeries = new SeriesXML(config);
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
        public int[] recorrerTorrent()
        {
            int videosMovidos = 0;
            int ficherosBorrados = 0;
            int errorBorrando = 0;
            int errorMoviendo = 0;
            int unsuported = 0;

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
                        if (borrarFichero(item)) ficherosBorrados++;
                        else errorBorrando++;
                        break;
                    case ".!ut":
                        if (borrarFichero(item)) ficherosBorrados++;
                        else errorBorrando++;
                        break;
                    case ".url":
                        if (borrarFichero(item)) ficherosBorrados++;
                        else errorBorrando++;
                        break;
                    //mover
                    case ".avi":
                        if (moverFichero(item)) videosMovidos++;
                        else errorMoviendo++;
                        break;
                    case ".mkv":
                        if (moverFichero(item)) videosMovidos++;
                        else errorMoviendo++;
                        break;
                    case ".mp4":
                        if (moverFichero(item)) videosMovidos++;
                        else errorMoviendo++;
                        break;
                    default:
                        unsuported++;
                        throw new TipoArchivoNoSoportadoException();
                }
            }
            return new int[] { videosMovidos, ficherosBorrados, errorMoviendo, errorBorrando, unsuported };
        }
        private bool borrarFichero(FileInfo fichero)
        {
            string nombreFichero = fichero.Name;
            try
            {
                fichero.Delete();
                xmlMediaLog.añadirEntrada(new Log("Borrado", "Fichero '" + nombreFichero + "' borrado correctamente"));
                return true;
            }
            catch (Exception e)
            {
                xmlErrorLog.añadirEntrada(new Log("Error borrando", "Error borrando '" + nombreFichero + "' \t" + e.ToString()));
                return false;
            }
        }
        private bool moverFichero(FileInfo fichero)
        {
            string nombreFichero = fichero.Name;
            string pathDestino = config.dirTrabajo + @"\" + fichero.Name;
            try
            {
                fichero.MoveTo(pathDestino);
                xmlMediaLog.añadirEntrada(new Log("Movido", "Fichero '" + nombreFichero + "' movido a '" + fichero.FullName + "'"));
                return true;
            }
            catch (Exception e)
            {
                xmlErrorLog.añadirEntrada(new Log("Error moviendo", "Error moviendo '" + nombreFichero + "' \t" + e.ToString()));
                return false;
            }
        }
        public FileInfo obtenerCoincidenciaBusqueda(string pat)
        {
            DirectoryInfo iomegaInfo = new DirectoryInfo(config.dirTrabajo);
            FileSystemInfo[] fsi;
            fsi = iomegaInfo.GetFileSystemInfos(pat);
            if (fsi.Length == 1 && fsi[0] is FileInfo)
            {
                return (FileInfo)fsi[0];
            }
            if (fsi.Length > 1)
            {
                throw new TooManySerieCoincidencesException();
            }
            return null;
        }
        public void ejecutarMovimiento(FileInfo fi, string dirSerie, string titulo, int temp, int cap, string ext)
        {
            string nombreOriginal = fi.Name;
            try
            {
                fi.MoveTo(dirSerie + @"\" + titulo + " " + temp + "0" + cap + ext);
                xmlMediaLog.añadirEntrada(new Log("Renombrado", "Fichero '" + nombreOriginal + "' renombrado a '" + fi.FullName + "'"));
            }
            catch (Exception e)
            {
                xmlErrorLog.añadirEntrada(new Log("Error renombrando", "Fichero '" + nombreOriginal + "' no se ha podido renombrar a  '" + fi.FullName + "' /n" + e.ToString()));
            }
        }

        private void renombrarVideos()
        {
            series = xmlSeries.leerSeries();
            series.Sort();
            foreach (Serie itSerie in series)
            {
                if (itSerie.estado.Equals("A"))
                {
                    itSerie.obtenerPatrones();
                    foreach (Patron itPatron in itSerie.patrones)
                    {
                        for (int temp = itSerie.temporadaActual; temp <= itSerie.numeroTemporadas; temp++)
                        {
                            for (int cap = 1; cap <= itSerie.capitulosPorTemporada; cap++)
                            {
                                FileInfo fi;
                                string dirSerie = @config.dirTrabajo + @"\" + itSerie.titulo + @"\Temporada" + temp + @"\";
                                Directory.CreateDirectory(dirSerie);
                                string[] strPatrones = new string[]
                                {
                                    itPatron.textoPatron + "*" + temp.ToString() + "0" + cap.ToString() + "*" + itSerie.extension,
                                    itPatron.textoPatron + "*" + temp.ToString() + "x0" + cap.ToString() + "*" + itSerie.extension,
                                    itPatron.textoPatron + "*" + temp.ToString() + cap.ToString() + "*" + itSerie.extension,
                                    itPatron.textoPatron + "*" + temp.ToString() + "x" + cap.ToString() + "*" + itSerie.extension
                                };

                                for (int i = 0; i <= 1; i++)
                                {
                                    if (cap >= 10) i += 2;
                                    fi = obtenerCoincidenciaBusqueda(strPatrones[i]);
                                    if (fi != null)
                                    {
                                        ejecutarMovimiento(fi, dirSerie, itSerie.titulo, temp, cap, itSerie.extension);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        //listeners
        private void buttonOrdenaSeries_Click(object sender, RoutedEventArgs e)
        {
            recorrerTorrent();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            List<String> mens = new List<string>();
            List<String> tmp = new List<string>();

            int[] numerosTorrent = recorrerTorrent();
            mens.Clear();
            mens.Add(DateTime.Now.ToString());
            mens.Add("Videos movidos: " + numerosTorrent[0]);
            mens.Add("Ficheros borrados: " + numerosTorrent[1]);
            mens.Add("Error moviendo " + numerosTorrent[2] + " ficheros");
            mens.Add("Error borrando " + numerosTorrent[3] + " ficheros");
            mens.Add("Ficheros no soportados: " + numerosTorrent[4]);
            mens.Add("");

            tmp = this.mensajes;
            this.mensajes.Clear();
            this.mensajes = mens;
            this.mensajes.AddRange(tmp); 

            listBox.Items.Clear();
            this.mensajes.ForEach(item => listBox.Items.Add(item));
            


        }
    }



}
