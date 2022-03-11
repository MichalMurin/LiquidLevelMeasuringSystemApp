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
        Task Send(string deviceName, string text);
        Task<string> Read(string deviceName);
    }

}
