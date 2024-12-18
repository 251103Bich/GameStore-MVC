using System;
using System.Configuration;
using System.Linq;
using GameStoreV1._0.DB;
using GameStoreV1._0.Models;
using log4net;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace GameStoreV1._0.Controllers
{

    public class PaymentController : Controller
    {
        private readonly ApplicationDBContext _db;
        public PaymentController(ApplicationDBContext _db)
        {
            this._db = _db;
        }
        private static readonly ILog log =
        LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public IActionResult PayAPI(long txt, string MID)
        {
            TempData["MID"] = MID;
            //Get Config Info
            string vnp_Returnurl = ConfigurationManager.AppSettings["vnp_Returnurl"]; //URL nhan ket qua tra ve 
            string vnp_Url = ConfigurationManager.AppSettings["vnp_Url"]; //URL thanh toan cua VNPAY 
            string vnp_TmnCode = ConfigurationManager.AppSettings["vnp_TmnCode"]; //Ma định danh merchant kết nối (Terminal Id)
            string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"]; //Secret Key

            //Get payment input
            OrderInfo order = new OrderInfo();
            order.OrderId = DateTime.Now.Ticks; // Giả lập mã giao dịch hệ thống merchant gửi sang VNPAY
            order.Amount = txt; // Giả lập số tiền thanh toán hệ thống merchant gửi sang VNPAY 100,000 VND
            order.Status = "0"; //0: Trạng thái thanh toán "chờ thanh toán" hoặc "Pending" khởi tạo giao dịch chưa có IPN
            order.CreatedDate = DateTime.Now;
            //Save order to db

            //Build URL for VNPAY
            VnPayLibrary vnpay = new VnPayLibrary();

            vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", "B46P075A");
            vnpay.AddRequestData("vnp_Amount", (order.Amount * 100).ToString()); //Số tiền thanh toán. Số tiền không mang các ký tự phân tách thập phân, phần nghìn, ký tự tiền tệ. Để gửi số tiền thanh toán là 100,000 VND (một trăm nghìn VNĐ) thì merchant cần nhân thêm 100 lần (khử phần thập phân), sau đó gửi sang VNPAY là: 10000000

            vnpay.AddRequestData("vnp_BankCode", "VNBANK");


            vnpay.AddRequestData("vnp_CreateDate", order.CreatedDate.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", "https://localhost:5001/Profile/addMoney");


            vnpay.AddRequestData("vnp_Locale", "vn");

            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang:" + order.OrderId);
            vnpay.AddRequestData("vnp_OrderType", "other"); //default value: other

            vnpay.AddRequestData("vnp_ReturnUrl", "https://localhost:5001/Payment/PayAPI1");
            vnpay.AddRequestData("vnp_TxnRef", order.OrderId.ToString()); // Mã tham chiếu của giao dịch tại hệ thống của merchant. Mã này là duy nhất dùng để phân biệt các đơn hàng gửi sang VNPAY. Không được trùng lặp trong ngày
            //Add Params of 2.1.0 Version
            //Billing
            string paymentUrl = vnpay.CreateRequestUrl("https://sandbox.vnpayment.vn/paymentv2/vpcpay.html", "YAGRYUIEQLUMRQFWSGMVRDVUAUYEBYYQ");
            log.InfoFormat("VNPAY URL: {0}", paymentUrl);

            return Redirect(paymentUrl);
        }

        public IActionResult PayAPI1()
        {
            var history = this._db.MoneyHistory.FirstOrDefault(m => m.MID == TempData["MID"].ToString());
            string userCookieValue = HttpContext.Request.Cookies["UserCookie"];
            Profile profileInfo = new Profile();
            profileInfo = this._db.profile.Find(userCookieValue);
            
            Profile profileInfo1 = this._db.profile.Find(userCookieValue);
            string currentUrl = HttpContext.Request.GetEncodedUrl();

            // Tạo đối tượng Uri từ URL hiện tại
            Uri myUri = new Uri(currentUrl);

            // Lấy các tham số từ URL nếu cần
            string vnp_Amount = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(myUri.Query)["vnp_Amount"];
            string vnp_TransactionStatus = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(myUri.Query)["vnp_TransactionStatus"];
            vnp_Amount = vnp_Amount.Substring(0, vnp_Amount.Length - 2);
            Console.WriteLine("vnp_Amount: " + vnp_Amount);
            Console.WriteLine("vnp_TransactionStatus: " + vnp_TransactionStatus);
            if (vnp_TransactionStatus == "00")
            {
                
                profileInfo1.money += long.Parse(vnp_Amount);
                history.status = "done";
                this._db.SaveChanges();
                TempData["WaitingMoney"] = "Success";
            }
            else
            {
                 history.status = "fail";
                 this._db.SaveChanges();
                TempData["WaitingMoney"] = "Fail";
            }
            return RedirectToAction("addMoney", "Profile");
        }

        public IActionResult revenue()
        {
            // TODO: Your code here
            return View();
        }
        



    }
}