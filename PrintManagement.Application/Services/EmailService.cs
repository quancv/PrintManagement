using MailKit.Net.Smtp;
using MimeKit;
using NETCore.MailKit.Core;
using NETCore.MailKit.Infrastructure.Internal;
using PrintManagement.Application.Handle.HandleEmail;
using PrintManagement.Application.IServices;
using PrintManagement.Application.Payloads.Respones;
using PrintManagement.Domain.Entities;
using PrintManagement.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IEmailService = PrintManagement.Application.IServices.IEmailService;

namespace PrintManagement.Application.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailConfiguration _emailConfiguration;
        private readonly IBaseRepository<ConfirmEmail> _baseConfirmRepository;
        private readonly IBaseRepository<User> _baseUserRepository;
        public EmailService(EmailConfiguration emailConfiguration, IBaseRepository<ConfirmEmail> baseConfirmRepository, IBaseRepository<User> baseUserRepository)
        {
            _emailConfiguration = emailConfiguration;
            _baseConfirmRepository = baseConfirmRepository;
            _baseUserRepository = baseUserRepository;
        }

        

        public string SendMail(EmailMessage emailMessage)
        {
            var message = CreateEmailMessage(emailMessage);
            Send(message);
            string emails = string.Join(", ", message.To);
            return ResponeMessage.GetEmailSuccessMessage(emails);
        }
        #region SendMail
        private MimeMessage CreateEmailMessage(EmailMessage emailMessage)
        {
            var mimeMessage = new MimeMessage();
            mimeMessage.From.Add(new MailboxAddress("Print Management System", _emailConfiguration.From));
            mimeMessage.To.AddRange(emailMessage.To);
            mimeMessage.Subject = emailMessage.Subject;
            mimeMessage.Body = new TextPart(MimeKit.Text.TextFormat.Text){ Text = emailMessage.Content};
            return mimeMessage;
        }
        private void Send(MimeMessage mimeMessage)
        {
            using var client = new SmtpClient();
            try
            {
                client.Connect(_emailConfiguration.SmtpServer, _emailConfiguration.Port, true);
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                client.Authenticate(_emailConfiguration.Username, _emailConfiguration.Password);
                client.Send(mimeMessage);
            } 
            catch
            {
                throw;
            }
            finally
            {
                client.Disconnect(true);
                client.Dispose();
            }
        }
        #endregion
    }
}
