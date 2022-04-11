using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.Bluetooth;
using Java.Util;
using LLMSapp.Droid;
using LLMSapp.Services;

[assembly: Xamarin.Forms.Dependency(typeof(AndroidBlueToothService))]

namespace LLMSapp.Droid
{
    public class AndroidBlueToothService : IBluetoothService
    {
        BluetoothAdapter bluetoothAdapter_ = BluetoothAdapter.DefaultAdapter;

        BluetoothSocket bluetoothSocket_ = null;

        private BluetoothManager manager_ = (BluetoothManager)Android.App.Application.Context.GetSystemService(Android.Content.Context.BluetoothService);

        private BluetoothDevice device_ = null;


        public bool IsConnected()
        {
            if (bluetoothSocket_ != null)
            {
                return bluetoothSocket_.IsConnected;
            }
            return false;            
        }

        public async Task<bool> Connect(string deviceName)
        {
            if (deviceName != null)
            {
                device_ = (from bd in bluetoothAdapter_?.BondedDevices
                                 where bd?.Name == deviceName
                                 select bd).FirstOrDefault();
                try
                {
                    await Task.Delay(1000);
                    bluetoothSocket_ = device_?.
                        CreateRfcommSocketToServiceRecord(
                        UUID.FromString("00001101-0000-1000-8000-00805f9b34fb"));

                    bluetoothSocket_?.Connect();
                }
                catch (Exception exp)
                {
                    Debug.WriteLine(exp.Message);
                    return false;
                }

                if (bluetoothSocket_ != null)
                {
                    if (bluetoothSocket_.IsConnected)
                    {
                        return true;
                    }
                }
            }
            return false;            
        }

        
        public IList<string> GetDeviceList()
        {
            var btdevice = bluetoothAdapter_?.BondedDevices.Select(i => i.Name).ToList();
            return btdevice;
        }

        public bool Send(char character)
        {
            try
            {
                char[] chars = { character };
                byte[] buffer = Encoding.UTF8.GetBytes(chars);
                bluetoothSocket_?.OutputStream.Write(buffer, 0, buffer.Length);
                return true;
            }
            catch (Exception exp)
            {
                Debug.WriteLine(exp.Message);
                bluetoothSocket_.Close();
                return false;
            }
        }

        public string Read(int count)
        {
            try
            {
                byte[] buffer = new byte[count];
                bluetoothSocket_?.InputStream.Read(buffer, 0, count);
                string ret = System.Text.Encoding.Default.GetString(buffer);
                return ret;
            }
            catch (Exception exp)
            {
                Debug.WriteLine(exp.Message);
                bluetoothSocket_.Close();
                return null;
            }
        }

        public bool IsBluetoothOn()
        {
            return manager_.Adapter.IsEnabled;
        }

        public bool TurnOnBluetooth()
        {
           return manager_.Adapter.Enable();
        }

        public bool TurnOfBluetooth()
        {
            return manager_.Adapter.Disable();
        }
    }

}

