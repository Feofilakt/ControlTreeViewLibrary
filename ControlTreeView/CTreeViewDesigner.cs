using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms.Design;
using System.ComponentModel;

namespace ControlTreeView
{
    class CTreeViewDesigner : ControlDesigner
    {
        protected override void PostFilterProperties(System.Collections.IDictionary properties)
        {
            //PropertyDescriptor propertyDesc = TypeDescriptor.GetProperties(typeof(CTreeView))["Controls"];
        //    PropertyDescriptor propertyDesc = TypeDescriptor.CreateProperty(
        //typeof(CTreeViewDesigner),
            //(PropertyDescriptor)properties["Controls"],
        //new Attribute[0]);
            //properties["Controls"] = propertyDesc;

            //properties.Remove("Controls");
            //properties.Add("Controls", propertyDesc);
            
            base.PostFilterProperties(properties);
        }
    }
}
