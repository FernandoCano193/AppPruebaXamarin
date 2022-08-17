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

        //RUTA DE LA FOTO O IMAGEN
        string RutaFoto;
        string IDUsuario;

        private async void btnGuardar_Clicked(object sender, EventArgs e)
        {
            await InsertarUsuarios();
            await SubirImagenFirebase();
            await EditarImagen();
        }

        private async Task EditarImagen()
        {
            VMUsuarios funcion = new VMUsuarios();
            MUsuarios parametros = new MUsuarios();

            //SE ASIGNAN LOS VALORES ACTUALIZADOS POR LA RUTA DE LA IMAGEN Y EL ID
            parametros.Icono = RutaFoto;
            parametros.IdUsuario = IDUsuario;

            parametros.Usuario = txtUsuario.Text;
            parametros.Password = txtPassword.Text;
            parametros.Estado = "Activo";

            await funcion.EditarImagen(parametros);

            await DisplayAlert("Listo", "Nuevo usuario agregado", "OK");

            //SE VUELVE A LLAMAR EL MOSTRAR USUARIO
            await MostrarUsuarios();

        }

        private async Task InsertarUsuarios()
        {
            VMUsuarios funcion = new VMUsuarios();
            var parametros = new MUsuarios();

            parametros.Usuario = txtUsuario.Text;
            parametros.Password = txtPassword.Text;
            parametros.Icono = "-";
            parametros.Estado = "-";


            IDUsuario = await funcion.Insertar_Usuario(parametros);
            //await DisplayAlert("Listo", "Nuevo usuario agregado", "OK");
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

        private async Task SubirImagenFirebase()
        {
            VMUsuarios funcion = new VMUsuarios();

            //RutaFoto RECUPERA LA RUTA WEB DE LA IMAGEN
            RutaFoto = await funcion.SubirImagenStorage(IDUsuario,Imagen.GetStream());
        }
    }
}