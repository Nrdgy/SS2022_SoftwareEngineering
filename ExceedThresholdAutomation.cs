using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoSe22_SE_SmartHome
{
    /// <summary>
    /// Automation zur Überwachung einer Überschreitung eines bestimmten Grenzwertes.
    /// Implementiert IAutomation.
    /// Ist Observer von Klassen, die ISensor implementieren.
    /// </summary>
    public class ExceedThresholdAutomation: IAutomation
    {
        public List<IAction> actions;
        public List<ISensor> sensors = new List<ISensor>();

        private double threshold;
        private Dictionary<string, IDisposable> cancellations = new Dictionary<string, IDisposable>();
        private const string ACTION_REASON_MSG = "überschritten";

        public ExceedThresholdAutomation(double threshold, List<IAction> actions)
        {
            this.actions = actions;  
            this.threshold = threshold;
        }

        public void AddAction(IAction action)
        {
            if (!actions.Contains(action))
            {
                actions.Add(action);    
            }
        }

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
        }

        public void OnNext(SensorData value)
        {
            if (CheckValue(value))
            {
                ExecuteActions(value);
            }
        }

        public void RemoveActions()
        {
            foreach(IAction action in actions.ToList())
            {
                actions.Remove(action);
            }
        }

        public void SetThreshold(double threshold)
        {
            this.threshold = threshold;
        }

        public double GetThreshold()
        {
            return this.threshold;
        }

        public List<ISensor> GetSensors()
        {
            return this.sensors;
        }

        public bool CheckValue(SensorData value)
        {
            if(value.Value > threshold)
            {
                return true;
            }
            return false;
        }

        public void ExecuteActions(SensorData data)
        {
            foreach(IAction action in actions)
            {
                action.Execute(data, threshold, ACTION_REASON_MSG);
            }
        }

        public void SetSubscriptions(List<ISensor> sensors)
        {
            foreach(ISensor sensor in sensors)
            {
                this.Subscribe(sensor);
            }
        }
        public virtual void Subscribe(ISensor sensor)
        {
            IDisposable cancellation = sensor.Subscribe(this);
            string name = sensor.GetSensorName();
            if (!cancellations.ContainsKey(name))
            {
                cancellations.Add(name, cancellation);
            }
            if (!sensors.Contains(sensor))
            {
                sensors.Add(sensor);
            }
        }
        public virtual void Unsubscribe(ISensor sensor)
        {
            string sensorName = sensor.GetSensorName();
            cancellations[sensorName].Dispose();
            cancellations.Remove(sensorName);

            this.sensors.Remove(sensor);
        }

        public void UnsubscribeAll()
        {
            foreach(ISensor sensor in sensors.ToList())
            {
                Unsubscribe(sensor);
            }
        }
    }
}
