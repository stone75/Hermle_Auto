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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Hermle_Auto
{
    /// <summary>
    /// ToggleButton.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ToggleButton : UserControl
    {
        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register("IsChecked", typeof(bool), typeof(ToggleButton),
                new PropertyMetadata(false, OnIsCheckedChanged));

        public bool IsChecked
        {
            get { return (bool)GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }

        public ToggleButton()
        {
            InitializeComponent();
            this.MouseDown += ToggleButton_MouseDown;
        }

        private void ToggleButton_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            IsChecked = !IsChecked;
        }

        private static void OnIsCheckedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (ToggleButton)d;
            control.AnimateSwitch();
        }


        private void AnimateSwitch()
        {
            // Calculate the target margin
            double targetLeftMargin = IsChecked ? this.ActualWidth - SwitchKnob.ActualWidth - 4 : 2;

            // Get the current margin
            var currentMargin = SwitchKnob.Margin;

            // Create the new Thickness with the target position
            var targetMargin = new Thickness(targetLeftMargin, currentMargin.Top, currentMargin.Right, currentMargin.Bottom);

            // Create a ThicknessAnimation
            var animation = new ThicknessAnimation
            {
                To = targetMargin,
                Duration = TimeSpan.FromSeconds(0.2),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut } // Optional for smooth transition
            };

            // Apply the animation to the Margin property
            SwitchKnob.BeginAnimation(FrameworkElement.MarginProperty, animation);
        }

     /*   private void AnimateSwitch()
        {
            double targetPosition = IsChecked ? this.ActualWidth - SwitchKnob.ActualWidth - 4 : 2;

            var animation = new DoubleAnimation
            {
                To = targetPosition,
                Duration = TimeSpan.FromSeconds(0.2)
            };

            SwitchKnob.BeginAnimation(MarginProperty, animation);
        }*/
    }
}
