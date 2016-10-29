using System.Windows.Controls;
using System.Windows;

namespace Msra.SA.ETWAnalysisStudio.Common
{
    public class FixedWidthGridViewColumn : GridViewColumn
    {
        #region Constructor

        static FixedWidthGridViewColumn()
        {
            WidthProperty.OverrideMetadata(typeof(FixedWidthGridViewColumn),
                new FrameworkPropertyMetadata(null, new CoerceValueCallback(OnCoerceWidth)));
        }

        private static object OnCoerceWidth(DependencyObject o, object baseValue)
        {
            FixedWidthGridViewColumn fwc = o as FixedWidthGridViewColumn;
            if (fwc != null)
                return fwc.FixedWidth;
            return 0.0;
        }

        #endregion

        #region FixedWidth

        public double FixedWidth
        {
            get { return (double)GetValue(FixedWidthProperty); }
            set { SetValue(FixedWidthProperty, value); }
        }

        public static readonly DependencyProperty FixedWidthProperty =
            DependencyProperty.Register("FixedWidth", typeof(double), typeof(FixedWidthGridViewColumn),
            new FrameworkPropertyMetadata(double.NaN, new PropertyChangedCallback(OnFixedWidthChanged)));

        private static void OnFixedWidthChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            FixedWidthGridViewColumn fwc = o as FixedWidthGridViewColumn;
            if (fwc != null)
                fwc.CoerceValue(WidthProperty);
        }

        #endregion
    }
}
