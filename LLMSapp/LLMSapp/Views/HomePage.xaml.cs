using System;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using LLMSapp.ViewModels;

namespace LLMSapp.Views
{
    public partial class HomePage : ContentPage
    {
        private static HomeViewModel _instance;
        public static HomeViewModel Instance { get { return _instance; } }
        public HomePage()
        {
            InitializeComponent();
            _instance = new HomeViewModel();
            BindingContext = _instance;
            
    }
    }
}