﻿using LLMSapp.Views;
using LLMSapp.Services;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Essentials;

namespace LLMSapp.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        private readonly IBluetoothService _blueToothService;

        public Command SaveCommand { get; }
        public Command SavePhoneNumberCommand { get; }
        public Command SetDistanceCommand { get; }
        public Command ShowSMSInfoCommand { get; }
        public Command ShowBuzzerInfoCommand { get; }
        public Command ShowLedInfoCommand { get; }
        public Command ShowDistanceInfoCommand { get; }
        public Command ShowExternAlarmCommand { get; }

        private bool _isSmsEnabled;
        public bool IsSMSEnabled
        {
            get
            {
                return _isSmsEnabled;
            }
            set
            {
                _isSmsEnabled = value;
                OnPropertyChanged(nameof(IsSMSEnabled));
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

        private bool _isExternAlarmEnabled;
        public bool IsExternAlarmEnabled
        {
            get
            {
                return _isExternAlarmEnabled;
            }
            set
            {
                _isExternAlarmEnabled = value;
                OnPropertyChanged(nameof(IsExternAlarmEnabled));
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

        public SettingsViewModel()
        {
            _blueToothService = DependencyService.Get<IBluetoothService>();
            Title = "Nastavenia";
            PhoneNumber = Preferences.Get("phoneNuberKey", string.Empty);
            IsSMSEnabled = Preferences.Get("isSmsEnabledKey", false);
            IsBuzzerEnabled = Preferences.Get("isBuzzEnabledKey", false);
            IsLedEnabled = Preferences.Get("isLedEnabledKey", false);
            IsExternAlarmEnabled = Preferences.Get("isExternAlarmKey", false);
            BorderDistance = Preferences.Get("borderDstKey", "30");
            SaveCommand = new Command(OnSave);
            SavePhoneNumberCommand = new Command(OnSaveNumber);
            SetDistanceCommand = new Command(OnSetDistance);
            ShowSMSInfoCommand = new Command(OnShowSMSInfo);
            ShowBuzzerInfoCommand = new Command(OnShowBuzzerInfo);
            ShowLedInfoCommand = new Command(OnShowLedInfo);
            ShowDistanceInfoCommand = new Command(OnShowDistanceInfo);
            ShowExternAlarmCommand = new Command(OnShowExternAlarmInfo);
        }

        private async void OnSave()
        {
            if (_blueToothService.IsConnected())
            {
                if (IsSMSEnabled && PhoneNumber.Length == 0)
                {
                    await Application.Current.MainPage.DisplayAlert("Chyba", "Telefónne číslo má zlý formát", "OK");
                    return;
                }
                if (BorderDistance.Length == 0)
                {
                    await Application.Current.MainPage.DisplayAlert("Upozornenie", "Vyplnte prosím všetky údaje", "OK");
                    return;
                }
                _blueToothService.Send('s');
                string settings = "";
                if (IsSMSEnabled)
                {
                    settings += '1';
                    settings += PhoneNumber + "-";
                }
                else
                {
                    settings += '0';
                }
                if (IsBuzzerEnabled)
                {
                    settings += '1';
                }
                else
                {
                    settings += '0';
                }
                if (IsLedEnabled)
                {
                    settings += '1';
                }
                else
                {
                    settings += '0';
                }
                if (IsExternAlarmEnabled)
                {
                    settings += '1';
                }
                else
                {
                    settings += '0';
                }
                settings += BorderDistance.PadLeft(3, '0') + '\n';
                bool control = false;
                foreach (var c in settings)
                {
                    control = _blueToothService.Send(c);
                    if (!control)
                    {
                        await Application.Current.MainPage.DisplayAlert("Upozornenie", "Zariadenie nie je pripojené", "OK");
                        HomePage.Instance.ConnectionStatus = "Pripojiť";
                        return;
                    }
                }
                SavePreferences();
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Upozornenie", "Zariadenie nie je pripojené", "OK");
            }
        }

        private async void OnSaveNumber()
        {
            if (PhoneNumber.Length != 10 || PhoneNumber[0] != '0')
            {
                await Application.Current.MainPage.DisplayAlert("Chyba", "Telefónne číslo má zlý formát, zadajte číslo vo formáte 09........", "OK");
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

        private async void OnShowExternAlarmInfo()
        {
            await Application.Current.MainPage.DisplayAlert("Info", "Pri prekročení nastavenej hladiny v nádobe sa zopne externý alarm - zariadenie pripojené k relé 2", "OK");
        }

        private void SavePreferences()
        {
            Preferences.Set("phoneNuberKey", PhoneNumber);
            Preferences.Set("isSmsEnabledKey", IsSMSEnabled);
            Preferences.Set("isBuzzEnabledKey", IsBuzzerEnabled);
            Preferences.Set("isLedEnabledKey", IsLedEnabled);
            Preferences.Set("borderDstKey", BorderDistance);
            Preferences.Set("isExternAlarmKey", IsExternAlarmEnabled);
        }
    }
}