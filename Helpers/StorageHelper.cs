
using System.Threading.Tasks;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.IO;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebappBruno.Helpers
{
    public class StorageHelper
    {
        public const string URL_Imagen_default = "https://brunocuenta.blob.core.windows.net/contenedorbruno/persona1.jpg";
         public static async Task<string>SubirArchivo (Stream contenido, string nombre, AzureStorageConfig config)
        {
            string url = $"https://{config.Cuenta}.blob.core.windows.net/{config.Contenedor}/{nombre}";
            Uri uri = new Uri (url);
            var credenciales = new StorageSharedKeyCredential(config.Cuenta, config.Llave);
            var Cliente = new BlobClient(uri, credenciales);
            await Cliente.UploadAsync (contenido);
            return url;
        }

        public static async Task<bool> EliminarArchivos(string url, AzureStorageConfig config)
        {
            Uri uri = new Uri(url);
            var credenciales = new StorageSharedKeyCredential(config.Cuenta, config.Llave);
            var Cliente = new BlobClient(uri, credenciales);
            var respuesta = await Cliente.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);
            return respuesta.Value;
        }
        public static string GetURLFromFileName(string name, AzureStorageConfig config)
        {
            return $"https://{config.Cuenta}.blob.core.windows.net/{config.Contenedor}/{name}";
        }
    }
}
