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
        BluetoothAdapter bluetoothAdapter = BluetoothAdapter.DefaultAdapter;

        BluetoothSocket bluetoothSocket = null;

        private BluetoothManager _manager = (BluetoothManager)Android.App.Application.Context.GetSystemService(Android.Content.Context.BluetoothService);

        private BluetoothDevice device = null;

        public bool IsConnected()
        {
            if (bluetoothSocket != null)
            {
                return bluetoothSocket.IsConnected;
            }
            return false;            
        }

        public async Task<bool> Connect(string deviceName)
        {
            if (deviceName != null)
            {
                device = (from bd in bluetoothAdapter?.BondedDevices
                                 where bd?.Name == deviceName
                                 select bd).FirstOrDefault();
                try
                {
                    await Task.Delay(1000);
                    bluetoothSocket = device?.
                        CreateRfcommSocketToServiceRecord(
                        UUID.FromString("00001101-0000-1000-8000-00805f9b34fb"));

                    bluetoothSocket?.Connect();
                }
                catch (Exception exp)
                {
                    Debug.WriteLine(exp.Message);
                    return false;
                }

                if (bluetoothSocket != null)
                {
                    if (bluetoothSocket.IsConnected)
                    {
                        return true;
                    }
                }
            }
            return false;
            
        }

        
        public IList<string> GetDeviceList()
        {
            var btdevice = bluetoothAdapter?.BondedDevices.Select(i => i.Name).ToList();
            return btdevice;
        }

        //public async Task Send(string text)
        //{
        //    try
        //    {
        //        byte[] buffer = Encoding.UTF8.GetBytes(text);
        //        bluetoothSocket?.OutputStream.Write(buffer, 0, buffer.Length);
        //    }
        //    catch (Exception exp)
        //    {
        //        Debug.WriteLine(exp.Message);
        //        throw exp;
        //    }
        //}

        public async Task<bool> Send(char text)
        {
            try
            {
                char[] str = { text };
                byte[] buffer = Encoding.UTF8.GetBytes(str);
                bluetoothSocket?.OutputStream.Write(buffer, 0, buffer.Length);
                return true;
            }
            catch (Exception exp)
            {
                Debug.WriteLine(exp.Message);
                bluetoothSocket.Close();
                return false;
            }
        }

        public async Task<string> Read(int count)
        {
            //BluetoothDevice device = (from bd in bluetoothAdapter?.BondedDevices
            //                          where bd?.Name == deviceName
            //                          select bd).FirstOrDefault();
            try
            {
                //await Task.Delay(1000);
                //BluetoothSocket bluetoothSocket = device?.
                //    CreateRfcommSocketToServiceRecord(
                //    UUID.FromString("00001101-0000-1000-8000-00805f9b34fb"));

                //bluetoothSocket?.Connect();
                byte[] buffer = new byte[count];
                bluetoothSocket?.InputStream.Read(buffer, 0, count);
                //bluetoothSocket.Close();
                string ret = System.Text.Encoding.Default.GetString(buffer);

                return ret;
            }
            catch (Exception exp)
            {
                Debug.WriteLine(exp.Message);
                bluetoothSocket.Close();
                return null;
            }
        }

        public bool IsBluetoothOn()
        {
            return _manager.Adapter.IsEnabled;
        }

        public bool TurnOnBluetooth()
        {
           return _manager.Adapter.Enable();
        }

        public bool TurnOfBluetooth()
        {
            return _manager.Adapter.Disable();
        }
    }

}

