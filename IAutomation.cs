using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoSe22_SE_SmartHome
{
    /// <summary>
    /// Interface für Automationen, die Sensoren überwachen und bei Erreichen des gesetzten Grenzwertes Aktionen ausführen.
    /// Implementierende Klassen sind Observer von SensorData.
    /// </summary>
    public interface IAutomation : IObserver<SensorData>
    {

        public void AddAction(IAction action);

        public void RemoveActions();

        public void SetThreshold(double threshold);

        public double GetThreshold();

        public List<ISensor> GetSensors();

        public void SetSubscriptions(List<ISensor> sensors);

        public void Unsubscribe(ISensor sensor);

        public void UnsubscribeAll();
    }


    /// <summary>
    /// Enum zur Verwaltung aller verfügbaren Automationstypen.
    /// </summary>
    public enum Automations
    {
        ExceedThresholdAutomation,
        FallBelowThresholdAutomation
    }
}
