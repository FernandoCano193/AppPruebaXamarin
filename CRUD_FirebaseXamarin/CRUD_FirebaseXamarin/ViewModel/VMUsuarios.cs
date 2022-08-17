using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        
        string RutaFoto;

        string IDUsuario;

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

        public async Task<string> Insertar_Usuario(MUsuarios parametros)
        {
            var data = await ConexionFirebase.firebase
                .Child("Usuarios")
                .PostAsync(new MUsuarios()
                {
                    Usuario = parametros.Usuario,
                    Password = parametros.Password,
                    Icono = parametros.Icono,
                    Estado = parametros.Estado
                });

            //MANERA DE RECUPERAR EL ID
            IDUsuario = data.Key;
            return IDUsuario;
        }

        public async Task<string> SubirImagenStorage(string IdUsuarios, Stream ImagenStream)
        {
            var datoAlmacenar = await new FirebaseStorage("ejemplocrud-96a6b.appspot.com")
                .Child("Usuarios")
                .Child(IdUsuarios + ".jpg")
                .PutAsync(ImagenStream);

            //SE RETORNA LA RUTA WEB DE LA IMAGEN O FOTO
            RutaFoto = datoAlmacenar;
            return RutaFoto;
        }

        public async Task EditarImagen(MUsuarios parametros)
        {
            //PROCEDIMIENTO PARA RECUPERAR EL ID AL CUAL SE DEBE DE EDITAR LA IMAGEN O FOTO
            var data = (await ConexionFirebase.firebase
                .Child("Usuarios")
                .OnceAsync<MUsuarios>()).Where(a => a.Key==parametros.IdUsuario).FirstOrDefault();

            await ConexionFirebase.firebase
                .Child("Usuarios")
                .Child(data.Key)
                .PutAsync(new MUsuarios()
                {
                    Usuario = parametros.Usuario,
                    Password = parametros.Password,
                    Estado = parametros.Estado,
                    Icono = parametros.Icono
                });
        }
    }
}
