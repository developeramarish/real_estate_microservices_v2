using DC.Business.Application.Contracts.Interfaces.Services;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DC.Business.Consumer.Email.SendGrid
{
    public class SendGridService : ISendGridService
    {
        private string _sendGridKey;
        private SendGridClient client;
        public SendGridService(string sendGridKey) {
            _sendGridKey = sendGridKey;

            client = new SendGridClient(_sendGridKey);
        }

        public async Task SendMessageUserCreated(string destination)
        {
            // SendGridClient client = new SendGridClient(_sendGridKey);

            var from = new EmailAddress("rohovets.taras@gmail.com", "ImoRentals");
            var subject = "New User created in ImoRentals Portal";
            var to = new EmailAddress(destination);
            var plainTextContent = "Thank you for regestring in our portal";
            var htmlContent = "<strong>Good selling or finding your new house!</strong>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);

            // TODO add using !!!!!!!!! 
            await client.SendEmailAsync(msg);
        }

        public async Task SendMessagePropertyCreated(string destination)
        {
            //SendGridClient client = new SendGridClient(_sendGridKey);

            var from = new EmailAddress("rohovets.taras@gmail.com", "ImoRentals");
            var subject = "New Property created in ImoRentals Portal";
            var to = new EmailAddress(destination);
            var plainTextContent = "Thank you for uisng our portal";
            var htmlContent = "<strong>Good selling!</strong>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);

            // TODO add using !!!!!!!!! 
            await client.SendEmailAsync(msg);
        }
    }
}
