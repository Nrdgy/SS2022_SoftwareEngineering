using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
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

namespace SoSe22_SE_SmartHome
{
    /// <summary>
    /// Interaktionslogik für SensorSelection.xaml
    /// </summary>
    public partial class SensorSelection : Window
    {
        List<String> sensorTypes;
        public SensorSelection()
        {
            InitializeComponent();
            DataContext = this;
            ComboBoxBinding = new ObservableCollection<string>();
            FillComboBox();
        }
        public ObservableCollection<string> ComboBoxBinding { get; set; }

        public void FillComboBox()
        {
            sensorTypes = Enum.GetNames(typeof(Sensors)).ToList();
            SensorTypes.ItemsSource = sensorTypes;
            Console.WriteLine(sensorTypes.Count());
        }

        /// <summary>
        /// Funktion, wenn Benutzer einen neuen Sensor hinzufügt.
        /// Übermittelt die Daten an die App.
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="args">Event Parameter</param>
        public void OnButtonPressed(object sender, RoutedEventArgs args)
        {
            String sensorName = SensorName.Text;
            if(SensorTypes.SelectedItem != null)
            {
                Enum.TryParse(SensorTypes.SelectedItem.ToString(), out Sensors selectedSensor);
                ((App)Application.Current).AddSensor(selectedSensor, sensorName);
            }
        }
    }
}
