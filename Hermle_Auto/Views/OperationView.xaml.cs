﻿using Hermle_Auto.Comm;
using HermleCS.Comm;
using HermleCS.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Net.WebRequestMethods;

namespace Hermle_Auto.Views
{
    /// <summary>
    /// OperationView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class OperationView : UserControl
    {
        public event RobotStatusLogger logger;

        public OperationView()
        {
            InitializeComponent();
            setMode(2);
        }

        private void btnParkingPosition_Click(object sender, RoutedEventArgs e)
        {
            logger?.Invoke("Parking Position Button Clicked...");

            //D d = D.Instance;
            //CommHTTPComponent http = CommHTTPComponent.Instance;

            // 1. TP 파일 이름 셋팅
            D.Instance.CURRENT_JOBNAME = "PARKING";

            ///*
            // * 일단 Job 이름만 셋팅 후, Resume 에서 실제 실행 예정.
            string res;

            try
            {
                D.Instance.CURRENT_JOBNAME = "PARKING";
                string url = C.ROBOT_SERVER + "/H_COMMAND?task_str=" + D.Instance.CURRENT_JOBNAME;
                logger?.Invoke($"{url}");
                C.log(url);
                res = CommHTTPComponent.Instance.GetAPI(url);

                HTTPResponse httpresponse = JsonSerializer.Deserialize<HTTPResponse>(res);
                if (httpresponse.result != 0)
                {
                    logger?.Invoke($"Result : {httpresponse.msg}");
                    MessageBox.Show("Command Error : " + httpresponse.msg);
                    return;
                }
            }
            catch (Exception ex)
            {
                logger?.Invoke($"Exception : {ex.Message}");
                MessageBox.Show("Robot HTTP Communication Exception : " + ex.Message);
            }
            //*/
        }
        private void btnExchangeGripperPosition_Click(object sender, RoutedEventArgs e)
        {

            logger?.Invoke("Exchange Gripper Position Button Clicked...");

            D.Instance.CURRENT_JOBNAME = "EXCHANGE_GRIPPER";

            ///*
            // * 일단 Job 이름만 셋팅 후, Resume 에서 실제 실행 예정.
            string res;

            try
            {
                D.Instance.CURRENT_JOBNAME = "EXCHANGE_GRIPPER";
                string url = C.ROBOT_SERVER + "/H_COMMAND?task_str=" + D.Instance.CURRENT_JOBNAME;
                logger?.Invoke($"{url}");
                C.log(url);
                res = CommHTTPComponent.Instance.GetAPI(url);

                HTTPResponse httpresponse = JsonSerializer.Deserialize<HTTPResponse>(res);
                if (httpresponse.result != 0)
                {
                    logger?.Invoke($"Result : {httpresponse.msg}");
                    MessageBox.Show("Command Error : " + httpresponse.msg);
                    return;
                }
            }
            catch (Exception ex)
            {
                logger?.Invoke($"Exception : {ex.Message}");
                MessageBox.Show("Robot HTTP Communication Exception : " + ex.Message);
            }
            //*/
        }
        private void btnRetractPosition_Click(object sender, RoutedEventArgs e)
        {
            logger?.Invoke("Retract Position Button Clicked...");

            //D d = D.Instance;
            //CommHTTPComponent http = CommHTTPComponent.Instance;

            // 1. TP 파일 이름 셋팅
            //d.CURRENT_JOBNAME = "CURRENT_RETRACT";
            D.Instance.CURRENT_JOBNAME = "CURRENT_RETRACT";

            ///*
            // * 일단 Job 이름만 셋팅 후, Resume 에서 실제 실행 예정.
            string res;
            try
            {
                D.Instance.CURRENT_JOBNAME = "CURRENT_RETRACT";
                string url = C.ROBOT_SERVER + "/H_COMMAND?task_str=" + D.Instance.CURRENT_JOBNAME;
                logger?.Invoke($"{url}");
                C.log(url);
                res = CommHTTPComponent.Instance.GetAPI(url);

                HTTPResponse httpresponse = JsonSerializer.Deserialize<HTTPResponse>(res);
                if (httpresponse.result != 0)
                {
                    logger?.Invoke($"Result : {httpresponse.msg}");
                    MessageBox.Show("Command Error : " + httpresponse.msg);
                    return;
                }
            }
            catch (Exception ex)
            {
                logger?.Invoke($"Result : {ex.Message}");
                MessageBox.Show("Robot HTTP Communication Exception : " + ex.Message);
            }
            //*/
        }

        private void Operaion_KioskValve_Open_MouseDown(object sender, MouseButtonEventArgs e)
        {
            logger?.Invoke("Kiosk Valve Open Button Down...");
            try
            {
                logger?.Invoke("[PLC] : Kiosk Valve Open Button Down...");
                CommPLC.Instance.WritePLC(McProtocol.Mitsubishi.PlcDeviceType.M, 2026, 1);
            }
            catch (Exception ex)
            {
                logger?.Invoke("예외상황 : " + ex.Message);
            }

        }

        private void Operaion_KioskValve_Open_MouseUp(object sender, MouseButtonEventArgs e)
        {
            logger?.Invoke("Kiosk Valve Open Button Up...");
            try
            {
                logger?.Invoke("[PLC] : Kiosk Valve Open Button Up...");
                CommPLC.Instance.WritePLC(McProtocol.Mitsubishi.PlcDeviceType.M, 2026, 0);
            }
            catch (Exception ex)
            {
                logger?.Invoke("예외상황 : " + ex.Message);
            }
        }

        private void Operaion_KioskValve_Close_MouseDown(object sender, MouseButtonEventArgs e)
        {
            logger?.Invoke("Kiosk Valve Close Button Down...");
            try
            {
                logger?.Invoke("[PLC] : Kiosk Valve Close Button Down...");
                CommPLC.Instance.WritePLC(McProtocol.Mitsubishi.PlcDeviceType.M, 2027, 1);
            }
            catch (Exception ex)
            {
                logger?.Invoke("예외상황 : " + ex.Message);
            }
        }

        private void Operaion_KioskValve_Close_MouseUp(object sender, MouseButtonEventArgs e)
        {
            logger?.Invoke("Kiosk Valve Open Button Up...");
            try
            {
                logger?.Invoke("[PLC] : Kiosk Valve Open Button Up...");
                CommPLC.Instance.WritePLC(McProtocol.Mitsubishi.PlcDeviceType.M, 2027, 0);
            }
            catch (Exception ex)
            {
                logger?.Invoke("예외상황 : " + ex.Message);
            }
        }

        public void setMode(int mode)
        {
            btnParkingPosition.IsEnabled = false;
            btnExchangeGripperPosition.IsEnabled = false;
            btnRetractPosition.IsEnabled = false;
            if (mode == 0)
            {
                btnParkingPosition.IsEnabled = true;
                btnExchangeGripperPosition.IsEnabled = true;
                btnRetractPosition.IsEnabled = true;
            }
            Operaion_KioskValve_Open.IsEnabled = false;
            Operaion_KioskValve_Close.IsEnabled = false;
            Operation_Indicator_Off.IsEnabled = false;
            Operation_Indicator_On.IsEnabled = false;
            Operation_Gripper_Open.IsEnabled = false;
            Operation_Gripper_Close.IsEnabled = false;
            Operation_CellLight_Off.IsEnabled = false;
            Operation_CellLight_On.IsEnabled = false;
            Operation_DoorInterlock_Off.IsEnabled = false;
            Operation_DoorInterlock_On.IsEnabled = false;
            Operation_InterlockHerlme_Off.IsEnabled = false;
            Operation_InterlockHerlme_On.IsEnabled = false;
            Operation_Gripper2_Open.IsEnabled = false;
            Operation_Gripper2_Close.IsEnabled = false;
            if (mode == 1)
            {
                Operaion_KioskValve_Open.IsEnabled = true;
                Operaion_KioskValve_Close.IsEnabled = true;
                Operation_Indicator_Off.IsEnabled = true;
                Operation_Indicator_On.IsEnabled = true;
                Operation_Gripper_Open.IsEnabled = true;
                Operation_Gripper_Close.IsEnabled = true;
                Operation_CellLight_Off.IsEnabled = true;
                Operation_CellLight_On.IsEnabled = true;
                Operation_DoorInterlock_Off.IsEnabled = true;
                Operation_DoorInterlock_On.IsEnabled = true;
                Operation_InterlockHerlme_Off.IsEnabled = true;
                Operation_InterlockHerlme_On.IsEnabled = true;
                Operation_Gripper2_Open.IsEnabled = true;
                Operation_Gripper2_Close.IsEnabled = true;
            }
        }
    }
}