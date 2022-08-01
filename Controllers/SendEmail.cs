using System;
using System.Net;
using System.Net.Mail;

namespace Hello_Travellers.Controllers
{
    internal class SendEmail
    {
        public void sendmail(string To,string body)
        {
             MailMessage mc = new MailMessage("190104113@aust.edu", To);
             mc.Subject = "subject";
             mc.Body =body;
             mc.IsBodyHtml = false;
             SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);

             smtp.EnableSsl = true;
             smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
             NetworkCredential nc = new NetworkCredential("190104113@aust.edu", "rohit4321");
             smtp.UseDefaultCredentials = false;
             smtp.Credentials = nc;
             smtp.Send(mc);
         
        }
    }
}