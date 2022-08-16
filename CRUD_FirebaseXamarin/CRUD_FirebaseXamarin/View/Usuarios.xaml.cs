using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using CRUD_FirebaseXamarin.ViewModel;
using CRUD_FirebaseXamarin.Model;
using Plugin.Media.Abstractions;
using Plugin.Media;
using System.Diagnostics;

namespace CRUD_FirebaseXamarin.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Usuarios : ContentPage
    {
        public Usuarios()
        {
            InitializeComponent();

            //SE MANDA LLAMAR EL PROCEDIMIENTO PARA MOSTRAR LOS USUARIOS
            MostrarUsuarios();
        }

        //VARIABLE DECLARADA PARA SUBIR LA IMAGEN
        MediaFile Imagen;

        private async void btnGuardar_Clicked(object sender, EventArgs e)
        {
            await InsertarUsuarios();
        }

        private async Task InsertarUsuarios()
        {
            VMUsuarios funcion = new VMUsuarios();
            var parametros = new MUsuarios();

            parametros.Usuario = txtUsuario.Text;
            parametros.Password = txtPassword.Text;
            parametros.Icono = "-";
            parametros.Estado = "-";


            await funcion.Insertar_Usuario(parametros);
            await DisplayAlert("Listo", "Nuevo usuario agregado", "OK");
            await MostrarUsuarios();
        }

        private async Task MostrarUsuarios()
        {

            VMUsuarios funcion = new VMUsuarios();
            var datos = await funcion.Mostrar_Usuarios();

            //"ListaUsuarios es un CollectionView"
            //SE MANDA LLAMAR LA VARIABLE DATOS INCIALIZADA ANTERIORMENTE
            ListaUsuarios.ItemsSource = datos;
        }

        private async void btnAgregarImagen_Clicked(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();
            try
            {
                Imagen = await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions()
                {
                    PhotoSize=PhotoSize.Medium
                });
                if (Imagen == null)
                    return;
                
                Foto.Source = ImageSource.FromStream(() =>
                {
                    var RutaImagen = Imagen.GetStream();
                    return RutaImagen;
                });
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}