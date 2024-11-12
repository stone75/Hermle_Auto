using System.Windows.Data;
using System.Windows;
using System.Globalization;

namespace Hermle_Auto.Converters
{
    public class EnumToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            //Console.WriteLine($"Convert {value.ToString()} {parameter.ToString()}");


            if (value == null || parameter == null || !(value is Enum))
            {
                Console.WriteLine($"Null");

                return Visibility.Collapsed;
            }
                


            var currentState = value.ToString();
            var stateStrings = parameter.ToString();
            var found = false;


            foreach (var state in stateStrings.Split(','))
            {

                //Console.WriteLine($"{value.ToString()} foreach {currentState.ToString()} {state.ToString()}");

                found = (currentState == state.Trim());

                if (found)
                {
                    Console.WriteLine($"found True");
                    break;
                }
                    
            }

            return found ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
