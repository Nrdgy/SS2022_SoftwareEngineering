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

namespace SoSe22_SE_SmartHome
{
    /// <summary>
    /// Interaktionslogik für SensorEdit.xaml
    /// </summary>
    public partial class SensorEdit : Window
    {
        public ISensor sensor;
        public SensorEdit(ISensor sensor)
        {
            InitializeComponent();
            this.sensor = sensor;
        }
        public void FillComboBoxes()
        {
            SensorNameBox.Text = sensor.GetSensorName();
        }

        /// <summary>
        /// Funktion, wenn Benutzer seine Änderungen speichert.
        /// Übermittelt die Daten an die App.
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="args">Event Argumente</param>
        public void OnSaveButtonPressed(object sender, RoutedEventArgs args)
        {
            string newName = SensorNameBox.Text;
            ((App)Application.Current).EditSensor(sensor, newName);
        }
    }
}
