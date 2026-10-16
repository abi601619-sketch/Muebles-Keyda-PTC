using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Modelo.Properties;

namespace Modelo
{
    // LOS DATOS DE LA EMPRESA (NOMBRE, TELÉFONO, CORREO, DIRECCIÓN Y LOGO) SON CONFIGURACIÓN DE USUARIO Y .NET LOS GUARDA
    // POR VERSIÓN DE LA APLICACIÓN (...\Vista.exe_Url_xxx\1.0.x.0\user.config). AL INSTALAR UNA VERSIÓN NUEVA ESOS DATOS
    // QUEDAN VACÍOS Y LOS PDF FALLAN CON "ERROR PDF 001". ESTA CLASE LOS RECUPERA DE LA VERSIÓN ANTERIOR MÁS RECIENTE QUE LOS TENGA.
    public static class RecuperadorConfiguracionEmpresa
    {
        private const string SeccionConfiguracion = "Modelo.Properties.Settings";

        private static readonly string[] Campos =
        {
            "NombreEmpresa", "TelefonoEmpresa", "CorreoEmpresa", "DireccionEmpresa", "LogoEmpresa"
        };

        // DEVUELVE true SI SE RECUPERARON LOS DATOS. NUNCA LANZA EXCEPCIONES PARA NO IMPEDIR QUE LA APLICACIÓN ABRA.
        public static bool Recuperar()
        {
            try
            {
                string archivoActual = ConfigurationManager
                    .OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath;

                return Recuperar(archivoActual);
            }
            catch (Exception)
            {
                return false;
            }
        }

        // archivoActual ES EL user.config DE LA VERSIÓN EN EJECUCIÓN; LAS DEMÁS VERSIONES ESTÁN EN CARPETAS HERMANAS
        public static bool Recuperar(string archivoActual)
        {
            try
            {
                Settings configuracion = Settings.Default;

                if (!string.IsNullOrWhiteSpace(configuracion.NombreEmpresa) ||
                    !string.IsNullOrWhiteSpace(configuracion.LogoEmpresa))
                {
                    return false;
                }

                // ...\<Compañía>\Vista.exe_Url_xxx\<versión>\user.config
                string carpetaAplicacion = Path.GetDirectoryName(Path.GetDirectoryName(archivoActual));
                string carpetaCompania = Path.GetDirectoryName(carpetaAplicacion);

                if (string.IsNullOrEmpty(carpetaCompania) || !Directory.Exists(carpetaCompania))
                    return false;

                string nombreAplicacion = Path.GetFileName(carpetaAplicacion);
                int guion = nombreAplicacion.IndexOf('_');
                string prefijo = guion > 0 ? nombreAplicacion.Substring(0, guion) : nombreAplicacion;

                var candidatos = Directory.GetDirectories(carpetaCompania, prefijo + "_*")
                    .SelectMany(carpeta => Directory.GetFiles(carpeta, "user.config", SearchOption.AllDirectories))
                    .Where(archivo => !string.Equals(archivo, archivoActual, StringComparison.OrdinalIgnoreCase))
                    .OrderByDescending(archivo => File.GetLastWriteTimeUtc(archivo));

                foreach (string archivo in candidatos)
                {
                    string[] valores = LeerValores(archivo);

                    if (valores == null)
                        continue;

                    for (int i = 0; i < Campos.Length; i++)
                    {
                        if (valores[i] != null)
                            configuracion[Campos[i]] = valores[i];
                    }

                    configuracion.Save();
                    return true;
                }

                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // DEVUELVE LOS VALORES EN EL ORDEN DE Campos, O null SI EL ARCHIVO NO TIENE NOMBRE NI LOGO
        private static string[] LeerValores(string archivo)
        {
            try
            {
                XElement seccion = XDocument.Load(archivo).Root?
                    .Element("userSettings")?
                    .Element(SeccionConfiguracion);

                if (seccion == null)
                    return null;

                string[] valores = new string[Campos.Length];

                for (int i = 0; i < Campos.Length; i++)
                {
                    string campo = Campos[i];

                    valores[i] = seccion.Elements("setting")
                        .FirstOrDefault(s => (string)s.Attribute("name") == campo)?
                        .Element("value")?.Value;
                }

                bool tieneDatos = !string.IsNullOrWhiteSpace(valores[0]) || !string.IsNullOrWhiteSpace(valores[4]);

                return tieneDatos ? valores : null;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
