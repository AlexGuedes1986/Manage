using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using BioTech.Models;
using BioTech.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BioTech.Services
{
    public static class SMTPService
    {
        public static void SendEmail(string urlLogo, EmailNotification emailReceiver, Proposal proposal)
        {
            var sc = new SmtpClient();
            sc.Host = "127.0.0.1";
            var mail = new MailMessage();
            mail.To.Add(new MailAddress(emailReceiver.Email));
            mail.Subject = $"Proposal {proposal.ProjectName} has been assigned to you";
            mail.IsBodyHtml = true;
            var paragraph = "";
            mail.From = new MailAddress("sender@bio-techconsulting.com");
            if (String.Equals(emailReceiver.Type, "pm"))
            {
                paragraph = $"Proposal <b>{proposal.ProjectName}</b> has been assigned to you as a Project Manager";
            }
            else
            {
                paragraph = $"Proposal <b>{proposal.ProjectName}</b> has been assigned to you as a Team Member";
            }
            mail.Body = "<html><head><meta http-equiv='Content-Type' content='text/html; charset=UTF-8' /><meta name='viewport' content='width=device-width, initial-scale=1.0' />" +
     "<title>Proposal assigned </title>" +
     "<style rel='stylesheet' type='text/css'>" +
         "@media only screen and (max-width: 600px) {" +
             ".wrapper {" +
                 "width: 100% !important;" +
                 "border: 0 !important;" +
             "}" +
             "body {" +
                 "background-color: #fff !important;" +
             "}" +
             ".no-bgr {" +
                 "background-color: #fff !important;" +
                 "height: 0 !important;" +
            "}" +
         "}" +
     "</style></head>" +
 "<body style='margin: 0 auto; padding: 0; zoom: 100%; background-color: #ffffff;' leftmargin='0' topmargin='0' marginwidth='0' marginheight='0'>" +
     "<table border='0' width='100%' height='100%' cellpadding='0' cellspacing='0' style='background-color: #ffffff;'>" +
         "<tbody>" +
             "<tr>" +
                 "<td style='padding: 0;'>" +           
            "<p>" + paragraph + "</p>" +
            "</td>" +
   "</tr>" +
    "<tr class='no-bgr' style='height: 150px; background-color: #ffffff;'>" +
                 "<td> <img width='181' src=" + urlLogo + " alt='BTC Logo' /></td>" +
             "</tr>" +
   "</tbody></table></body></html>"; ;

            sc.Send(mail);


        }
    }
}
