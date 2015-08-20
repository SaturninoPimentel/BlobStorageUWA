###How to: Subir imágenes al Blob Storage de Azure desde UWA
El post original lo puede leer en [el blog del satur](https://saturninopimentel.com/how-to-subir-imagenes-al-blob-storage-de-azure-desde-uwa/) 

El **Blob Storage** de **Windows Azure** es el servicio de almacenamiento para grandes cantidades de datos estructurados, en este post veremos cómo subir una imagen a él desde una **Universal Windows App**

####Configurando el Storage en Windows Azure

Como primer paso vamos entrar en el portal de **Azure** y vamos a agregar un nuevo servicio de almacenamiento, esto se logra seleccionando data services -> storage -> Quick create, después asignaremos un subdominio y su ubicación en los servidores de **Azure** y daremos clic en **Create Storage Account**.

![](https://saturninopimentel.com/content/images/2015/08/Blob5.png)

Una vez **Azure** termine el proceso de creación te enviará al dashboard, debes seleccionar la opción **containers** y dar clic en **Create a container**.
![](https://saturninopimentel.com/content/images/2015/08/Blob6.png)

Esto nos mostrará un pop-up como el de la imagen siguiente donde tienes que especificar un nombre para tu nuevo contenedor y dar clic en el botón de aceptar.

![](https://saturninopimentel.com/content/images/2015/08/Blob7.png)

Al finalizar el proceso **Azure** nos mostrará la siguiente pantalla donde debes dar clic en la opción **Manage Access Keys**.

![](https://saturninopimentel.com/content/images/2015/08/Blob8.png)

Te mostrará un pop-up como el siguiente, debes copiar el nombre y la llave primaria ya que la utilizaremos más adelante.

![](https://saturninopimentel.com/content/images/2015/08/blob9.png)

####Proyecto Universal Windows App
Con lo anterior hemos terminado el proceso en **Azure** y ahora vamos a agregar el proyecto de nuget, para ello da clic derecho en las referencias y selecciona la opción **Manage Nuget Packages**.

![](https://saturninopimentel.com/content/images/2015/08/Blob1.png)

Después busca el paquete **Windows.Azure.Storage** y da clic en instalar.

![](https://saturninopimentel.com/content/images/2015/08/Blob2.png)

Te mostrará los paquetes a instalar y debes dar clic en **Ok**.

![](https://saturninopimentel.com/content/images/2015/08/Blob3.png)

Después te pedirá autorización y debes aceptar.

![](https://saturninopimentel.com/content/images/2015/08/Blob4.png)

Una vez instalados vamos a crear una clase que nos ayudará a subir los archivos llamada BlobStorageHelper, en esta clase vamos a crear tres campos, el primero será de tipo **CloudStorageAccount** que nos servirá para administrar la cuenta de almacenamiento, los otros dos van a ser de tipo **string** y almacenarán el nombre de la cuenta y la llave que guardamos en uno de los pasos anteriores como se muestra en el siguiente código.
```language-csharp
private readonly CloudStorageAccount _storageAccount;
private string _accountName = "saturblobtest";
string _key = "";
```
En el constructor vamos a crear un campo de tipo **StorageCredentials** al cual proporcionaremos el nombre de la cuenta y la llave, después crearemos la instancia de **_storageAccount** y en el constructor pasaremos la instancia de **StorageCredentials** y como segundo parámetro indicaremos si se utilizará el transporte **HTTPS** por medio de un tipo **bool**.
```language-csharp
StorageCredentials storageCredentials = new StorageCredentials(_accountName, _key);
_storageAccount = new CloudStorageAccount(storageCredentials, true);
```
Ahora vamos a crear un método que nos permita subir nuestras imágenes al **Blob Storage** en el cual pasaremos el nombre del contenedor, el nombre del archivo y por ultimo una instancia de **StorageFile**.
A grandes rasgos en este método crearemos un cliente encargado de manejar el **Blob Storage**, después obtendremos una referencia al contenedor y nos aseguramos de crearlo si es que este no existe, por último obtenemos una referencia a un bloque del Blob y subimos el archivo.
```language-csharp
        public async Task UploadImageAsync(string containerName, string fileName, StorageFile file)
        {
            //Creamos el cliente del Blob Storage.
            CloudBlobClient cloudBlobClient = _storageAccount.CreateCloudBlobClient();
            //Crearemos el contenedor del Blob
            CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(containerName);
            //Nos aseguraremos de crear el contenedor si no existe. 
            await cloudBlobContainer.CreateIfNotExistsAsync();
            //Obtenemos un bloque del blob
            CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(fileName);
            //Subimos el archivo a Azure
            await cloudBlockBlob.UploadFromFileAsync(file);
        }
```
Ahora crea una pantalla como la que se muestra a continuación.
![](https://saturninopimentel.com/content/images/2015/08/wp_ss_20150820_0001.png)

En el code-behind de nuestra pantalla vamos a agregar un campo de tipo **StorageFile** que nos servirá para almacenar la imagen.
```language-csharp
private StorageFile _storageFile;
```
En el método encargado de controlar el clic del botón "**tomar imagen**" agregaremos el siguiente código para poder tomar una fotografía y mostrarla.
```language-csharp
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
```
Después en el método encargado de controlar el clic del botón "**Subir imagen**" agregaremos el siguiente código que utilizará la clase que hemos creado para subir la imagen al **Blob Storage**. (Cambia el nombre de los controles por los que tú utilices)
```language-csharp
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
```  
Y con esto ya podrás agregar las imágenes al **Blob Storage** de **Azure** desde una **Universal Windows App**, ahora te invito a que pruebes la app desde tu escritorio o desde tu teléfono, en mi caso lo haré desde el teléfono.

![](https://saturninopimentel.com/content/images/2015/08/wp_ss_20150820_0002.png)
![](https://saturninopimentel.com/content/images/2015/08/wp_ss_20150820_0003.png)

Por último te invito a comprobar desde el portal de **Azure** que la imagen se encuentra en el contenedor.

![](https://saturninopimentel.com/content/images/2015/08/Blob11.png)

Puedes copiar y pegar la url de la imagen en un explorador para verla tal como se muestra en la siguiente imagen.

![](https://saturninopimentel.com/content/images/2015/08/blob12-1.png)

Como ya es costumbre es código lo dejo en [GitHub](https://github.com/Satur01/BlobStorageUWA).
¡Saludos!
