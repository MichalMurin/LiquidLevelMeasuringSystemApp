using System;
using LLMSapp.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Xamarin.Forms;
using System.Threading.Tasks;
using System.Globalization;
using Xamarin.Essentials;
namespace LLMSapp.ViewModels
{
    /// <summary>
    /// Trieda pre spravu domovskej obrazovky
    /// </summary>
    public class HomeViewModel : BaseViewModel
    {
        /// <summary>
        /// instancia rozhrania IBluetoothService pre spravu bluetooth
        /// </summary>
        private readonly IBluetoothService _blueToothService;

        /// <summary>
        /// Vzdialenost hladiny od senzora
        /// </summary>
        private string _waterLevel;
        public string WaterLevel
        {
            get
            {
                return _waterLevel;
            }
            set
            {
                _waterLevel = value;
                OnPropertyChanged(nameof(WaterLevel));
            }
        }

        /// <summary>
        /// casovy udaj o poslednom nacitavani hladiny
        /// </summary>
        private string _lastRefreshTime;
        public string LastRefreshTime
        {
            get
            {
                return _lastRefreshTime;
            }
            set
            {
                _lastRefreshTime = value;
                OnPropertyChanged(nameof(LastRefreshTime));
            }
        }

        /// <summary>
        /// status pripojenia Bluetooth
        /// </summary>
        private string _connectionStatus;
        public string ConnectionStatus
        {
            get
            {
                return _connectionStatus;
            }
            set
            {
                _connectionStatus = value;
                OnPropertyChanged(nameof(ConnectionStatus));
            }
        }

        /// <summary>
        /// zoznam sparovanych zariadeni
        /// </summary>
        private IList<string> _deviceList;
        public IList<string> DeviceList
        {
            get
            {
                if (_deviceList == null)
                    _deviceList = new ObservableCollection<string>();
                return _deviceList;
            }

            set
            {
                _deviceList = value;
            }
        }

        /// <summary>
        /// zvolene zariadenie ku ktoremu sa pripaja
        /// </summary>
        private string _selectedDevice;
        public string SelectedDevice
        {
            get
            {
                return _selectedDevice;
            }
            set
            {
                _selectedDevice = value;
            }
        }

        /// <summary>
        ///Konstruktor triedy
        /// </summary>
        public HomeViewModel()
        {
            Title = "Domov";
            ConnectionStatus = "Pripojiť";
            WaterLevel = Preferences.Get("waterLevelKey", "   cm");
            LastRefreshTime = Preferences.Get("lastRefreshedTimeKey", string.Empty);
            _blueToothService = DependencyService.Get<IBluetoothService>();
            var list = _blueToothService.GetDeviceList();
            DeviceList.Clear();
            foreach (var item in list)
            {
                DeviceList.Add(item);
            }

        }

        /// <summary>
        /// metoda pre spravu vypnuteho BT
        /// </summary>
        private async void OfBtHandling()
        {
            bool answer = await Application.Current.MainPage.DisplayAlert("Nemáte zapnuté bluetooth!", "Chcete zapnúť bluetooth?", "ÁNO", "NIE");
            if (answer)
            {
                _blueToothService.TurnOnBluetooth();
            }
        }
        /// <summary>
        /// Prikaz pre pripojenie BT zariadenia
        /// </summary>
        public ICommand ConnectCommand => new Command(async () =>
        {
            if (_blueToothService.IsConnected() && _blueToothService.IsBluetoothOn())
            {
                await Application.Current.MainPage.DisplayAlert("Upozornenie", "Zariadenie je už pripojené", "OK");
            }
            else
            {
                ConnectionStatus = "Pripojiť";
                if (_blueToothService.IsBluetoothOn())
                {
                    if (SelectedDevice != null)
                    {
                        if (await _blueToothService.Connect(SelectedDevice))
                        {
                            ConnectionStatus = "Pripojené";
                        }
                        else
                        {
                            await Application.Current.MainPage.DisplayAlert("Upozornenie", "Zariadenie sa nepodarilo pripojiť, uistite sa, že je zariadenie zapnuté", "OK");
                        }
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("Upozornenie", "Vyberte zariadenie zo zoznamu!", "OK");
                    }
                }
                else
                {
                    OfBtHandling();
                }
                      
            }            
        });        

        /// <summary>
        /// metoda na nacitanie sparovanych zariadeni
        /// </summary>
        public ICommand RefreshDeviceListCommand => new Command(async () =>
        {
            await GetBtDevices();
        });

        /// <summary>
        /// prikaz na nacitanie vysky hladiny
        /// </summary>
        public ICommand GetDistanceCommand => new Command(async () =>
        {
            if (_blueToothService.IsConnected())
            {
                bool control = false;
                control = _blueToothService.Send('g');
                if (control)
                {
                    WaterLevel = _blueToothService.Read(3) + " cm";
                    WaterLevel = WaterLevel.TrimStart('0');
                    DateTime localDate = DateTime.Now;
                    var culture = new CultureInfo("sk-SK");
                    LastRefreshTime = localDate.ToString(culture);
                    SetPreferences();
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Upozornenie", "Zariadenie nie je pripojené", "OK");
                    ConnectionStatus = "Pripojiť";
                }
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Upozornenie", "Zariadenie nie je pripojené", "OK");
                ConnectionStatus = "Pripojiť";
            }
        });
        
        /// <summary>
        /// metoda na nacitanie Bluetooth zariadeni
        /// </summary>
        /// <returns></returns>
        private async Task GetBtDevices()
        {
            if (_blueToothService.IsBluetoothOn())
            {
                var list = _blueToothService.GetDeviceList();
                DeviceList.Clear();
                foreach (var item in list)
                {
                    DeviceList.Add(item);
                }
                if (DeviceList.Count == 0)
                {
                    await Application.Current.MainPage.DisplayAlert("Upozornenie", "Nepodarilo sa nájsť žiadne bluetooth zariadenia, uistite sa," +
                        " že máte zapnutý bluetooth a zariadenie je správne zapnuté a spárované", "OK");
                }
            }
            else
            {
                OfBtHandling();
            }
            
        }

        /// <summary>
        /// metoda na nastavenie preferencii aplikacie
        /// </summary>
        private void SetPreferences()
        {
            Preferences.Set("lastRefreshedTimeKey", LastRefreshTime);
            Preferences.Set("waterLevelKey", WaterLevel);
        }

    }


}