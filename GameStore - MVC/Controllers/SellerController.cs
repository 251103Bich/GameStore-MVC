using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GameStore.Models;
using GameStoreV1._0.DB;
using GameStoreV1._0.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GameStoreV1._0.Controllers
{
    public class SellerController : Controller
    {
        private readonly ApplicationDBContext _db;

        public SellerController(ApplicationDBContext _db)
        {
            this._db = _db;
        }

        // Phương thức Index được sử dụng để hiển thị danh sách các trò chơi của một người bán cụ thể.
        // Đầu tiên, nó truy cập vào cookie để lấy tên của người bán.
        // Sau đó, nó thực hiện một truy vấn để lấy danh sách các trò chơi của người bán từ cơ sở dữ liệu và trả về một view chứa danh sách này.
        public IActionResult Index()
        {
            try
            {
                // Lấy Seller Name từ cookie và gán vào biến sellerNamae
                string sellerNamaz = HttpContext.Request.Cookies["SellerCookie"];
                if (sellerNamaz == null)
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception e)
            {
                return RedirectToAction("Index", "Home");
            }

            string sellerNamae = HttpContext.Request.Cookies["SellerCookie"];

            // Truy vấn cơ sở dữ liệu để lấy danh sách các trò chơi của người bán
            var query = (from s in this._db.game
                         where s.sellerName == sellerNamae
                         select new Game
                         {
                             // Chọn ra các thuộc tính cần thiết của trò chơi
                             GID = s.GID,
                             gameName = s.gameName,
                             price = s.price,
                             picture = s.picture,
                             date = s.date,
                             updateDate = s.updateDate,
                             description = s.description,
                             configuration = s.configuration,
                             sellerName = s.sellerName,
                             status = s.status
                         }).ToList();
                         var sortedList = query.OrderByDescending(s => s.updateDate == null ? s.date : s.updateDate)
                      .ToList();
            // Gán kết quả truy vấn vào một danh sách trò chơi của người bán
            List<Game> sellerGame = query;

            // Trả về view với danh sách trò chơi của người bán
            return View(sortedList);
        }

        // Phương thức AddGame được sử dụng để hiển thị trang thêm trò chơi mới.
        public IActionResult AddGame()
        {
            try
            {
                // Lấy Seller Name từ cookie và gán vào biến sellerNamae
                string sellerNamaz = HttpContext.Request.Cookies["SellerCookie"];
                if (sellerNamaz == null)
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception e)
            {
                return RedirectToAction("Index", "Home");
            }

            // Truy vấn cơ sở dữ liệu để lấy danh sách tất cả các Category.
            var query = (from c in this._db.Category
                         select new Category
                         {
                             CID = c.CID,
                             categoryName = c.categoryName
                         }).ToList();
            // Tạo một đối tượng ViewCategory chứa tất cả các danh mục để truyền đến view.
            ViewCategory viewCategory = new ViewCategory
            {
                AllCategory = query
            };

            if (TempData["IDErrorMessage"] != null)
            {
                ViewBag.IDMessage = TempData["IDErrorMessage"];
            }

            if (TempData["PictureErrorMessage"] != null)
            {
                ViewBag.PictureMessage = TempData["PictureErrorMessage"];
            }

            if (TempData["FileErrorMessage"] != null)
            {
                ViewBag.FileMessage = TempData["FileErrorMessage"];
            }

            // Trả về view AddGame với danh sách tất cả các danh mục.
            return View(viewCategory);
        }

        static string GetFirstCharacters(string input)
        {
            string[] words = input.Split(' ');
            string result = "";

            foreach (string word in words)
            {
                if (!string.IsNullOrEmpty(word))
                {
                    result += word[0];
                }
            }

            // Nếu words có kích thước lớn hơn 10, chọn ngẫu nhiên 10 ký tự theo thứ tự
            if (words.Length > 10)
            {
                var random = new Random();
                var selectedWords = words.OrderBy(x => random.Next()).Take(10).ToArray();
                result = string.Join("", selectedWords.Select(word => word[0]));
            }

            return result;
        }

        // Phương thức AddGame dùng để xử lý yêu cầu POST khi người dùng thêm một trò chơi mới.
        [HttpPost]
        public IActionResult AddGame(ViewCategory obj, List<string> selectedCategory, IFormFile gamePicture, IFormFile gameFile)
        {
            try
            {
                // Tạo một đối tượng Game mới.
                Game g = new Game();
                // Lấy tên của người bán từ cookie.
                string sellerNamae = HttpContext.Request.Cookies["SellerCookie"];

                string tempID = sellerNamae + " " + GetFirstCharacters(obj.gameName);
                DateTime now = DateTime.Now;
                string formattedDate = now.ToString("ddMMyyyyHHmmss");
                g.date = DateTime.Now;

                obj.GID = tempID + formattedDate;

                var check = this._db.game.Find(obj.GID);

                if (check != null)
                {

                    TempData["IDErrorMessage"] = "Game already exist";
                    ViewBag.IDMessage = TempData["IDErrorMessage"];

                    // Truy vấn cơ sở dữ liệu để lấy danh sách tất cả các Category.
                    var query = (from c in this._db.Category
                                 select new Category
                                 {
                                     CID = c.CID,
                                     categoryName = c.categoryName
                                 }).ToList();
                    // Tạo một đối tượng ViewCategory chứa tất cả các danh mục để truyền đến view.
                    ViewCategory viewCategory = new ViewCategory
                    {
                        AllCategory = query
                    };

                    return View(viewCategory);
                }

                // Gán các thuộc tính của trò chơi từ đối tượng ViewCategory được gửi từ form.
                g.GID = obj.GID;
                g.gameName = obj.gameName;
                g.price = obj.price;
                g.picture = obj.picture;
                g.description = obj.description;
                g.configuration = obj.configuration;
                g.downloadFile = obj.downloadFile;
                g.sellerName = sellerNamae;
                g.status = "3";// Đặt status có giá trị mặc định là 3 có ý nghĩa là trò chơi đang được chờ duyệt.

                if (gamePicture.ContentType == "image/jpeg" || gamePicture.ContentType == "image/jpg")
                {
                    // Lấy tên file từ đối tượng IFormFile
                    var fileName = Path.GetFileName(gamePicture.FileName);

                    // Kết hợp đường dẫn đến thư mục lưu trữ với tên file để tạo đường dẫn đầy đủ
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "upload", "img", fileName);

                    // Sử dụng FileStream để tạo và ghi dữ liệu của file vào đường dẫn đã chỉ định
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        gamePicture.CopyTo(stream);
                    }

                    // Lưu tên file vào trường 'picture' của đối tượng Game
                    g.picture = fileName;
                }
                else
                {
                    // Thiết lập thông báo lỗi vào ViewBag khi hình ảnh không hợp lệ
                    TempData["PictureErrorMessage"] = "Invalid picture";
                    ViewBag.PictureMessage = TempData["PictureErrorMessage"];

                    // Lưu ID của game vào một biến cục bộ
                    string id = obj.GID;

                    // Truy vấn cơ sở dữ liệu để lấy danh sách tất cả các Category.
                    var query = (from c in this._db.Category
                                 select new Category
                                 {
                                     CID = c.CID,
                                     categoryName = c.categoryName
                                 }).ToList();
                    // Tạo một đối tượng ViewCategory chứa tất cả các danh mục để truyền đến view.
                    ViewCategory viewCategory = new ViewCategory
                    {
                        AllCategory = query
                    };

                    // Chuyển hướng người dùng đến action "Edit" để hiển thị lại trang chỉnh sửa với thông báo lỗi
                    return View(viewCategory);
                }

                // Lấy tên file từ đối tượng IFormFile
                var gameFileName = Path.GetFileName(gameFile.FileName);
                if (gameFileName.EndsWith(".rar", StringComparison.OrdinalIgnoreCase))
                {


                    // Kết hợp đường dẫn đến thư mục lưu trữ với tên file để tạo đường dẫn đầy đủ
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "upload", "file", gameFileName);

                    // Sử dụng FileStream để tạo và ghi dữ liệu của file vào đường dẫn đã chỉ định
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        gameFile.CopyTo(stream);
                    }

                    // Lưu tên file vào trường 'picture' của đối tượng Game
                    g.downloadFile = gameFileName;
                }
                else
                {
                    // Thiết lập thông báo lỗi vào ViewBag khi hình ảnh không hợp lệ
                    TempData["FileErrorMessage"] = "Invalid file";
                    ViewBag.FileMessage = TempData["FileErrorMessage"];

                    // Lưu ID của game vào một biến cục bộ
                    string id = obj.GID;

                    // Truy vấn cơ sở dữ liệu để lấy danh sách tất cả các Category.
                    var query = (from c in this._db.Category
                                 select new Category
                                 {
                                     CID = c.CID,
                                     categoryName = c.categoryName
                                 }).ToList();
                    // Tạo một đối tượng ViewCategory chứa tất cả các danh mục để truyền đến view.
                    ViewCategory viewCategory = new ViewCategory
                    {
                        AllCategory = query
                    };

                    // Chuyển hướng người dùng đến action "Edit" để hiển thị lại trang chỉnh sửa với thông báo lỗi
                    return View(viewCategory);
                }

                // Thêm đối tượng Game mới vào cơ sở dữ liệu.
                this._db.game.Add(g);
                // Thực hiện lưu thay đổi.
                this._db.SaveChanges();

                // Tạo danh sách các CategoryConnect để liên kết trò chơi với các danh mục đã chọn.
                var selectCate = new List<CategoryConnect>();
                CategoryConnect ccon = new CategoryConnect();

                // Duyệt qua danh sách các danh mục đã chọn.
                foreach (string cid in selectedCategory)
                {
                    // Gán CID và GID cho mỗi CategoryConnect và lưu vào cơ sở dữ liệu.
                    ccon.CID = cid;
                    ccon.GID = obj.GID;
                    //Thực hiện thêm mới category
                    this._db.CategoryConnect.Add(ccon);
                    //Lưu thay đổi
                    this._db.SaveChanges();
                }

                // Chuyển hướng người dùng đến action Index để hiển thị danh sách trò chơi.
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                TempData["IDErrorMessage"] = "Invalid Game Information";
                ViewBag.IDMessage = TempData["IDErrorMessage"];

                // Truy vấn cơ sở dữ liệu để lấy danh sách tất cả các Category.
                var query = (from c in this._db.Category
                             select new Category
                             {
                                 CID = c.CID,
                                 categoryName = c.categoryName
                             }).ToList();
                // Tạo một đối tượng ViewCategory chứa tất cả các danh mục để truyền đến view.
                ViewCategory viewCategory = new ViewCategory
                {
                    AllCategory = query
                };

                return View(viewCategory);
            }
        }


        // Phương thức Edit được sử dụng để hiển thị trang chỉnh sửa thông tin trò chơi.
        // Tham số id được sử dụng để xác định trò chơi cần chỉnh sửa dựa trên ID của nó.
        public IActionResult Edit(string id)
        {

            try
            {
                // Lấy Seller Name từ cookie và gán vào biến sellerNamae
                string sellerNamaz = HttpContext.Request.Cookies["SellerCookie"];
                if (sellerNamaz == null)
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception e)
            {
                return RedirectToAction("Index", "Home");
            }

            // Lấy tên của người bán từ cookie.
            string sellerNamae = HttpContext.Request.Cookies["SellerCookie"];

            // Truy vấn cơ sở dữ liệu để lấy thông tin về trò chơi cần chỉnh sửa.
            var query = (from g in this._db.game
                         where g.GID == id
                         select new Game
                         {
                             // Gán các giá trị cho Game
                             GID = g.GID,
                             gameName = g.gameName,
                             price = g.price,
                             picture = g.picture,
                             date = g.date,
                             updateDate = g.updateDate,
                             description = g.description,
                             configuration = g.configuration,
                             sellerName = sellerNamae,
                             status = g.status,
                             downloadFile = g.downloadFile
                         }).ToList();

            // Truy vấn cơ sở dữ liệu để lấy danh sách các danh mục đã được liên kết với trò chơi.
            var query2 = (from c in this._db.Category
                          join cc in this._db.CategoryConnect on c.CID equals cc.CID
                          where cc.GID == id
                          select new Category
                          {
                              //Gán các giá trị cho Category
                              CID = cc.CID,
                              categoryName = c.categoryName
                          }).ToList();

            // Truy vấn cơ sở dữ liệu để lấy danh sách tất cả các danh mục có sẵn.
            var query3 = (from c in this._db.Category
                          select new Category
                          {
                              //Gán các giá trị cho Category
                              CID = c.CID,
                              categoryName = c.categoryName
                          }).ToList();

            // Tạo một đối tượng ViewCategory chứa thông tin trò chơi, danh sách các danh mục đã được liên kết và tất cả các danh mục.
            ViewCategory viewCategory = new ViewCategory
            {
                //Thêm các thông tin cần thiết của trò chơi được chọn
                GID = query[0].GID,
                gameName = query[0].gameName,
                price = query[0].price,
                picture = query[0].picture,
                date = query[0].date,
                updateDate = query[0].updateDate,
                description = query[0].description,
                configuration = query[0].configuration,
                sellerName = sellerNamae,
                downloadFile = query[0].downloadFile,
                status = query[0].status,
                //Thêm danh sách category của trò chơi
                viewCategory = query2,
                //Thêm danh sách các Category khả dụng
                AllCategory = query3
            };

            Game gameInfo = query[0];

            if(TempData["IDErrorMessage"] != null) {
                ViewBag.IDMessage = TempData["IDErrorMessage"];
            }

            // Trả về view Edit với thông tin của trò chơi cần chỉnh sửa và danh sách các danh mục.
            return View(viewCategory);
        }

        // Phương thức Edit được sử dụng để xử lý yêu cầu POST khi người dùng chỉnh sửa thông tin trò chơi.
        // Phương thức này sẽ nhận các thông tin mới của trò chơi và danh sách các danh mục đã được chọn.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ViewCategory obj, List<string> selectedCategory, IFormFile gamePicture, IFormFile gameFile)
        {
            try
            {
                // Lấy tên của người bán từ cookie.
                string sellerNamae = HttpContext.Request.Cookies["SellerCookie"];

                // Khởi tạo một đối tượng Game mới.
                Game g = new Game();

                // Kiểm tra xem dữ liệu gửi lên có hợp lệ không.
                if (ModelState.IsValid)
                {
                    // Truy vấn cơ sở dữ liệu để lấy danh sách các danh mục đã được liên kết với trò chơi và xóa chúng.
                    var changeCate = (from c in this._db.CategoryConnect
                                      where c.GID == obj.GID
                                      select new CategoryConnect
                                      {
                                          CID = c.CID,
                                          GID = c.GID
                                      }).ToList();

                    // Xóa các liên kết danh mục cũ trong cơ sở dữ liệu.
                    foreach (var item in changeCate)
                    {
                        this._db.CategoryConnect.Remove(item);
                        this._db.SaveChanges();
                    }

                    // Tạo mới các liên kết danh mục mới dựa trên danh sách danh mục đã được chọn.
                    foreach (string cid in selectedCategory)
                    {
                        CategoryConnect cc = new CategoryConnect();
                        cc.CID = cid;
                        cc.GID = obj.GID;
                        this._db.CategoryConnect.Add(cc);
                        this._db.SaveChanges();
                    }

                    // Cập nhật thông tin của trò chơi và lưu vào cơ sở dữ liệu.
                    g.GID = obj.GID;
                    g.gameName = obj.gameName;
                    g.price = obj.price;
                    g.picture = obj.picture;
                    g.date = obj.date;
                    g.updateDate = DateTime.Now;
                    g.description = obj.description;
                    g.configuration = obj.configuration;
                    g.sellerName = sellerNamae;
                    g.status = obj.status;
                    g.downloadFile = obj.downloadFile;

                    //Kiểm tra tên game có null không
                    if (gamePicture != null)
                    {
                        //Nếu tên game không null thì kiểm tra tên file có kết thúc với .jpg không
                        if (gamePicture.ContentType == "image/jpeg" || gamePicture.ContentType == "image/jpg")
                        {
                            // Lấy tên file từ đối tượng IFormFile
                            var fileName = Path.GetFileName(gamePicture.FileName);

                            // Kết hợp đường dẫn đến thư mục lưu trữ với tên file để tạo đường dẫn đầy đủ
                            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "upload", "img", fileName);

                            // Sử dụng FileStream để tạo và ghi dữ liệu của file vào đường dẫn đã chỉ định
                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                gamePicture.CopyTo(stream);
                            }

                            // Lưu tên file vào trường 'picture' của đối tượng Game
                            g.picture = fileName;
                        }
                        else
                        {
                            // Thiết lập thông báo lỗi vào ViewBag khi hình ảnh không hợp lệ
                            ViewBag.Message = "Invalid picture";

                            // Lưu ID của game vào một biến cục bộ
                            string id = obj.GID;

                            // Chuyển hướng người dùng đến action "Edit" để hiển thị lại trang chỉnh sửa với thông báo lỗi
                            return RedirectToAction("Edit");
                        }
                    }

                    //Kiểm tra tên game có null không
                    if (gameFile != null)
                    {
                        // Lấy tên file từ đối tượng IFormFile
                        var fileName = Path.GetFileName(gameFile.FileName);
                        //Nếu tên game không null thì kiểm tra tên file có kết thúc với .jpg không
                        if (fileName.EndsWith(".rar", StringComparison.OrdinalIgnoreCase))
                        {


                            // Kết hợp đường dẫn đến thư mục lưu trữ với tên file để tạo đường dẫn đầy đủ
                            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "upload", "file", fileName);

                            // Sử dụng FileStream để tạo và ghi dữ liệu của file vào đường dẫn đã chỉ định
                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                gameFile.CopyTo(stream);
                            }

                            // Lưu tên file vào trường 'picture' của đối tượng Game
                            g.downloadFile = fileName;
                        }
                        else
                        {
                            // Thiết lập thông báo lỗi vào ViewBag khi hình ảnh không hợp lệ
                            ViewBag.Message = "Invalid picture";

                            // Lưu ID của game vào một biến cục bộ
                            string id = obj.GID;

                            // Chuyển hướng người dùng đến action "Edit" để hiển thị lại trang chỉnh sửa với thông báo lỗi
                            return RedirectToAction("Edit");
                        }
                    }

                    this._db.game.Update(g);
                    this._db.SaveChanges();

                    // Chuyển hướng người dùng đến action Index để hiển thị danh sách trò chơi sau khi chỉnh sửa thành công.
                    return RedirectToAction("Index");
                }

                // Nếu dữ liệu không hợp lệ, trả về view Edit với dữ liệu gửi lên.
                return View(obj);
            }
            catch (Exception e)
            {
                TempData["IDErrorMessage"] = "Invalid Game Information";
                ViewBag.IDMessage = TempData["IDErrorMessage"];

                // Truy vấn cơ sở dữ liệu để lấy danh sách tất cả các Category.
                var query = (from c in this._db.Category
                             select new Category
                             {
                                 CID = c.CID,
                                 categoryName = c.categoryName
                             }).ToList();
                // Tạo một đối tượng ViewCategory chứa tất cả các danh mục để truyền đến view.
                ViewCategory viewCategory = new ViewCategory
                {
                    AllCategory = query
                };

                return RedirectToAction("Edit");
            }
        }

        public IActionResult RevenueView()
        {

            try
            {
                // Lấy Seller Name từ cookie và gán vào biến sellerNamae
                string sellerNamaz = HttpContext.Request.Cookies["SellerCookie"];
                if (sellerNamaz == null)
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception e)
            {
                return RedirectToAction("Index", "Home");
            }

            string sellerNamae = HttpContext.Request.Cookies["SellerCookie"];

            string sqlquery = "select mm.[username] as sellerName,"
                + " mm.[date],"
                + " YEAR(mm.[date]) AS [year],"
                + " MONTH(mm.[date]) AS [month],"
                + " DAY(mm.[date]) AS [day],"
                + " mm.sellerMoney as sellerPlusMoney,"
                + " p.[money] as sellerMoney,"
                + " od.GID,"
                + " g.gameName,"
                + " g.picture,"
                + " g.[status] from MoneyManagement mm"
                + " join OrderDetail od on od.ODID = mm.ODID"
                + " join Game g on g.GID = od.GID"
                + " join [Profile] p on p.username = mm.username"
                + " where sellerName = '" + sellerNamae + "'"
                + "and g.[status] != '3'"
                + " and g.[status] != '4'"
                + "order by mm.[date]";

            List<SellerRevenue> sellerRev = new List<SellerRevenue>();

            using (var command = this._db.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = sqlquery;
                this._db.Database.OpenConnection();

                using (var result = command.ExecuteReader())
                {
                    while (result.Read())
                    {
                        SellerRevenue item = new SellerRevenue
                        {
                            sellerName = result.GetString(result.GetOrdinal("sellerName")),
                            date = result.GetDateTime(result.GetOrdinal("date")),
                            year = result.GetInt32(result.GetOrdinal("year")),
                            month = result.GetInt32(result.GetOrdinal("month")),
                            day = result.GetInt32(result.GetOrdinal("day")),
                            sellerPlusMoney = result.GetInt64(result.GetOrdinal("sellerPlusMoney")),
                            sellerMoney = result.GetInt64(result.GetOrdinal("sellerMoney")),
                            GID = result.GetString(result.GetOrdinal("GID")),
                            gameName = result.GetString(result.GetOrdinal("gameName")),
                            picture = result.GetString(result.GetOrdinal("picture")),
                            status = result.GetString(result.GetOrdinal("status"))
                        };

                        sellerRev.Add(item);
                    }
                }
            }

            return View(sellerRev);
        }

        public IActionResult DetailGameRevenue(string id)
        {

            try
            {
                // Lấy Seller Name từ cookie và gán vào biến sellerNamae
                string sellerNamaz = HttpContext.Request.Cookies["SellerCookie"];
                if (sellerNamaz == null)
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception e)
            {
                return RedirectToAction("Index", "Home");
            }

            string sellerNamae = HttpContext.Request.Cookies["SellerCookie"];

            string sqlquery = "select mm.[username] as sellerName,"
                + " o.username as cusName,"
                + " mm.[date],"
                + " YEAR(mm.[date]) AS [year],"
                + " MONTH(mm.[date]) AS [month],"
                + " DAY(mm.[date]) AS [day],"
                + " od.price as gamePrice,"
                + " mm.sellerMoney as sellerPlusMoney,"
                + " p.[money] as sellerMoney,"
                + " od.GID,"
                + " g.gameName,"
                + " g.picture,"
                + " g.[status] from MoneyManagement mm"
                + " join OrderDetail od on od.ODID = mm.ODID"
                + " join [Order] o on o.OID = od.OID"
                + " join Game g on g.GID = od.GID"
                + " join [Profile] p on p.username = mm.username"
                + " where sellerName = '" + sellerNamae + "'"
                + " and g.GID = '" + id + "'"
                + " and g.[status] != '3'"
                + " and g.[status] != '4'"
                + " order by mm.[date]";

            List<SellerRevenue> sellerRev = new List<SellerRevenue>();

            using (var command = this._db.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = sqlquery;
                this._db.Database.OpenConnection();

                using (var result = command.ExecuteReader())
                {
                    while (result.Read())
                    {
                        SellerRevenue item = new SellerRevenue
                        {
                            sellerName = result.GetString(result.GetOrdinal("sellerName")),
                            cusName = result.GetString(result.GetOrdinal("cusName")),
                            date = result.GetDateTime(result.GetOrdinal("date")),
                            year = result.GetInt32(result.GetOrdinal("year")),
                            month = result.GetInt32(result.GetOrdinal("month")),
                            day = result.GetInt32(result.GetOrdinal("day")),
                            gamePrice = result.GetInt64(result.GetOrdinal("gamePrice")),
                            sellerPlusMoney = result.GetInt64(result.GetOrdinal("sellerPlusMoney")),
                            sellerMoney = result.GetInt64(result.GetOrdinal("sellerMoney")),
                            GID = result.GetString(result.GetOrdinal("GID")),
                            gameName = result.GetString(result.GetOrdinal("gameName")),
                            picture = result.GetString(result.GetOrdinal("picture")),
                            status = result.GetString(result.GetOrdinal("status"))
                        };

                        sellerRev.Add(item);
                    }
                }
            }
            sellerRev.OrderByDescending(hh => hh.date).ToList();
            return View(sellerRev);
        }



    }
}