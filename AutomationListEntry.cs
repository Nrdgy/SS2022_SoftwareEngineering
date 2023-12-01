using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoSe22_SE_SmartHome
{
    /// <summary>
    /// Zugrundeliegende Klasse für ListViews für Automationen.
    /// Beinhaltet alle relevanten Informationen zu Automationen.
    /// </summary>
    public class AutomationListEntry
    {
        public double Threshold { get; set; }
        public string AutomationType { get; set; }
        public string SensorsCommaSeparated { get; set; }  

        public List<ISensor> Sensors { get; set; } 
        public IAutomation Automation { get; set; } 
    }
}
