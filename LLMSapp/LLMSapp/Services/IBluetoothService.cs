using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LLMSapp.Services
{
    /// <summary>
    /// Rozhranie pre spravu bluetooth 
    /// </summary>
    public interface IBluetoothService
    {
        /// <summary>
        /// metoda na pripojenie mobilneho zariadenia k zariadeniu bluetooth
        /// </summary>
        /// <param name="deviceName">nazov zariadenia ku ktoremu sa pripaja</param>
        /// <returns>true - ak pripojenie prebehlo uspesne</returns>
        Task<bool> Connect(string deviceName);

        /// <summary>
        /// metoda zisti ci je zariadenie pripojene 
        /// </summary>
        /// <returns>true - ak je zariadenie pripojene</returns>
        bool IsConnected();

        /// <summary>
        /// metoda na zistenie vsetkych sparovanych BT zariadeni
        /// </summary>
        /// <returns>zoznam sparovanych zariadeni</returns>
        IList<string> GetDeviceList();

        /// <summary>
        /// metoda na odoslanie znaku prostrednictvom BT
        /// </summary>
        /// <param name="character">odosielany znak</param>
        /// <returns>true - ak odoslanie prebehlo uspesne</returns>
        bool Send(char character);

        /// <summary>
        /// metoda ktora prijima data cez bluetooth
        /// </summary>
        /// <param name="count">pocet prijimanych znakov</param>
        /// <returns>retazec priajtych znakov</returns>
        string Read(int count);

        /// <summary>
        /// metoda na zistenie, ci je v mobilnom zariadeni zapnute Bluetooth
        /// </summary>
        /// <returns>true - ak je BT zapnute</returns>
        bool IsBluetoothOn();

        /// <summary>
        /// Zapnutie Bluetooth
        /// </summary>
        /// <returns>true - ak sa BT podarilo zapnut</returns>
        bool TurnOnBluetooth();

        /// <summary>
        /// Vypnutie Bluetooth
        /// </summary>
        /// <returns>ture - ak sa BT podarilo vypnut</returns>
        bool TurnOfBluetooth();
    }

}
