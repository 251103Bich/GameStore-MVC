using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using GameStoreV1._0.DB;
using GameStoreV1._0.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GameStoreV1._0.Controllers
{
    public class GameController : Controller
    {
        private readonly ApplicationDBContext _db;
        public GameController(ApplicationDBContext _db)
        {
            this._db = _db;
        }

        // Phương thức ViewGame được sử dụng để hiển thị chi tiết của một trò chơi cụ thể.
        // Tham số id là ID của trò chơi cần hiển thị.
        public IActionResult ViewGame(string id)
        {
            // Tìm kiếm thông tin về trò chơi trong cơ sở dữ liệu dựa trên ID.
            Game ga = this._db.game.Find(id);
            // Nếu không tìm thấy trò chơi, chuyển hướng người dùng về trang chủ.
            if (ga == null)
            {
                return RedirectToAction("Index", "Home");
            }

            // Tạo một câu truy vấn SQL để lấy danh sách các danh mục đã được liên kết với trò chơi.
            String sqlQuery = "select cc.CID, c.categoryName from CategoryConnect cc join Category c on c.CID = cc.CID where cc.GID = '" + id + "'";

            // Thực thi truy vấn và lấy danh sách các danh mục.
            List<Category> categoryList = this._db.Set<Category>().FromSqlRaw(sqlQuery).ToList();

            // Tạo một đối tượng ViewCategory chứa thông tin chi tiết của trò chơi và danh sách các danh mục.
            ViewGameDetail view = new ViewGameDetail
            {
                viewedGame = ga,
                viewCategory = categoryList
            };
            var sessionUser = HttpContext.Request.Cookies["UserCookie"];
            //Check if feedback is added
            var check = this._db.feedback.Any(m => m.username == sessionUser && m.GID == id);
            //if feedback is added
            if (check == true)
            {
                //Set fbAdded to true
                ViewBag.fbAdded = "true";
            }
            //query to check if game is added
            var query = from o in this._db.order
                        join od in this._db.orderDetail on o.OID equals od.OID
                        where o.username == sessionUser && od.GID == id
                        select new
                        {
                            o.username,
                            od.GID
                        };
            //Check if game is added
            var check2 = query.Any();
            if (check2 == true)
            {
                //Set gameAdded to true
                ViewBag.gameAdded = "true";
            }
            ViewBag.UserID = sessionUser;
            ViewBag.Rate = Math.Round(Rate(id), 1);
            ViewBag.ListFeedback = feedbackViewModel(id);
            // Trả về view ViewGame với thông tin chi tiết của trò chơi.
            return View(view);
        }

        // Phương thức SearchGame được sử dụng để hiển thị trang tìm kiếm trò chơi.
        public IActionResult SearchGame()
        {
            // Trả về view SearchGame để người dùng có thể thực hiện tìm kiếm.
            return View();
        }

        // Phương thức SearchGame được sử dụng để xử lý yêu cầu POST khi người dùng thực hiện tìm kiếm trò chơi.
        [HttpPost]
        public IActionResult SearchGame(string searchString)
        {
            // Kiểm tra xem chuỗi tìm kiếm có rỗng hoặc null không.
            if (string.IsNullOrEmpty(searchString))
            {
                // Nếu chuỗi tìm kiếm rỗng hoặc null, trả về view SearchGame.
                return View();
            }

            //Thực hiện tìm kiếm game theo chuỗi searchString được nhập.
            var filterGame = (from g in this._db.game
                              where g.gameName.Contains(searchString) && g.status == "1"
                              select new Game
                              {
                                  GID = g.GID,
                                  gameName = g.gameName,
                                  price = g.price,
                                  picture = g.picture,
                                  date = g.date,
                                  description = g.description,
                                  configuration = g.configuration,
                                  sellerName = g.sellerName,
                                  status = g.status
                              }).ToList();

            // Trả về view với danh sách các trò chơi tìm kiếm được.
            return View(filterGame);
        }

        // Phương thức CategoryList được sử dụng để hiển thị danh sách các danh mục.
        public IActionResult CategoryList()
        {
            // Lấy danh sách các danh mục từ cơ sở dữ liệu.
            var catelist = this._db.Category.ToList();
            var glist = this._db.game.Where(m => m.status == "1").ToList();

            List<Game> gamelist = glist;
            List<Category> clist = catelist;

            viewCategorylist viewct = new viewCategorylist
            {
                GamesList = gamelist,
                Categories = clist

            };


            // Trả về view với danh sách các danh mục.
            return View(viewct);
        }

        // Phương thức searchCategory được sử dụng để tìm kiếm các trò chơi thuộc một danh mục cụ thể dựa trên ID của danh mục.
        public IActionResult searchCategory(string id)
        {
            // Thực hiện truy vấn để lấy danh sách các trò chơi thuộc danh mục có ID là id.
            var query = (from cc in this._db.CategoryConnect
                         join g in this._db.game
                         on cc.GID equals g.GID
                         where cc.CID == id && g.status == "1"
                         select new Game
                         {
                             GID = g.GID,
                             gameName = g.gameName,
                             price = g.price,
                             picture = g.picture,
                             date = g.date,
                             description = g.description,
                             configuration = g.configuration,
                             sellerName = g.sellerName,
                             status = g.status

                         }).ToList();
            // Chuyển danh sách trò chơi tìm thấy thành một danh sách.
            List<Game> s = query;

            var name = this._db.Category.Find(id);
            ViewBag.x= name.categoryName;

            // Trả về view hiển thị danh sách các trò chơi thuộc danh mục cụ thể.
            return View(s);
        }
        [HttpPost]
        public IActionResult AddFeedback(ViewGameDetail obj)
        {
            //Get user
            var user = HttpContext.Request.Cookies["UserCookie"];
            //Check if model is valid
            if (ModelState.IsValid)
            {
                //Define new feedback
                var newfeedback = new Feedback
                {
                    username = user,
                    GID = obj.currentGame,
                    feedback = obj.feedback,
                    rate = obj.rate,
                    status = "1",
                    date = DateTime.Now
                };
                //Add new feedback
                this._db.feedback.Add(newfeedback);
                this._db.SaveChanges();
            }
            //Return to view game 
            return RedirectToAction("ViewGame", "Game", new { id = obj.currentGame });
        }
        public List<Feedback> ListFeedback(string id)
        {
            //Get all feedback
            return _db.feedback.Where(x => x.GID == id).OrderBy(x => x.date).ToList();
        }

        public List<FeedbackViewModel> feedbackViewModel(string id)
        {
            var model = (from a in _db.feedback
                         join b in _db.game on a.GID equals b.GID
                         join c in _db.profile on a.username equals c.username
                         where b.GID == id
                         select new
                         {
                             GID = a.GID,
                             username = a.username,
                             feedback = a.feedback,
                             fullname = c.fullname,
                             rate = a.rate,
                             status = a.status,
                             date = a.date
                         }).AsEnumerable().Select(x => new FeedbackViewModel()
                         {
                             GID = x.GID,
                             username = x.username,
                             feedback = x.feedback,
                             fullname = x.fullname,
                             rate = x.rate,
                             status = x.status,
                             date = x.date
                         });
            return model.ToList();
        }

        /*
          Calculate the number of rates
        */
        public double Rate(string id)
        {
            var query = _db.feedback.Where(x => x.GID == id);
            int count = query.Count();
            int sum = query.Sum(x => x.rate);
            ViewBag.count = count;
            double erate = (double)sum / count;
            return erate;
        }

        public IActionResult sellerGame(string id)
        {
            var segame = (from g in this._db.game
                          where g.sellerName == id && g.status == "1"
                          select new Game
                          {
                              GID = g.GID,
                              gameName = g.gameName,
                              price = g.price,
                              picture = g.picture,
                              date = g.date,
                              description = g.description,
                              configuration = g.configuration,
                              sellerName = g.sellerName,
                              status = g.status
                          }).ToList();
            HttpContext.Session.SetString("SellerName", segame[0].sellerName);
            return View(segame);
        }
    }
}