using System;
using System.IO;
using System.Net.Sockets;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace Modelo.Servicios
{
    public class ServicioCorreo
    {
        public static void EnviarCodigo(string correoDestino, string codigo)
        {
            var mensaje = new MimeMessage();

            mensaje.From.Add(new MailboxAddress("Sistema de Ventas", "abi601619@gmail.com"));

            mensaje.To.Add(MailboxAddress.Parse(correoDestino));

            mensaje.Subject = "Recuperación de contraseña";

            mensaje.Body = new TextPart("html")
            {
                Text = $@" <html>
                <body>

                    <h2>¿Olvidaste tu contraseña?</h2>

                    <p>
                        Hemos recibido una solicitud
                        para recuperar tu contraseña.
                    </p>

                    <p>
                        Tu código de verificación es:
                    </p>

                    <h1>{codigo}</h1>

                    <p>
                        Este código es válido durante
                        <strong>10 minutos</strong>.
                    </p>

                    <p>
                        Si no solicitaste recuperar tu contraseña,
                        puedes ignorar este correo.
                    </p>

                </body>
                </html>"
            };

            using (var cliente = new SmtpClient())
            {
                try
                {
                    cliente.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);

                    cliente.Authenticate("abi601619@gmail.com", "wrsj cwmi vsyq pwoh");

                    cliente.Send(mensaje);

                    cliente.Disconnect(true);
                }
                catch (AuthenticationException ex)
                {
                    throw new Exception("Gmail rechazó el usuario o la contraseña de aplicación con la que el sistema envía los correos.\n\n" + ex.Message, ex);
                }
                catch (SmtpCommandException ex)
                {
                    throw new Exception("El servidor de correo rechazó el envío (" + ex.StatusCode + ").\n\n" + ex.Message, ex);
                }
                catch (SslHandshakeException ex)
                {
                    throw new Exception("No se pudo establecer la conexión segura con smtp.gmail.com. Revise la fecha y hora del equipo y el antivirus.\n\n" + ex.Message, ex);
                }
                catch (Exception ex) when (ex is SocketException || ex is IOException || ex is TimeoutException)
                {
                    throw new Exception("No se pudo conectar con smtp.gmail.com (puerto 587). Revise la conexión a internet o si un firewall o antivirus bloquea el puerto.\n\n" + ex.Message, ex);
                }
            }
        }
    }
}
