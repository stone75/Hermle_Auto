using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;
using System.Windows.Input;

namespace Hermle_Auto.ViewModels
{
    public class AutomatViewModel : ObservableObject
    {
        public ICommand StartAutomatCommand { get; }

        public AutomatViewModel()
        {
			StartAutomatCommand = new RelayCommand<string?>(StartAutomat);
        }

        private void StartAutomat(string? param)
        {
            MessageBox.Show("Started Automat");
        }
    }

    
}
