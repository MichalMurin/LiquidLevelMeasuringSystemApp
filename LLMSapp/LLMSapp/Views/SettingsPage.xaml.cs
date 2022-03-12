using LLMSapp.ViewModels;
using LLMSapp.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LLMSapp.Views
{
    public partial class SettingsPage : ContentPage
    {

        public SettingsPage()
        {
            InitializeComponent();
            BindingContext =  new SettingsViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }
    }
}