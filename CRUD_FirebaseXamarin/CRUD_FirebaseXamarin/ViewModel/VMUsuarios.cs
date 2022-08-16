using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using CRUD_FirebaseXamarin.Model;
using CRUD_FirebaseXamarin.Services;
using Firebase.Database.Query;
using Firebase.Storage;

namespace CRUD_FirebaseXamarin.ViewModel
{
    public class VMUsuarios
    {
        //SE ENLISTAN LOS USUARIOS
        List<MUsuarios> Usuarios = new List<MUsuarios>();
        string RutaIcono;

        public async Task<List<MUsuarios>> Mostrar_Usuarios()
        {
            var data = await ConexionFirebase.firebase
                .Child("Usuarios")
                .OrderByKey()
                .OnceAsync<MUsuarios>();

            foreach (var datos in data)
            {
                MUsuarios parametros = new MUsuarios();
                parametros.IdUsuario = datos.Key;
                parametros.Usuario = datos.Object.Usuario;
                parametros.Password = datos.Object.Password;
                parametros.Icono = datos.Object.Icono;
                parametros.Estado = datos.Object.Estado;
                Usuarios.Add(parametros);
            }

            //RETORNA LOS DATOS
            return Usuarios;
        }

        public async Task Insertar_Usuario(MUsuarios parametros)
        {
            await ConexionFirebase.firebase
                .Child("Usuarios")
                .PostAsync(new MUsuarios()
                {
                    Usuario = parametros.Usuario,
                    Password = parametros.Password,
                    Icono = parametros.Icono,
                    Estado = parametros.Estado
                });
        }

        public async Task<string> SubirImagenStorage(string IdUsuarios, Stream ImagenStream)
        {
            var datoAlmacenar = await new FirebaseStorage("ejemplocrud-96a6b.appspot.com/")
                .Child("Usuarios")
                .Child(IdUsuarios + ".jpg")
                .PutAsync(ImagenStream);

            //SE RETORNA LA UBICACION DEL ICONO
            RutaIcono = datoAlmacenar;
            return RutaIcono;
        }
    }
}
