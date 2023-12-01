using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaktionslogik für AutomationSelection.xaml
    /// </summary>
    public partial class AutomationSelection : Window
    {
        List<String> automationTypes;
        List<String> actionTypes;

        List<ISensor> sensors;
        public AutomationSelection()
        {
            InitializeComponent();
            sensors = new List<ISensor>(((App)Application.Current).sensorList);
            DataContext = this;
            FillComboBoxes();
        }

        /// <summary>
        /// Liste der verfügbaren Sensoren, die einer Automation hinzugefügt werden können.
        /// </summary>
        public ObservableCollection<AutomationSensorListEntry> sensorsComboBoxBinding { get; set; }


        public void FillComboBoxes()
        {
            
            sensorsComboBoxBinding = new ObservableCollection<AutomationSensorListEntry>();
            foreach(ISensor sensor in sensors)
            {
                AutomationSensorListEntry sensorListEntry = new AutomationSensorListEntry(sensor.GetSensorName(), sensor.GetType().ToString().Split('.').Last());
                sensorsComboBoxBinding.Add(sensorListEntry);
            }
            
            automationTypes = Enum.GetNames(typeof(Automations)).ToList();
            AutomationType.ItemsSource = automationTypes;
            
            actionTypes = Enum.GetNames(typeof(Actions)).ToList();
            ActionType.ItemsSource = actionTypes;
        }

        /// <summary>
        /// Funktion, wenn Benutzer auf Hinzufügen drückt.
        /// Alle relevanten Informationen für eine neue Automation werden gesammelt und an die App-Logik weitergegeben.
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="args">Event Argumente</param>
        public void OnButtonPressed(object sender, RoutedEventArgs args)
        {
            if(SensorList.SelectedItems.Count > 0)
            {
                System.Collections.IList selection = (System.Collections.IList)SensorList.SelectedItems;
                List<AutomationSensorListEntry> selectedEntries = selection.Cast<AutomationSensorListEntry>().ToList();
                List<ISensor> sensorsToBeObserved = new List<ISensor>();
                foreach(AutomationSensorListEntry entry in selectedEntries)
                {
                    sensorsToBeObserved.Add(sensors.FirstOrDefault(i => i.GetSensorName() == entry.Name));
                }
                
                Enum.TryParse(AutomationType.SelectedItem.ToString(), out Automations selectedAutomation);
                Enum.TryParse(ActionType.SelectedItem.ToString(), out Actions selectedAction);
                
                double threshold = Double.Parse(Threshold.Text);

                ((App)Application.Current).AddAutomation(selectedAutomation, threshold, selectedAction, sensorsToBeObserved);
            }
        }
    }
}
