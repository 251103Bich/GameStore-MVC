using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using GameStoreV1._0.Models;
using GameStoreV1._0.DB;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace GameStoreV1._0.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDBContext _db;

        public HomeController(ApplicationDBContext _db)
        {
            this._db = _db;
        }

        // Phương thức Index được sử dụng để hiển thị trang chủ.
        public IActionResult Index()
        {
            // Kiểm tra xem có tồn tại cookie "UserCookie" hay không.
            string userCookieValue = HttpContext.Request.Cookies["UserCookie"];
            if (userCookieValue != null)
            {
                // Nếu tồn tại, lấy thông tin profile tương ứng từ cơ sở dữ liệu và lưu vào Session.
                Profile obj = this._db.profile.Find(userCookieValue);
                HttpContext.Session.SetString("User", obj.username);
                HttpContext.Session.SetString("Money", obj.money.ToString());
                Response.Cookies.Append("UserCookie", obj.username.Trim(), new CookieOptions
                {
                    Expires = DateTime.Now.AddMinutes(15000), // Thời gian sống của cookie
                    HttpOnly = true, // Chỉ có thể truy cập thông qua HTTP
                });

            }

            //Danh sach game noi bat. Lua chon ra 8 game ban chay nhat.
            string sqlQuery1 = "select top 8 g.GID, g.gameName, g.price, g.picture, g.[date], g.[description], g.[configuration], g.sellerName, g.[status], g.downloadFile, g.updateDate from Game g"
            + " join OrderDetail od on od.GID = g.GID"
            + " where g.[status] = '1'"
            + " group by g.GID, g.gameName, g.price, g.picture, g.[date], g.[description], g.[configuration], g.sellerName, g.[status], g.downloadFile, g.updateDate"
            + " order by COUNT(*) desc";
            //Danh sach game moi ra mat. Lua chon ra 8 game moi nhat
            string sqlQuery2 = "select top 8 g.GID, g.gameName, g.price, g.picture, g.[date], g.[description], g.[configuration], g.sellerName, g.[status], g.downloadFile, g.updateDate from Game g"
            + " Where g.[status] = '1'"
            + " Order by g.[date] DESC";

            // Lấy danh sách các trò chơi từ cơ sở dữ liệu.
            List<Game> gamesList1 = (from g in this._db.game
            where g.status == "1"
            select new Game
            {
                GID = g.GID,
                gameName = g.gameName,
                price = g.price,
                picture = g.picture,
                date = g.date,
                description = g.description,
                configuration = g.configuration,
                status = g.status
            }).ToList();
            // Lấy danh sách các trò chơi bán chạy nhất từ cơ sở dữ liệu.
            List<Game> gamesList2 = this._db.Set<Game>().FromSqlRaw(sqlQuery1).ToList();
            // Lấy danh sách các trò chơi mới nhất từ cơ sở dữ liệu.
            List<Game> gamesList3 = this._db.Set<Game>().FromSqlRaw(sqlQuery2).ToList();

            // Tạo một đối tượng GamesView chứa danh sách các trò chơi nổi bật và mới ra mắt.
            GamesView viewListsGames = new GamesView
            {
                GamesList1 = gamesList1,
                GamesList2 = gamesList2,
                GamesList3 = gamesList3
            };

            // Trả về view Index với danh sách các trò chơi.
            return View(viewListsGames);
        }
        public IActionResult HelpView()
        {
            // TODO: Your code here
            return View();
        }
        public IActionResult ContactView()
        {
            // TODO: Your code here
            return View();
        }
        


    }
}
