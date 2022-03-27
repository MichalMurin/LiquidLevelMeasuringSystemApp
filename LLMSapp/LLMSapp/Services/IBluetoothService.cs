using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LLMSapp.Services
{
    public interface IBluetoothService
    {
        Task<bool> Connect(string deviceName);

        bool IsConnected();
        IList<string> GetDeviceList();
        //Task Send(string text);
        Task<bool> Send(char text);
        Task<string> Read(int count);

        bool IsBluetoothOn();
        bool TurnOnBluetooth();
        bool TurnOfBluetooth();
    }

}
