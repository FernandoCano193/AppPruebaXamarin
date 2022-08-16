using System;
using System.Collections.Generic;
using System.Text;
using Firebase.Database;

namespace CRUD_FirebaseXamarin.Services
{
    public class ConexionFirebase
    {
        public static FirebaseClient firebase = new FirebaseClient("https://ejemplocrud-96a6b-default-rtdb.firebaseio.com/");
    }
}
