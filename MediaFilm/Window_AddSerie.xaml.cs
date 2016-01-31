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
    }
}
