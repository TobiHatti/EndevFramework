using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace EndevFramework
{
    public class BindingManager
    {
        /// <summary>
        /// For internal use only.
        /// Stores data of one Binding-Element
        /// </summary>
        public class BindingElement
        {
            public Control EFControl { get; set; } = null;
            public Type EFType { get; set; } = null;
            public string EFConfigKey { get; set; } = null;
            public string EFConfigValue { get; set; } = null;
            public string EFControlProperty { get; set; } = null;
            

            public BindingElement(Control pControl, string pConfigKey)
            {
                EFControl = pControl;
                EFType = pControl.GetType();
                EFConfigKey = pConfigKey;
            }

            public BindingElement(Control pControl,string pControlProperty, Type pExpectedType, string pConfigKey)
            {
                EFControl = pControl;
                EFType = pExpectedType;
                EFConfigKey = pConfigKey;
                EFControlProperty = pControlProperty;
            }

            public void Bind()
            {
                if(EFControlProperty == null)
                {
                    if(EFControl.GetType() == typeof(TextBox)) (EFControl as TextBox).Text = EFConfigValue;


                }
                else
                {
                    PropertyInfo prop = EFControl.GetType().GetProperty(EFControlProperty);
                    if (null != prop && prop.CanWrite)
                    {
                        prop.SetValue(EFControl, EFConfigValue, null);
                    }
                }
            }
        }

        public List<BindingElement> LBindingElements = new List<BindingElement>();

        public BindingManager()
        {

        }

        /// <summary>
        /// Adds a control-element to the binding-list
        /// Suitable for default WinForm-Controls
        /// </summary>
        /// <param name="pControl">The control that should be bound</param>
        /// <param name="pConfigKey">The key for data-assigning in the Config-File</param>
        public void AddBinding(Control pControl, string pConfigLink)
        {
            LBindingElements.Add(new BindingElement(pControl, pConfigLink));
        }

        /// <summary>
        /// Adds a control-element to the binding-list
        /// Suitable for external controls and frameworks
        /// </summary>
        /// <param name="pControlProperty">The control-property that should be bound, e.g. myLabel.Text</param>
        /// <param name="pExpectedType">The datatype the binding should expect</param>
        /// <param name="pConfigKey">The key for data-assigning in the Config-File</param>
        public void AddBinding(Control pControl, string pControlProperty, Type pExpectedType, string pConfigKey)
        {
            LBindingElements.Add(new BindingElement(pControl, pControlProperty, pExpectedType, pConfigKey));
        }

        public void LoadBinding(string pConfigFile)
        {
            StreamReader sr = new StreamReader(pConfigFile);
            Dictionary<string, string> DConfigData = new Dictionary<string, string>();
            string line;

            while((line = sr.ReadLine()) != null)
            {
                DConfigData.Add(line.Split('=')[0], line.Replace(line.Split('=')[0] + "=",""));
            }

            sr.Close();

            foreach(BindingElement be in LBindingElements)
            {
                foreach (KeyValuePair<string, string> cd in DConfigData)
                {
                    if (cd.Key == be.EFConfigKey) be.EFConfigValue = cd.Value;
                }
            }


        }

        public void FillBindings()
        {
            foreach(BindingElement bl in LBindingElements)
            {
                bl.Bind();
            }
        }

        public void SaveBindings()
        {

        }
    }
}
