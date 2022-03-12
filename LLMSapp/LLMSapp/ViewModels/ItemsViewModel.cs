using LLMSapp.Models;
using LLMSapp.Views;
using LLMSapp.Services;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace LLMSapp.ViewModels
{
    public class ItemsViewModel : BaseViewModel
    {
        private readonly IBluetoothService _blueToothService;
        public Command SaveCommand { get; }
        public Command SavePhoneNumberCommand { get; }
        public Command SetDistanceCommand { get; }
        public Command ShowSMSInfoCommand { get; }
        public Command ShowBuzzerInfoCommand { get; }
        public Command ShowLedInfoCommand { get; }
        public Command ShowDistanceInfoCommand { get; }

        private bool _isCallEnabled;
        public bool IsCallEnabled
        {
            get
            {
                return _isCallEnabled;
            }
            set
            {
                _isCallEnabled = value;
                OnPropertyChanged(nameof(IsCallEnabled));
            }
        }

        private bool _isBuzzerEnabled;
        public bool IsBuzzerEnabled
        {
            get
            {
                return _isBuzzerEnabled;
            }
            set
            {
                _isBuzzerEnabled = value;
                OnPropertyChanged(nameof(IsBuzzerEnabled));
            }
        }

        private bool _isLedEnabled;
        public bool IsLedEnabled
        {
            get
            {
                return _isLedEnabled;
            }
            set
            {
                _isLedEnabled = value;
                OnPropertyChanged(nameof(IsLedEnabled));
            }
        }

        private string _phoneNumber;
        public string PhoneNumber
        {
            get
            {
                return _phoneNumber;
            }
            set
            {
                _phoneNumber = value;
                OnPropertyChanged(nameof(PhoneNumber));
            }
        }

        private string _borderDistance;
        public string BorderDistance
        {
            get
            {
                return _borderDistance;
            }
            set
            {
                _borderDistance = value;
                OnPropertyChanged(nameof(BorderDistance));
            }
        }

        public ItemsViewModel()
        {
            _blueToothService = DependencyService.Get<IBluetoothService>();
            Title = "Nastavenia";
            IsCallEnabled = false;
            IsBuzzerEnabled = false;
            IsLedEnabled = false;
            BorderDistance = "20";
            SaveCommand = new Command(OnSave);
            SavePhoneNumberCommand = new Command(OnSaveNumber);
            SetDistanceCommand = new Command(OnSetDistance);
            ShowSMSInfoCommand = new Command(OnShowSMSInfo);
            ShowBuzzerInfoCommand = new Command(OnShowBuzzerInfo);
            ShowLedInfoCommand = new Command(OnShowLedInfo);
            ShowDistanceInfoCommand = new Command(OnShowDistanceInfo);
    }
        
        private async void OnSave()
        {
            ///TODO
            /// skontrolovat vstup a poslat do zariadenia
            await _blueToothService.Send("1");
        }

        private async void OnSaveNumber()
        {
            if (PhoneNumber.Length > 13 || PhoneNumber.Length < 10)
            {
                await Application.Current.MainPage.DisplayAlert("Chyba", "Telefónne číslo má zlý formát", "OK");
                PhoneNumber = string.Empty;
            }
        }

        private async void OnSetDistance()
        {
            int dst;
            Int32.TryParse(BorderDistance, out dst);
            if (dst < 10)
            {
                await Application.Current.MainPage.DisplayAlert("Chyba", "Minimálna možná hodnota je 10 cm", "OK");
                BorderDistance = "10";
            }
            if (dst > 400)
            {
                await Application.Current.MainPage.DisplayAlert("Chyba", "Maximálna možná hodnota je 400 cm", "OK");
                BorderDistance = "400";
            }
        }

        private async void OnShowSMSInfo()
        {
            await Application.Current.MainPage.DisplayAlert("Info", "Pri prekročení nastavenej hladiny v nádobe sa odošle SMS správa na zadané telefónne číslo", "OK");
        }
        private async void OnShowLedInfo()
        {
            await Application.Current.MainPage.DisplayAlert("Info", "Pri prekročení nastavenej hladiny v nádobe sa bude blikať signalizačná LED", "OK");
        }

        private async void OnShowBuzzerInfo()
        {
            await Application.Current.MainPage.DisplayAlert("Info", "Pri prekročení nastavenej hladiny v nádobe sa spustí varovný alarm", "OK");
        }

        private async void OnShowDistanceInfo()
        {
            await Application.Current.MainPage.DisplayAlert("Info", "Pri prekročení definovanej vzdialenosti sa v zariadení spustí alarmový stav" +
                " - zariadenie spustí vodné čerpadlo a pokúsi sa hladinu znížiť" +
                " [ Hraničná vzdialenosť musí byť v intervale <10, 400> ] ", "OK");
        }
    }
}