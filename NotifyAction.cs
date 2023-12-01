using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoSe22_SE_SmartHome
{
    /// <summary>
    /// Aktion, die eine Benachrichtigung aussendet. Implementiert IAction.
    /// Erstellt eine UWP-Benachrichtigung mit relevanten Informationen, die im Windows Benachrichtigungscenter erscheint.
    /// </summary>
    public class NotifyAction : IAction
    {
        public string notificationContent;
        public void Execute(SensorData data, double threshold, string reason)
        {
            notificationContent = String.Format("Sensor {0} hat Grenze {1} mit Wert {2} {3}!", data.Name, threshold, data.Value, reason);
            new ToastContentBuilder()
                .AddText("Sensorüberwachung ausgeschlagen:")
                .AddText("Sensor: " + notificationContent)
                .Show();
        }
    }


    /// <summary>
    /// Enum zur Verwaltung aller verfügbaren Aktionen.
    /// </summary>
    public enum Actions
    {
        NotifyAction
    }
}
