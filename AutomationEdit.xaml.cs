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
    /// Interaktionslogik für AutomationEdit.xaml
    /// </summary>
    public partial class AutomationEdit : Window
    {
        List<String> actionTypes;

        List<ISensor> availableSensors;
        List<ISensor> oldObservedSensors;
        
        public IAutomation automation;
        public AutomationEdit(IAutomation automation)
        {
            InitializeComponent();
            this.automation = automation;
            this.oldObservedSensors = automation.GetSensors();
            availableSensors = new List<ISensor>(((App)Application.Current).sensorList);
            DataContext = this;
            FillComboBoxes();
        }

        public ObservableCollection<AutomationSensorListEntry> sensorsComboBoxBinding { get; set; }


        public void FillComboBoxes()
        {
            sensorsComboBoxBinding = new ObservableCollection<AutomationSensorListEntry>();

            /// Vorselektiere die Sensoren, die bereits zur Automation gehören
            foreach (ISensor sensor in availableSensors)
            {
                AutomationSensorListEntry sensorListEntry = new AutomationSensorListEntry(sensor.GetSensorName(), sensor.GetType().ToString().Split('.').Last());
                sensorsComboBoxBinding.Add(sensorListEntry);
                if (oldObservedSensors.Contains(sensor))
                {
                    SensorList.SelectedItem = sensorListEntry;
                    SensorList.Focus();
                    SensorList.UpdateLayout();
                }
            }

            List<string> automationType = new List<string>();
            automationType.Add(automation.GetType().ToString().Split('.').Last());
            AutomationType.ItemsSource = automationType;
            AutomationType.SelectedItem = automationType[0];

            actionTypes = Enum.GetNames(typeof(Actions)).ToList();
            ActionType.ItemsSource = actionTypes;
            Threshold.Text = automation.GetThreshold().ToString();
        }

        /// <summary>
        /// Funktion, wenn Benutzer auf Speichern drückt.
        /// Der Automationstyp ist nicht änderbar.
        /// Neue zu beobachtenden Sensoren werden der Automation hinzugefügt.
        /// Neuer Grenzwert wird, falls gegeben, übernommen.
        /// Neue Aktion wird übernommen.
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="args">Event Argumente</param>
        public void OnButtonPressed(object sender, RoutedEventArgs args)
        {
            if (SensorList.SelectedItems.Count > 0)
            {
                System.Collections.IList selection = (System.Collections.IList)SensorList.SelectedItems;
                List<AutomationSensorListEntry> selectedEntries = selection.Cast<AutomationSensorListEntry>().ToList();
                List<ISensor> sensorsToBeObserved = new List<ISensor>();
                
                foreach (AutomationSensorListEntry entry in selectedEntries)
                {
                    sensorsToBeObserved.Add(availableSensors.FirstOrDefault(i => i.GetSensorName() == entry.Name));
                }

                Enum.TryParse(AutomationType.SelectedItem.ToString(), out Automations selectedAutomation);
                Enum.TryParse(ActionType.SelectedItem.ToString(), out Actions selectedAction);

                double threshold = Double.Parse(Threshold.Text);

                ((App)Application.Current).EditAutomation(automation, threshold, selectedAction, sensorsToBeObserved);
            }
        }
    }
}
