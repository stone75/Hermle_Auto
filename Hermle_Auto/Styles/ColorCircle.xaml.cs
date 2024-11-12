﻿using System;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Hermle_Auto
{
    /// <summary>
    /// ColorCircle.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ColorCircle : UserControl
    {
        private bool isGreen = true;
        private Storyboard toRedAnimation;
        private Storyboard toGreenAnimation;

        public ColorCircle()
        {
            InitializeComponent();

            toRedAnimation = (Storyboard)FindResource("ToRedAnimation");
            toGreenAnimation = (Storyboard)FindResource("ToGreenAnimation");

            this.MouseDown += ColorCircle_MouseDown;
        }

        private void ColorCircle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (isGreen)
            {
                toRedAnimation.Begin();
            }
            else
            {
                toGreenAnimation.Begin();
            }

            isGreen = !isGreen;
        }
    }
}
