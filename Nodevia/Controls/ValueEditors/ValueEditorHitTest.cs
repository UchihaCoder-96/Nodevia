using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Nodevia.Controls.ValueEditors
{
    internal class ValueEditorHitTest
    {
        public static bool IsInsideValueEditor(DependencyObject start)
        {
            DependencyObject? current = start;

            while (current is not null)
            {
                if (current is TextBox or CheckBox or ComboBox or RepeatButton or ToggleButton or Popup)
                    return true;

                current = LogicalTreeHelper.GetParent(current) ?? VisualTreeHelper.GetParent(current);
            }

            return false;
        }
    }
}
