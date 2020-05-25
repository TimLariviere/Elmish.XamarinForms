using CSharpCounterApp.Components;
using Xamarin.Forms;

namespace CSharpCounterApp
{
    public class CounterView : FabulousCounterView
    {
        public static readonly BindableProperty ValueProperty = BindableProperty.Create("Value", typeof(int),
            typeof(CounterView), 0, defaultBindingMode: BindingMode.OneWayToSource);

        public override int Value
        {
            get => (int) GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }
    }
}