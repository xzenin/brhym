using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace WordPicker
{
    public static class UIBoundDataUpdateExtn
    {
        static List<object> controls = null;
        /// <summary>
        /// Updated
        /// </summary>
        /// <param name="element"></param>
        public static void UpdateUITextBox1(this FrameworkElement element)
        {
            if (element is TextBox)
            {
                element.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            }
        }
        public static void UpdateUIElement(this Panel root)
        {
            var controlInvoker = new ChildControls();
            if (controls == null)
            {
                controls = controlInvoker.GetChildren(root, 4);
            }
            foreach (var element in controls)
            {
                if (element is FrameworkElement)
                {
                    if (element is TextBox)
                    {
                        ((FrameworkElement)element).GetBindingExpression(TextBox.TextProperty).UpdateSource();
                    }
                    if(element is ListBox)
                    {

                    }
                    if(element is ComboBox)
                    {

                    }
                }
            }
        }
       
    }
    public class ChildControls
    {
        private List<object> lstChildren;

        public List<object> GetChildren(Visual p_vParent, int p_nLevel)
        {
            if (p_vParent == null)
            {
                throw new ArgumentNullException("Element {0} is null!", p_vParent.ToString());
            }

            this.lstChildren = new List<object>();

            this.GetChildControls(p_vParent, p_nLevel);

            return this.lstChildren;

        }

        private void GetChildControls(Visual p_vParent, int p_nLevel)
        {
            int nChildCount = VisualTreeHelper.GetChildrenCount(p_vParent);

            for (int i = 0; i <= nChildCount - 1; i++)
            {
                Visual v = (Visual)VisualTreeHelper.GetChild(p_vParent, i);

                lstChildren.Add((object)v);

                if (VisualTreeHelper.GetChildrenCount(v) > 0)
                {
                    GetChildControls(v, p_nLevel + 1);
                }
            }
        }
    }
}
