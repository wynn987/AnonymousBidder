﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Configuration;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;




namespace AnonymousBidder.Common


{
    public class EmailHelper
    {
        #region Email
        public static string FixBase64ForImage(string Image)
        {
            System.Text.StringBuilder sbText = new System.Text.StringBuilder(Image, Image.Length);
            sbText.Replace("\r\n", string.Empty); sbText.Replace(" ", string.Empty);
            return sbText.ToString();
        }



        public static bool SendMail(string fromAddress, string toAddress, string subject, string body, string cc, string sectionName, string attachmentFilename="", string pathImageEmbedded = "", string bcc = "")
        {
            string sectionPath = "mailSettings/" + sectionName;
            SmtpSection mailSetting = (SmtpSection)ConfigurationManager.GetSection(sectionPath);
            string HOST = mailSetting.Network.Host;
            int PORT = mailSetting.Network.Port;
            string USERNAME = mailSetting.Network.UserName;
            var parent = Directory.GetParent(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).FullName).FullName;
            StreamReader sr = new StreamReader(parent + "\\abc.txt");
            sr.ReadLine();
            sr.ReadLine();
            sr.ReadLine();
            sr.ReadLine();
            sr.ReadLine();
            sr.ReadLine();
            sr.ReadLine();
            string mailPassword = sr.ReadLine();
            string PASSWORD = Sercurity.Decrypt(mailPassword);
            string ADMIN_MAIL = mailSetting.From;
            string ADMIN_MAIL_NAME = "AnonymousBidder Administrator";
            string mailFrom = !string.IsNullOrEmpty(fromAddress) ? fromAddress : ADMIN_MAIL;
            try
            {
                MailMessage mailMessage = BuildMailMessage(ADMIN_MAIL_NAME, mailFrom, toAddress, subject, body, pathImageEmbedded);

                if (!string.IsNullOrEmpty(cc))
                {
                    var toCcList = cc.Split(',');

                    if (toCcList != null && toCcList.Any())
                    {
                        foreach (var toCc in toCcList)
                        {
                            mailMessage.CC.Add(new MailAddress(toCc));
                        }
                    }
                }

                if (!string.IsNullOrEmpty(bcc))
                {
                    var toBccList = bcc.Split(',');

                    if (toBccList != null && toBccList.Any())
                    {
                        foreach (var toBcc in toBccList)
                        {
                            mailMessage.Bcc.Add(new MailAddress(toBcc));
                        }
                    }
                }

                
                if (!string.IsNullOrEmpty(attachmentFilename))
                {

                  
                    Byte[] bytes = Convert.FromBase64String(attachmentFilename);
                    Attachment att = new Attachment(new MemoryStream(bytes), "test.png");
                    mailMessage.Attachments.Add(att);


                    
                }


                using (var smtpClient = new SmtpClient(HOST, PORT))
                {
                    var credential = new NetworkCredential(USERNAME, PASSWORD);
                    smtpClient.Credentials = credential;
                    smtpClient.EnableSsl = true;
                    smtpClient.Send(mailMessage);
                    mailMessage.Dispose();
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
                throw ex;
            }
        }
        private static MailMessage BuildMailMessage(string ADMIN_MAIL_NAME, string fromAddress, string toAddress, string subject, string body, string pathImageEmbedded)
        {
            MailMessage message = new MailMessage
            {
                From = new MailAddress(fromAddress, ADMIN_MAIL_NAME),
                Subject = subject,
                BodyEncoding = Encoding.UTF8,
                IsBodyHtml = true,
            };






            if (!string.IsNullOrEmpty(pathImageEmbedded))
            {
                AlternateView avHtml = AlternateView.CreateAlternateViewFromString(body, null, System.Net.Mime.MediaTypeNames.Text.Html);

                LinkedResource img = new LinkedResource(pathImageEmbedded, System.Net.Mime.MediaTypeNames.Image.Jpeg);
                img.ContentId = "ImageEmbedded";
                img.ContentType.MediaType = System.Net.Mime.MediaTypeNames.Image.Jpeg;
                img.TransferEncoding = System.Net.Mime.TransferEncoding.Base64;
                img.ContentType.Name = img.ContentId;
                img.ContentLink = new Uri("cid:" + img.ContentId);
                avHtml.LinkedResources.Add(img);

                message.AlternateViews.Add(avHtml);
            }
            else
            {
                message.Body = body;
            }
            string[] tos = toAddress.Split(';');
            foreach (string to in tos)
                message.To.Add(new MailAddress(to));

            return message;
        }
        #endregion
    }
}
