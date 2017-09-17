using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Configuration;
using System.Net.Mail;
using System.Net.Mime;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace AnonymousBidder.Common
{
    public class Utilities
    {
        public static string RemoveSpecialCharacters(string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') 
                    || (c >= 'A' && c <= 'Z') 
                    || (c >= 'a' && c <= 'z') 
                    || c == '.' || c == '_' 
                    || c == ' ' || c == '-'
                    )
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }
        public static string GetCountryCodeByString(string code)
        {
            //Get the country code only
            if (code.Contains('('))
            {
                code = code.Split('(')[1];
            }
            if (code.Contains(')'))
            {
                code = code.Split(')')[0];
            }
            return code;
        }
        public static List<BaseClass> GetCountryList()
        {
            List<BaseClass> countries = new List<BaseClass>();
            foreach (CultureInfo culture in CultureInfo.GetCultures(CultureTypes.SpecificCultures))
            {
                RegionInfo country = new RegionInfo(culture.LCID);
                if (!countries.Any(p => p.Text == country.EnglishName))
                {
                    BaseClass _country = new BaseClass()
                    {
                        Text = country.EnglishName,
                        Str_Value = country.DisplayName,
                        IntValue = culture.LCID,
                    };

                    countries.Add(_country);
                }
            }
            return countries.OrderBy(p => p.Text).ToList();
        }
        public static string GetCountryNameByCode(int countryCode)
        {
            List<BaseClass> countries = new List<BaseClass>();
            countries = GetCountryList();
            if (countries.Any())
            {
                var firstOrDefault = countries.FirstOrDefault(x => x.IntValue == countryCode);
                if (firstOrDefault != null)
                    return firstOrDefault.Text;
            }
            return string.Empty;
        }
        public static Phone ParsePhoneNumber(string phone)
        {
            if (!string.IsNullOrEmpty(phone))
            {
                string[] part = phone.Split(' ');
                if (part != null)
                {
                    if (part.Length == 3)
                    {
                        return new Phone() { CountryCode = part[0], AreaCode = part[1], PhoneNumber = part[2] };
                    }
                    if (part.Length >= 4)
                    {
                        return new Phone() { CountryCode = part[0], AreaCode = part[1], PhoneNumber = part[2], Ext = part[3] };
                    }

                }
            }
            return new Phone() { CountryCode = "+65", AreaCode = string.Empty, PhoneNumber = string.Empty, Ext = string.Empty };
        }
        public Phone ParseNumberPhone(string phone)
        {
            if (!string.IsNullOrEmpty(phone))
            {
                string[] part = phone.Trim().Split(' ');
                if (part != null)
                {
                    if (part.Length == 3)
                    {
                        return new Phone() { CountryCode = part[0], AreaCode = part[1], PhoneNumber = part[2] };
                    }
                    if (part.Length >= 4)
                    {
                        return new Phone() { CountryCode = part[0], AreaCode = part[1], PhoneNumber = part[2], Ext = part[3] };
                    }

                }
            }
            return new Phone() { CountryCode = "+65", AreaCode = string.Empty, PhoneNumber = string.Empty, Ext = string.Empty };
        }
        public string UniteNumberPhone(string[] parts)
        {
            if (parts != null && parts.Length == 3)
            {
                string Number = string.Format("{0} {1} {2}", parts[0].Trim(), parts[1].Trim(), parts[2].Trim());
                if (!string.IsNullOrEmpty(Number))
                    return Number;
            }
            if (parts != null && parts.Length == 4)
            {
                string Number = string.Format("{0} {1} {2} {3}", parts[0].Trim(), parts[1].Trim(), parts[2].Trim(), parts[3].Trim());
                if (!string.IsNullOrEmpty(Number))
                    return Number;
            }
            return string.Empty;
        }
        public static string MergeNumberPhone(Phone phone)
        {
            if (phone != null)
            {
                string Number = string.Format("{0} {1} {2} {3}",
                                                phone.CountryCode != null ? phone.CountryCode.Trim() : "",
                                                phone.AreaCode != null ? phone.AreaCode.Trim() : "",
                                                phone.PhoneNumber != null ? phone.PhoneNumber.Trim() : "",
                                                phone.Ext != null ? phone.Ext.Trim() : "");
                if (!string.IsNullOrEmpty(Number))
                    return Number.Trim();
            }
            return string.Empty;
        }
        public DateTime? MergeDate(DateAndTime date)
        {
            if (date != null)
            {
                if (date.date != null)
                {
                    DateTime dt = new DateTime(date.date.Value.Year, date.date.Value.Month, date.date.Value.Day, date.hour, date.minute, 0, 0);
                    return dt;
                }


            }
            return null;
        }
        public List<BaseClass> Get24HourDateFormat()
        {
            DateTime start = DateTime.ParseExact("00:00", "HH:mm", null);
            DateTime end = DateTime.ParseExact("23:59", "HH:mm", null);
            int interval = 60;
            var lstTimeIntervals = new List<BaseClass>();
            for (DateTime i = start; i <= end; i = i.AddMinutes(interval))
                lstTimeIntervals.Add(new BaseClass() { IntValue = int.Parse(i.ToString("HH")), Text = i.ToString("hh tt") });
            return lstTimeIntervals;
        }
        public List<BaseClass> GetMinuteDateFormat()
        {
            DateTime start = DateTime.ParseExact("00:00", "HH:mm", null);
            DateTime end = DateTime.ParseExact("01:00", "HH:mm", null);
            int interval = 60;
            var lstTimeIntervals = new List<BaseClass>();
            for (DateTime i = start; i < end; i = i.AddSeconds(interval))
                lstTimeIntervals.Add(new BaseClass() { IntValue = int.Parse(i.ToString("mm")), Text = i.ToString("mm") });
            return lstTimeIntervals;
        }
        public DateTime ConvertFromUnixTimestamp(double timestamp)
        {
            var origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return origin.AddSeconds(timestamp);
        }
        public double ConvertToUnixTimestamp(DateTime date)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            TimeSpan diff = date - origin;
            return Math.Floor(diff.TotalSeconds);
        }
        public static string GetValidFileName(string rawFileName)
        {
            string fileName = rawFileName;
            string specialChars = @"@$^\/:*?""<>|#%&.{}~";
            Array.ForEach(specialChars.ToCharArray(), specialChar => fileName = fileName.Replace(specialChar, ' '));
          //  fileName = fileName.Replace(" ", string.Empty);
            return fileName;
        }

        #region Email
        //if send mail successfully
        //return 1
        //else return 0

        public static bool SendMail(string fromAddress, string toAddress, string subject,
            string body, string cc, string host, int port, string username, string password,
            string from, bool enableSsl, bool useDefaultCredential = false,
            string pathImageEmbedded = "", List<Attachment> attachment = null, string fromName = "", string bcc="")
        {
            bool flag;
            string str2 = string.IsNullOrEmpty(fromName) ? "Administrator" : fromName;
            string str3 = (!string.IsNullOrEmpty(fromAddress) ? fromAddress : from);
            try
            {
                MailMessage mailMessage = Utilities.BuildMailMessage(str2, str3, toAddress, subject, body, pathImageEmbedded, attachment);
                if (!string.IsNullOrEmpty(cc))
                {
                    string[] strArrays = cc.Split(new char[] { ',' });
                    if ((strArrays == null ? false : strArrays.Any<string>()))
                    {
                        string[] strArrays1 = strArrays;
                        for (int i = 0; i < (int)strArrays1.Length; i++)
                        {
                            string str4 = strArrays1[i];
                            mailMessage.CC.Add(new MailAddress(str4));
                        }
                    }
                }
                if (!string.IsNullOrEmpty(bcc))
                {
                    string[] bccArrays = bcc.Split(new char[] { ',' });
                    if ((bccArrays == null ? false : bccArrays.Any<string>()))
                    {
                        string[] bccArrays1 =bccArrays;
                        for (int i = 0; i < (int)bccArrays1.Length; i++)
                        {
                            string bcc4 = bccArrays1[i];
                            mailMessage.Bcc.Add(new MailAddress(bcc4));
                        }
                    }
                }
                SmtpClient smtpClient = new SmtpClient(host, port);
                try
                {
                    smtpClient.Credentials = new NetworkCredential(username, password);
                    smtpClient.EnableSsl = enableSsl;
                    smtpClient.Send(mailMessage);
                    mailMessage.Dispose();
                }
                finally
                {
                    if (smtpClient != null)
                    {
                        ((IDisposable)smtpClient).Dispose();
                    }
                }
                flag = true;
            }
            catch (Exception exception)
            {
                flag = false;
            }
            return flag;
        }

        public static bool SendMail(string fromAddress, string toAddress, string subject,
            string body, string cc, string sectionName,
            string pathImageEmbedded = "", List<Attachment> attachment = null, string fromName = "", string bcc="")
        {
            bool flag;
            string str = string.Concat("mailSettings/", sectionName);
            SmtpSection section = (SmtpSection)ConfigurationManager.GetSection(str);
            string host = section.Network.Host;
            int port = section.Network.Port;
            string userName = section.Network.UserName;
            string str1 = Sercurity.Decrypt(section.Network.Password);
            string from = section.From;
            bool enableSsl = section.Network.EnableSsl;
            string str2 = string.IsNullOrEmpty(fromName) ? "Administrator" : fromName; 
            string str3 = (!string.IsNullOrEmpty(fromAddress) ? fromAddress : from);
            try
            {
                MailMessage mailMessage = Utilities.BuildMailMessage(str2, str3, toAddress, subject, body, pathImageEmbedded, attachment);
                if (!string.IsNullOrEmpty(cc))
                {
                    string[] strArrays = cc.Split(new char[] { ',' });
                    if ((strArrays == null ? false : strArrays.Any<string>()))
                    {
                        string[] strArrays1 = strArrays;
                        for (int i = 0; i < (int)strArrays1.Length; i++)
                        {
                            string str4 = strArrays1[i];
                            mailMessage.CC.Add(new MailAddress(str4));
                        }
                    }
                }
                if (!string.IsNullOrEmpty(bcc))
                {
                    string[] bccArrays = bcc.Split(new char[] { ',' });
                    if ((bccArrays == null ? false : bccArrays.Any<string>()))
                    {
                        string[] bccArrays1 = bccArrays;
                        for (int i = 0; i < (int)bccArrays1.Length; i++)
                        {
                            string bcc4 = bccArrays1[i];
                            mailMessage.Bcc.Add(new MailAddress(bcc4));
                        }
                    }
                }
                SmtpClient smtpClient = new SmtpClient(host, port);
                try
                {
                    smtpClient.Credentials = new NetworkCredential(userName, str1);
                    smtpClient.EnableSsl = enableSsl;
                    smtpClient.Send(mailMessage);
                    mailMessage.Dispose();
                }
                finally
                {
                    if (smtpClient != null)
                    {
                        ((IDisposable)smtpClient).Dispose();
                    }
                }
                flag = true;
            }
            catch (Exception exception)
            {
                flag = false;
            }
            return flag;
        }

        public static bool SendMail(string fromAddress, string toAddress, string subject,
            string body, string cc, string host, int port, string username, string password,
            string from, bool enableSsl, bool useDefaultCredential = false,
            byte[] image = null, List<Attachment> attachments = null, string fromName = "", string bcc = "")
        {
            bool flag;
            string displayName = string.IsNullOrEmpty(fromName) ? "Administrator" : fromName;
            string str3 = (!string.IsNullOrEmpty(fromAddress) ? fromAddress : from);
            try
            {
                MailMessage mailMessage = new MailMessage()
                {
                    From = new MailAddress(str3, displayName),
                    Subject = subject,
                    BodyEncoding = Encoding.UTF8,
                    IsBodyHtml = true
                };
                if (image != null && image.Length > 0)
                {
                    // Create the HTML view
                    AlternateView htmlView = AlternateView.CreateAlternateViewFromString(
                                                                body,
                                                                Encoding.UTF8,
                                                                MediaTypeNames.Text.Html);
                    // Create a plain text message for client that don't support HTML
                    AlternateView plainView = AlternateView.CreateAlternateViewFromString(
                                                                Regex.Replace(body,
                                                                            "<[^>]+?>",
                                                                            string.Empty),
                                                                Encoding.UTF8,
                                                                MediaTypeNames.Text.Plain);
                    // Get image stream from assembly resource 
                    //var resourceStream = GetResourceStream(pathImage);
                    Stream resourceStream = new MemoryStream(image);

                    string mediaType = MediaTypeNames.Image.Jpeg;
                    LinkedResource img = new LinkedResource(resourceStream, mediaType);

                    // Make sure you set all these values!!!
                    img.ContentId = "ImageEmbedded";
                    img.ContentType.MediaType = mediaType;
                    img.TransferEncoding = TransferEncoding.Base64;
                    img.ContentType.Name = img.ContentId;
                    img.ContentLink = new Uri("cid:" + img.ContentId);
                    htmlView.LinkedResources.Add(img);
                    //////////////////////////////////////////////////////////////
                    mailMessage.AlternateViews.Add(plainView);
                    mailMessage.AlternateViews.Add(htmlView);
                }
                else
                {
                    mailMessage.Body = body;
                }
                mailMessage.IsBodyHtml = true;

                //set the priority
                mailMessage.Priority = MailPriority.Normal;
                string[] strArrayTo = toAddress.Split(new char[] { ';' });
                for (int i = 0; i < (int)strArrayTo.Length; i++)
                {
                    string str = strArrayTo[i];
                    mailMessage.To.Add(new MailAddress(str));
                }
                if (attachments != null)
                {
                    foreach (Attachment attachment in attachments)
                    {
                        mailMessage.Attachments.Add(attachment);
                    }
                }
                if (!string.IsNullOrEmpty(cc))
                {
                    string[] strArrays = cc.Split(new char[] { ',' });
                    if ((strArrays == null ? false : strArrays.Any<string>()))
                    {
                        string[] strArrays1 = strArrays;
                        for (int i = 0; i < (int)strArrays1.Length; i++)
                        {
                            string str4 = strArrays1[i];
                            mailMessage.CC.Add(new MailAddress(str4));
                        }
                    }
                }
                if (!string.IsNullOrEmpty(bcc))
                {
                    string[] bccArrays = bcc.Split(new char[] { ',' });
                    if ((bccArrays == null ? false : bccArrays.Any<string>()))
                    {
                        string[] bccArrays1 = bccArrays;
                        for (int i = 0; i < (int)bccArrays1.Length; i++)
                        {
                            string bcc4 = bccArrays1[i];
                            mailMessage.Bcc.Add(new MailAddress(bcc4));
                        }
                    }
                }
                SmtpClient smtpClient = new SmtpClient(host, port);
                try
                {
                    smtpClient.Credentials = new NetworkCredential(username, password);
                    smtpClient.EnableSsl = enableSsl;
                    smtpClient.Send(mailMessage);
                    mailMessage.Dispose();
                }
                finally
                {
                    ((IDisposable)smtpClient).Dispose();
                }
                flag = true;
            }
            catch (Exception exception)
            {
                flag = false;
            }
            return flag;
        }
        private static MailMessage BuildMailMessage(string ADMIN_MAIL_NAME, string fromAddress, string toAddress, string subject, string body, string pathImageEmbedded, List<Attachment> attachments = null)
        {
            MailMessage mailMessage = new MailMessage()
            {
                From = new MailAddress(fromAddress, ADMIN_MAIL_NAME),
                Subject = subject,
                BodyEncoding = Encoding.UTF8,
                IsBodyHtml = true
            };
            MailMessage mailMessage1 = mailMessage;
            if (string.IsNullOrEmpty(pathImageEmbedded))
            {
                mailMessage1.Body = body;
            }
            else
            {
                AlternateView alternateView = AlternateView.CreateAlternateViewFromString(body, null, "text/html");
                LinkedResource linkedResource = new LinkedResource(pathImageEmbedded, "image/jpeg")
                {
                    ContentId = "ImageEmbedded"
                };
                linkedResource.ContentType.MediaType = "image/jpeg";
                linkedResource.TransferEncoding = TransferEncoding.Base64;
                linkedResource.ContentType.Name = linkedResource.ContentId;
                linkedResource.ContentLink = new Uri(string.Concat("cid:", linkedResource.ContentId));
                alternateView.LinkedResources.Add(linkedResource);
                mailMessage1.AlternateViews.Add(alternateView);
            }
            string[] strArrays = toAddress.Split(new char[] { ';' });
            for (int i = 0; i < (int)strArrays.Length; i++)
            {
                string str = strArrays[i];
                mailMessage1.To.Add(new MailAddress(str));
            }
            if (attachments != null)
            {
                foreach (Attachment attachment in attachments)
                {
                    mailMessage1.Attachments.Add(attachment);
                }
            }
            return mailMessage1;
        }

        public static string CreateRandomCode()
        {
            string _allowedChars = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789";
            //string _allowedChars = _allowedChars;
            Random randNum = new Random();
            char[] chars = new char[6];
            int allowedCharCount = _allowedChars.Length;

            for (int i = 0; i < 6; i++)
            {
                chars[i] = _allowedChars[(int)((_allowedChars.Length) * randNum.NextDouble())];
            }

            return new string(chars);
        }

        public static string CreatePasswordHash(string password, string userid)
        {
            string saltkey = string.IsNullOrWhiteSpace(userid) ? string.Empty : userid.Substring(0, 4);
            string saltAndPassword = password + "{" + saltkey + "}";
            return HashPasswordForStoringInConfigFile(saltAndPassword);
        }

        private static string HashPasswordForStoringInConfigFile(string password)
        {
            HashAlgorithm algorithm;
            if (password == null)
            {
                throw new ArgumentNullException("password");
            }
            algorithm = MD5.Create();
            //Convert.ToBase64String(algorithm.ComputeHash(Encoding.UTF8.GetBytes(password)), 0);
            return HexStringFromBytes(algorithm.ComputeHash(Encoding.UTF8.GetBytes(password)));
        }

        public static string HexStringFromBytes(byte[] bytes)
        {
            var sb = new StringBuilder();
            foreach (byte b in bytes)
            {
                var hex = b.ToString("x2");
                sb.Append(hex);
            }
            return sb.ToString();
        }

        #endregion

        #region | For Calendar Scheduler |
        public List<object> LoadFrequencyChoices()
        {
            return new List<object>()
            {
                new {ID = 1, Name = "Daily"},
                new {ID = 2, Name = "Weekly"},
                new {ID = 4, Name = "Monthly"},
                new {ID = 16, Name = "Annually"}
            };

        }

        public List<object> LoadDaysOfWeekChoices()
        {
            var daysOfWeek = new List<object>()
            {
                new { Name = "Sat" },
                new { Name = "Sun" },
                new { Name = "Mon" },
                new { Name = "Tue" },
                new { Name = "Wed" },
                new { Name = "Thu" },
                new { Name = "Fri" }
            };

            return daysOfWeek;
        }
        public static List<int> GetDates(int year, int month)
        {
            return Enumerable.Range(1, DateTime.DaysInMonth(year, month))  // Days: 1, 2 ... 31 etc.
                             .Select(day => new DateTime(year, month, day).Day) // Map each day to a date
                             .ToList(); // Load dates into a list
        }
        #endregion

        // Returns the human-readable file size for an arbitrary, 64-bit file size
        //  The default format is "0.### XB", e.g. "4.2 KB" or "1.434 GB"
        public static string GetSizeReadable(long i)
        {
            string sign = (i < 0 ? "-" : "");
            double readable = (i < 0 ? -i : i);
            string suffix;
            if (i >= 0x1000000000000000) // Exabyte
            {
                suffix = "EB";
                readable = (double)(i >> 50);
            }
            else if (i >= 0x4000000000000) // Petabyte
            {
                suffix = "PB";
                readable = (double)(i >> 40);
            }
            else if (i >= 0x10000000000) // Terabyte
            {
                suffix = "TB";
                readable = (double)(i >> 30);
            }
            else if (i >= 0x40000000) // Gigabyte
            {
                suffix = "GB";
                readable = (double)(i >> 20);
            }
            else if (i >= 0x100000) // Megabyte
            {
                suffix = "MB";
                readable = (double)(i >> 10);
            }
            else if (i >= 0x400) // Kilobyte
            {
                suffix = "KB";
                readable = (double)i;
            }
            else
            {
                return i.ToString(sign + "0 B"); // Byte
            }
            readable = readable / 1024;

            return sign + readable.ToString("0.### ") + suffix;
        }
        public static string CreateDateSuffix(DateTime date)
        {
            // Get day...
            var day = date.Day;

            // Get day modulo...
            var dayModulo = day % 10;

            // Convert day to string...
            var suffix = day.ToString(CultureInfo.InvariantCulture);

            // Combine day with correct suffix...
            suffix += (day == 11 || day == 12 || day == 13) ? "th" :
                (dayModulo == 1) ? "st" :
                (dayModulo == 2) ? "nd" :
                (dayModulo == 3) ? "rd" :
                "th";

            // Return result...
            return suffix;
        }

        #region SMS
        // Use your account SID and authentication token instead
        // of the placeholders shown here.
        private static string accountSID = ConfigurationManager.AppSettings["AccountSID"];
        private static string authToken = ConfigurationManager.AppSettings["AuthToken"];
        private static string fromNumber = ConfigurationManager.AppSettings["FromNumber"];

        //public static void SendSMS(string toNumber, string content)
        //{
        //    try
        //    {
        //        //Testing purpose
        //        if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings["SMSTo"]))
        //        {
        //            toNumber = ConfigurationManager.AppSettings["SMSTo"];
        //        }

        //        //  if (string.IsNullOrEmpty(accountSID) || string.IsNullOrEmpty(authToken) || string.IsNullOrEmpty(fromNumber))
        //        // LoggingHelper.WriteLog(string.Format("Invalid service information, please check in app.config. AccountSID: {0}. AuthToken: {1}. FromNumber: {2}", accountSID, authToken, fromNumber), BusinessLogic.EIP._0001.LogType.Warning);

        //        TwilioRestClient client;
        //        client = new TwilioRestClient(accountSID, authToken);

        //        // Send an SMS message.
        //        Message result = client.SendMessage(fromNumber, toNumber, content);

        //        //if (result.RestException != null)
        //        //{
        //        //    //an exception occurred making the REST call
        //        //    LoggingHelper.WriteLog(result.RestException.Message, BusinessLogic.EIP._0001.LogType.Error);
        //        //}
        //        //else
        //        //{
        //        //    LoggingHelper.WriteLog(string.Format("Message sent to {0} successfully.", toNumber));
        //        //}
        //    }
        //    catch (Exception ex)
        //    {
        //        // LoggingHelper.WriteLog(ex);
        //    }
        //}
        #endregion

        #region Convert Date To Timezone
        public static string TimeZoneName
        {
            get
            {
                return ConfigurationManager.AppSettings["TimeZoneName"];
            }
        }

        /// <summary>
        /// A conversion from DateTime.Now to current local TimeZone DateTime
        /// </summary>
        public static DateTime LocalDateTimeNow
        {
            get
            {
                return ConvertUCTDateTimeToLocal(DateTime.Now);
                //return DateTime.Now;
            }
        }

        /// <summary>
        /// Convert current local TimeZone DateTime to UTC DateTime
        /// </summary>
        /// <param name="localTime"></param>
        /// <returns></returns>
        public static DateTime ConvertLocalDateTimeToUTC(DateTime localTime)
        {
            TimeZoneInfo localTimeZone = TimeZoneInfo.FindSystemTimeZoneById(TimeZoneName);
            DateTime uctTime = TimeZoneInfo.ConvertTimeToUtc(localTime, localTimeZone);

            return uctTime;
        }

        /// <summary>
        /// Convert Current UTC DateTime to current local TimeZone
        /// </summary>
        /// <param name="utcTime"></param>
        /// <returns></returns>
        public static DateTime ConvertUCTDateTimeToLocal(DateTime utcTime)
        {
            TimeZoneInfo localTimeZone = TimeZoneInfo.FindSystemTimeZoneById(TimeZoneName);
            DateTime uctTime = TimeZoneInfo.ConvertTime(utcTime, localTimeZone);

            return uctTime;
        }
        #endregion

        public static string UperCaseWord(string text)
        {
            string ret = string.Empty;
            if (!string.IsNullOrEmpty(text))
            {
                string[] arrs = text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string item in arrs)
                {
                    ret += item[0].ToString().ToUpper() + (item.Length >= 2 ? item.Substring(1).ToLower() : string.Empty) + " ";
                }

            }
            return ret;
        }
        public static String GetFileName(String hrefLink)
        {
            return Path.GetFileName(Uri.UnescapeDataString(hrefLink).Replace("/", "\\"));
        }
        public static string GetContentType(string fileExtension)
        {
            if (string.IsNullOrEmpty(fileExtension))
                return string.Empty;

            string contentType = string.Empty;
            switch (fileExtension)
            {
                case ".htm":
                case ".html":
                    contentType = "text/HTML";
                    break;

                case ".txt":
                    contentType = "text/plain";
                    break;

                case ".doc":
                case ".rtf":
                case ".docx":
                    contentType = "Application/msword";
                    break;

                case ".xls":
                case ".xlsx":
                    contentType = "Application/x-msexcel";
                    break;

                case ".jpg":
                case ".jpeg":
                    contentType = "image/jpeg";
                    break;

                case ".gif":
                    contentType = "image/GIF";
                    break;

                case ".pdf":
                    contentType = "application/pdf";
                    break;
            }

            return contentType;
        }
        public static string MonthName(int m)
        {
            string res;
            switch (m)
            {
                case 1:
                    res = "Jan";
                    break;
                case 2:
                    res = "Feb";
                    break;
                case 3:
                    res = "Mar";
                    break;
                case 4:
                    res = "Apr";
                    break;
                case 5:
                    res = "May";
                    break;
                case 6:
                    res = "Jun";
                    break;
                case 7:
                    res = "Jul";
                    break;
                case 8:
                    res = "Aug";
                    break;
                case 9:
                    res = "Sep";
                    break;
                case 10:
                    res = "Oct";
                    break;
                case 11:
                    res = "Nov";
                    break;
                case 12:
                    res = "Dec";
                    break;
                default:
                    res = "Jan";
                    break;
            }
            return res;
        }
        public static byte[] ConvertToBytes(HttpPostedFileBase image)
        {
            byte[] imageBytes = null;
            BinaryReader reader = new BinaryReader(image.InputStream);
            imageBytes = reader.ReadBytes((int)image.ContentLength);
            return imageBytes;
        }
    }

    public class Phone
    {
        [UIHint("CountryCode")]
        public string CountryCode { get; set; }
        public string AreaCode { get; set; }
        public string PhoneNumber { get; set; }
        public string Ext { get; set; }

        public static Phone Parse(string phone)
        {
            if (!string.IsNullOrEmpty(phone))
            {
                string[] part = phone.Split(' ');
                if (part != null)
                {
                    if (part.Length == 3)
                    {
                        return new Phone() { CountryCode = part[0], AreaCode = part[1], PhoneNumber = part[2] };
                    }
                    if (part.Length >= 4)
                    {
                        return new Phone() { CountryCode = part[0], AreaCode = part[1], PhoneNumber = part[2], Ext = part[3] };
                    }

                }
            }
            return new Phone() { CountryCode = "+65", AreaCode = string.Empty, PhoneNumber = string.Empty, Ext = string.Empty };
        }
        public string ToString()
        {
            if (this != null)
            {
                string Number = string.Format("{0} {1} {2} {3}",
                                                this.CountryCode != null ? this.CountryCode.Trim() : "",
                                                this.AreaCode != null ? this.AreaCode.Trim() : "",
                                                this.PhoneNumber != null ? this.PhoneNumber.Trim() : "",
                                                this.Ext != null ? this.Ext.Trim() : "");
                if (!string.IsNullOrEmpty(Number))
                    return Number.Trim();
            }
            return string.Empty;
        }
    }

    public class BaseClass
    {
        public Nullable<int> IntValue { get; set; }
        public string Str_Value { get; set; }
        public string Text { get; set; }
    }

    public class DateAndTime
    {
        public DateAndTime()
        {

        }
        public DateAndTime(DateTime? d, int h, int m)
        {
            date = d;
            hour = h;
            minute = m;
        }

        public DateTime? date { get; set; }
        public int hour { get; set; }
        public int minute { get; set; }

        public static List<BaseClass> Get24HourDateFormat()
        {
            DateTime start = DateTime.ParseExact("00:00", "HH:mm", null);
            DateTime end = DateTime.ParseExact("23:59", "HH:mm", null);
            int interval = 60;
            var lstTimeIntervals = new List<BaseClass>();
            for (DateTime i = start; i <= end; i = i.AddMinutes(interval))
                lstTimeIntervals.Add(new BaseClass() { IntValue = int.Parse(i.ToString("HH")), Text = i.ToString("hh tt") });
            return lstTimeIntervals;
        }

        public static List<BaseClass> GetMinuteDateFormat()
        {
            DateTime start = DateTime.ParseExact("00:00", "HH:mm", null);
            DateTime end = DateTime.ParseExact("01:00", "HH:mm", null);
            int interval = 60;
            var lstTimeIntervals = new List<BaseClass>();
            for (DateTime i = start; i < end; i = i.AddSeconds(interval))
                lstTimeIntervals.Add(new BaseClass() { IntValue = int.Parse(i.ToString("mm")), Text = i.ToString("mm") });
            return lstTimeIntervals;
        }

        public DateTime? ToDateTime()
        {
            if (date != null)
            {
                DateTime dt = new DateTime(date.Value.Year, date.Value.Month, date.Value.Day, hour, minute, 0, 0);
                return dt;
            }
            return null;
        }

        public static DateTime? MergeDate(DateAndTime date)
        {
            if (date != null)
            {
                if (date.date != null)
                {
                    DateTime dt = new DateTime(date.date.Value.Year, date.date.Value.Month, date.date.Value.Day, date.hour, date.minute, 0, 0);
                    return dt;
                }


            }
            return null;
        }

        #region Utils
        public static string CreatePasswordHash(string password, string userid)
        {
            string saltkey = string.IsNullOrWhiteSpace(userid) ? string.Empty : userid.Substring(0, 4);
            string saltAndPassword = password + "{" + saltkey + "}";
            return HashPasswordForStoringInConfigFile(saltAndPassword);
        }

        private static string HashPasswordForStoringInConfigFile(string password)
        {
            HashAlgorithm algorithm;
            if (password == null)
            {
                throw new ArgumentNullException("password");
            }
            algorithm = MD5.Create();
            return HexStringFromBytes(algorithm.ComputeHash(Encoding.UTF8.GetBytes(password)));
        }

        public static string HexStringFromBytes(byte[] bytes)
        {
            var sb = new StringBuilder();
            foreach (byte b in bytes)
            {
                var hex = b.ToString("x2");
                sb.Append(hex);
            }
            return sb.ToString();
        }

        public static string CreateRandomCode()
        {
            string _allowedChars = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789";
            //string _allowedChars = _allowedChars;
            Random randNum = new Random();
            char[] chars = new char[6];
            int allowedCharCount = _allowedChars.Length;

            for (int i = 0; i < 6; i++)
            {
                chars[i] = _allowedChars[(int)((_allowedChars.Length) * randNum.NextDouble())];
            }

            return new string(chars);
        }

        #endregion
    }
}
