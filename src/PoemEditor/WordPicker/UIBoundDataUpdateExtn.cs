using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace WordPicker
{
    public static class UIBoundDataUpdateExtn
    {
        /// <summary>
        /// Updated
        /// </summary>
        /// <param name="element"></param>
        public static void UpdateUITextBox(this FrameworkElement element)
        {
            if (element is TextBox)
            {
                element.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            }
        }
        public static void UpdateUI(this Panel root)
        {
            foreach (UIElement element in root.Children)
            {
                if (element is FrameworkElement)
                {
                    ((FrameworkElement)element).GetBindingExpression(TextBox.TextProperty).UpdateSource();
                }
            }
        }
       
    }
}
