﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Msra.SA.ETWAnalysisStudio.Common
{
    public class LevelToIndentConverter : IValueConverter
    {
        private const double IndentSize = 19.0;

        public object Convert(object o, Type type, object parameter, CultureInfo culture)
        {
            return new Thickness((int)o * IndentSize, 0, 0, 0);
        }

        public object ConvertBack(object o, Type type, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
