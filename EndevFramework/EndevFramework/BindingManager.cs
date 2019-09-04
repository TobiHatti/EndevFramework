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
            public bool EFReadOnly { get; set; } = false;
            

            public BindingElement(Control pControl, string pConfigKey, bool pReadOnly)
            {
                EFControl = pControl;
                EFType = pControl.GetType();
                EFConfigKey = pConfigKey;
                EFReadOnly = pReadOnly;
            }

            public BindingElement(Control pControl,string pControlProperty, Type pExpectedType, string pConfigKey, bool pReadOnly)
            {
                EFControl = pControl;
                EFType = pExpectedType;
                EFConfigKey = pConfigKey;
                EFControlProperty = pControlProperty;
                EFReadOnly = pReadOnly;
            }

            public void Bind()
            {
                if(EFControlProperty == null)
                {
                    if      (EFControl.GetType() == typeof(Button))         (EFControl as Button).Text = EFConfigValue;
                    else if (EFControl.GetType() == typeof(Label))          (EFControl as Label).Text = EFConfigValue;
                    else if (EFControl.GetType() == typeof(LinkLabel))      (EFControl as LinkLabel).Text = EFConfigValue;
                    else if (EFControl.GetType() == typeof(MaskedTextBox))  (EFControl as MaskedTextBox).Text = EFConfigValue;
                    else if (EFControl.GetType() == typeof(RichTextBox))    (EFControl as RichTextBox).Text = EFConfigValue;
                    else if (EFControl.GetType() == typeof(TextBox))        (EFControl as TextBox).Text = EFConfigValue;

                    else if (EFControl.GetType() == typeof(CheckBox))       (EFControl as CheckBox).Checked = bool.Parse(EFConfigValue);
                    else if (EFControl.GetType() == typeof(DateTimePicker)) (EFControl as DateTimePicker).Value = DateTime.Parse(EFConfigValue);
                    else if (EFControl.GetType() == typeof(NumericUpDown))  (EFControl as NumericUpDown).Value = decimal.Parse(EFConfigValue);
                    else if (EFControl.GetType() == typeof(ProgressBar))    (EFControl as ProgressBar).Value = int.Parse(EFConfigValue);
                    else if (EFControl.GetType() == typeof(RadioButton))    (EFControl as RadioButton).Checked = bool.Parse(EFConfigValue);

                    else if (EFControl.GetType() == typeof(WebBrowser))     (EFControl as WebBrowser).Url = new Uri(EFConfigValue);

                    //else if (EFControl.GetType() == typeof(MonthCalendar))  (EFControl as MonthCalendar).??? = EFConfigValue;

                    //else if (EFControl.GetType() == typeof(ComboBox))       (EFControl as ComboBox).Text = EFConfigValue;
                    //else if (EFControl.GetType() == typeof(ListBox))        (EFControl as ListBox).Text = EFConfigValue;
                    //else if (EFControl.GetType() == typeof(ListView))       (EFControl as ListView).Text = EFConfigValue;

                    //else if (EFControl.GetType() == typeof(TreeView))       (EFControl as TreeView).Text = EFConfigValue;
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

            public void Read()
            {
                if (!EFReadOnly)
                {
                    if (EFControlProperty == null)
                    {
                        if (EFControl.GetType() == typeof(Button)) EFConfigValue = (EFControl as Button).Text;
                        else if (EFControl.GetType() == typeof(Label)) EFConfigValue = (EFControl as Label).Text;
                        else if (EFControl.GetType() == typeof(LinkLabel)) EFConfigValue = (EFControl as LinkLabel).Text;
                        else if (EFControl.GetType() == typeof(MaskedTextBox)) EFConfigValue = (EFControl as MaskedTextBox).Text;
                        else if (EFControl.GetType() == typeof(RichTextBox)) EFConfigValue = (EFControl as RichTextBox).Text;
                        else if (EFControl.GetType() == typeof(TextBox)) EFConfigValue = (EFControl as TextBox).Text;

                        else if (EFControl.GetType() == typeof(CheckBox)) EFConfigValue = (EFControl as CheckBox).Checked.ToString();
                        else if (EFControl.GetType() == typeof(DateTimePicker)) EFConfigValue = (EFControl as DateTimePicker).Value.ToString();
                        else if (EFControl.GetType() == typeof(NumericUpDown)) EFConfigValue = (EFControl as NumericUpDown).Value.ToString();
                        else if (EFControl.GetType() == typeof(ProgressBar)) EFConfigValue = (EFControl as ProgressBar).Value.ToString();
                        else if (EFControl.GetType() == typeof(RadioButton)) EFConfigValue = (EFControl as RadioButton).Checked.ToString();

                        else if (EFControl.GetType() == typeof(WebBrowser)) EFConfigValue = (EFControl as WebBrowser).Url.ToString();

                        //else if (EFControl.GetType() == typeof(MonthCalendar))  (EFControl as MonthCalendar).??? = EFConfigValue;

                        //else if (EFControl.GetType() == typeof(ComboBox))       (EFControl as ComboBox).Text = EFConfigValue;
                        //else if (EFControl.GetType() == typeof(ListBox))        (EFControl as ListBox).Text = EFConfigValue;
                        //else if (EFControl.GetType() == typeof(ListView))       (EFControl as ListView).Text = EFConfigValue;

                        //else if (EFControl.GetType() == typeof(TreeView))       (EFControl as TreeView).Text = EFConfigValue;
                    }
                    else
                    {
                        PropertyInfo prop = EFControl.GetType().GetProperty(EFControlProperty);
                        if (null != prop && prop.CanWrite)
                        {
                            EFConfigValue = prop.GetValue(EFControl, null).ToString();
                        }
                    }
                }
            }
        }

        public List<BindingElement> LBindingElements = new List<BindingElement>();

        /// <summary>
        /// Adds a control-element to the binding-list
        /// Suitable for default WinForm-Controls
        /// </summary>
        /// <param name="pControl">The control that should be bound</param>
        /// <param name="pConfigKey">The key for data-assigning in the Config-File</param>
        /// <param name="pReadOnly">Sets if the value in this control should be saved</param>
        public void AddBinding(Control pControl, string pConfigKey, bool pReadOnly = false)
        {
            LBindingElements.Add(new BindingElement(pControl, pConfigKey, pReadOnly));
        }

        /// <summary>
        /// Adds a control-element to the binding-list
        /// Suitable for external controls and frameworks
        /// </summary>
        /// <param name="pControl">The control that should be bound</param>
        /// <param name="pControlProperty">The control-property that should be bound, e.g. "Text"</param>
        /// <param name="pExpectedType">The datatype the binding should expect</param>
        /// <param name="pConfigKey">The key for data-assigning in the Config-File</param>
        /// <param name="pReadOnly">The key for data-assigning in the Config-File</param>

        public void AddBinding(Control pControl, string pControlProperty, Type pExpectedType, string pConfigKey, bool pReadOnly = false)
        {
            LBindingElements.Add(new BindingElement(pControl, pControlProperty, pExpectedType, pConfigKey, pReadOnly));
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

        public void SaveBindings(string pConfigFile)
        {
            foreach (BindingElement bl in LBindingElements)
            {
                bl.Read();
            }

            StreamReader sr = new StreamReader(pConfigFile);
            Dictionary<string, string> DConfigData = new Dictionary<string, string>();
            string line;

            while ((line = sr.ReadLine()) != null)
            {
                DConfigData.Add(line.Split('=')[0], line.Replace(line.Split('=')[0] + "=", ""));
            }

            sr.Close();

            Dictionary<string, string> DNewConfig = new Dictionary<string, string>(DConfigData);

            bool entrySaved = false;
            for(int i = 0; i < LBindingElements.Count; i++)
            {
                if (!LBindingElements[i].EFReadOnly)
                {
                    entrySaved = false;
                    foreach (KeyValuePair<string, string> cd in DConfigData)
                    {
                        if (cd.Key == LBindingElements[i].EFConfigKey)
                        {
                            DNewConfig[cd.Key] = LBindingElements[i].EFConfigValue;
                            entrySaved = true;
                        }
                    }

                    if (!entrySaved) DNewConfig.Add(LBindingElements[i].EFConfigKey, LBindingElements[i].EFConfigValue);
                }
            }


            StreamWriter sw = new StreamWriter(pConfigFile);

            foreach (KeyValuePair<string, string> cd in DNewConfig)
            {
                sw.WriteLine($"{cd.Key}={cd.Value}");
            }

            sw.Close();

            

        }
    }
}
