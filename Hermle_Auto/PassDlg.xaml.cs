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
using System.Windows.Shapes;

namespace Hermle_Auto
{
    /// <summary>
    /// PassDlg.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PassDlg : Window
    {
        private const string CORRECT_PASSWORD = "1111";


        public PassDlg()
        {
            InitializeComponent();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (passwordBox.Password == CORRECT_PASSWORD)
            {
                mainTabControl.Visibility = Visibility.Visible;
                //statusText.Text = "비밀번호가 맞습니다.";
                //statusText.Foreground = Brushes.Green;
            }
            else
            {
                mainTabControl.Visibility = Visibility.Collapsed;
                //statusText.Text = "비밀번호가 틀렸습니다.";
                //statusText.Foreground = Brushes.Red;
            }
        }
    }
}
