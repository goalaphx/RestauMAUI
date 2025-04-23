using CommunityToolkit.Mvvm.ComponentModel;

namespace Restaurant.ViewModels;

// Using ObservableObject from the toolkit handles INotifyPropertyChanged
public partial class BaseViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotBusy))] // Update IsNotBusy when IsBusy changes
    bool _isBusy;

    [ObservableProperty]
    string _title;

    public bool IsNotBusy => !IsBusy;
}