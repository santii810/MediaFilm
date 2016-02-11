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
    /// Lógica de interacción para AddPatron.xaml
    /// </summary>
    public partial class AddPatron : Window
    {
        SeriesXML gestorSeries = new SeriesXML(MainWindow.config);
        List<Serie> series = new List<Serie>();

        public AddPatron()
        {
            InitializeComponent();
            series = gestorSeries.leerSeries();
            foreach (Serie item in series)
            {
            listBoxSeries.Items.Add(item.titulo);
            }

        }

        private void listBoxSeries_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
