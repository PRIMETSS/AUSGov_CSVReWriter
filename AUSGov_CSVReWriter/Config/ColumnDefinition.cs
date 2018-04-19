using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace AUSGov_CSVReWriter.Config
{

    [Serializable, XmlRoot("Column")]
    public class Column
    {
        [XmlElement(ElementName = "Field")]
        public Field Field { get; set; }
    }

    public class Field
    {
        [XmlAttribute(AttributeName = "Name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "IsFixedSize")]
        public bool IsFixedSize { get; set; }

        [XmlAttribute(AttributeName = "DelimitPossition")]
        public int DelimitPossition { get; set; }
    }    
}