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
    /// Klasse zur Übermittlung von Sensorwerten.
    /// Diese Klasse ist die Grundlage des Observer-Patterns der Anwendung und wird als Datenformat zur Übertragung verwendet.
    /// </summary>
    public class SensorData
    {
        private string unit;
        private double value;
        private Sensors sensorType;
        private string name;

        internal SensorData(string unit, double value)
        {
            this.unit = unit;
            this.value = value;
        }

        public string Unit
        {
            get { return this.unit; }
        }

        public double Value
        {
            get { return this.value; }
            set { this.value = value; } 
        }

        public string Name
        {
            get { return this.name; }
            set { this.name = value; }   
        }
    }
}
