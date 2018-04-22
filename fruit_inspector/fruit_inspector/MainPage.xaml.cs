﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace fruit_inspector
{
	public partial class MainPage : ContentPage
	{
		public MainPage()
		{
			InitializeComponent();

            CameraButton.Clicked += CameraButton_Clicked;
            
		}

        private async void CameraButton_Clicked(object sender, EventArgs e)
        {
            Plugin.Media.Abstractions.MediaFile photo = await Plugin.Media.CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions() { });

            if (photo != null)
            {
                PhotoImage.Source = ImageSource.FromStream(() => { return photo.GetStream(); });
            }
        }
	}
}
