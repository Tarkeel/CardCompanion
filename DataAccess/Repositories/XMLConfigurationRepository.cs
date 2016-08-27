using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml.Linq;

namespace DataAccess.Repositories
{
    sealed class XMLConfigurationRepository : AbstractConfigurationRepository
    {
        #region Attributes and Constructor
        private XMLRepositoryFactory factory;
        private XDocument document;
        private Dictionary<string, string> settings;
        internal XMLConfigurationRepository(XMLRepositoryFactory _factory)
        {
            factory = _factory;
            document = _factory.Document;
            settings = new Dictionary<string, string>();
        }
        #endregion
        #region Interface Overrides
        public override string GetValue(string Setting)
        {
            string value;
            //Check if the setting is stored
            if (settings.TryGetValue(Setting, out value)) { return value; }
            //If not, find the correct element in the file
            XElement element = FindElementByKey(Setting);
            //Send the element to parsing, which will return null if it's blank.
            return ParseSetting(element);
        }
        public override void SetValue(string Setting, string Value, bool persist = true)
        {
            //Find the element
            XElement element = FindElementByKey(Setting);
            if (element == null)
            {
                //Setting isn't stored, so create it
                element = new XElement("Setting",
                    new XAttribute("Key", Setting),
                    new XAttribute("Value", Value));
                document.Root.Element("Config").Add(element);
            }
            else
            {
                //Update the element
                XAttribute _value = element.Attribute("Value");
                if (_value == null) { element.Add(new XAttribute("Key", Value)); }
                else { _value.Value = Value; }
                //Update dictionary
                settings[Setting] = Value;
            }
            if (persist) { factory.Save(); }
        }
        public override void ClearSetting(string Setting, bool persist = true)
        {
            //Find the element
            XElement element = FindElementByKey(Setting);
            //Remove from document
            if (element != null) { element.Remove(); }
            //Remove from dictionary
            settings.Remove(Setting);
            if (persist) { factory.Save(); }
        }
        #endregion
        #region XML Handling
        internal XElement FindElementByKey(string key)
        {
            return (from XElement in document.Root.Element("Config").Elements()
                    where XElement.Attribute("Key").Value.Equals(key)
                    select XElement).FirstOrDefault();
        }
        internal string ParseSetting(XElement element)
        {
            //Check input
            if (element == null) { return null; }
            //Setting attribute
            string setting = "";
            XAttribute _setting = element.Attribute("Key");
            if (_setting != null) { setting = _setting.Value; }
            //Value attribute
            string value = "";
            XAttribute _value = element.Attribute("Value");
            if (_value != null) { value = _value.Value; }
            //Store and return
            settings.Add(setting, value);
            return value;
        }
        #endregion
    }
}