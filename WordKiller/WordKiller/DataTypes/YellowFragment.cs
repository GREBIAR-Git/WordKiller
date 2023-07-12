using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using WordKiller.Commands;
using WordKiller.Scripts;
using System.Xml.Serialization;

namespace WordKiller.DataTypes
{
    [Serializable]
    public class YellowFragment
    {
        public string? Text { get; set; }

        public int index;
        [XmlIgnore]
        public int Index 
        { 
            get => index; 
            set
            {
                index = value;
                TemplateHelper.NeedSave = true;
            }
        }
    }
}
