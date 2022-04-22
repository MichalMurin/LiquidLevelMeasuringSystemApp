using LLMSapp.Views;
using LLMSapp.Services;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Essentials;

namespace LLMSapp.ViewModels
{
    /// <summary>
    /// trieda pre spravu karty nastavenia
    /// </summary>
    public class SettingsViewModel : BaseViewModel
    {
        /// <summary>
        /// instancia rozhrania IBluetoothService pre spravu bluetooth
        /// </summary>
        private readonly IBluetoothService _blueToothService;

        /// <summary>
        /// prikaz na ulozenie nastaveni
        /// </summary>
        public Command SaveCommand { get; }

        /// <summary>
        /// prikaz na ulozenie nastaveneho telefonneho cisla
        /// </summary>
        public Command SavePhoneNumberCommand { get; }

        /// <summary>
        /// prikaz na ulozenie nastavenej vzdialenosti
        /// </summary>
        public Command SetDistanceCommand { get; }

        /// <summary>
        /// prikaz na zobrazenie informacii o moznosti SMS
        /// </summary>
        public Command ShowSMSInfoCommand { get; }

        /// <summary>
        /// prikaz na zobrazenie informacii o moznosti Buzzer
        /// </summary>
        public Command ShowBuzzerInfoCommand { get; }

        /// <summary>
        /// prikaz na zobrazenie informacii o moznosti LED
        /// </summary>
        public Command ShowLedInfoCommand { get; }

        /// <summary>
        /// prikaz na zobrazenie informacii o vzdialenosti
        /// </summary>
        public Command ShowDistanceInfoCommand { get; }

        /// <summary>
        /// prikaz na zobrazenie informacii o moznosti Externeho alarmu
        /// </summary>
        public Command ShowExternAlarmCommand { get; }

        /// <summary>
        /// premenna pre ukladanie informacie o povoleni SMS
        /// </summary>
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

        /// <summary>
        /// premenna pre ukladanie informacie o povoleni Buzzera
        /// </summary>
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

        /// <summary>
        /// premenna pre ukladanie informacie o povoleni LED
        /// </summary>
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

        /// <summary>
        /// premenna pre ukladanie informacie o povoleni externeho alarmu
        /// </summary>
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

        /// <summary>
        /// premenna pre ukladanie telefonneho cisla
        /// </summary>
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

        /// <summary>
        /// premenna pre ukladanie nastavenej vzdialenosti
        /// </summary>
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

        /// <summary>
        /// Konstruktor triedy
        /// </summary>
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

        /// <summary>
        /// metoda na ulozenie nastaveni
        /// </summary>
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

        /// <summary>
        /// metoda na ulozenie telefonneho cisla
        /// </summary>
        private async void OnSaveNumber()
        {
            if (PhoneNumber.Length != 10 || PhoneNumber[0] != '0')
            {
                await Application.Current.MainPage.DisplayAlert("Chyba", "Telefónne číslo má zlý formát, zadajte číslo vo formáte 09........", "OK");
                PhoneNumber = string.Empty;
            }
        }

        /// <summary>
        /// metoda na nastavenie vzdialenosti
        /// </summary>
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

        /// <summary>
        /// metoda pre ukazanie informacii o SMS
        /// </summary>
        private async void OnShowSMSInfo()
        {
            await Application.Current.MainPage.DisplayAlert("Info", "Pri prekročení nastavenej hladiny v nádobe sa odošle SMS správa na zadané telefónne číslo", "OK");
        }

        /// <summary>
        /// metoda pre ukazanie informacii o LED
        /// </summary>
        private async void OnShowLedInfo()
        {
            await Application.Current.MainPage.DisplayAlert("Info", "Pri prekročení nastavenej hladiny v nádobe sa bude blikať signalizačná LED", "OK");
        }

        /// <summary>
        /// metoda pre ukazanie informacii o Buzzri
        /// </summary>
        private async void OnShowBuzzerInfo()
        {
            await Application.Current.MainPage.DisplayAlert("Info", "Pri prekročení nastavenej hladiny v nádobe sa spustí varovný alarm", "OK");
        }

        /// <summary>
        /// metoda pre ukazanie informacii o vzdialenosti 
        /// </summary>
        private async void OnShowDistanceInfo()
        {
            await Application.Current.MainPage.DisplayAlert("Info", "Pri prekročení definovanej vzdialenosti sa v zariadení spustí alarmový stav" +
                " - zariadenie spustí vodné čerpadlo a pokúsi sa hladinu znížiť" +
                " [ Hraničná vzdialenosť musí byť v intervale <10, 400> ] ", "OK");
        }

        /// <summary>
        /// metoda pre ukazanie informacii o externom alarme
        /// </summary>
        private async void OnShowExternAlarmInfo()
        {
            await Application.Current.MainPage.DisplayAlert("Info", "Pri prekročení nastavenej hladiny v nádobe sa zopne externý alarm - zariadenie pripojené k relé 2", "OK");
        }

        /// <summary>
        /// metoda na nastavenie preferencii aplikacie
        /// </summary>
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