using LLMSapp.Models;
using LLMSapp.Views;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace LLMSapp.ViewModels
{
    public class ItemsViewModel : BaseViewModel
    {
        public Command SaveCommand { get; }
        public Command SavePhoneNumberCommand { get; }

        private bool _isCallEnabled;
        public bool IsCallEnabled
        {
            get
            {
                return _isCallEnabled;
            }
            set
            {
                _isCallEnabled = value;
                OnPropertyChanged(nameof(IsCallEnabled));
            }
        }

        private string _phoneNumber;
        public string PhoneNumber
        {
            get
            {
                return _phoneNumber;
            }
            set
            {
                _phoneNumber = value;
                OnPropertyChanged(nameof(PhoneNumber));
            }
        }
        public ItemsViewModel()
        {
            Title = "Nastavenia";
            IsCallEnabled = false;
            SaveCommand = new Command(OnSave);
            SavePhoneNumberCommand = new Command(OnSaveNumber);
        }
        
        private async void OnSave()
        {
            
        }

        private async void OnSaveNumber()
        {

        }

    }
}