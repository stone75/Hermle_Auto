using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;
using System.Windows.Input;

namespace Hermle_Auto.ViewModels
{
    public class TeachViewModel : ObservableObject
    {
        public ICommand TeachCommand { get; }

        public TeachViewModel()
        {
			TeachCommand = new RelayCommand<string?>(TeachAction);
        }

        private void TeachAction(string? param)
        {
            var message = "";
            if(param.StartsWith("_"))
            {
				message = param.Substring(1, param.Length - 1);
                message = $"Teach {message} Action!";
            }
            else
            {
				message = $"{param} Action!";
			}
            MessageBox.Show(message);
        }
    }

    
}
