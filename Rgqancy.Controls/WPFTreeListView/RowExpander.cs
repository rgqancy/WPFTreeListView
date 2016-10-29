using System.Windows;
using System.Windows.Controls;

namespace Msra.SA.ETWAnalysisStudio.Common
{
    public class RowExpander : Control
    {
        static RowExpander()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RowExpander), new FrameworkPropertyMetadata(typeof(RowExpander)));
        }
    }
}
