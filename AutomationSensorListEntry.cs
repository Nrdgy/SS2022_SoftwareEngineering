using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SoSe22_SE_SmartHome
{
    /// <summary>
    /// Zugrundeliegende Klasse für den Sensor-ListView im AutomationSelection-Fenster.
    /// Beinhaltet relevante Informationen zu allen aktuell aktiven Sensoren.
    /// </summary>
    public class AutomationSensorListEntry : INotifyPropertyChanged
    {
        // Only for internal use
        public string Name { get; set; }   
        public string Type { get; set; }    
        public bool IsChecked { get; set; } 
       
        public AutomationSensorListEntry(string name, string type)
        {
            Name = name;
            Type = type;
            IsChecked = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if(this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
