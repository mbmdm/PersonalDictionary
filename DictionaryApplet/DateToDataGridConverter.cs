using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Globalization;

namespace PersonalDictionary
{
    public class DateToDataGridConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime)
            {
                string[] variants = { "Сегодня", "Вчера", "Не этой недели", "В этом месяца", "В этом году", "Давным давно" };

                DateTime date = new DateTime(((DateTime)value).Year, ((DateTime)value).Month, ((DateTime)value).Day);
                DateTime now = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

                if (date == now)
                    return variants[0];
                else if ((now - date).Days == 1)
                    return variants[1];
                else if ((now - date).Days <= 7)
                    return variants[2];
                else if ((now - date).Days <= 30)
                    return variants[3];
                else if ((now - date).Days <= 365)
                    return variants[4];
                else
                    return variants[5];

                return "sort error";
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
