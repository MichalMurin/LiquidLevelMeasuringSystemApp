using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LLMSapp.Services
{
    public interface IBluetoothService
    {
        IList<string> GetDeviceList();
        Task Send(string deviceName, string text);
        Task<string> Read(string deviceName);
    }

}
