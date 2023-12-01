using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace SoSe22_SE_SmartHome
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application, IObserver<SensorData>
    {
        public List<ISensor> sensorList = new List<ISensor>();
        public List<IAutomation> automationList = new List<IAutomation>();
        private Dictionary<string, IDisposable> cancellations = new Dictionary<string, IDisposable>();
       
        /// <summary>
        /// Funktion zum Erstellen eines neuen Sensors eines Typ aus dem Enum Sensors.
        /// Wenn der Name bereits in Verwendung ist, wird kein neuer Sensor erstellt.
        /// Der Sensor wird eingeschaltet und die App abonniert den Sensor als Observer.
        /// </summary>
        /// <param name="sensorType">Sensortyp gemäß Enum Sensors</param>
        /// <param name="name">Sensorname, kann null sein, muss aber einmalig vorkommen.</param>
        public void AddSensor(Sensors sensorType, String name = null)
        {
            if (String.IsNullOrEmpty(name))
            {
                name = "Sensor " + (sensorList.Count + 1);
            }
            if(!sensorList.Any(sensor => sensor.GetSensorName().Equals(name)))
            {
                ISensor sensor = NewSensor(sensorType, sensorList.Count + 1, name);
                sensorList.Add(sensor);
                sensor.EnableSensor();
                Subscribe(sensor);
                OnNewSensorCreated(this, sensor);
            }
        }

        /// <summary>
        /// Funktion zum Bearbeiten eines existenten Sensors.
        /// Sowohl der Name des Sensors wird aktulisiert, als auch das entsprechende IDisposable zum Deabonnieren.
        /// </summary>
        /// <param name="sensor">Der zu aktualisierende Sensor</param>
        /// <param name="newName">Der neue Name des Sensors</param>
        public void EditSensor(ISensor sensor, string newName)
        {
            System.Diagnostics.Debug.WriteLine("App: Change sensor name from " + sensor.GetSensorName() + " to " + newName);
            string oldName = sensor.GetSensorName();
            string[] names = new string[] { oldName, newName };

            foreach (ISensor listEntry in sensorList)
            {
                if (listEntry.GetSensorName().Equals(oldName))
                {
                    IDisposable cancellation = cancellations[oldName];
                    cancellations.Remove(oldName);
                    cancellations.Add(newName, cancellation);
                    listEntry.SetSensorName(newName);

                    OnSensorEdited(this, names);
                    break;
                }
            }
        }

        /// <summary>
        /// Funktion zum Erstellen einer neuen Automation eines Typ aus dem Enum Automations.
        /// Diese überwacht n Sensoren und führt bei Auslösung eines Grenzwertes eine Aktion aus.
        /// </summary>
        /// <param name="automationType">Der Automationstyp gemäß dem Enum Automations</param>
        /// <param name="threshold">Der Grenzwert</param>
        /// <param name="action">Die auszuführende Aktion, sobald der Grenzwert ausgelöst wurde</param>
        /// <param name="sensors">Eine Liste von zu überwachenden Sensoren</param>
        public void AddAutomation(Automations automationType, double threshold, Actions action, List<ISensor> sensors)
        {
            IAutomation automation = NewAutomation(automationType, threshold, action);
            automation.SetSubscriptions(sensors);
            automationList.Add(automation);
            OnNewAutomationCreated(this, automation);
        }

        /// <summary>
        /// Funktion zum Bearbeiten einer existenten Automation.
        /// Die Automation erhält einen neuen Grenzwert, eine neue Aktion, sowie eine neue Liste an Sensoren.
        /// Die Automation deabonniert Sensoren, die nicht mehr überwacht werden sollen und abonniert diejenigen, die neu dazugekommen sind.
        /// </summary>
        /// <param name="automation">Die zu bearbeitende Automation</param>
        /// <param name="threshold">Der neue Grenzwert.</param>
        /// <param name="action">Die neue Aktion</param>
        /// <param name="sensors">Eine Liste an Sensoren</param>
        public void EditAutomation(IAutomation automation, double threshold, Actions action, List<ISensor> sensors)
        {
            foreach(IAutomation listEntry in automationList)
            {
                if (listEntry.Equals(automation))
                {
                    listEntry.SetThreshold(threshold);
                    listEntry.RemoveActions();
                    listEntry.AddAction(NewAction(action));
                    
                    List<ISensor> oldSensorList = listEntry.GetSensors();
                    List<ISensor> obsoleteSensors = oldSensorList.Except(sensors).ToList();
                    foreach(ISensor obsoleteSensor in obsoleteSensors)
                    {
                        listEntry.Unsubscribe(obsoleteSensor);
                    }

                    listEntry.SetSubscriptions(sensors);

                    OnAutomationEdited(this, automation);
                    break;
                }
            }
        }

        /// <summary>
        /// Funktion zum Löschen einer existenten Automation.
        /// </summary>
        /// <param name="automation">Die zu löschende Automation</param>
        public void DeleteAutomation(IAutomation automation)
        {
            if (automationList.Contains(automation))
            {
                automationList.Remove(automation);
            }
        }

        /// <summary>
        /// EventListener zur Übergabe der Handlung an die jeweiligen Interaktionslogiken der Fenster
        /// </summary>
        public static event EventHandler<ISensor> OnNewSensorCreated = delegate { };
        public static event EventHandler<SensorData> OnSensorDataReceived = delegate { };
        public static event EventHandler<string[]> OnSensorEdited = delegate { };

        public static event EventHandler<IAutomation> OnNewAutomationCreated = delegate { };
        public static event EventHandler<IAutomation> OnAutomationEdited = delegate { };

        /// <summary>
        /// Funktion zum Erstellen des konkreten Sensorobjekts anhand des Enumtypen.
        /// </summary>
        /// <param name="sensorType">Der Enumtyp</param>
        /// <param name="id">Die ID des neuen Sensors</param>
        /// <param name="name">Der Name des neuen Sensors</param>
        /// <returns>Eine konkretes Objekt von ISensor</returns>
        public ISensor NewSensor(Sensors sensorType, int id, String name = null)
        {
            switch (sensorType)
            {
                case Sensors.LightSensor:
                    return new LightSensor(id, name);
                case Sensors.CO2Sensor:
                    return new CO2Sensor(id, name);
                case Sensors.TemperatureSensor:
                    return new TemperatureSensor(id, name);
                default: return new TemperatureSensor(id, name);
            }
        }

        /// <summary>
        /// Funktion zum Erstellen des konkreten Aktionobjekts anhand des Enumtypen.
        /// </summary>
        /// <param name="actionType">Der Enumtyp</param>
        /// <returns>Eine konkretes Objekt von IAction</returns>
        public IAction NewAction(Actions actionType)
        {
            switch (actionType)
            {
                case Actions.NotifyAction:
                    return new NotifyAction();
                default: return new NotifyAction();
            }
                
        }

        /// <summary>
        /// Funktion zum Erstellen des konkreten Automationsobjekts anhand des Enumtypen.
        /// </summary>
        /// <param name="automationType">Der Enumtyp</param>
        /// <param name="threshold">Der Grenzwert</param>
        /// <param name="action">Die Aktion</param>
        /// <returns>Eine konkretes Objekt von IAutomation</returns>
        public IAutomation NewAutomation(Automations automationType, double threshold, Actions action)
        {
            List<IAction> actions = new List<IAction>();
            actions.Add(NewAction(action));

            switch (automationType)
            {
                case Automations.ExceedThresholdAutomation:
                    return new ExceedThresholdAutomation(threshold, actions);
                case Automations.FallBelowThresholdAutomation:
                    return new FallBelowThresholdAutomation(threshold, actions);
                default: return new ExceedThresholdAutomation(threshold, actions);
            }
        }

        /// <summary>
        /// Funktion des Observer-Patterns, ungenutzt.
        /// </summary>
        public void OnCompleted()
        {
        }

        /// <summary>
        /// Funktion des Observer-Patterns, ungenutzt.
        /// </summary>
        public void OnError(Exception error)
        {
        }

        /// <summary>
        /// Funktion des Observer-Patterns, wenn das Observable neue Daten bereitstellt.
        /// </summary>
        /// <param name="value">Die neuen Daten des Observables</param>
        public void OnNext(SensorData value)
        {
            if (value.Unit.Equals("END"))
            {
                System.Diagnostics.Debug.WriteLine("App: Remove subscription for: " + value.Name);
                Unsubscribe(value.Name);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("App: SensorData received: " + value.Name);
                OnSensorDataReceived(this, value);
            }
        }

        /// <summary>
        /// Funktion des Observer-Patterns, um einem Observable des Typs ISensor zu abonnieren.
        /// </summary>
        /// <param name="sensor">Der zu abonnierende Sensor</param>
        public virtual void Subscribe(ISensor sensor)
        {
            IDisposable cancellation = sensor.Subscribe(this);
            string name = sensor.GetSensorName();
            cancellations.Add(name, cancellation);
        }

        /// <summary>
        /// Funktion des Observer-Patterns, um einem Observable des Typs ISensor zu deabonnieren.
        /// </summary>
        /// <param name="sensorName">Name des zu deabonnierenden Sensors</param>
        public virtual void Unsubscribe(string sensorName)
        {
            
            cancellations[sensorName].Dispose();
            cancellations.Remove(sensorName);
        }

        /// <summary>
        /// Funktion zum Beenden aller aktuell laufenden Prozesse, wenn Applikation beendet werden soll-
        /// </summary>
        public void ShutdownApplication()
        {
            foreach(IAutomation automation in automationList.ToList())
            {
                automation.UnsubscribeAll();
                automationList.Remove(automation);
            }
            foreach(ISensor sensor in sensorList.ToList())
            {
                sensor.DisableSensor();
                Unsubscribe(sensor.GetSensorName());
                sensorList.Remove(sensor);
            }
        }
    }
}
