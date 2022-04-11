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
    public class HomeViewModel : BaseViewModel
    {
        private readonly IBluetoothService _blueToothService;

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

        private async void OfBtHandling()
        {
            bool answer = await Application.Current.MainPage.DisplayAlert("Nemáte zapnuté bluetooth!", "Chcete zapnúť bluetooth?", "ÁNO", "NIE");
            if (answer)
            {
                _blueToothService.TurnOnBluetooth();
            }
        }
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

        public ICommand RefreshDeviceListCommand => new Command(async () =>
        {
            await GetBtDevices();
        });

        public ICommand GetDistanceCommad => new Command(async () =>
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

        private void SetPreferences()
        {
            Preferences.Set("lastRefreshedTimeKey", LastRefreshTime);
            Preferences.Set("waterLevelKey", WaterLevel);
        }

    }


}