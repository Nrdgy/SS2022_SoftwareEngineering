using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoSe22_SE_SmartHome
{
    /// <summary>
    /// Interface für Aktionen, die von Automationen ausgelöst werden können.
    /// </summary>
    public interface IAction
    {
        /// <summary>
        /// Funktion zur Ausführung einer Aktion.
        /// </summary>
        /// <param name="data">Für Auslösung verantwortliche Daten</param>
        /// <param name="threshold">Der in der Automation gesetzte Grenzwert</param>
        /// <param name="reason">Der Grund, warum die Aktion ausgelöst wird.</param>
        public void Execute(SensorData data, double threshold, string reason);
    }
}
