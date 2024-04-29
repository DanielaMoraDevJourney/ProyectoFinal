using System.Net;
using System.Net.Mail;

namespace ProyectoIntegradosPZ_KS_DM.Data
{
    public class Email
    {
        public void Enviar(string correo, string token)
        {
            Correo(correo, token);

        }
        void Correo(string correo_receptor, string token)
        {
            string correo_emisor = "@hotmail.com";
            string clave_emisor = "";


            MailAddress receptor = new(correo_receptor);
            MailAddress emisor = new(correo_emisor);
            MailMessage email = new(emisor, receptor);
            email.Subject = "Activación de cuenta";
            email.Body = @"<!DOCTYPE html>
                            <html>
                            <head>
                                 <title> Activación de la cuenta </title>
                            </head>
                            <body>
                             <h2>Activación de la cuenta</h2>
                             <p> Para activar su cuenta haga clic en el enlace: </p>                            
                             <a href= 'https://localhost:7094/Cuenta/Token?valor=" + token +"'>Activar cuenta</a></body> </html>";
               
            email.IsBodyHtml = true;
            SmtpClient smtp = new();
            smtp.Host = "smtp.office365.com";
            smtp.Port = 587;
            smtp.Credentials = new NetworkCredential(correo_emisor, clave_emisor);
            smtp.EnableSsl = true;

            try
            {
                smtp.Send(email);
            }
            catch (SystemException)
            {
                throw;
            }
        
        
        }


    }
 }
