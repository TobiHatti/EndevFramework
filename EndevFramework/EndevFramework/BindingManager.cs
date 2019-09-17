using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace EndevFramework
{
    public class BindingManager
    {
        public abstract class BindingElementBase
        {
            public Control Control { get; set; } = null;
            public string ValueProperty { get; set; } = null;
            public Type ValueType { get; set; } = null;
            public string ConfigKey { get; set; } = null;
            public bool ReadOnly { get; set; } = false;
        }

        public class BindingElement : BindingElementBase
        {
            public object ControlValue { get; set; } = null;

            public BindingElement(Control pControl, string pValueProperty, Type pValueType, string pConfigKey, bool pReadOnly = false)
            {
                this.Control = pControl;
                this.ValueProperty = pValueProperty;
                this.ValueType = pValueType;
                this.ConfigKey = pConfigKey;
                this.ReadOnly = pReadOnly;
            }
        }

        public class DataBindingElement : BindingElementBase
        {
            public List<List<object>> ControlValues { get; set; } = null;

            public DataBindingElement(Control pControl, string pValueProperty, Type pValueType, string pConfigKey, bool pReadOnly = false)
            {
                this.Control = pControl;
                this.ValueProperty = pValueProperty;
                this.ValueType = pValueType;
                this.ConfigKey = pConfigKey;
                this.ReadOnly = pReadOnly;
            }
        }

        public List<BindingElement> LBindingElements = new List<BindingElement>();
        public List<DataBindingElement> LDataBindingElements = new List<DataBindingElement>();

        private string configLoadFile = "";
        private string configSaveFile = "";

        public void Bind(Control pControl, string pValueProperty, string pConfigKey, bool pReadOnly = false)
        {
            // Check if the ValueProperty accepts multiple values -> DataBindingElement
            PropertyInfo propertyInfo = pControl.GetType().GetProperty(pValueProperty);
            Type valueType = propertyInfo.PropertyType;

            if(propertyInfo.PropertyType.GetInterface(typeof(IEnumerable<>).FullName) != null)
            {
                // BindingElement
                LBindingElements.Add(new BindingElement(pControl, pValueProperty, valueType, pConfigKey, pReadOnly));
            }
            else
            {
                // DataBindingElement
                LDataBindingElements.Add(new DataBindingElement(pControl, pValueProperty, valueType, pConfigKey, pReadOnly));
            }

            //propertyInfo.SetValue(null, Convert.ChangeType(pPropertyValue, propertyInfo.PropertyType), null);

        }


        public void LoadFromFile(string pFilePath)
        {
            configLoadFile = pFilePath;

            StreamReader sw = new StreamReader(configLoadFile);
            string line = "";

            string tmpKey;
            string tmpDataLine;
            List<object> tmpList;
            List<List<object>> tmpListList;


            while((line = sw.ReadLine()) != null)
            {
                // Skip line if comment or empty
                if (line.StartsWith("#") || string.IsNullOrWhiteSpace(line) || string.IsNullOrEmpty(line)) continue;

                //Check for DataBindings
                if(line.StartsWith("["))
                {
                    // Get Key
                    tmpKey = line.TrimStart('[').TrimEnd(']');

                    // Find right element
                    foreach (DataBindingElement dbe in LDataBindingElements)
                    {
                        if (dbe.ConfigKey == tmpKey)
                        {
                            tmpListList = new List<List<object>>();

                            // Fetch Data-Entries
                            while ((tmpDataLine = sw.ReadLine()) != null && tmpDataLine.StartsWith("{"))
                            {
                                tmpList = new List<object>();
                                foreach (string linePiece in tmpDataLine.Split(','))
                                    tmpList.Add(linePiece.TrimStart('{').TrimEnd('}'));

                                tmpListList.Add(tmpList);
                            }

                            dbe.ControlValues = tmpListList;
                        }
                    }
                }
                else
                {
                    // Set values
                    foreach (BindingElement be in LBindingElements)
                        if (be.ConfigKey == line.Split('=')[0])
                            be.ControlValue = line.Replace(line.Split('=')[0]+"=","");
                }
            }

        }

        public void SaveToFile(string pFilePath)
        {
            configSaveFile = pFilePath;
        }

        public void FillBindings()
        {

        }

        public void ReloadBindings()
        {
            LoadFromFile(configLoadFile);
            FillBindings();
        }
    }
}
