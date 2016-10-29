using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Msra.SA.ETWAnalysisStudio.Common
{
    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:DateTimePicker"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:DateTimePicker;assembly=DateTimePicker"
    ///
    /// You will also need to add a project reference from the project where the XAML file lives
    /// to this project and Rebuild to avoid compilation errors:
    ///
    ///     Right click on the target project in the Solution Explorer and
    ///     "Add Reference"->"Projects"->[Browse to and select this project]
    ///
    ///
    /// Step 2)
    /// Go ahead and use your control in the XAML file.
    ///
    ///     <MyNamespace:DateTimePicker/>
    ///
    /// </summary>
    [TemplatePart(Name = DateTimePicker.ElementDate, Type = typeof(TextBox))]
    //[TemplatePart(Name = DateTimePicker.ElementNumberDown, Type = typeof(Button))]
    //[TemplatePart(Name = DateTimePicker.ElementNumberUp, Type = typeof(Button))]
    public class DateTimePicker : Control
    {
        #region Constants

        private const string ElementDate = "PART_Date";
        private const string ElementNumberDown = "PART_Down";
        private const string ElementNumberUp = "PART_Up";

        #endregion Constants

        #region Data

        //private Button m_DownButton;
        //private Button m_UpButton;
        private TextBox m_TextBox;

        private int m_Postion;
        #endregion
        static DateTimePicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DateTimePicker), new FrameworkPropertyMetadata(typeof(DateTimePicker)));
        }

        #region DisplayDate

        /// <summary>
        /// Gets or sets the date to display.
        /// </summary>
        /// 
        public DateTime DisplayDate
        {
            get { return (DateTime)GetValue(DisplayDateProperty); }
            set { SetValue(DisplayDateProperty, value); }
        }

        /// <summary>
        /// Identifies the DisplayDate dependency property.
        /// </summary>
        public static readonly DependencyProperty DisplayDateProperty =
            DependencyProperty.Register(
            "DisplayDate",
            typeof(DateTime),
            typeof(DateTimePicker),
            new PropertyMetadata(DateTime.Now.AddMinutes(20), OnDisplayDateChanged, CoerceDisplayDate));

        /// <summary>
        /// DisplayDateProperty property changed handler.
        /// </summary>
        /// <param name="d">Calendar that changed its DisplayDate.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnDisplayDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DateTimePicker c = d as DateTimePicker;
            Debug.Assert(c != null);

        }

        private static object CoerceDisplayDate(DependencyObject d, object value)
        {
            var c = d as DateTimePicker;

            DateTime date = (DateTime)value;


            return value;
        }
        #endregion DisplayDate

        #region datetime Format

        public string Format
        {
            get { return (string)GetValue(FormatProperty); }
            set { SetValue(FormatProperty, value); }
        }

        public static readonly DependencyProperty FormatProperty =
          DependencyProperty.Register("Format", typeof(string), typeof(DateTimePicker),
                                      new PropertyMetadata("hh:mm:ss", FormatChangedCallback));

        private static void FormatChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dateTimePicker = d as DateTimePicker;

        }

        private ITimePickerFormat m_TimePickerFormat = new FormatTime();
        public ITimePickerFormat TimePickerFormat
        {
            get { return m_TimePickerFormat; }
            set { m_TimePickerFormat = value; }
        }

        private void ShowDatetime()
        {
            DateTime dateTime = DisplayDate;
            string showText = Format;
            showText = Regex.Replace(showText, @"[y]+", FormatShow('y', dateTime.Year));
            showText = Regex.Replace(showText, @"[M]+", FormatShow('M', dateTime.Month));
            showText = Regex.Replace(showText, @"[d]+", FormatShow('d', dateTime.Day));
            showText = Regex.Replace(showText, @"[h]+", FormatShow('h', dateTime.Hour));
            showText = Regex.Replace(showText, @"[m]+", FormatShow('m', dateTime.Minute));
            showText = Regex.Replace(showText, @"[s]+", FormatShow('s', dateTime.Second));
            m_TextBox.Text = showText;
        }

        private string FormatShow(char latter, int number)
        {
            string rFormat;
            string formatText = Format;
            int count = formatText.Where(c => c == latter).Count();
            if (Math.Pow(10, count) > number)
            {
                rFormat = number.ToString("D" + count);
            }
            else
            {
                string s = number.ToString();
                rFormat = s.Substring(s.Length - count);
            }
            return rFormat;
        }

        #endregion

        #region Public Methods
        public override void OnApplyTemplate()
        {
            //if (m_DownButton != null)
            //{
            //    m_DownButton.Click -= DownButton_Click;
            //}

            //if (m_UpButton != null)
            //{
            //    m_UpButton.Click -= UpButton_Click;
            //}

            if (m_TextBox != null)
            {
                m_TextBox.GotFocus -= TextBox_GotFocus;
                m_TextBox.LostFocus -= TextBox_LostFocus;
            }

            base.OnApplyTemplate();

            //m_DownButton = GetTemplateChild(ElementNumberDown) as Button;
            //if (m_DownButton != null) m_DownButton.Click += DownButton_Click;

            //m_UpButton = GetTemplateChild(ElementNumberUp) as Button;
            //if (m_UpButton != null) m_UpButton.Click += UpButton_Click;

            m_TextBox = GetTemplateChild(ElementDate) as TextBox;
            if (m_TextBox != null)
            {
                m_TextBox.GotFocus += TextBox_GotFocus;
                m_TextBox.LostFocus += TextBox_LostFocus;
                ShowDatetime();
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            ValidateText(m_TextBox.Text);
        }

        #region Check text


        private void ValidateText(string text)
        {
            ///(?<hh>\d+):(?<mm>\d+):(?<ss>\d+)
            /*
               # string strRegexConn = @"((?:server=(?<server>[^;\(\)]+)[;]).*(?:provider=(?<provider>[^;]+);).*(?:host=(?<host>[\w\d\.]+)).*(?:port=(?<port>\d+)).*(?:service_name=(?<servicename>[^\(\)]+)).*(?:user\sid=(?<userid>[^;]+)).*(?:password=(?<password>[^;]+)))|(?:provider=(?<provider>[^;]+);).*?(?:server=(?<server>[^;\(\)]+)[;]).*?(?:database=(?<database>[^;]+).*?(?:user\sid=(?<userid>[^;]+)).*(?:password=(?<password>[^;]+)))";
                #  string str="provider=Microsoft Sql Server 2000;server=172.16.10.100;database=ObxMetabaseHDDemo;User ID=sa;Password=123;MultipleActiveResultSets=True";
                # Regex r = new Regex(strRegexConn, RegexOptions.IgnoreCase)；
                # Match m = r.Match(str)；
                #  string provider = m.Groups["provider"].Value;
                #  string server= m.Groups["server"].Value;
             */
            string regexText = Format;
            regexText = Regex.Replace(regexText, @"[y]+", @"(?<yy>[0-9]+)");
            regexText = Regex.Replace(regexText, @"[M]+", @"(?<MM>[0-9]+)");
            regexText = Regex.Replace(regexText, @"[d]+", @"(?<dd>[0-9]+)");
            regexText = Regex.Replace(regexText, @"[h]+", @"(?<hh>[0-9]+)");
            regexText = Regex.Replace(regexText, @"[m]+", @"(?<mm>[0-9]+)");
            regexText = Regex.Replace(regexText, @"[s]+", @"(?<ss>[0-9]+)");
            Regex r = new Regex(regexText);
            Match match = r.Match(text);

            DateTime currentTime = DisplayDate;
            try
            {
                string year = match.Groups["yy"].Value;
                string month = match.Groups["MM"].Value;
                string day = match.Groups["dd"].Value;
                string hour = match.Groups["hh"].Value;
                string minute = match.Groups["mm"].Value;
                string second = match.Groups["ss"].Value;
                DateTime formatDateTime = DateTime.MinValue;
                DateTime now = DisplayDate;
                if (!string.IsNullOrEmpty(year))
                {
                    formatDateTime = formatDateTime.AddYears(Convert.ToInt32(year) - 1);
                }
                else
                {
                    formatDateTime = formatDateTime.AddYears(now.Year - 1);
                }

                if (!string.IsNullOrEmpty(month))
                {
                    formatDateTime = formatDateTime.AddMonths(Convert.ToInt32(month) - 1);
                }
                else
                {
                    formatDateTime = formatDateTime.AddMonths(now.Month - 1);
                }

                if (!string.IsNullOrEmpty(day))
                {
                    formatDateTime = formatDateTime.AddDays(Convert.ToInt32(day) - 1);
                }
                else
                {
                    formatDateTime = formatDateTime.AddDays(now.Day - 1);
                }

                if (!string.IsNullOrEmpty(hour))
                {
                    formatDateTime = formatDateTime.AddHours(Convert.ToInt32(hour));
                }
                else
                {
                    formatDateTime = formatDateTime.AddHours(now.Hour);
                }

                if (!string.IsNullOrEmpty(minute))
                {
                    formatDateTime = formatDateTime.AddMinutes(Convert.ToInt32(minute));
                }
                else
                {
                    formatDateTime = formatDateTime.AddMinutes(now.Minute);
                }

                if (!string.IsNullOrEmpty(second))
                {
                    formatDateTime = formatDateTime.AddSeconds(Convert.ToInt32(second));
                }
                else
                {
                    formatDateTime = formatDateTime.AddSeconds(now.Second);
                }

                DisplayDate = formatDateTime;
            }
            catch (Exception)
            {
                DisplayDate = currentTime;
            }
            finally
            {
                ShowDatetime();
            }
        }

        #endregion

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            ShowDatetime();
        }

        private void UpButton_Click(object sender, RoutedEventArgs e)
        {
            m_Postion = m_TextBox.SelectionStart;

            try
            {
                DisplayDate = TimePickerFormat.Format(Format, m_Postion, true, DisplayDate);
            }
            catch { }
            finally
            {
                ShowDatetime();
            }
            m_TextBox.SelectionStart = m_Postion;

        }

        void DownButton_Click(object sender, RoutedEventArgs e)
        {
            m_Postion = m_TextBox.SelectionStart;
            try
            {
                DisplayDate = TimePickerFormat.Format(Format, m_Postion, false, DisplayDate);
            }
            catch { }
            finally
            {
                ShowDatetime();
            }
            m_TextBox.SelectionStart = m_Postion;
        }
        #endregion

        private class FormatTime : ITimePickerFormat
        {

            #region Implementation of ITimePickerFormat

            public DateTime Format(string format, int chengedPostion, bool isUp)
            {

                return Format(format, chengedPostion, isUp, DateTime.Now);
            }

            public DateTime Format(string format, int chengedPostion, bool isUp, DateTime baseTime)
            {
                string timeFormat = format;
                DateTime time = baseTime;
                // hh:mm:ss => !h!h!:!m!m!:!s!s!  => 9! length= 8
                //chengedPostion max= 8 min = 0
                if (chengedPostion > timeFormat.Length)
                {
                    throw new ArgumentOutOfRangeException("chengedPostion");
                }
                if (chengedPostion >= timeFormat.Length - 1)
                    chengedPostion = timeFormat.Length - 1;
                if (chengedPostion < 0)
                    chengedPostion = 0;
                int operand = isUp ? 1 : -1;
                char positionchar = timeFormat[chengedPostion];
                LetterPostion postion = SameLetterPostion(positionchar, timeFormat, chengedPostion);
                switch (positionchar)
                {
                    case ':':
                        return Format(format, chengedPostion - 1, isUp, baseTime);
                    case '-':
                        return Format(format, chengedPostion - 1, isUp, baseTime);
                    case 'h':
                        return time.AddHours(operand);
                    case 'm':
                        operand = (int)Math.Pow(10, postion.RightCount) * operand;
                        return time.AddMinutes(operand);
                    case 's':
                        operand = (int)Math.Pow(10, postion.RightCount) * operand;
                        return time.AddSeconds(operand);
                    case 'd':
                        return time.AddDays(operand);
                    case 'M':
                        return time.AddMonths(operand);
                    case 'y':
                        operand = (int)Math.Pow(10, postion.RightCount) * operand;
                        if (time.Year + operand < 1 || time.Year + operand > 9999)
                        {
                            return time;
                        }
                        return time.AddYears(operand);
                    default:
                        throw new NotImplementedException();
                }
            }

            private LetterPostion SameLetterPostion(char letter, string format, int postion)
            {
                LetterPostion count = new LetterPostion();
                int leftPostion = postion;
                int rightPostion = postion;

                while (leftPostion > 0)
                {
                    leftPostion--;
                    if (format[leftPostion] == letter)
                        count.LeftCount++;
                    else
                        break;
                }
                while (rightPostion < format.Length - 1)
                {
                    ++rightPostion;
                    if (format[rightPostion] == letter)
                        count.RightCount++;
                    else
                        break;
                }
                return count;
            }

            private class LetterPostion
            {
                public int LeftCount { get; set; }
                public int RightCount { get; set; }
                public int Count
                {
                    get { return LeftCount + RightCount + 1; }
                }
            }
            #endregion
        }

    }


    public interface ITimePickerFormat
    {
        DateTime Format(string format, int chengedPostion, bool isUp);
        DateTime Format(string format, int chengedPostion, bool isUp, DateTime baseTime);
    }
}
