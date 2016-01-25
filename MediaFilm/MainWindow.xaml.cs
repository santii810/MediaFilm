using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;

namespace MediaFilm
{
    public partial class MainWindow : Window
    {

        #region variables globales
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
        #endregion

        #region main
        public MainWindow()
        {
            InitializeComponent();
            config = xmlConfig.leerConfig();
            xmlMediaLog = new LoggerXML(config.mediaLog);
            xmlErrorLog = new LoggerXML(config.errorLog);
            xmlSeries = new SeriesXML(config);
        }
        #endregion

        #region Metodos privados sencillos
         /// <summary>
        /// funcion recursiva que devuelve todos los ficheros dentro de la carpeta.
        /// </summary>
        /// <param name="filesInfo">The files information.</param>
        /// <returns></returns>
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
        /// <summary>
        /// Borra el fichero enviado como parametro.
        /// </summary>
        /// <param name="fichero">Fichero a borrar.</param>
        /// <returns></returns>
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
        /// <summary>
        /// Mueve el fichero enviado como parametro al directorio de trabajo seleccionado en la configuracion.
        /// </summary>
        /// <param name="fichero">The fichero.</param>
        /// <returns></returns>
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
        /// <summary>
        /// Busca en el directorio de trabajo si existe algun fichero que coincida con el patron enviado.
        /// </summary>
        /// <param name="pat">Patron a buscar</param>
        /// <returns>FileInfo del fichero si hay coincidencias</returns>
        /// <exception cref="TooManySerieCoincidencesException"></exception>
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
        /// <summary>
        /// Cuenta los ficheros de tipo video que hay en el directorio de trabajo.
        /// </summary>
        /// <returns>Numero de ficheros con extension de video en el directorio de trabajo</returns>
        /// <exception cref="DirectoryNotFoundException">Directorio de trabajo no encontrado</exception>
        private int contarFicherosARenombrar()
        {
            int retorno = 0;
            DirectoryInfo dir = new DirectoryInfo(config.dirTrabajo);
            if (!dir.Exists) throw new DirectoryNotFoundException("Directorio de trabajo no encontrado");
            FileSystemInfo[] filesInfo = dir.GetFileSystemInfos();
            foreach (FileInfo item in filesInfo)
                if (item.Extension.Equals(".mkv") || item.Extension.Equals(".avi") || item.Extension.Equals(".mp4")) retorno++;
            return retorno;
        }
        /// <summary>
        /// Borra todos los directorios vacios de la ruta proporcionada, asi como el propio directorio.
        /// </summary>
        /// <param name="dir">El directorio.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Starting directory is a null reference or an empty</exception>
        private int borrarDirectoriosVacios(string dir)
        {
            int retorno = 0;
            if (String.IsNullOrEmpty(dir))
                throw new ArgumentException(
                    "Starting directory is a null reference or an empty string", "dir");
            try
            {
                foreach (var d in Directory.EnumerateDirectories(dir))
                {
                    retorno += borrarDirectoriosVacios(d);
                }

                var entries = Directory.EnumerateFileSystemEntries(dir);

                if (!entries.Any())
                {
                    try
                    {
                        Directory.Delete(dir);
                        retorno++;
                    }
                    catch (UnauthorizedAccessException) { }
                    catch (DirectoryNotFoundException) { }
                }
            }
            catch (UnauthorizedAccessException) { }
            return retorno;
        }
        #endregion


        #region Metodos privados complejos
        /// <summary>
        /// Recorre la carpeta torrent borrando los elementos no necesarios y llevando los ficheros a la carpeta de trabajo.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="DirectoryNotFoundException">Directorio de descargas no encontrado</exception>
        /// <exception cref="TipoArchivoNoSoportadoException"></exception>
        public int[] recorrerTorrent()
        {
            int videosMovidos = 0;
            int ficherosBorrados = 0;
            int errorBorrando = 0;
            int errorMoviendo = 0;
            int unsuported = 0;
            int directoriosBorrados = 0;
            Stopwatch tiempo = Stopwatch.StartNew();

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
            directoriosBorrados = borrarDirectoriosVacios(config.dirTorrent);
            Directory.CreateDirectory(config.dirTorrent);
            return new int[] { videosMovidos, ficherosBorrados, errorMoviendo, errorBorrando, unsuported, Convert.ToInt32(tiempo.ElapsedMilliseconds), directoriosBorrados };
        }
        /// <summary>
        /// Recorre el Directorio de trabajo buscando coincidencias y renombrando dichas concidencias.
        /// </summary>
        /// <returns>Retorna el array de numeros necesario para mostrar los mensajes en consola</returns>
        private int[] renombrarVideos()
        {
            Stopwatch tiempo = Stopwatch.StartNew();
            int videosRenombrados = 0;
            int erroresRenombrado = 0;
            int numeroPatrones = 0;
            int seriesActivas = 0;
            series = xmlSeries.leerSeries();
            series.Sort();
            foreach (Serie itSerie in series)
            {
                if (itSerie.estado.Equals("A"))
                {
                    itSerie.obtenerPatrones(config);
                    //calculo de patrones: Numero de patrones de la serie en el xml * temporadas activas de la serie * numero de capitulos de cada temporada * 4 (strings que se comprueban en cada patron)
                    numeroPatrones += (itSerie.patrones.Count * ((itSerie.numeroTemporadas - itSerie.temporadaActual) + 1) * itSerie.capitulosPorTemporada) * 4;
                    seriesActivas++;
                    foreach (Patron itPatron in itSerie.patrones)
                    {
                        for (int temp = itSerie.temporadaActual; temp <= itSerie.numeroTemporadas; temp++)
                        {
                            for (int cap = 1; cap <= itSerie.capitulosPorTemporada; cap++)
                            {
                                FileInfo fi;
                                string dirSerie = @config.dirSeries + @"\" + itSerie.titulo + @"\Temporada" + temp + @"\";
                                string[] strPatrones = new string[]
                                {
                                    itPatron.textoPatron + "*" + temp.ToString() + "0" + cap.ToString() + "*" + itSerie.extension,
                                    itPatron.textoPatron + "*" + temp.ToString() + "x0" + cap.ToString() + "*" + itSerie.extension,
                                    itPatron.textoPatron + "*" + temp.ToString() + cap.ToString() + "*" + itSerie.extension,
                                    itPatron.textoPatron + "*" + temp.ToString() + "x" + cap.ToString() + "*" + itSerie.extension
                                };

                                for (int i = 0; i <= 1; i++)
                                {
                                    if (cap >= 10) fi = obtenerCoincidenciaBusqueda(strPatrones[i + 2]);
                                    else fi = obtenerCoincidenciaBusqueda(strPatrones[i]);
                                    if (fi != null)
                                    {
                                        if (ejecutarMovimiento(fi, dirSerie, itSerie.titulo, temp, cap, itSerie.extension)) videosRenombrados++;
                                        else erroresRenombrado++;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return new int[] { videosRenombrados, erroresRenombrado, numeroPatrones, seriesActivas, Convert.ToInt32(tiempo.ElapsedMilliseconds) };
        }
        /// <summary>
        /// Mueve los ficheros.
        /// </summary>
        /// <param name="fi">Fichero a mover</param>
        /// <param name="dirSerie">Directorio de destino</param>
        /// <param name="titulo">Titulo del fichero.</param>
        /// <param name="temp">The temporada.</param>
        /// <param name="cap">The capitulo.</param>
        /// <param name="ext">The extension.</param>
        /// <returns> Retorna true si el movimiento se realiza correctamente</returns>
        public bool ejecutarMovimiento(FileInfo fi, string dirSerie, string titulo, int temp, int cap, string ext)
        {
            string nombreOriginal = fi.Name;
            //Crea todos los directorios y subdirectorios en la ruta de acceso especificada, a menos que ya existan.
            Directory.CreateDirectory(dirSerie);
            try
            {
                if (cap < 10)
                {
                    fi.MoveTo(dirSerie + @"\" + titulo + " " + temp + "0" + cap + ext);
                }
                else
                {
                    fi.MoveTo(dirSerie + @"\" + titulo + " " + temp + cap + ext);
                }
                xmlMediaLog.añadirEntrada(new Log("Renombrado", "Fichero '" + nombreOriginal + "' renombrado a '" + fi.FullName + "'"));
                return true;
            }
            catch (Exception e)
            {
                xmlErrorLog.añadirEntrada(new Log("Error renombrando", "Fichero '" + nombreOriginal + "' no se ha podido renombrar a  '" + fi.FullName + "' /n" + e.ToString()));
                return false;
            }
        }

        #endregion

        #region Mensajes
        private void mostrarMensajesRespuestaTorrent(int[] numerosTorrent)
        {
            List<String> mens = new List<string>();
            List<String> tmp = new List<string>();

            mens.Add(DateTime.Now.ToString() + "\tEXTRACCION DE VIDEOS");
            if (numerosTorrent[0] != 0 || numerosTorrent[2] != 0)
                mens.Add("Videos movidos: " + numerosTorrent[0] + ", errores: " + numerosTorrent[2]);
            if (numerosTorrent[1] != 0 || numerosTorrent[3] != 0)
                mens.Add("Ficheros borrados: " + numerosTorrent[1] + " errores: " + numerosTorrent[3]);
            if (numerosTorrent[4] != 0)
                mens.Add("Ficheros no soportados: " + numerosTorrent[4]);
            if (numerosTorrent[6] != 0)
                mens.Add("Borrados " + numerosTorrent[6] + " directorios vacios");
            mens.Add("Tiempo de ejecucion: " + numerosTorrent[5] + "ms");
            mens.Add("");

            tmp.AddRange(this.mensajes);
            this.mensajes.Clear();
            this.mensajes = mens;
            this.mensajes.AddRange(tmp);
            listBox.Items.Clear();
            this.mensajes.ForEach(item => listBox.Items.Add(item));
        }
        private void mostrarMensajesRespuestaRenombrado(int[] numerosRenombrado)
        {
            List<String> mens = new List<string>();
            List<String> tmp = new List<string>();

            mens.Add(DateTime.Now.ToString() + "\tRENOMBRADO DE VIDEOS");
            if (numerosRenombrado[0] != 0 || numerosRenombrado[1] != 0)
                mens.Add("Videos renombrados: " + numerosRenombrado[0] + ", errores: " + numerosRenombrado[1]);
            if (contarFicherosARenombrar() != 0)
                mens.Add("Videos a falta de renombrar: " + contarFicherosARenombrar());
            mens.Add("Patrones ejecutados: " + numerosRenombrado[2] + " referentes a " + numerosRenombrado[3] + " series activas");
            mens.Add("Tiempo de ejecucion: " + numerosRenombrado[4] + "ms");
            mens.Add("");

            tmp.AddRange(this.mensajes);
            this.mensajes.Clear();
            this.mensajes = mens;
            this.mensajes.AddRange(tmp);
            listBox.Items.Clear();
            this.mensajes.ForEach(item => listBox.Items.Add(item));
        }
        #endregion

        #region listeners
        private void buttonOrdenaSeries_Click(object sender, RoutedEventArgs e)
        {
            MenuItemRecorrerTorrent_Click(new object(), new RoutedEventArgs());
            MenuItemRemonbrar_Click(new object(), new RoutedEventArgs());
        }
        private void MenuItemRecorrerTorrent_Click(object sender, RoutedEventArgs e)
        {
            mostrarMensajesRespuestaTorrent(recorrerTorrent());
        }
        private void MenuItemRemonbrar_Click(object sender, RoutedEventArgs e)
        {
            mostrarMensajesRespuestaRenombrado(renombrarVideos());
        }
        private void MenuItemAñadirSerie_Click(object sender, RoutedEventArgs e)
        {

        }


    }
    #endregion
}
