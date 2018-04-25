using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Microsoft.Cognitive.CustomVision.Prediction;
using Microsoft.Cognitive.CustomVision.Prediction.Models;
using Plugin.Media;
using Plugin.Media.Abstractions;

namespace fruit_inspector
{
	public partial class MainPage : ContentPage
	{
		public MainPage()
		{
			InitializeComponent();

            CameraButton.Clicked += CameraButton_Clicked;

            SuccessButton.IsVisible = false;
            FailureButton.IsVisible = false;
        }

        private async void CameraButton_Clicked(object sender, EventArgs e)
        {
//#if WINDOWS_PHONE
            //todo
            await CrossMedia.Current.Initialize();
            PhotoImage.Source = "";
            ResultLabel.Text = "";
//#endif

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await DisplayAlert(
                    "No Camera",
                    ":( No camera available.",
                    "OK");
                return;
            }

            //MediaFile photo = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions()
            //    {
            //        Directory = "FruitInspector",
            //        Name = "test.jpg"
            //    });
            MediaFile photo = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions(){ });

            if (photo != null)
            {
                PhotoImage.Source = ImageSource.FromFile(photo.Path);
                //PhotoImage.Source = ImageSource.FromStream(() => { return photo.GetStream(); });

                string prediction = await RecognizePicture(photo);
                ResultLabel.Text = prediction; //RecognizePicture(photo);
                ResultLabel.IsVisible = true;
            }


            // 2. Overlay image with text representing results of prediction
            // 3. Enable success/failure buttons to affirm/unaffirm results
            // 4. Send image/tag to API for improvement of model
            // 5. Retrain model
        }
        private async Task<string> RecognizePicture(MediaFile file)
        {
            string message = "Nothing recognized...";

            try
            {
                // 1. Send to CustomVision.ai
                string predictionKey = "ec639b00591d498bba6da92b47f88262";
                PredictionEndpoint endpoint = new PredictionEndpoint() { ApiKey = predictionKey };
                string projectId = "55dfeb9d-02f4-4a57-9b8d-2d132f0a2b9c";
                var result = endpoint.PredictImage(Guid.Parse(projectId), file.GetStream());
                
                double prob = 0;
                foreach (var c in result.Predictions)
                {
                    if (c.Probability > 0.7)
                    {
                        message = c.Tag;
                        prob = c.Probability;
                    }
                    else if (c.Probability > prob)
                    {
                        prob = c.Probability;
                        message = "Maybe " + c.Tag;
                    }
                }
            }
            catch (Exception e)
            {
                message = e.Message;
            }

            return message;
        }

    }
}
