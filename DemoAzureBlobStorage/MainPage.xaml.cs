using System;
using Windows.Media.Capture;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using DemoAzureBlobStorage.Helpers;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace DemoAzureBlobStorage
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private StorageFile _storageFile;

        private async void btnTakeImage_OnClick(object sender, RoutedEventArgs e)
        {
            CameraCaptureUI cameraCaptureUi = new CameraCaptureUI();

            cameraCaptureUi.PhotoSettings.AllowCropping = false;
            cameraCaptureUi.PhotoSettings.MaxResolution = CameraCaptureUIMaxPhotoResolution.MediumXga;

            _storageFile =
               await cameraCaptureUi.CaptureFileAsync(CameraCaptureUIMode.Photo);

            if (_storageFile != null)
            {
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.SetSource(await _storageFile.OpenReadAsync());
                CurrentImage.Source = bitmapImage;
            }

        }

        private async void btnUploadImage_OnClick(object sender, RoutedEventArgs e)
        {
            if (_storageFile != null)
            {
                if (!string.IsNullOrEmpty(txtName.Text) && !string.IsNullOrEmpty(txtContainerName.Text))
                {
                    BlobStorageHelper blobStorageHelper = new BlobStorageHelper();

                    await blobStorageHelper.UploadImageAsync(txtContainerName.Text, $"{txtName.Text}.jpg", _storageFile);

                    MessageDialog messageDialog = new MessageDialog("La imagen se ha subido con éxito.");
                    await messageDialog.ShowAsync();
                }
                else
                {
                    MessageDialog messageDialog = new MessageDialog("Necesitas asignar un nombre a la imagen y al contenedor");
                    await messageDialog.ShowAsync();
                }
            }
            else
            {
                MessageDialog messageDialog = new MessageDialog("Debes tener una imagen para subir al blob");
                await messageDialog.ShowAsync();
            }

        }
    }
}