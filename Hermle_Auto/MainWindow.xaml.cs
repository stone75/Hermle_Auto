﻿using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Hermle_Auto
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            CheckRobotAndPLC();

            Closing += MainWindow_Closing;

        }

        //윈도우가 종료될때 자식 컨트롤의 종료 메시지 호출
        private void MainWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            MainControl.CloseUserControl();
        }

        public void CheckRobotAndPLC()
        {

        }


    }
}