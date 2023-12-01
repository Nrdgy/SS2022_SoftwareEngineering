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
    /// Zugrundeliegende Klasse für ListViews für Sensoren.
    /// Beinhaltet alle relevanten Informationen zu Sensoren.
    /// Teilt zudem mit, wenn Eigenschaften verändert wurden.
    /// </summary>
    public class SensorListEntry : INotifyPropertyChanged
    {
        // Only for internal use
        private string _SensorData;
        private string _Name;
        public string Name
        {
            get
            {
                return this._Name;
            }
            set
            {
                this._Name = value;
                NotifyPropertyChanged();
            }
        }
        
        public string Type { get; set; }    

        public string SensorData {
            get
            {
                return this._SensorData;
            }
            set
            {
             this._SensorData = value;   
                NotifyPropertyChanged();
            } 
        }

        public ISensor Sensor   { get; set; }


        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if(this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
