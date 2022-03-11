using System;
using LLMSapp.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Xamarin.Forms;

namespace LLMSapp.ViewModels
{
    public class AboutViewModel : BaseViewModel
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

        public AboutViewModel()
        {
            ConnectionStatus = "Pripojiť";
            WaterLevel = "-- cm";
            Title = "Domov";
            _blueToothService = DependencyService.Get<IBluetoothService>();
            var list = _blueToothService.GetDeviceList();
            DeviceList.Clear();
            foreach (var item in list)
            {
                DeviceList.Add(item);
            }
        }
        public ICommand ConnectCommand => new Command(async () =>
        {
            if (_blueToothService.IsConnected())
            {
                await Application.Current.MainPage.DisplayAlert("Upozornenie", "Zariadenie je uz pripojene", "Ok");
            }
            else
            {
                if (await _blueToothService.Connect(SelectedDevice))
                {
                    ConnectionStatus = "Pripojené";
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Upozornenie", "Zariadenie sa nepodarilo pripojit, uistite sa ze je zariadenie zapnute", "Ok");
                }
            }            
        });  
    }

}