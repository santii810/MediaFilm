using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace MediaFilm
{
    /// <summary>
    /// Lógica de interacción para Window_AddSerie.xaml
    /// </summary>
    public partial class Window_AddSerie : Window
    {

        public int temporadas;
        public int capitulos;

        public Window_AddSerie()
        {
            InitializeComponent();
            comboBox.Items.Add("avi");
            comboBox.Items.Add("mkv");
            comboBox.Items.Add("mp4");
        }



        private void temporadasUp_Click(object sender, RoutedEventArgs e)
        {
            temporadas++;
            this.textBoxTemporadas.Text = temporadas.ToString();
        }

        private void capitulosUp_Click(object sender, RoutedEventArgs e)
        {
            capitulos++;
            this.textBoxCapitulos.Text = capitulos.ToString();

        }

        private void CapitulosDown_Click(object sender, RoutedEventArgs e)
        {
            capitulos--;
            this.textBoxCapitulos.Text = capitulos.ToString();

        }

        private void temporadasDown_Click(object sender, RoutedEventArgs e)
        {
            temporadas--;
            this.textBoxTemporadas.Text = temporadas.ToString();
        }

        private void textBoxTemporadas_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void textBoxCapitulos_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void textBox_NombreSerie_TextChanged_1(object sender, TextChangedEventArgs e)
        {

        }

        private void button_AddSerie_Click(object sender, RoutedEventArgs e)
        {
            Serie tmp = new Serie
            {
                titulo = textBox_NombreSerie.Text,
                numeroTemporadas = temporadas,
                capitulosPorTemporada = capitulos,
                temporadaActual = 1,
                estado = "A"
            };
            switch (comboBox.SelectedIndex)
            {
                case 0:
                    tmp.extension = ".avi";
                    break;
                case 1:
                    tmp.extension = ".mkv";
                    break;
                case 2:
                    tmp.extension = ".mp4";
                    break;
                default:
                    break;
            }
            SeriesXML gestorSeries = new SeriesXML(MainWindow.config);


        }
    }
}
