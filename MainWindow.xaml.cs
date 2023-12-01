using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;


namespace SoSe22_SE_SmartHome
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            SensorListBinding = new ObservableCollection<SensorListEntry>();
            AutomationListBinding = new ObservableCollection<AutomationListEntry>();

            App.OnNewSensorCreated += App_OnNewSensorCreated;
            App.OnSensorDataReceived += App_OnSensorDataReceived;
            App.OnSensorEdited += App_OnSensorEdited;
            App.OnNewAutomationCreated += App_OnNewAutomationCreated;
            App.OnAutomationEdited += App_OnAutomationEdited;
        }

        /// <summary>
        /// Liste der aktuellen Sensoren.
        /// </summary>
        public ObservableCollection<SensorListEntry> SensorListBinding { get; set; }

        /// <summary>
        /// Liste der aktuellen Automationen.
        /// </summary>
        public ObservableCollection<AutomationListEntry> AutomationListBinding { get; set; }


        /// <summary>
        /// Funktion, wenn der Benutzer eine Automation hinzufügen möchte.
        /// Öffnet ein AutomationSelection-Fenster.
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="args">Event Argumente</param>
        public void OnAddAutomationButtonPressed(object sender, RoutedEventArgs args)
        {
            var window = new AutomationSelection();
            window.Owner = this;
            window.ShowDialog();
        }

        /// <summary>
        /// Funktion, wenn der Benutzer einen Sensor hinzufügen möchte.
        /// Öffnet ein SensorSelection-Fenster.
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="args">Event Argumente</param>
        public void OnAddSensorButtonPressed(object sender, RoutedEventArgs args)
        {
            var window = new SensorSelection();
            window.Owner = this;
            window.ShowDialog();
        }

        /// <summary>
        /// Funktion, wenn der Benutzer einen Sensor löschen möchte.
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="args">Event Argumente</param>
        public void OnSensorListItemDelete(object sender, RoutedEventArgs args)
        {
            var item = ((sender as Button)?.Tag as ListViewItem)?.DataContext;
            SensorListEntry entry = (item as SensorListEntry);
            entry.Sensor.DisableSensor();
            SensorListBinding.Remove(entry);
        }

        /// <summary>
        /// Funktion, wenn der Benutzer einen Sensor bearbeiten möchte.
        /// Öffnet ein SensorEdit-Fenster.
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="args">Event Argumente</param>
        public void OnSensorListItemEdit(object sender, RoutedEventArgs args)
        {
            var item = ((sender as Button)?.Tag as ListViewItem)?.DataContext;
            SensorListEntry entry = (item as SensorListEntry);
            var window = new SensorEdit(entry.Sensor);
            window.Owner = this;
            window.ShowDialog();
        }

        /// <summary>
        /// Funktion, wenn der Benutzer eine Automation löschen möchte.
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="args">Event Argumente</param>
        public void OnAutomationListItemDelete(object sender, RoutedEventArgs args)
        {
            var item = ((sender as Button)?.Tag as ListViewItem)?.DataContext;
            AutomationListEntry entry = (item as AutomationListEntry);
            entry.Automation.UnsubscribeAll();
            entry.Automation.RemoveActions();
            AutomationListBinding.Remove(entry);
            ((App)Application.Current).DeleteAutomation(entry.Automation);
        }

        /// <summary>
        /// Funktion, wenn der Benutzer eine Automation löschen möchte.
        /// Öffnet ein AutomationEdit-Fenster.
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="args">Event Argumente</param>
        public void OnAutomationListItemEdit(object sender, RoutedEventArgs args)
        {
            var item = ((sender as Button)?.Tag as ListViewItem)?.DataContext;
            AutomationListEntry entry = (item as AutomationListEntry);
            var window = new AutomationEdit(entry.Automation);
            window.Owner = this;
            window.ShowDialog();
        }

        /// <summary>
        /// Funktion, wenn Fenster über X-Schaltfläche geschlossen wird.
        /// Versucht, alle aktiven Threads und Verknüpfungen zu stoppen/aufzulösen.
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="args">Event Arguemente</param>
        public void OnWindowClosing(object sender, CancelEventArgs args)
        {
            System.Diagnostics.Debug.WriteLine(String.Format("Window: User requested shutdown"));
            
            ((App)Application.Current).ShutdownApplication();
            Application.Current.Shutdown();
        }

        /// <summary>
        /// Event, wenn App mitteilt, dass eine neue Automation erstellt wurde.
        /// Erstellt einen neuen Listeneintrag für AutomationListView.
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="automation">Die neue Automation</param>
        void App_OnNewAutomationCreated(object sender, IAutomation automation)
        {
            AutomationListEntry listEntry = new AutomationListEntry();

            listEntry.Automation = automation;
            listEntry.Threshold = automation.GetThreshold();
            listEntry.AutomationType = automation.GetType().ToString().Split('.').Last();
            
            List<ISensor> sensors = automation.GetSensors();
            List<string> sensorNames = new List<string>();
            listEntry.Sensors = sensors;
            sensors.ForEach(sensor => sensorNames.Add(sensor.GetSensorName()));
            listEntry.SensorsCommaSeparated = String.Join(", ", sensorNames);
            
            AutomationListBinding.Add(listEntry);
        }

        /// <summary>
        /// Event, wenn App mitteilt, dass eine Automation bearbeitet wurde.
        /// Passt den entsprechenden Listeneintrag mit den neuen Daten an.
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="automation">Die neue Automation</param>
        void App_OnAutomationEdited(object sender, IAutomation automation)
        {
            foreach(AutomationListEntry entry in AutomationListBinding)
            {
                if (entry.Automation == automation)
                {
                    entry.Sensors.Clear();
                    entry.Sensors = automation.GetSensors();

                    entry.Threshold = automation.GetThreshold();
                    break;
                }
            }
        }

        /// <summary>
        /// Event, wenn App mitteilt, dass ein neuer Sensor erstellt wurde.
        /// Erstellt einen neuen Listeneintrag für SensorListView.
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="sensor">Der neue Sensor</param>
        void App_OnNewSensorCreated(object sender, ISensor sensor)
        {
            SensorListEntry listEntry = new SensorListEntry();
            ISensor Sensor = ((App)Application.Current).sensorList.Last();
            listEntry.Type = Sensor.GetType().ToString().Split('.').Last();
            listEntry.Name = Sensor.GetSensorName();
            listEntry.Sensor = sensor;
            SensorListBinding.Add(listEntry);
        }

        /// <summary>
        /// Event, wenn App mitteilt, dass ein Sensor bearbeitet wurde.
        /// Sucht den entsprechendne Listeneintrag anhand des alten Sensornamens und aktualisiert die Daten.
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="names">Der alte und neue Sensorname als string-Array</param>
        void App_OnSensorEdited(object sender, string[] names)
        {
            foreach(SensorListEntry listEntry in SensorListBinding)
            {
                var item = SensorListBinding.FirstOrDefault(i => i.Name == names[0]);
                if (item != null)
                {
                    item.Name = String.Empty;
                    item.Name = names[1];
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Window: Did not find sensor to change it's name");

                }
            }
        }

        /// <summary>
        /// Event, wenn App mitteilt, dass ein Sensor neue Daten übertragen hat.
        /// Sucht den entsprechendne Listeneintrag des Sensors und aktualisert den Sensorwert.
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="data">Die neuen Sensorwerte</param>
        void App_OnSensorDataReceived(object sender, SensorData data)
        {
            System.Diagnostics.Debug.WriteLine(String.Format("Window: SensorData of {0} received: {1}", data.Name, data.Value));
            string name = data.Name;
            var item = SensorListBinding.FirstOrDefault(i => i.Name == name);
            if (item != null)
            {
                item.SensorData = String.Empty;
                item.SensorData = String.Format("{0} {1}", data.Value, data.Unit);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Window: Item empty");

            }
        }
    }
}
