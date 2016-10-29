using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TestApp.Tabs
{
    /// <summary>
    /// Interaction logic for RgqancySample.xaml
    /// </summary>
    public partial class RgqancySample : UserControl
    {
        public RgqancySample()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            wPFTreeListView1.Model = new RgqancyModel();
        }
    }
}
