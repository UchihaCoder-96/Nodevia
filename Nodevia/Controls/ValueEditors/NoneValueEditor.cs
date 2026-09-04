using System.Windows;

namespace Nodevia.Controls.ValueEditors;

public class NoneValueEditor : PortValueEditor
{
    static NoneValueEditor()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(NoneValueEditor),
            new FrameworkPropertyMetadata(typeof(NoneValueEditor)));
    }
}

