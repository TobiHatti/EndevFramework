using System;
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
            public string EFConfigLink { get; set; } = null;
            public object EFControlProperty { get; set; } = null;

            public BindingElement(Control pControl, string pConfigLink)
            {
                EFControl = pControl;
                EFType = pControl.GetType();
                EFConfigLink = pConfigLink;
            }

            public BindingElement(ref object pControlProperty, Type pExpectedType, string pConfigLink)
            {
                EFType = pExpectedType;
                EFConfigLink = pConfigLink;
                EFControlProperty = pControlProperty;
            }
        }
        

        public BindingManager()
        {

        }

        public void AddBinding(Control pControl, string pConfigLink)
        {

        }

        public void AddBinding(ref object pControlProperty, Type pExpectedType, string pConfigLink)
        {

        }
    }
}
