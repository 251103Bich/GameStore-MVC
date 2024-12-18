using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GameStoreV1._0.DB;
using GameStoreV1._0.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Net.Mail;
using System.Collections.Specialized;
using Microsoft.AspNetCore.Http.Extensions;
namespace GameStoreV1._0.Controllers
{

    public class ProfileController : Controller
    {
        private readonly ApplicationDBContext _db;

        public ProfileController(ApplicationDBContext db)
        {
            this._db = db;
        }

        public IActionResult Index()
        {
            // TODO: Your code here
            return View("Index", "Home");
        }

        public static string GetMd5(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                // Convert the input string to a byte array and compute the hash.
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to a hexadecimal string.
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2"));
                }

                return sb.ToString();
            }
        }

        public IActionResult RegisterSeller()
        {
            // TODO: Your code here
            return View();
        }

        [HttpPost]
        public IActionResult RegisterSeller(Profile cus)
        {
            DateTime ngayHienTai = DateTime.Now;
            Profile temp = this._db.profile.Find(cus.username);
            if (temp == null)
            {
                Profile obj = new Profile();
                obj.username = cus.username.Trim();
                obj.password = GetMd5(cus.password.Trim());
                obj.email = cus.email.Trim();
                obj.gender = "";
                obj.birthday = null;
                obj.money = 0;
                obj.type = "2";
                obj.status = "5";
                obj.date = ngayHienTai;
                // TODO: Your code here

                var user = this._db.profile.FirstOrDefault(m => m.email == cus.email);
                if (user != null)
                {
                    ViewBag.Exist = "Email is exist in System";

                    return View();
                }

                this._db.profile.Add(obj);
                this._db.SaveChanges();
                ViewBag.Success = "In Your Mail Click Link Confirm";

                RandomStringGenerator randomStringGenerator = new RandomStringGenerator();
                string token = randomStringGenerator.GenerateRandomString(50);
                var user1 = this._db.profile.FirstOrDefault(m => m.token == token);
                while (user1 != null)
                {
                    token = randomStringGenerator.GenerateRandomString(50);
                    user1 = this._db.profile.FirstOrDefault(m => m.token == token);
                }
                user = this._db.profile.FirstOrDefault(m => m.email == cus.email);
                user.token = token;
                this._db.SaveChanges();
                string url = "https://localhost:5001/Profile/Register1" + "/" + token;
                // Cấu hình thông tin email
                var fromAddress = new MailAddress("luan251103@gmail.com", "Shopping");
                var toAddress = new MailAddress(cus.email, "Recipient Name");
                const string fromPassword = "hyqy eywi fmtq iwry";
                const string subject = "Xác nhận tài khoản";

                // Cấu hình client SMTP
                var smtpClient = new SmtpClient
                {
                    Host = "smtp.gmail.com", // Địa chỉ SMTP server
                    Port = 587, // Port SMTP server
                    EnableSsl = true, // Kích hoạt SSL
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
                };

                // Tạo message
                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = @"
    <!DOCTYPE html>
    <html lang=""en"">
    <head>
        <meta charset=""UTF-8"">
        <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
        <title>Password Reset</title>
        <style>
            body {
                font-family: Arial, sans-serif;
                margin: 0;
                padding: 0;
                background-color: #f9f9f9;
            }
            .container {
                max-width: 600px;
                margin: 0 auto;
                padding: 20px;
                background-color: #ffffff;
                border-radius: 10px;
                box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
            }
            h1 {
                color: #333333;
                text-align: center;
            }
            p {
                color: #666666;
                font-size: 16px;
                line-height: 1.6;
            }
            a {
                display: inline-block;
                padding: 10px 20px;
                background-color: #FAFAD2;
                color: #ffffff;
                text-decoration: none;
                border-radius: 5px;
                margin-top: 20px;
            }
            a:hover {
                background-color: #0056b3;
            }
            .footer {
                margin-top: 20px;
                text-align: center;
                color: #999999;
                font-size: 14px;
            }
        </style>
    </head>
    <body>
        <div class=""container"">
            <h1>[Wong's Store]</h1>
            <p>Hi " + user.username + @",</p>
            <p>You recently requested to reset your password for your [Product Name] account. Use the button below to reset it. This password reset is only valid for the next 24 hours.</p>
            <p><a href=""" + url + @""">Confirm Mail</a></p>
            <p>Thanks,<br>The [Wong's Store] Team</p>
            <p>If you’re having trouble with the button above, Enter URL below:</p>
            <p><a href=""https://www.facebook.com/profile.php?id=100027614479047"">Link Here</a></p>
            <p class=""footer"">© 2019 [Wong's Store]. All rights reserved.<br>[Wong's Store, LLC]<br>1234 Street Rd.<br>Suite 1234</p>
        </div>
    </body>
    </html>
",

                    IsBodyHtml = true
                })
                {
                    try
                    {
                        smtpClient.Send(message);
                        Console.WriteLine("Email xác nhận đã được gửi đi thành công.");
                        ViewBag.Success = "Email did give for you!";
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Lỗi khi gửi email: {ex.Message}");
                    }
                }
                return View();

            }
            else
            {
                ViewBag.Exist = "Account is exist";
            }
            return View();

        }


        public IActionResult Register()
        {
            // TODO: Your code here
            return View();
        }

        public IActionResult Register1(string id)
        {
            // TODO: Your code here
            Profile profile = this._db.profile.FirstOrDefault(m => m.token == id);
            if (profile != null)
            {
                if (profile.status == "5" && profile.type == "2")
                {
                    profile.status = "3";
                    profile.token = null;
                    this._db.SaveChanges();
                    TempData["RegisterSeller"] = "Register Seller Success! Waiting Admin Accept Your Account";
                    return RedirectToAction("Success");
                }
                profile.status = "1";
                profile.token = null;
                this._db.SaveChanges();
                return RedirectToAction("Success");
            }
            return NotFound();
        }




        [HttpPost]
        public IActionResult Register(Profile cus)
        {
            DateTime ngayHienTai = DateTime.Now;
            Profile temp = this._db.profile.Find(cus.username);
            if (temp == null)
            {
                Profile obj = new Profile();
                obj.username = cus.username.Trim();
                obj.password = GetMd5(cus.password.Trim());
                obj.email = cus.email.Trim();
                obj.gender = "";
                obj.birthday = null;
                obj.money = 0;
                obj.type = "1";
                obj.status = "5";
                obj.date = ngayHienTai;
                // TODO: Your code here

                var user = this._db.profile.FirstOrDefault(m => m.email == cus.email);
                if (user != null)
                {
                    ViewBag.Exist = "Email is exist in System";

                    return View();
                }

                this._db.profile.Add(obj);
                this._db.SaveChanges();
                ViewBag.Success = "In Your Mail Click Link Confirm";

                RandomStringGenerator randomStringGenerator = new RandomStringGenerator();
                string token = randomStringGenerator.GenerateRandomString(50);
                var user1 = this._db.profile.FirstOrDefault(m => m.token == token);
                while (user1 != null)
                {
                    token = randomStringGenerator.GenerateRandomString(50);
                    user1 = this._db.profile.FirstOrDefault(m => m.token == token);
                }
                user = this._db.profile.FirstOrDefault(m => m.email == cus.email);
                user.token = token;
                this._db.SaveChanges();
                string url = "https://localhost:5001/Profile/Register1" + "/" + token;
                // Cấu hình thông tin email
                var fromAddress = new MailAddress("luan251103@gmail.com", "Shopping");
                var toAddress = new MailAddress(cus.email, "Recipient Name");
                const string fromPassword = "hyqy eywi fmtq iwry";
                const string subject = "Xác nhận tài khoản";

                // Cấu hình client SMTP
                var smtpClient = new SmtpClient
                {
                    Host = "smtp.gmail.com", // Địa chỉ SMTP server
                    Port = 587, // Port SMTP server
                    EnableSsl = true, // Kích hoạt SSL
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
                };

                // Tạo message
                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = @"
    <!DOCTYPE html>
    <html lang=""en"">
    <head>
        <meta charset=""UTF-8"">
        <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
        <title>Password Reset</title>
        <style>
            body {
                font-family: Arial, sans-serif;
                margin: 0;
                padding: 0;
                background-color: #f9f9f9;
            }
            .container {
                max-width: 600px;
                margin: 0 auto;
                padding: 20px;
                background-color: #ffffff;
                border-radius: 10px;
                box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
            }
            h1 {
                color: #333333;
                text-align: center;
            }
            p {
                color: #666666;
                font-size: 16px;
                line-height: 1.6;
            }
            a {
                display: inline-block;
                padding: 10px 20px;
                background-color: #FAFAD2;
                color: #ffffff;
                text-decoration: none;
                border-radius: 5px;
                margin-top: 20px;
            }
            a:hover {
                background-color: #0056b3;
            }
            .footer {
                margin-top: 20px;
                text-align: center;
                color: #999999;
                font-size: 14px;
            }
        </style>
    </head>
    <body>
        <div class=""container"">
            <h1>[Wong's Store]</h1>
            <p>Hi " + user.username + @",</p>
            <p>You recently requested to reset your password for your [Product Name] account. Use the button below to reset it. This password reset is only valid for the next 24 hours.</p>
            <p><a href=""" + url + @""">Confirm Mail</a></p>
            <p>Thanks,<br>The [Wong's Store] Team</p>
            <p>If you’re having trouble with the button above, Enter URL below:</p>
            <p><a href=""https://www.facebook.com/profile.php?id=100027614479047"">Link Here</a></p>
            <p class=""footer"">© 2019 [Wong's Store]. All rights reserved.<br>[Wong's Store, LLC]<br>1234 Street Rd.<br>Suite 1234</p>
        </div>
    </body>
    </html>
",

                    IsBodyHtml = true
                })
                {
                    try
                    {
                        smtpClient.Send(message);
                        Console.WriteLine("Email xác nhận đã được gửi đi thành công.");
                        ViewBag.Success = "Email did give for you!";
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Lỗi khi gửi email: {ex.Message}");
                    }
                }
                return View();

            }
            else
            {
                ViewBag.Exist = "Account is exist";
            }
            return View();
        }


        public async Task LoginWithGoogle()
        {
            await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme, new AuthenticationProperties()
            {
                RedirectUri = Url.Action("GoogleRespone")
            });
        }
        public async Task<IActionResult> GoogleRespone()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            var claim = result.Principal.Identities.FirstOrDefault().Claims.Select(claim => new
            {
                claim.Issuer,
                claim.OriginalIssuer,
                claim.Type,
                claim.Value
            });

            var userData = new Dictionary<int, string>();
            int i = 0;
            foreach (var item in claim)
            {
                userData[i++] = item.Value;
            }


            DateTime ngayHienTai = DateTime.Now;
            string email = userData[4];
            Profile temp = this._db.profile.FirstOrDefault(m => m.email == email);
            if (temp == null)
            {
                temp = new Profile();
                temp.username = email;
                temp.password = GetMd5(email);
                temp.email = email;
                temp.gender = "";
                temp.birthday = null;
                temp.money = 0;
                temp.type = "1";
                temp.status = "1";
                temp.date = ngayHienTai;
                // TODO: Your code here
                this._db.profile.Add(temp);
                this._db.SaveChanges();
                ViewBag.Success = "Register is Success || Your Account is waiting accept!";
            }
            else
            {
                if (temp.status.Equals("5") && temp.type.Equals("2"))
                {

                    ViewBag.Fail = "Your Account is waiting confirm email!";
                    return View("Login");
                }
                if (temp.status.Equals("4"))
                {

                    ViewBag.Fail = "Your account has been rejected!";
                    return View("Login");
                }
                if (temp.status.Equals("3"))
                {

                    ViewBag.Fail = "Your account is waiting accept!";
                    return View("Login");
                }
                if (temp.status.Equals("2"))
                {

                    ViewBag.Fail = "Your account is Disable!";
                    return View("Login");
                }

            }
            HttpContext.Session.SetString("User", temp.username);
            HttpContext.Session.SetString("Money", temp.money.ToString());
            Response.Cookies.Append("UserCookie", temp.username.Trim(), new CookieOptions

            {
                Expires = DateTime.Now.AddMinutes(15000), // Thời gian sống của cookie
                HttpOnly = true, // Chỉ có thể truy cập thông qua HTTP
            });
            if (temp.type.Equals("2"))
            {
                Response.Cookies.Append("SellerCookie", temp.username.Trim(), new CookieOptions

                {
                    Expires = DateTime.Now.AddMinutes(15000), // Thời gian sống của cookie
                    HttpOnly = true, // Chỉ có thể truy cập thông qua HTTP
                });
            }
            if (temp.type.Equals("3"))
            {
                Response.Cookies.Append("AdminCookie", temp.username.Trim(), new CookieOptions

                {
                    Expires = DateTime.Now.AddMinutes(15000), // Thời gian sống của cookie
                    HttpOnly = true, // Chỉ có thể truy cập thông qua HTTP
                });
            }
            return RedirectToAction("Index", "Home");

        }
        public IActionResult Login()
        {

            foreach (var cookie in Request.Cookies.Keys)
            {
                Response.Cookies.Delete(cookie);
            }
            HttpContext.Session.Clear();
            // TODO: Your code here
            return View();
        }
        public IActionResult Header()
        {
            // TODO: Your code here
            return View();
        }
        public IActionResult ResetPassWord(string id)
        {
            Profile profile = this._db.profile.FirstOrDefault(m => m.token == id && m.status == "1");
            if (profile == null)
            {
                return RedirectToAction("Login");
            }
            return View();
        }

        [HttpPost]
        public IActionResult ResetPassWord(ResetPass resetPass, string id)
        {
            Profile profile = this._db.profile.FirstOrDefault(m => m.token == id && m.status == "1");
            profile.password = GetMd5(resetPass.newPass.Trim());

            profile.token = null;
            this._db.SaveChanges();
            return RedirectToAction("Success");
        }
        public IActionResult Success()
        {
            if (TempData["RegisterSeller"] != null)
            {
                ViewBag.Success = TempData["RegisterSeller"];
                return View();
            }
            ViewBag.Success = "Success";
            // TODO: Your code here
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            Profile obj = this._db.profile.Find(username.Trim());
            if (obj != null)
            {
                if (obj.status.Equals("5"))
                {

                    ViewBag.Fail = "Your Account is waiting confirm email!";
                    return View();
                }
                if (obj.status.Equals("4"))
                {

                    ViewBag.Fail = "Your account has been rejected!";
                    return View();
                }
                if (obj.status.Equals("3"))
                {

                    ViewBag.Fail = "Your account is waiting accept!";
                    return View();
                }
                if (obj.status.Equals("2"))
                {

                    ViewBag.Fail = "Your account is Disable!";
                    return View();
                }
                if (obj.password.Equals(GetMd5(password.Trim())))
                {
                    HttpContext.Session.SetString("User", username);
                    HttpContext.Session.SetString("Money", obj.money.ToString());
                    Response.Cookies.Append("UserCookie", username.Trim(), new CookieOptions

                    {
                        Expires = DateTime.Now.AddMinutes(15000), // Thời gian sống của cookie
                        HttpOnly = true, // Chỉ có thể truy cập thông qua HTTP
                    });
                    if (obj.type.Equals("2"))
                    {
                        Response.Cookies.Append("SellerCookie", username.Trim(), new CookieOptions

                        {
                            Expires = DateTime.Now.AddMinutes(15000), // Thời gian sống của cookie
                            HttpOnly = true, // Chỉ có thể truy cập thông qua HTTP
                        });
                    }
                    if (obj.type.Equals("3"))
                    {
                        Response.Cookies.Append("AdminCookie", username.Trim(), new CookieOptions

                        {
                            Expires = DateTime.Now.AddMinutes(15000), // Thời gian sống của cookie
                            HttpOnly = true, // Chỉ có thể truy cập thông qua HTTP
                        });
                    }

                    return RedirectToAction("Index", "Home");
                }
            }
            ViewBag.Fail = "Username or Password is wrong!";
            return View();
        }

        public IActionResult Information()
        {
            string userCookieValue = HttpContext.Request.Cookies["UserCookie"];
            if (TempData["SuccessMessage"] != null)
            {
                ViewBag.SuccessMessage = TempData["SuccessMessage"];
            }

            // Kiểm tra có thông báo lỗi hay không
            if (TempData["ErrorMessage"] != null)
            {
                ViewBag.ErrorMessage = TempData["ErrorMessage"];
            }

            if (TempData["SuccessMessage1"] != null)
            {
                ViewBag.SuccessMessage1 = TempData["SuccessMessage1"];
            }
            if (userCookieValue != null)
            {
                var count = this._db.orderDetail
                .Join(this._db.order, detail => detail.OID, order => order.OID, (detail, order) => new { Detail = detail, Order = order })
                .Where(joinResult => joinResult.Order.username == userCookieValue)
                .Count();
                HttpContext.Session.SetString("count", count.ToString());
                Profile profileInfo = this._db.profile.Find(userCookieValue);
                ChangePassword change = new ChangePassword();
                change.username = profileInfo.username;
                change.password = profileInfo.password;
                change.email = profileInfo.email;
                change.gender = profileInfo.gender;
                change.money = profileInfo.money;
                change.birthday = profileInfo.birthday;
                change.fullname = profileInfo.fullname;
                change.PasswordAgain = "";
                return View(change);
            }

            return RedirectToAction("Index", "Home");
        }

        public IActionResult ForgotPassword()
        {
            // TODO: Your code here

            return View();
        }
        [HttpPost]
        public IActionResult ForgotPassword(string email)
        {
            var user = this._db.profile.FirstOrDefault(m => m.email == email && m.status == "1");
            if (user == null)
            {
                ViewBag.Fail = "Email doesn't exist in System, Account is waiting accept Or Account is disable!!!";

                return View();
            }
            RandomStringGenerator randomStringGenerator = new RandomStringGenerator();
            string token = randomStringGenerator.GenerateRandomString(50);
            var user1 = this._db.profile.FirstOrDefault(m => m.token == token);
            while (user1 != null)
            {
                token = randomStringGenerator.GenerateRandomString(50);
                user1 = this._db.profile.FirstOrDefault(m => m.token == token);
            }
            user.token = token;
            this._db.SaveChanges();
            string url = "https://localhost:5001/Profile/ResetPassWord" + "/" + token;
            // Cấu hình thông tin email
            var fromAddress = new MailAddress("luan251103@gmail.com", "Shopping");
            var toAddress = new MailAddress(email, "Recipient Name");
            const string fromPassword = "hyqy eywi fmtq iwry";
            const string subject = "Xác nhận tài khoản";

            // Cấu hình client SMTP
            var smtpClient = new SmtpClient
            {
                Host = "smtp.gmail.com", // Địa chỉ SMTP server
                Port = 587, // Port SMTP server
                EnableSsl = true, // Kích hoạt SSL
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };

            // Tạo message
            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = @"
    <!DOCTYPE html>
    <html lang=""en"">
    <head>
        <meta charset=""UTF-8"">
        <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
        <title>Password Reset</title>
        <style>
            body {
                font-family: Arial, sans-serif;
                margin: 0;
                padding: 0;
                background-color: #f9f9f9;
            }
            .container {
                max-width: 600px;
                margin: 0 auto;
                padding: 20px;
                background-color: #ffffff;
                border-radius: 10px;
                box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
            }
            h1 {
                color: #333333;
                text-align: center;
            }
            p {
                color: #666666;
                font-size: 16px;
                line-height: 1.6;
            }
            a {
                display: inline-block;
                padding: 10px 20px;
                background-color: #FAFAD2;
                color: #ffffff;
                text-decoration: none;
                border-radius: 5px;
                margin-top: 20px;
            }
            a:hover {
                background-color: #0056b3;
            }
            .footer {
                margin-top: 20px;
                text-align: center;
                color: #999999;
                font-size: 14px;
            }
        </style>
    </head>
    <body>
        <div class=""container"">
            <h1>[Wong's Store]</h1>
            <p>Hi " + user.username + @",</p>
            <p>You recently requested to reset your password for your [Product Name] account. Use the button below to reset it. This password reset is only valid for the next 24 hours.</p>
            <p><a href=""" + url + @""">Reset your password</a></p>
            <p>Thanks,<br>The [Wong's Store] Team</p>
            <p>If you’re having trouble with the button above, Enter URL below:</p>
            <p><a href=""https://www.facebook.com/profile.php?id=100027614479047"">Link Here</a></p>
            <p class=""footer"">© 2019 [Wong's Store]. All rights reserved.<br>[Wong's Store, LLC]<br>1234 Street Rd.<br>Suite 1234</p>
        </div>
    </body>
    </html>
",

                IsBodyHtml = true
            })
            {
                try
                {
                    smtpClient.Send(message);
                    Console.WriteLine("Email xác nhận đã được gửi đi thành công.");
                    ViewBag.Success = "Email did give for you!";
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Lỗi khi gửi email: {ex.Message}");
                }
            }
            return View();
        }


        [HttpPost]
        public IActionResult Information(string password, string PasswordAgain)
        {
            string userCookieValue = HttpContext.Request.Cookies["UserCookie"];
            Profile profileInfo = this._db.profile.Find(userCookieValue);

            if (password.Trim() == PasswordAgain.Trim())
            {
                profileInfo.password = GetMd5(password.Trim());
                TempData["SuccessMessage"] = "Password changed successfully!";
                this._db.SaveChanges();
            }
            else
            {
                ViewBag.Message = "Two Password is Different";
                TempData["ErrorMessage"] = "Two passwords are different.";
                return RedirectToAction("Information");
            }
            return RedirectToAction("Information");

        }
        [HttpPost]
        public IActionResult ChangeInformation(string fullname, string gender, DateTime birthday)
        {
            string userCookieValue = HttpContext.Request.Cookies["UserCookie"];
            Profile profileInfo = this._db.profile.Find(userCookieValue);
            try
            {
                profileInfo.fullname = fullname;
                profileInfo.gender = gender;
                profileInfo.birthday = birthday;
                TempData["SuccessMessage1"] = "Information changed successfully!";
                this._db.SaveChanges();
            }
            catch (System.Exception)
            {
                profileInfo.birthday = null;
                TempData["SuccessMessage1"] = "Information changed successfully!";
                this._db.SaveChanges();

            }
            // // TODO: Your code here
            return RedirectToAction("Information");
        }

        public IActionResult Logout()
        {
            foreach (var cookie in Request.Cookies.Keys)
            {
                Response.Cookies.Delete(cookie);
            }
            HttpContext.Session.Clear();
            // TODO: Your code here
            return RedirectToAction("Index", "Home");
        }

        public IActionResult ShoppingCart()
        {
            if (TempData["khongcotien"] != null)
            {
                ViewBag.Message = TempData["khongcotien"];
            }
            if (TempData["cotien"] != null)
            {
                ViewBag.Success = TempData["cotien"];
            }
            string userCookieValue = HttpContext.Request.Cookies["UserCookie"];
            if (userCookieValue != null)
            {
                var query = (from s in this._db.shoppingCart
                             join c in this._db.game on s.GID equals c.GID
                             where s.username == userCookieValue && c.status == "1"
                             select new ViewShoppingCart
                             {
                                 GID = c.GID,
                                 gameName = c.gameName,
                                 picture = c.picture,
                                 price = c.price
                             }).ToList();
                List<ViewShoppingCart> s1 = query;
                return View(s1);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public IActionResult addCart(string id)
        {
            // TODO: Your code here
            ShoppingCart s = new ShoppingCart();
            string userCookieValue = HttpContext.Request.Cookies["UserCookie"];
            if (userCookieValue != null)
            {
                s.username = userCookieValue;
                s.GID = id;
                Game check1 = this._db.game.Find(id);
                OrderDetail check = this._db.orderDetail.Find(userCookieValue + check1.gameName + check1.price.ToString());
                if (check == null)
                {
                    ShoppingCart temp = this._db.shoppingCart.FirstOrDefault(item => item.GID == id && item.username == userCookieValue);
                    if (temp == null)
                    {
                        this._db.shoppingCart.Add(s);
                        this._db.SaveChanges();
                        Wishlist w = this._db.wishlist.FirstOrDefault(m => m.username == userCookieValue && m.GID == id);
                        if (w != null)
                        {
                            this._db.wishlist.Remove(w);
                            this._db.SaveChanges();
                        }
                        return RedirectToAction("ShoppingCart");
                    }
                    return RedirectToAction("ShoppingCart");
                }
                return RedirectToAction("ViewGame", "Game", id);
            }
            return RedirectToAction("Login");
        }

        public IActionResult deleteCart(string id)
        {
            // TODO: Your code here  
            string userCookieValue = HttpContext.Request.Cookies["UserCookie"];

            var temp = this._db.shoppingCart.FirstOrDefault(m => m.GID == id && m.username == userCookieValue);
            if (temp == null)
            {
                return NotFound();
            }
            if (temp != null)
            {
                this._db.shoppingCart.Remove(temp);
                this._db.SaveChanges();

                return RedirectToAction("ShoppingCart");
            }
            return RedirectToAction("ShoppingCart");
        }

        [HttpPost]
        public IActionResult Pay(long txtTotalMoney)
        {
            DateTime now = DateTime.Now;
            string formattedDateTime = now.ToString("dd/MM/yyyy HH:mm:ss");
            string userCookieValue = HttpContext.Request.Cookies["UserCookie"];
            Profile checkWallet = this._db.profile.Find(userCookieValue);
            if (checkWallet.money < txtTotalMoney)
            {
                TempData["khongcotien"] = "Nghèo còn ham hố!";
                return RedirectToAction("ShoppingCart");
            }
            TempData["cotien"] = "Pay Success";
            var query = (from s in this._db.shoppingCart
                         join c in this._db.game on s.GID equals c.GID
                         where s.username == userCookieValue && c.status == "1"
                         select new ViewShoppingCart
                         {
                             GID = c.GID,
                             gameName = c.gameName,
                             picture = c.picture,
                             price = c.price
                         }).ToList();
            List<ViewShoppingCart> s1 = query;
            Order orders = new Order();
            orders.OID = userCookieValue + formattedDateTime;
            orders.username = userCookieValue;
            orders.total = txtTotalMoney;
            // DateTime date = DateTime.Parse(formattedDateTime);
            orders.date = DateTime.Now;
            this._db.order.Add(orders);
            this._db.SaveChanges();
            foreach (var item in s1)
            {
                var a = _db.game.FirstOrDefault(a => a.GID == item.GID);
                if (a == null)
                {
                    return NotFound();
                }

                OrderDetail order = new OrderDetail();
                order.OID = userCookieValue + formattedDateTime;
                order.GID = item.GID;
                order.price = item.price;
                string odid = userCookieValue + item.gameName + item.price.ToString();
                order.ODID = userCookieValue + item.gameName + item.price.ToString();
                this._db.orderDetail.Add(order);
                this._db.SaveChanges();

                manageMoney(order.price, a.sellerName, formattedDateTime, odid);
            }
            var usersToDelete = this._db.shoppingCart.Where(u => u.username == userCookieValue);
            this._db.shoppingCart.RemoveRange(usersToDelete);
            this._db.SaveChanges();

            Profile s2 = this._db.profile.Find(userCookieValue);
            s2.money = s2.money - txtTotalMoney;
            this._db.SaveChanges();
            try
            {
                var fromAddress = new MailAddress("luan251103@gmail.com", "Shopping");
                var toAddress = new MailAddress(checkWallet.email, "Recipient Name");
                const string fromPassword = "hyqy eywi fmtq iwry";
                const string subject = "Payment Successful";


                // Cấu hình client SMTP
                var smtpClient = new SmtpClient
                {
                    Host = "smtp.gmail.com", // Địa chỉ SMTP server
                    Port = 587, // Port SMTP server
                    EnableSsl = true, // Kích hoạt SSL
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
                };

                // Tạo message
                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = $"Dear Customer,\n\nThank you for your payment. Your order has been successfully processed.\n\nTotal Amount: {txtTotalMoney:N0} VND. \n\nBest Regards,\n[Wong'sStore]"
                })
                {
                    try
                    {
                        smtpClient.Send(message);
                        Console.WriteLine("Email xác nhận đã được gửi đi thành công.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Lỗi khi gửi email: {ex.Message}");
                    }
                }
            }
            catch (System.Exception)
            {

                Console.WriteLine($"Lỗi khi gửi email: ");
            }

            // TODO: Your code here
            return RedirectToAction("ShoppingCart", "Profile");
        }
        public void manageMoney(long money, string seller, string date, string odid)
        {
            double mn = (double)money;
            // DateTime dt = DateTime.Parse(date);
            double temp1 = mn * 0.1;
            long am = (long)Math.Ceiling(temp1);
            double temp2 = mn - temp1;
            long sm = (long)Math.Floor(temp2);
            MoneyManagement m = new MoneyManagement();
            m.username = seller;
            m.ODID = odid;
            m.admin = "user01";//Change admin name here
            m.adMoney = am;
            m.sellerMoney = sm;
            m.date = DateTime.Now;
            this._db.moneyManagement.Add(m);
            this._db.SaveChanges();
            Profile p1 = this._db.profile.Find(seller);
            if (p1 != null)
            {
                p1.money += sm;
                this._db.Update(p1);
                this._db.SaveChanges();
            }
            Profile p2 = this._db.profile.Find("user01");
            if (p2 != null)
            {
                p2.money += am;
                this._db.Update(p2);
                this._db.SaveChanges();
            }
        }
        public IActionResult addMoney()
        {
            string userCookieValue = HttpContext.Request.Cookies["UserCookie"];
            if (userCookieValue == null)
            {
                return RedirectToAction("Login");
            }

            if (TempData["WaitingMoney"] != null && TempData["WaitingMoney"].ToString() == "Success")
            {
                ViewBag.success = "Success";
            }
            if (TempData["WaitingMoney"] != null && TempData["WaitingMoney"].ToString() == "Fail")
            {
                ViewBag.Fail = "Fail";
            }
            TempData["WaitingMoney"] = null;
            userCookieValue = HttpContext.Request.Cookies["UserCookie"];
            Profile profileInfo = new Profile();
            profileInfo = this._db.profile.Find(userCookieValue);
            TempData["WaitingMoney"] = null;

            return View(profileInfo);
        }

        [HttpPost]
        public IActionResult addMoney(string txtmoney)
        {

            string userCookieValue = HttpContext.Request.Cookies["UserCookie"];
            Profile profileInfo = this._db.profile.Find(userCookieValue);
            long newMoney = long.Parse(txtmoney);
            // profileInfo.money += newMoney;
            // this._db.SaveChanges();

            DateTime now = DateTime.Now;
            string formattedDateTime = now.ToString("dd/MM/yyyy HH:mm:ss");
            string mid = profileInfo.username + formattedDateTime;

            MoneyHistory m = new MoneyHistory();
            m.MID = mid;
            m.username = profileInfo.username;
            // DateTime date = DateTime.Parse(formattedDateTime);
            m.date = DateTime.Now;
            m.money = newMoney;
            m.status = "waiting";

            this._db.MoneyHistory.Add(m);
            this._db.SaveChanges();


            return RedirectToAction("PayAPI", "Payment", new { txt = newMoney, MID = mid });
        }

        public IActionResult historyMoney()
        {
            string userCookieValue = HttpContext.Request.Cookies["UserCookie"];
            if (userCookieValue == null)
            {
                return RedirectToAction("Login");
            }
            userCookieValue = HttpContext.Request.Cookies["UserCookie"];
            var query = (from mh in this._db.MoneyHistory
                         where mh.username == userCookieValue
                         select new MoneyHistory
                         {
                             MID = mh.MID,
                             username = mh.username,
                             money = mh.money,
                             date = mh.date,
                             status = mh.status
                         }).OrderByDescending(mh => mh.date).ToList();
            List<MoneyHistory> s = query;
            return View(s);
        }

        public IActionResult shoppingHistory()
        {
            string userCookieValue = HttpContext.Request.Cookies["UserCookie"];
            if (userCookieValue == null)
            {
                return RedirectToAction("Login");
            }
            userCookieValue = HttpContext.Request.Cookies["UserCookie"];
            var query = (from od in this._db.orderDetail
                         join oo in this._db.order
                         on od.OID equals oo.OID
                         where oo.username == userCookieValue
                         select new OrderDetail
                         {
                             OID = od.OID,
                             GID = od.GID,
                             price = od.price,
                             ODID = od.ODID
                         }).OrderByDescending(od => od.ODID).ToList();
            List<OrderDetail> s = query;
            return View(s);
        }

        public IActionResult WishList()
        {
            string userCookieValue = HttpContext.Request.Cookies["UserCookie"];
            if (userCookieValue != null)
            {
                var wl = from a in this._db.game
                         join b in this._db.wishlist
                         on a.GID equals b.GID
                         where b.username == userCookieValue && a.status == "1"
                         select a;
                IEnumerable<Game> list = wl.ToList();
                return View(list);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public IActionResult addToWishList(string id)
        {
            // TODO: Your code here
            Wishlist wl = new Wishlist();
            string userCookieValue = HttpContext.Request.Cookies["UserCookie"];
            if (userCookieValue != null)
            {
                wl.username = userCookieValue;
                wl.GID = id;
                Wishlist temp = this._db.wishlist.FirstOrDefault(m => m.GID == id && m.username == userCookieValue);
                if (temp == null)
                {
                    this._db.wishlist.Add(wl);
                    this._db.SaveChanges();
                    ShoppingCart w = this._db.shoppingCart.FirstOrDefault(m => m.GID == id && m.username == userCookieValue);
                    if (w != null)
                    {
                        this._db.shoppingCart.Remove(w);
                        this._db.SaveChanges();
                    }

                    return RedirectToAction("WishList");
                }
                return RedirectToAction("WishList");
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        public IActionResult RemoveWishList(string id)
        {
            string userCookieValue = HttpContext.Request.Cookies["UserCookie"];

            var list = (from s in this._db.wishlist
                        where s.GID == id && s.username == userCookieValue
                        select new Wishlist
                        {
                            GID = s.GID,
                            username = s.username
                        }).ToList();

            if (list != null)
            {
                this._db.wishlist.Remove(list[0]);
                this._db.SaveChanges();
                return RedirectToAction("WishList");
            }
            return RedirectToAction("WishList");
        }


        public IActionResult Library()
        {
            string userCookieValue = HttpContext.Request.Cookies["UserCookie"];
            if (userCookieValue != null)
            {
                var lb = (from od in this._db.orderDetail
                          join o in this._db.order
                          on od.OID equals o.OID
                          join g in this._db.game
                          on od.GID equals g.GID
                          where o.username == userCookieValue
                          select new Game
                          {
                              GID = g.GID,
                              gameName = g.gameName,
                              picture = g.picture,
                              price = g.price,
                              downloadFile = g.downloadFile
                          }).ToList();
                List<Game> ga = lb;
                return View(ga);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

    }
}