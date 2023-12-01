using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SoSe22_SE_SmartHome
{
    /// <summary>
    /// Enum zur Verwaltung aller verfügbaren Sensortypen.
    /// </summary>
    public enum Sensors
    {
        TemperatureSensor,
        LightSensor,
        CO2Sensor,
    }

    /// <summary>
    /// Interface für Sensoren, die Sensorwerte in Form von SensorData erzeugen.
    /// </summary>
    public interface ISensor : IObservable<SensorData>
    {
        /// <summary>
        /// Startet die Erzeugung von Sensorwerten im definierten Intervall und Wertebereich.
        /// </summary>
        void EnableSensor();
        void DisableSensor();

        /// <summary>
        /// Zugrundeliegende Funktion, die SensorData erzeugt und für Observer zu Verfügung stellt.
        /// </summary>
        void GetSensorData();

        string GetSensorName();
        void SetSensorName(string sensorName);
    }

    /// <summary>
    /// Interne Klasse für Observer zum Deabonnieren des Observables.
    /// </summary>
    /// <typeparam name="SensorData"></typeparam>
    internal class Unsubscriber<SensorData> : IDisposable
    {
        private List<IObserver<SensorData>> _observers;
        private IObserver<SensorData> _observer;

        internal Unsubscriber(List<IObserver<SensorData>> observers, IObserver<SensorData> observer)
        {
            this._observers = observers;
            this._observer = observer;
        }

        public void Dispose()
        {
            if (_observers.Contains(_observer))
                _observers.Remove(_observer);
        }
    }

    /// <summary>
    /// Sensor zur Überwachung des Lichtlevels. Implementiert ISensor.
    /// Erzeugt double-Sensorwerte mit der Einheit "Lux".
    /// Sensorwerte liegen im Bereich von 500 bis 1000.
    /// Das Intervall des Sensors beträgt 15 Sekunden.
    /// </summary>
    public class LightSensor : ISensor
    {
        public int id;
        public string name;

        public bool stop = false;

        private Thread thread;

        // Observer
        private List<IObserver<SensorData>> observers;
        
        public List<SensorData> sensorData;


        // Constants
        const double MIN_SENSOR_MEASUREMENT = 500;
        const double MAX_SENSOR_MEASUREMENT = 1000;
        const string UNIT = "Lux";
        const int INTERVAL = 15000;

        public LightSensor(int id, string name = null)
        {
            this.id = id;
            this.name = name;

            observers = new List<IObserver<SensorData>>();
            sensorData = new List<SensorData>();
        }

        public void EnableSensor()
        {
            System.Diagnostics.Debug.WriteLine(String.Format("Sensor: Sensor {0} enabled", this.name));
            thread = new Thread(new ThreadStart(GetSensorData));
            thread.Start();
        }

        public void DisableSensor()
        {
            stop = true;
        }

        public void GetSensorData()
        {
            Random rand = new Random();
            while (!stop)
            {
                double measurement = rand.NextDouble() * (MAX_SENSOR_MEASUREMENT - MIN_SENSOR_MEASUREMENT) + MIN_SENSOR_MEASUREMENT;
                measurement = Math.Round(measurement, 2, MidpointRounding.AwayFromZero);

                System.Diagnostics.Debug.WriteLine(String.Format("Sensor: Sensor {0} next measurement {1}", this.name, measurement));

                SensorData data = new SensorData(UNIT, measurement);
                data.Name = this.name;
                sensorData.Add(data);
                foreach (var observer in observers)
                {
                    observer.OnNext(data);
                }
                Thread.Sleep(INTERVAL);
            }
            foreach (var observer in observers.ToList())
            {
                SensorData data = new SensorData("END", 0);
                data.Name = this.name;
                observer.OnNext(data);
            }
        }
        public string GetSensorName()
        {
            return name;
        }

        public void SetSensorName(string sensorName)
        {
            this.name = sensorName;
        }

        public IDisposable Subscribe(IObserver<SensorData> observer)
        {
            if (!observers.Contains(observer))
            {
                observers.Add(observer);
            }
            return new Unsubscriber<SensorData>(observers, observer);
        }
    }

    /// <summary>
    /// Sensor zur Überwachung des CO2-Levels. Implementiert ISensor.
    /// Erzeugt double-Sensorwerte mit der Einheit "PPM".
    /// Sensorwerte liegen im Bereich von 500 bis 2100.
    /// Das Intervall des Sensors beträgt 15 Sekunden.
    /// </summary>
    public class CO2Sensor : ISensor
    {
        public int id;
        public string name;

        public bool stop = false;

        private Thread thread;

        // Observer
        private List<IObserver<SensorData>> observers;
        public List<SensorData> sensorData;


        // Constants
        const double MIN_SENSOR_MEASUREMENT = 500;
        const double MAX_SENSOR_MEASUREMENT = 2100;
        const string UNIT = "PPM";
        const int INTERVAL = 15000;

        public CO2Sensor(int id, string name = null)
        {
            this.id = id;
            this.name = name;

            observers = new List<IObserver<SensorData>>();
            sensorData = new List<SensorData>();
        }

        public void EnableSensor()
        {
            System.Diagnostics.Debug.WriteLine(String.Format("Sensor: Sensor {0} enabled", this.name));
            thread = new Thread(new ThreadStart(GetSensorData));
            thread.Start();
        }

        public void DisableSensor()
        {
            stop = true;
        }

        public void GetSensorData()
        {
            Random rand = new Random();
            while (!stop)
            {
                double measurement = rand.NextDouble() * (MAX_SENSOR_MEASUREMENT - MIN_SENSOR_MEASUREMENT) + MIN_SENSOR_MEASUREMENT;
                measurement = Math.Round(measurement, 0, MidpointRounding.AwayFromZero);

                System.Diagnostics.Debug.WriteLine(String.Format("Sensor: Sensor {0} next measurement {1}", this.name, measurement));

                SensorData data = new SensorData(UNIT, measurement);
                data.Name = this.name;
                sensorData.Add(data);
                foreach (var observer in observers)
                {
                    observer.OnNext(data);
                }
                Thread.Sleep(INTERVAL);
            }
            foreach (var observer in observers.ToList())
            {
                SensorData data = new SensorData("END", 0);
                data.Name = this.name;
                observer.OnNext(data);
            }
        }
        public string GetSensorName()
        {
            return name;
        }
        public void SetSensorName(string sensorName)
        {
            this.name = sensorName;
        }
        public IDisposable Subscribe(IObserver<SensorData> observer)
        {
            if (!observers.Contains(observer))
            {
                observers.Add(observer);
            }
            return new Unsubscriber<SensorData>(observers, observer);
        }
    }

    /// <summary>
    /// Sensor zur Überwachung der Temperatur. Implementiert ISensor.
    /// Erzeugt double-Sensorwerte mit der Einheit "Celsius".
    /// Sensorwerte liegen im Bereich von 15 bis 30.
    /// Das Intervall des Sensors beträgt 15 Sekunden.
    /// </summary>
    public class TemperatureSensor : ISensor
    {
        public int id;
        public string name;

        public bool stop = false;

        private Thread thread;

        // Observer
        private List<IObserver<SensorData>> observers;
        
        public List<SensorData> sensorData;


        // Constants
        const double MIN_SENSOR_MEASUREMENT = 15;
        const double MAX_SENSOR_MEASUREMENT = 30;
        const string UNIT = "Celsius";
        const int INTERVAL = 15000;

        public TemperatureSensor(int id, string name = null)
        {
            this.id = id;
            this.name = name;

            observers = new List<IObserver<SensorData>>();
            sensorData = new List<SensorData>();
        }

        public void EnableSensor()
        {
            System.Diagnostics.Debug.WriteLine(String.Format("Sensor: Sensor {0} enabled", this.name));
            thread = new Thread(new ThreadStart(GetSensorData));
            thread.Start();
        }

        public void DisableSensor()
        {
            stop = true;
        }

        public void GetSensorData()
        {
            Random rand = new Random();
            while (!stop)
            {
                double measurement = rand.NextDouble() * (MAX_SENSOR_MEASUREMENT - MIN_SENSOR_MEASUREMENT) + MIN_SENSOR_MEASUREMENT;
                measurement = Math.Round(measurement, 2, MidpointRounding.AwayFromZero);

                System.Diagnostics.Debug.WriteLine(String.Format("Sensor: Sensor {0} next measurement {1}", this.name, measurement));

                SensorData data = new SensorData(UNIT, measurement);
                data.Name = this.name;
                sensorData.Add(data);
                foreach (var observer in observers)
                {
                    observer.OnNext(data);
                }
                Thread.Sleep(INTERVAL);
            }
            foreach (var observer in observers.ToList())
            {
                SensorData data = new SensorData("END", 0);
                data.Name = this.name;
                observer.OnNext(data);
            }
        }
        public string GetSensorName()
        {
            return name;
        }
        public void SetSensorName(string sensorName)
        {
            this.name = sensorName;
        }
        public IDisposable Subscribe(IObserver<SensorData> observer)
        {
            if (!observers.Contains(observer))
            {
                observers.Add(observer);
            }
            return new Unsubscriber<SensorData>(observers, observer);
        }
    }

}
