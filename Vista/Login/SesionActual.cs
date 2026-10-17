namespace Vista.Login
{
    // USUARIO QUE INICIO SESION. SE LLENA AL ENTRAR (LOGIN) Y SE LIMPIA AL VOLVER AL LOGIN (CERRAR SESION).
    internal static class SesionActual
    {
        public static int IdUsuario { get; private set; }
        public static string Nombre { get; private set; }
        public static string Usuario { get; private set; }
        public static string Rol { get; private set; }

        public static bool Activa
        {
            get { return IdUsuario > 0; }
        }

        public static void Iniciar(int idUsuario, string nombre, string usuario, string rol)
        {
            IdUsuario = idUsuario;
            Nombre = nombre;
            Usuario = usuario;
            Rol = rol;
        }

        public static void Cerrar()
        {
            IdUsuario = 0;
            Nombre = null;
            Usuario = null;
            Rol = null;
        }
    }
}
