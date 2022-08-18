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
        string Estado;
        bool EstadoImagen;

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
                {
                    EstadoImagen = false;
                    return;
                }
                else
                {
                    Foto.Source = ImageSource.FromStream(() =>
                    {
                        var RutaImagen = Imagen.GetStream();
                        return RutaImagen;
                    });

                    EstadoImagen = true;

                }
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

        private async void btnIcono_Clicked(object sender, EventArgs e)
        {
            //SE RECUPERA EL ID AL HACER CLICK
            IDUsuario = (sender as ImageButton).CommandParameter.ToString();
            await ObtenerDatosUsuarios();
        }

        private async Task ObtenerDatosUsuarios()
        {
            var funcion = new VMUsuarios();
            var parametros = new MUsuarios();
            parametros.IdUsuario = IDUsuario;
            var datos = await funcion.ObtenerDatosUsuarios(parametros);

            foreach(var fila in datos)
            {
                txtPassword.Text = fila.Password;
                txtUsuario.Text = fila.Usuario;
                //FOTO SE SACO DEL NOMBRE QUE SE LE DIO AL OBJETO DE IMAGE EN EL XAMEL
                Foto.Source = fila.Icono;
                Estado = fila.Estado;
                RutaFoto = fila.Icono;
            }
        }

        private async Task EliminarUsuario()
        {
            VMUsuarios funcion = new VMUsuarios();
            MUsuarios parametros = new MUsuarios();
            parametros.IdUsuario = IDUsuario;
            await funcion.EliminarUsuarios(parametros);
        }

        private async Task EliminarImagen()
        {
            VMUsuarios funcion = new VMUsuarios();
            //SE PASA EL NOMBRE DE LA IMAGEN QUE EN ESTE CASO ES IDUsuario.jpg
            await funcion.EliminarImagen(IDUsuario + ".jpg");
        }
        private async void btnEliminar_Clicked(object sender, EventArgs e)
        {
            await EliminarUsuario();
            await EliminarImagen();
            await MostrarUsuarios();
        }

        private async void btnEditar_Clicked(object sender, EventArgs e)
        {
            if(EstadoImagen==true)
            {
                await EliminarImagen();
                await SubirImagenFirebase();
            }
            await EditarImagen();
        }
    }
}