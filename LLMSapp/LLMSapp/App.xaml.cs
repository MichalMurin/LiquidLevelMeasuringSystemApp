using LLMSapp.Services;
using LLMSapp.Views;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LLMSapp
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();
            MainPage = new AppShell();
        }
    }
}
