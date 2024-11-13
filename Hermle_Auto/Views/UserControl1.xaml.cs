using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

using HermleCS.Comm;
using Hermle_Auto.ViewModels;
using HermleCS.Data;

namespace Hermle_Auto.Views
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class UserControl1 : UserControl
    {
        UserControl1ViewModel userControl1ViewModel = new UserControl1ViewModel();
        private CommHTTPComponent httpclient = CommHTTPComponent.Instance;

        public UserControl1()
        {
            InitializeComponent();

            this.DataContext = userControl1ViewModel;

            workPieceView.WorkPieceChanged += userControl1ViewModel.OnWorkPieceUpdate;
            //workPieceView.Visibility = Visibility.Visible;

            httpclient.MessageReceived += (addr, message) =>
            {
                string log = "Log : " + addr + " / " + message;

                if (logText.Dispatcher.CheckAccess())
                {
                    logText.Text = log;
                }
                else
                {
                    logText.Dispatcher.Invoke(() =>
                    {
                        logText.Text = log;
                    });
                }
            };
        }

  
        private void OpenPasswordWindow_Click(object sender, RoutedEventArgs e)
        {
            // Password.xaml 창을 불러오기
            PassDlg passwordWindow = new PassDlg();
            passwordWindow.ShowDialog(); // 모달 창으로 실행
            
        }

        private void OpenCommunicationWindow_Click(object sender, RoutedEventArgs e)
        {
            // Password.xaml 창을 불러오기
            CommunicationWindow Window = new CommunicationWindow();
            Window.ShowDialog(); // 모달 창으로 실행
            
        }


        private void OpenPLC_CHECK_Click(object sender, RoutedEventArgs e)
        {
            // Password.xaml 창을 불러오기
            //CommunicationWindow Window = new CommunicationWindow();
            //Window.ShowDialog(); // 모달 창으로 실행

            PLC_Monitor_Window Window = new PLC_Monitor_Window();
            Window.ShowDialog(); // 모달 창으로 실행

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }

   
}
