using LLMSapp.Models;
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
    public partial class ItemsPage : ContentPage
    {

        public ItemsPage()
        {
            InitializeComponent();

            BindingContext =  new ItemsViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }
    }
}