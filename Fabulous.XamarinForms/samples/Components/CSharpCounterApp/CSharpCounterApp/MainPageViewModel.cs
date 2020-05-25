using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CSharpCounterApp
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private int _counter1;
        private int _counter2;

        public int Counter1
        {
            get => _counter1;
            set
            {
                _counter1 = value;
                OnPropertyChanged(nameof(Total));
            }
        }
        
        public int Counter2
        {
            get => _counter2;
            set
            {
                _counter2 = value;
                OnPropertyChanged(nameof(Total));
            }
        }
        
        public int Total => Counter1 + Counter2;
    }
}