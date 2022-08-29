using System;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;
using System.Web.Mvc;

namespace Hello_Travellers.Controllers
{
    internal class SendEmail
    {
        public void sendmail(string To, string body, string sub)
        {
             MailMessage mc = new MailMessage("190104113@aust.edu", To);
             mc.Subject =sub;
             mc.Body =body;
             mc.IsBodyHtml = false;
             SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);

             smtp.EnableSsl = true;
             smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
             NetworkCredential nc = new NetworkCredential("190104113@aust.edu", "Rohit1234");
             smtp.UseDefaultCredentials = false;
             smtp.Credentials = nc;


            try {

                smtp.Send(mc);
            }
                catch(Exception ex)
            {
                Debug.Write(ex);
            }

        }
    }
}