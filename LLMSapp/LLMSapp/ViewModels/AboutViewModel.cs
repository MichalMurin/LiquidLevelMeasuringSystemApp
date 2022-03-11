using System;
using LLMSapp.Models;
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

        private string _inputText;
        public string InputText
        {
            get
            {
                return _inputText;
            }
            set
            {
                _inputText = value;
                OnPropertyChanged(nameof(InputText));
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

        private string _sendMessage;
        public string SendMessage
        {
            get
            {
                return _sendMessage;
            }
            set
            {
                _sendMessage = value;
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

        public ICommand ReadCommand => new Command(async () =>
        {
            InputText = await _blueToothService.Read(SelectedDevice);
        });

        public ICommand SendCommand => new Command(async () =>
        {
            await _blueToothService.Send(SelectedDevice, SendMessage);
        });

        public ICommand RedCommand => new Command(async () =>
        {
            await _blueToothService.Send(SelectedDevice, "r");
        });

        public ICommand GreenCommand => new Command(async () =>
        {
            await _blueToothService.Send(SelectedDevice, "g");
        });

        public ICommand BlueCommand => new Command(async () =>
        {
            await _blueToothService.Send(SelectedDevice, "b");
        });

        public ICommand BuzzCommand => new Command(async () =>
        {
            await _blueToothService.Send(SelectedDevice, "1");
        });

        public AboutViewModel()
        {
            Title = "TestPage";
            _blueToothService = DependencyService.Get<IBluetoothService>();
            InputText = "Start of program";
            var list = _blueToothService.GetDeviceList();
            DeviceList.Clear();
            foreach (var item in list)
            {
                DeviceList.Add(item);
            }
        }
    }

}