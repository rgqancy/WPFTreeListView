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
    /// Interaction logic for PersonSample.xaml
    /// </summary>
    public partial class PersonSample : UserControl
    {
        public PersonSample()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int c1 = 2500;
            int c2 = 3;
            int c3 = 1;
            var model = PersonModel.CreateTestModel(c1, c2, c3);
            _tree.Model = model;
            MessageBox.Show("Loaded Data");
        }
    }
}
