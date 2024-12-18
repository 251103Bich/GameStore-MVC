using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
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
    public class AdminController : Controller
    {
        private readonly ApplicationDBContext _db;
        public AdminController(ApplicationDBContext _db)
        {
            this._db = _db;
        }
        public IActionResult Management()
        {
            string adminck = HttpContext.Request.Cookies["AdminCookie"];
            // Đây là một truy vấn LINQ để lấy danh sách các hồ sơ từ cơ sở dữ liệu.
            // Cụ thể, chúng ta đang truy vấn bảng "profile".
            if (adminck == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var userlist = (from pf in this._db.profile
                                // Chọn từng dòng dữ liệu trong bảng profile và chuyển chúng thành các đối tượng Profile mới.
                            where pf.type == "1"
                            select new Profile
                            {
                                // Gán các thuộc tính của đối tượng Profile mới từ các thuộc tính tương ứng trong bảng profile.

                                username = pf.username,
                                // Tên đăng nhập của người dùng.
                                email = pf.email,
                                // Địa chỉ email của người dùng.
                                fullname = pf.fullname,
                                // Họ và tên đầy đủ của người dùng.
                                gender = pf.gender,
                                // Giới tính của người dùng.
                                birthday = pf.birthday,
                                // Ngày sinh của người dùng.
                                money = pf.money,
                                // Số tiền trong tài khoản của người dùng.
                                type = pf.type,
                                // Loại tài khoản của người dùng (ví dụ: người dùng thường, quản trị viên, v.v.).
                                status = pf.status,
                                // Trạng thái của tài khoản (ví dụ: đã xác minh, chưa xác minh, v.v.).
                                date = pf.date
                                // Ngày tạo hồ sơ.
                            }).ToList();
            // Chuyển kết quả truy vấn thành một danh sách và lưu vào biến userlist.

            // Đây là một truy vấn LINQ để lấy danh sách các người bán từ cơ sở dữ liệu.
            // Cụ thể, chúng ta đang truy vấn bảng "profile" để lấy các người bán.

            var sellerlist = (from pf in this._db.profile
                                  // Chọn từng dòng dữ liệu trong bảng profile.

                              where pf.type == "2"
                              // Chỉ chọn những người dùng có loại tài khoản là "2" (đại diện cho người bán).
                              && pf.status != "3"
                              // Loại bỏ các hồ sơ có trạng thái là "3" (ví dụ: tạm khóa).
                              && pf.status != "4"
                              // Loại bỏ các hồ sơ có trạng thái là "4" (ví dụ: bị khóa vĩnh viễn).

                              select new Profile
                              {
                                  // Tạo mới các đối tượng Profile từ các dòng dữ liệu đã lọc.

                                  username = pf.username,
                                  // Tên đăng nhập của người dùng.
                                  email = pf.email,
                                  // Địa chỉ email của người dùng.
                                  fullname = pf.fullname,
                                  // Họ và tên đầy đủ của người dùng.
                                  gender = pf.gender,
                                  // Giới tính của người dùng.
                                  birthday = pf.birthday,
                                  // Ngày sinh của người dùng.
                                  money = pf.money,
                                  // Số tiền trong tài khoản của người dùng.
                                  status = pf.status,
                                  // Trạng thái của tài khoản.
                                  date = pf.date
                                  // Ngày tạo hồ sơ.
                              }).ToList();
            // Chuyển kết quả truy vấn thành một danh sách và lưu vào biến sellerlist.

            // Đây là một truy vấn LINQ để lấy danh sách các trò chơi từ cơ sở dữ liệu.
            // Trong trường hợp này, chúng ta đang truy vấn bảng "game" để lấy thông tin về các trò chơi.

            var gamelist = (from g in this._db.game
                            join p in this._db.profile on g.sellerName equals p.username
                            // Chọn từng dòng dữ liệu trong bảng game và chuyển chúng thành các đối tượng Game mới.
                            where g.status == "0" && p.status == "1"
                           || g.status == "1"
                           || g.status == "2"
                            select new Game
                            {
                                // Gán các thuộc tính của đối tượng Game mới từ các thuộc tính tương ứng trong bảng game.

                                GID = g.GID,
                                // ID của trò chơi.
                                gameName = g.gameName,
                                // Tên của trò chơi.
                                price = g.price,
                                // Giá của trò chơi.
                                picture = g.picture,
                                // Đường dẫn đến hình ảnh của trò chơi.
                                date = g.date,
                                // Ngày phát hành trò chơi.
                                description = g.description,
                                // Mô tả về trò chơi.
                                configuration = g.configuration,
                                // Cấu hình yêu cầu của trò chơi.
                                sellerName = g.sellerName,
                                // Tên của người bán trò chơi.
                                status = g.status,
                                // Trạng thái của trò chơi.
                                downloadFile = g.downloadFile
                            }).ToList();
            // Chuyển kết quả truy vấn thành một danh sách và lưu vào biến gamelist.

            var applicationList = (from pf in
            this._db.profile
                                   where pf.type == "2"
                                   &&
                                    pf.status == "3"
                                   select
                                   new Profile
                                   {
                                       username = pf.username,
                                       email = pf.email,
                                       fullname = pf.fullname,
                                       gender = pf.gender,
                                       birthday = pf.birthday,
                                       money = pf.money,
                                       type = pf.type,
                                       status = pf.status,
                                       date = pf.date,

                                   }).ToList();

            var applicationList1 = (from pf in
            this._db.profile
                                    where pf.type == "2"
                                    &&
                                     pf.status == "4"
                                    select
                                    new Profile
                                    {
                                        username = pf.username,
                                        email = pf.email,
                                        fullname = pf.fullname,
                                        gender = pf.gender,
                                        birthday = pf.birthday,
                                        money = pf.money,
                                        type = pf.type,
                                        status = pf.status,
                                        date = pf.date,

                                    }).ToList();
            var gamelist1 = (from g in this._db.game
                             join p in this._db.profile on g.sellerName equals p.username
                             // Chọn từng dòng dữ liệu trong bảng game và chuyển chúng thành các đối tượng Game mới.
                             where g.status == "3" && p.status == "1"
                             select new Game
                             {
                                 // Gán các thuộc tính của đối tượng Game mới từ các thuộc tính tương ứng trong bảng game.

                                 GID = g.GID,
                                 // ID của trò chơi.
                                 gameName = g.gameName,
                                 // Tên của trò chơi.
                                 price = g.price,
                                 // Giá của trò chơi.
                                 picture = g.picture,
                                 // Đường dẫn đến hình ảnh của trò chơi.
                                 date = g.date,
                                 // Ngày phát hành trò chơi.
                                 description = g.description,
                                 // Mô tả về trò chơi.
                                 configuration = g.configuration,
                                 // Cấu hình yêu cầu của trò chơi.
                                 sellerName = g.sellerName,
                                 // Tên của người bán trò chơi.
                                 status = g.status,
                                 // Trạng thái của trò chơi.
                                 downloadFile = g.downloadFile
                             }).ToList();




            var gamelist2 = (from g in this._db.game
                             join p in this._db.profile on g.sellerName equals p.username
                             // Chọn từng dòng dữ liệu trong bảng game và chuyển chúng thành các đối tượng Game mới.
                             where g.status == "4" && p.status == "1"
                             select new Game
                             {
                                 // Gán các thuộc tính của đối tượng Game mới từ các thuộc tính tương ứng trong bảng game.

                                 GID = g.GID,
                                 // ID của trò chơi.
                                 gameName = g.gameName,
                                 // Tên của trò chơi.
                                 price = g.price,
                                 // Giá của trò chơi.
                                 picture = g.picture,
                                 // Đường dẫn đến hình ảnh của trò chơi.
                                 date = g.date,
                                 // Ngày phát hành trò chơi.
                                 description = g.description,
                                 // Mô tả về trò chơi.
                                 configuration = g.configuration,
                                 // Cấu hình yêu cầu của trò chơi.
                                 sellerName = g.sellerName,
                                 // Tên của người bán trò chơi.
                                 status = g.status,
                                 // Trạng thái của trò chơi.
                                 downloadFile = g.downloadFile
                             }).ToList();

            // Khởi tạo các danh sách chứa thông tin về người dùng, người bán, và các ứng dụng.
            List<Game> GList = gamelist;
            // Danh sách các trò chơi.
            List<Profile> UList = userlist;
            // Danh sách các thông tin người dùng.
            List<Profile> SList = sellerlist;
            // Danh sách các thông tin người bán.
            List<Profile> AList = applicationList;
            // Danh sách các thông tin ứng dụng.
            List<Profile> AList1 = applicationList1;
            // Danh sách các thông tin ứng dụng mở rộng.
            List<Game> GList1 = gamelist1;

            List<Game> GList2 = gamelist2;

            // Tạo một đối tượng của lớp Managerview để chứa dữ liệu cho màn hình quản lý.
            Managerview screenManager = new Managerview
            {
                // Gán các danh sách vào thuộc tính tương ứng của đối tượng Managerview.
                ProfileList = UList,
                // Danh sách thông tin người dùng.
                GamesList = GList,
                // Danh sách các trò chơi.
                SellerList = SList,
                // Danh sách thông tin người bán.
                ApplicationList = AList,
                // Danh sách thông tin ứng dụng.
                ApplicationList1 = AList1,
                // Danh sách thông tin ứng dụng mở rộng.
                GamesList1 = GList1,

                GamesList2 = GList2,
            };

            // Trả về một View (giao diện) chứa dữ liệu của đối tượng Managerview để hiển thị trên màn hình.
            return View(screenManager);
        }

        public IActionResult Detail(string id)
        {
            string adminck = HttpContext.Request.Cookies["AdminCookie"];
            // Đây là một truy vấn LINQ để lấy danh sách các hồ sơ từ cơ sở dữ liệu.
            // Cụ thể, chúng ta đang truy vấn bảng "profile".
            if (adminck == null)
            {
                return RedirectToAction("Index", "Home");
            }
            HttpContext.Session.SetString("check", "4");
            try
            {
                var information = (from pf in
            this._db.profile
                                   where pf.username == id
                                   select
                                   new Profile
                                   {
                                       username = pf.username,
                                       email = pf.email,
                                       fullname = pf.fullname,
                                       gender = pf.gender,
                                       birthday = pf.birthday,
                                       money = pf.money,
                                       type = pf.type,
                                       status = pf.status,
                                       date = pf.date,

                                   }).ToList();
                return View(information[0]);
            }
            catch (System.Exception)
            {
                return NotFound();
            }
        }
        public IActionResult Detail1(string id)
        {
            string adminck = HttpContext.Request.Cookies["AdminCookie"];
            // Đây là một truy vấn LINQ để lấy danh sách các hồ sơ từ cơ sở dữ liệu.
            // Cụ thể, chúng ta đang truy vấn bảng "profile".
            if (adminck == null)
            {
                return RedirectToAction("Index", "Home");
            }
            HttpContext.Session.SetString("check", "5");
            try
            {
                var information = (from pf in
            this._db.profile
                                   where pf.username == id
                                   select
                                   new Profile
                                   {
                                       username = pf.username,
                                       email = pf.email,
                                       fullname = pf.fullname,
                                       gender = pf.gender,
                                       birthday = pf.birthday,
                                       money = pf.money,
                                       type = pf.type,
                                       status = pf.status,
                                       date = pf.date,

                                   }).ToList();
                return View(information[0]);
            }
            catch (System.Exception)
            {
                return NotFound();
            }
        }
        [HttpPost]
        public IActionResult Detail(string btnAccept, string btnDecline, string status, string username)
        {
            var profileToUpdate = this._db.profile.FirstOrDefault
            (p
             =>
             p.username
             ==
              username
              );
            if (profileToUpdate != null)
            {
                if (!string.IsNullOrEmpty(btnAccept))
                {
                    profileToUpdate.username = profileToUpdate.username;
                    profileToUpdate.password = profileToUpdate.password;
                    profileToUpdate.email = profileToUpdate.email;
                    profileToUpdate.fullname = profileToUpdate.fullname;
                    profileToUpdate.gender = profileToUpdate.gender;
                    profileToUpdate.birthday = profileToUpdate.birthday;
                    profileToUpdate.money = profileToUpdate.money;
                    profileToUpdate.type = profileToUpdate.type;
                    profileToUpdate.date = profileToUpdate.date;
                    profileToUpdate.status = "1";
                    this._db.profile.Update(profileToUpdate);
                    this._db.SaveChanges();
                    return RedirectToAction("Management", "Admin");
                }
                else if (!string.IsNullOrEmpty(btnDecline))
                {
                    profileToUpdate.username = profileToUpdate.username;
                    profileToUpdate.password = profileToUpdate.password;
                    profileToUpdate.email = profileToUpdate.email;
                    profileToUpdate.fullname = profileToUpdate.fullname;
                    profileToUpdate.gender = profileToUpdate.gender;
                    profileToUpdate.birthday = profileToUpdate.birthday;
                    profileToUpdate.money = profileToUpdate.money;
                    profileToUpdate.type = profileToUpdate.type;
                    profileToUpdate.date = profileToUpdate.date;
                    profileToUpdate.status = "4";
                    this._db.profile.Update(profileToUpdate);
                    this._db.SaveChanges();
                    return RedirectToAction("Management", "Admin");
                }

            }
            return NotFound();


            // Sau khi xử lý, bạn có thể chuyển hướng hoặc trả về một kết quả khác tùy theo logic của ứng dụng

        }

        // Phương thức để xóa một người dùng trong cơ sở dữ liệu
        public IActionResult deleteUser(string id)
        {
            string adminck = HttpContext.Request.Cookies["AdminCookie"];
            // Đây là một truy vấn LINQ để lấy danh sách các hồ sơ từ cơ sở dữ liệu.
            // Cụ thể, chúng ta đang truy vấn bảng "profile".
            if (adminck == null)
            {
                return RedirectToAction("Index", "Home");
            }
            HttpContext.Session.SetString("check", "1");
            try
            {
                // Lấy thông tin của người dùng từ cơ sở dữ liệu dựa trên username (id)
                var information = (from pf in this._db.profile
                                       // Lấy dữ liệu từ bảng "profile"
                                   where pf.username == id
                                   // Lọc bản ghi với username (id) trùng khớp
                                   select new Profile
                                   // Tạo một đối tượng Profile mới từ kết quả truy vấn
                                   {
                                       // Sao chép các thuộc tính của người dùng vào đối tượng Profile mới
                                       username = pf.username,
                                       // Sao chép username
                                       email = pf.email,
                                       // Sao chép email
                                       fullname = pf.fullname,
                                       // Sao chép fullname
                                       gender = pf.gender,
                                       // Sao chép gender
                                       birthday = pf.birthday,
                                       // Sao chép birthday
                                       money = pf.money,
                                       // Sao chép money
                                       type = pf.type,
                                       // Sao chép type
                                       status = pf.status,
                                       // Sao chép status
                                       date = pf.date
                                       // Sao chép date
                                   }).ToList();
                // Chuyển kết quả truy vấn thành danh sách và lưu vào biến "information"

                ViewBag.Message3 = TempData["Message3"];
                // Trả về View hiển thị thông tin của người dùng đầu tiên trong danh sách (nếu có)
                return View(information[0]);
                // Trả về View hiển thị thông tin người dùng
            }
            catch (System.Exception)
            {
                // Trong trường hợp có lỗi xảy ra (ví dụ: không tìm thấy người dùng), trả về trang NotFound
                return NotFound();
                // Trả về trang NotFound
            }
        }

        [HttpPost]
        public IActionResult deleteUser(string btnAccept, string btnDecline, string status, string username)
        {
            // Tìm kiếm thông tin người dùng cần cập nhật trong cơ sở dữ liệu
            var profileToUpdate = this._db.profile.FirstOrDefault(p
            =>
            p.username
            ==
            username
            );

            // Kiểm tra xem người dùng có tồn tại không
            if (profileToUpdate != null)
            {
                // Nếu người dùng tồn tại và nút "Chấp nhận" được nhấn
                if (!string.IsNullOrEmpty(btnAccept))
                {

                    // Cập nhật username của profile với giá trị hiện tại (không thay đổi)
                    profileToUpdate.username = profileToUpdate.username;

                    // Cập nhật password của profile với giá trị hiện tại (không thay đổi)
                    profileToUpdate.password = profileToUpdate.password;

                    // Cập nhật email của profile với giá trị hiện tại (không thay đổi)
                    profileToUpdate.email = profileToUpdate.email;

                    // Cập nhật fullname của profile với giá trị hiện tại (không thay đổi)
                    profileToUpdate.fullname = profileToUpdate.fullname;

                    // Cập nhật gender của profile với giá trị hiện tại (không thay đổi)
                    profileToUpdate.gender = profileToUpdate.gender;

                    // Cập nhật birthday của profile với giá trị hiện tại (không thay đổi)
                    profileToUpdate.birthday = profileToUpdate.birthday;

                    // Cập nhật money của profile với giá trị hiện tại (không thay đổi)
                    profileToUpdate.money = profileToUpdate.money;

                    // Cập nhật type của profile với giá trị hiện tại (không thay đổi)
                    profileToUpdate.type = profileToUpdate.type;

                    // Cập nhật date của profile với giá trị hiện tại (không thay đổi)
                    profileToUpdate.date = profileToUpdate.date;

                    // Đặt trạng thái của profile thành "0" để đánh dấu là đã xoá
                    profileToUpdate.status = "2";
                    ViewBag.Message3 = "Delete account successfully";

                    if (profileToUpdate.type == "2")
                    {
                        string sqlQuery = "update game set status = '5' where sellerName = '" + profileToUpdate.username + "' and status = '1'";
                        this._db.Database.ExecuteSqlRaw(sqlQuery);
                    }

                }
                // Nếu người dùng tồn tại và nút "Từ chối" được nhấn
                else if (!string.IsNullOrEmpty(btnDecline))
                {
                    // Cập nhật username của profile với giá trị hiện tại (không thay đổi)
                    profileToUpdate.username = profileToUpdate.username;

                    // Cập nhật password của profile với giá trị hiện tại (không thay đổi)
                    profileToUpdate.password = profileToUpdate.password;

                    // Cập nhật email của profile với giá trị hiện tại (không thay đổi)
                    profileToUpdate.email = profileToUpdate.email;

                    // Cập nhật fullname của profile với giá trị hiện tại (không thay đổi)
                    profileToUpdate.fullname = profileToUpdate.fullname;

                    // Cập nhật gender của profile với giá trị hiện tại (không thay đổi)
                    profileToUpdate.gender = profileToUpdate.gender;

                    // Cập nhật birthday của profile với giá trị hiện tại (không thay đổi)
                    profileToUpdate.birthday = profileToUpdate.birthday;

                    // Cập nhật money của profile với giá trị hiện tại (không thay đổi)
                    profileToUpdate.money = profileToUpdate.money;

                    // Cập nhật type của profile với giá trị hiện tại (không thay đổi)
                    profileToUpdate.type = profileToUpdate.type;

                    // Cập nhật date của profile với giá trị hiện tại (không thay đổi)
                    profileToUpdate.date = profileToUpdate.date;

                    // Đặt trạng thái của profile thành "1" để đánh dấu là đã khôi phục
                    profileToUpdate.status = "1";
                    ViewBag.Message3 = "Recover account successfully";

                    if (profileToUpdate.type == "2")
                    {
                        string sqlQuery = "update game set status = '1' where sellerName = '" + profileToUpdate.username + "' and status = '5'";
                        this._db.Database.ExecuteSqlRaw(sqlQuery);
                    }
                }

                // Lưu các thay đổi vào cơ sở dữ liệu
                this._db.profile.Update(profileToUpdate);
                this._db.SaveChanges();


                // Chuyển hướng người dùng đến action "Management" của controller "Admin" sau khi cập nhật thành công
                return View(profileToUpdate);
            }

            // Trả về trang NotFound nếu không tìm thấy thông tin người dùng

            return NotFound();

            // Sau khi xử lý, bạn có thể chuyển hướng hoặc trả về một kết quả khác tùy theo logic của ứng dụng
        }
        // Phương thức detailGame được sử dụng để hiển thị chi tiết của một trò chơi cụ thể.
        // Tham số id là ID của trò chơi cần hiển thị.
        public IActionResult detailGame(string id)
        {
            string adminck = HttpContext.Request.Cookies["AdminCookie"];
            // Đây là một truy vấn LINQ để lấy danh sách các hồ sơ từ cơ sở dữ liệu.
            // Cụ thể, chúng ta đang truy vấn bảng "profile".
            if (adminck == null)
            {
                return RedirectToAction("Index", "Home");
            }
            HttpContext.Session.SetString("check", "3");
            // Tìm kiếm thông tin về trò chơi trong cơ sở dữ liệu dựa trên ID.
            Game ga = this._db.game.Find(id);

            // Nếu không tìm thấy trò chơi, chuyển hướng người dùng về trang quản lý.
            if (ga == null)
            {
                return RedirectToAction("Management", "Admin");
            }

            // Tạo một câu truy vấn SQL để lấy danh sách các danh mục đã được liên kết với trò chơi.
            String sqlQuery = "select cc.CID, c.categoryName from CategoryConnect cc join Category c on c.CID = cc.CID where cc.GID = '" + id + "'";

            // Thực thi truy vấn và lấy danh sách các danh mục.
            List<Category> categoryList = this._db.Set<Category>().FromSqlRaw(sqlQuery).ToList();

            // Tạo một đối tượng ViewCategory chứa thông tin chi tiết của trò chơi và danh sách các danh mục.
            ViewCategory viewCategory = new ViewCategory
            {
                GID = ga.GID,
                gameName = ga.gameName,
                price = ga.price,
                picture = ga.picture,
                date = ga.date,
                description = ga.description,
                configuration = ga.configuration,
                sellerName = ga.sellerName,
                status = ga.status,
                viewCategory = categoryList,
                downloadFile = ga.downloadFile
            };
            ViewBag.Message3 = TempData["Message3"];

            // Trả về view ViewGame với thông tin chi tiết của trò chơi.
            return View(viewCategory);
        }

        public IActionResult detailGame1(string id)
        {
            string adminck = HttpContext.Request.Cookies["AdminCookie"];
            // Đây là một truy vấn LINQ để lấy danh sách các hồ sơ từ cơ sở dữ liệu.
            // Cụ thể, chúng ta đang truy vấn bảng "profile".
            if (adminck == null)
            {
                return RedirectToAction("Index", "Home");
            }
            HttpContext.Session.SetString("check", "6");
            // Tìm kiếm thông tin về trò chơi trong cơ sở dữ liệu dựa trên ID.
            Game ga = this._db.game.Find(id);

            // Nếu không tìm thấy trò chơi, chuyển hướng người dùng về trang quản lý.
            if (ga == null)
            {
                return RedirectToAction("Management", "Admin");
            }

            // Tạo một câu truy vấn SQL để lấy danh sách các danh mục đã được liên kết với trò chơi.
            String sqlQuery = "select cc.CID, c.categoryName from CategoryConnect cc join Category c on c.CID = cc.CID where cc.GID = '" + id + "'";

            // Thực thi truy vấn và lấy danh sách các danh mục.
            List<Category> categoryList = this._db.Set<Category>().FromSqlRaw(sqlQuery).ToList();

            // Tạo một đối tượng ViewCategory chứa thông tin chi tiết của trò chơi và danh sách các danh mục.
            ViewCategory viewCategory = new ViewCategory
            {
                GID = ga.GID,
                gameName = ga.gameName,
                price = ga.price,
                picture = ga.picture,
                date = ga.date,
                description = ga.description,
                configuration = ga.configuration,
                sellerName = ga.sellerName,
                status = ga.status,
                downloadFile = ga.downloadFile,
                viewCategory = categoryList
            };

            // Trả về view ViewGame với thông tin chi tiết của trò chơi.
            return View(viewCategory);
        }
        [HttpPost]
        public IActionResult Recorvery(string username)
        {
            Profile profile = this._db.profile.Find(username);
            if (profile == null)
            {
                return NotFound();
            }
            profile.status = "3";
            this._db.SaveChanges();
            return RedirectToAction("Management", "Admin");
        }


        // Phương thức để xóa một trò chơi trong cơ sở dữ liệu
        public IActionResult deleteGamee(string id)
        {
            // Tìm kiếm trò chơi trong cơ sở dữ liệu với id được cung cấp
            var ga = this._db.game.Find(id);

            // Cập nhật thuộc tính GID của trò chơi với chính giá trị hiện tại (không thay đổi)
            // (Dòng này có vẻ không cần thiết vì giá trị không thay đổi)
            ga.GID = ga.GID;

            // Cập nhật thuộc tính gameName của trò chơi với chính giá trị hiện tại (không thay đổi)
            ga.gameName = ga.gameName;

            // Cập nhật thuộc tính price của trò chơi với chính giá trị hiện tại (không thay đổi)
            ga.price = ga.price;

            // Cập nhật thuộc tính picture của trò chơi với chính giá trị hiện tại (không thay đổi)
            ga.picture = ga.picture;

            // Cập nhật thuộc tính date của trò chơi với chính giá trị hiện tại (không thay đổi)
            ga.date = ga.date;

            // Cập nhật thuộc tính description của trò chơi với chính giá trị hiện tại (không thay đổi)
            ga.description = ga.description;

            // Cập nhật thuộc tính configuration của trò chơi với chính giá trị hiện tại (không thay đổi)
            ga.configuration = ga.configuration;

            // Cập nhật thuộc tính sellerName của trò chơi với chính giá trị hiện tại (không thay đổi)
            ga.sellerName = ga.sellerName;

            // Đặt trạng thái của trò chơi thành "0" để đánh dấu là đã bị xóa
            ga.status = "0";

            ga.downloadFile = ga.downloadFile;

            // Lưu các thay đổi vào cơ sở dữ liệu
            this._db.SaveChanges();
            TempData["Message3"] = "Delete game successfully";

            // Chuyển hướng người dùng đến action "Management" của controller "Admin" sau khi xóa thành công
            return RedirectToAction("detailGame", "Admin", new { id = id });
        }
        // Phương thức để khôi phục một trò chơi đã xoá trong cơ sở dữ liệu
        public IActionResult recoveryGame(string id)
        {
            // Tìm kiếm trò chơi trong cơ sở dữ liệu với id được cung cấp
            var ga = this._db.game.Find(id);

            // Cập nhật thuộc tính GID của trò chơi với chính giá trị hiện tại (không thay đổi)
            // (Dòng này có vẻ không cần thiết vì giá trị không thay đổi)
            ga.GID = ga.GID;

            // Cập nhật thuộc tính gameName của trò chơi với chính giá trị hiện tại (không thay đổi)
            ga.gameName = ga.gameName;

            // Cập nhật thuộc tính price của trò chơi với chính giá trị hiện tại (không thay đổi)
            ga.price = ga.price;

            // Cập nhật thuộc tính picture của trò chơi với chính giá trị hiện tại (không thay đổi)
            ga.picture = ga.picture;

            // Cập nhật thuộc tính date của trò chơi với chính giá trị hiện tại (không thay đổi)
            ga.date = ga.date;

            // Cập nhật thuộc tính description của trò chơi với chính giá trị hiện tại (không thay đổi)
            ga.description = ga.description;

            // Cập nhật thuộc tính configuration của trò chơi với chính giá trị hiện tại (không thay đổi)
            ga.configuration = ga.configuration;

            // Cập nhật thuộc tính sellerName của trò chơi với chính giá trị hiện tại (không thay đổi)
            ga.sellerName = ga.sellerName;

            // Đặt trạng thái của trò chơi thành "1" để đánh dấu là đã được khôi phục
            ga.status = "1";

            ga.downloadFile = ga.downloadFile;

            // Lưu các thay đổi vào cơ sở dữ liệu
            this._db.SaveChanges();
            TempData["Message3"] = "Recover game successfully";

            // Chuyển hướng người dùng đến action "Management" của controller "Admin" sau khi khôi phục thành công
            return RedirectToAction("detailGame", "Admin", new { id = id });
        }

        public IActionResult deleteGamee1(string id)
        {
            // Tìm kiếm trò chơi trong cơ sở dữ liệu với id được cung cấp
            var ga = this._db.game.Find(id);

            // Cập nhật thuộc tính GID của trò chơi với chính giá trị hiện tại (không thay đổi)
            // (Dòng này có vẻ không cần thiết vì giá trị không thay đổi)
            ga.GID = ga.GID;

            // Cập nhật thuộc tính gameName của trò chơi với chính giá trị hiện tại (không thay đổi)
            ga.gameName = ga.gameName;

            // Cập nhật thuộc tính price của trò chơi với chính giá trị hiện tại (không thay đổi)
            ga.price = ga.price;

            // Cập nhật thuộc tính picture của trò chơi với chính giá trị hiện tại (không thay đổi)
            ga.picture = ga.picture;

            // Cập nhật thuộc tính date của trò chơi với chính giá trị hiện tại (không thay đổi)
            ga.date = ga.date;

            // Cập nhật thuộc tính description của trò chơi với chính giá trị hiện tại (không thay đổi)
            ga.description = ga.description;

            // Cập nhật thuộc tính configuration của trò chơi với chính giá trị hiện tại (không thay đổi)
            ga.configuration = ga.configuration;

            // Cập nhật thuộc tính sellerName của trò chơi với chính giá trị hiện tại (không thay đổi)
            ga.sellerName = ga.sellerName;

            // Đặt trạng thái của trò chơi thành "0" để đánh dấu là đã bị xóa
            ga.status = "4";

            ga.downloadFile = ga.downloadFile;

            // Lưu các thay đổi vào cơ sở dữ liệu
            this._db.SaveChanges();

            // Chuyển hướng người dùng đến action "Management" của controller "Admin" sau khi xóa thành công
            return RedirectToAction("Management", "Admin");
        }
        // Phương thức để khôi phục một trò chơi đã xoá trong cơ sở dữ liệu
        public IActionResult recoveryGame1(string id)
        {
            // Tìm kiếm trò chơi trong cơ sở dữ liệu với id được cung cấp
            var ga = this._db.game.Find(id);

            // Cập nhật thuộc tính GID của trò chơi với chính giá trị hiện tại (không thay đổi)
            // (Dòng này có vẻ không cần thiết vì giá trị không thay đổi)
            ga.GID = ga.GID;

            // Cập nhật thuộc tính gameName của trò chơi với chính giá trị hiện tại (không thay đổi)
            ga.gameName = ga.gameName;

            // Cập nhật thuộc tính price của trò chơi với chính giá trị hiện tại (không thay đổi)
            ga.price = ga.price;

            // Cập nhật thuộc tính picture của trò chơi với chính giá trị hiện tại (không thay đổi)
            ga.picture = ga.picture;

            // Cập nhật thuộc tính date của trò chơi với chính giá trị hiện tại (không thay đổi)
            ga.date = ga.date;

            // Cập nhật thuộc tính description của trò chơi với chính giá trị hiện tại (không thay đổi)
            ga.description = ga.description;

            // Cập nhật thuộc tính configuration của trò chơi với chính giá trị hiện tại (không thay đổi)
            ga.configuration = ga.configuration;

            // Cập nhật thuộc tính sellerName của trò chơi với chính giá trị hiện tại (không thay đổi)
            ga.sellerName = ga.sellerName;

            // Đặt trạng thái của trò chơi thành "1" để đánh dấu là đã được khôi phục
            ga.status = "1";

            ga.downloadFile = ga.downloadFile;

            // Lưu các thay đổi vào cơ sở dữ liệu
            this._db.SaveChanges();

            // Chuyển hướng người dùng đến action "Management" của controller "Admin" sau khi khôi phục thành công
            return RedirectToAction("Management", "Admin");
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

        public IActionResult Addnewaccount()
        {
            string adminck = HttpContext.Request.Cookies["AdminCookie"];
            // Đây là một truy vấn LINQ để lấy danh sách các hồ sơ từ cơ sở dữ liệu.
            // Cụ thể, chúng ta đang truy vấn bảng "profile".
            if (adminck == null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public IActionResult Addnewaccount(Profile p)
        {
            var pu = this._db.profile.Find(p.username);
            var pg = this._db.profile.FirstOrDefault(m => m.email == p.email);
            if (pu == null && pg == null)
            {
                p.status = "1";
                p.fullname = "";
                p.birthday = null;
                p.gender = "";
                p.password = GetMd5(p.password).ToLower();
                p.date = DateTime.Now;
                this._db.profile.Add(p);
                this._db.SaveChanges();
                ViewBag.Message1 = "Add new account successfully";
                return View();
            }
            if (pu != null)
            {
                ViewBag.Message2 = "Account already exists";

            }
            if (pg != null)
            {
                ViewBag.Message2 = "Email already exists";

            }
            return View();

        }
        public IActionResult Revenue()
        {
            string adminck = HttpContext.Request.Cookies["AdminCookie"];
            // Đây là một truy vấn LINQ để lấy danh sách các hồ sơ từ cơ sở dữ liệu.
            // Cụ thể, chúng ta đang truy vấn bảng "profile".
            if (adminck == null)
            {
                return RedirectToAction("Index", "Home");
            }

            try
            {
                string adminName = HttpContext.Request.Cookies["AdminCookie"];
                if (adminName == null)
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception e)
            {
                return RedirectToAction("Index", "Home");
            }

            string adminName2 = HttpContext.Request.Cookies["AdminCookie"];

            string sqlquery = "select mm.[admin] as adusername,"
                + " mm.[date],"
                + " YEAR(mm.[date]) AS [year],"
                + " MONTH(mm.[date]) AS [month],"
                + " DAY(mm.[date]) AS [day],"
                + " mm.sellerMoney as sellerMoneyPlus,"
                + " mm.admoney as adMoneyPlus,"
                + " p.[money] as admoney,"
                + " od.GID,"
                + " g.gameName,"
                + " g.picture,"
                + " g.[status] from MoneyManagement mm"
                + " join OrderDetail od on od.ODID = mm.ODID"
                + " join Game g on g.GID = od.GID"
                + " join [Profile] p on p.username = mm.[admin]"
                + " where [admin] = '" + "user01" + "'"
                + "and g.[status] != '3'"
                + " and g.[status] != '4'"
                + "order by mm.[date]";

            List<AdminRevenueViewModel> adRev = new List<AdminRevenueViewModel>();

            using (var command = this._db.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = sqlquery;
                this._db.Database.OpenConnection();

                using (var result = command.ExecuteReader())
                {
                    while (result.Read())
                    {
                        AdminRevenueViewModel item = new AdminRevenueViewModel
                        {
                            adusername = result.GetString(result.GetOrdinal("adusername")),
                            date = result.GetDateTime(result.GetOrdinal("date")),
                            year = result.GetInt32(result.GetOrdinal("year")),
                            month = result.GetInt32(result.GetOrdinal("month")),
                            day = result.GetInt32(result.GetOrdinal("day")),
                            sellerMoneyPlus = result.GetInt64(result.GetOrdinal("sellerMoneyPlus")),
                            adMoneyPlus = result.GetInt64(result.GetOrdinal("adMoneyPlus")),
                            admoney = result.GetInt64(result.GetOrdinal("adMoney")),
                            GID = result.GetString(result.GetOrdinal("GID")),
                            gameName = result.GetString(result.GetOrdinal("gameName")),
                            picture = result.GetString(result.GetOrdinal("picture")),
                            status = result.GetString(result.GetOrdinal("status"))
                        };

                        adRev.Add(item);
                    }
                }
            }
            var data = _db.moneyManagement.ToList();
            var rfs = data.GroupBy(mm => mm.username)
                             .Select(g => new RevenueFromSellers
                             {
                                 Username = g.Key,
                                 TotalSellerMoney = g.Sum(mm => mm.adMoney)
                             }).OrderByDescending(m => m.TotalSellerMoney)
                             .ToList();
            ViewBag.SellerList = rfs;
            return View(adRev);
        }
        public IActionResult DetailSellerRevenue(string id)
        {
            string adminck = HttpContext.Request.Cookies["AdminCookie"];
            // Đây là một truy vấn LINQ để lấy danh sách các hồ sơ từ cơ sở dữ liệu.
            // Cụ thể, chúng ta đang truy vấn bảng "profile".
            if (adminck == null)
            {
                return RedirectToAction("Index", "Home");
            }

            try
            {
                string adminName = HttpContext.Request.Cookies["AdminCookie"];
                if (adminName == null)
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception e)
            {
                return RedirectToAction("Index", "Home");
            }

            string sqlquery = "select mm.[username] as sellerName,"
                + " mm.[date],"
                + " YEAR(mm.[date]) AS [year],"
                + " MONTH(mm.[date]) AS [month],"
                + " DAY(mm.[date]) AS [day],"
                + " mm.admoney as sellerPlusMoney,"
                + " p.[money] as sellerMoney,"
                + " od.GID,"
                + " g.gameName,"
                + " g.picture,"
                + " g.[status] from MoneyManagement mm"
                + " join OrderDetail od on od.ODID = mm.ODID"
                + " join Game g on g.GID = od.GID"
                + " join [Profile] p on p.username = mm.username"
                + " where sellerName = '" + id + "'"
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
            string adminck = HttpContext.Request.Cookies["AdminCookie"];
            // Đây là một truy vấn LINQ để lấy danh sách các hồ sơ từ cơ sở dữ liệu.
            // Cụ thể, chúng ta đang truy vấn bảng "profile".
            if (adminck == null)
            {
                return RedirectToAction("Index", "Home");
            }

            try
            {
                string adminName = HttpContext.Request.Cookies["AdminCookie"];
                if (adminName == null)
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception e)
            {
                return RedirectToAction("Index", "Home");
            }

            string adminName2 = HttpContext.Request.Cookies["AdminCookie"];

            string sqlquery = "select mm.[admin] as adusername,"
                + " o.username as cusName,"
                + " mm.[date],"
                + " YEAR(mm.[date]) AS [year],"
                + " MONTH(mm.[date]) AS [month],"
                + " DAY(mm.[date]) AS [day],"
                + " od.price as gamePrice,"
                + " mm.sellerMoney as sellerMoneyPlus,"
                + " mm.admoney as adMoneyPlus,"
                + " p.[money] as admoney,"
                + " od.GID,"
                + " g.gameName,"
                + " g.picture,"
                + " g.[status] from MoneyManagement mm"
                + " join OrderDetail od on od.ODID = mm.ODID"
                + " join [Order] o on o.OID = od.OID"
                + " join Game g on g.GID = od.GID"
                + " join [Profile] p on p.username = mm.username"
                + " where [admin] = 'user01'"
                + " and g.GID = '" + id + "'"
                + " and g.[status] != '3'"
                + " and g.[status] != '4'"
                + " order by mm.[date]";

            List<AdminRevenueViewModel> adRev = new List<AdminRevenueViewModel>();

            using (var command = this._db.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = sqlquery;
                this._db.Database.OpenConnection();

                using (var result = command.ExecuteReader())
                {
                    while (result.Read())
                    {
                        AdminRevenueViewModel item = new AdminRevenueViewModel
                        {
                            adusername = result.GetString(result.GetOrdinal("adusername")),
                            cusName = result.GetString(result.GetOrdinal("cusName")),
                            date = result.GetDateTime(result.GetOrdinal("date")),
                            year = result.GetInt32(result.GetOrdinal("year")),
                            month = result.GetInt32(result.GetOrdinal("month")),
                            day = result.GetInt32(result.GetOrdinal("day")),
                            gamePrice = result.GetInt64(result.GetOrdinal("gamePrice")),
                            sellerMoneyPlus = result.GetInt64(result.GetOrdinal("sellerMoneyPlus")),
                            adMoneyPlus = result.GetInt64(result.GetOrdinal("adMoneyPlus")),
                            admoney = result.GetInt64(result.GetOrdinal("admoney")),
                            GID = result.GetString(result.GetOrdinal("GID")),
                            gameName = result.GetString(result.GetOrdinal("gameName")),
                            picture = result.GetString(result.GetOrdinal("picture")),
                            status = result.GetString(result.GetOrdinal("status"))
                        };

                        adRev.Add(item);
                    }
                }
            }

            return View(adRev);
        }

        public IActionResult toWaiting(string id)
        {
            // Tìm kiếm trò chơi trong cơ sở dữ liệu với id được cung cấp
            var ga = this._db.game.Find(id);

            // Cập nhật thuộc tính GID của trò chơi với chính giá trị hiện tại (không thay đổi)
            // (Dòng này có vẻ không cần thiết vì giá trị không thay đổi)
            ga.GID = ga.GID;

            // Cập nhật thuộc tính gameName của trò chơi với chính giá trị hiện tại (không thay đổi)
            ga.gameName = ga.gameName;

            // Cập nhật thuộc tính price của trò chơi với chính giá trị hiện tại (không thay đổi)
            ga.price = ga.price;

            // Cập nhật thuộc tính picture của trò chơi với chính giá trị hiện tại (không thay đổi)
            ga.picture = ga.picture;

            // Cập nhật thuộc tính date của trò chơi với chính giá trị hiện tại (không thay đổi)
            ga.date = ga.date;

            // Cập nhật thuộc tính description của trò chơi với chính giá trị hiện tại (không thay đổi)
            ga.description = ga.description;

            // Cập nhật thuộc tính configuration của trò chơi với chính giá trị hiện tại (không thay đổi)
            ga.configuration = ga.configuration;

            // Cập nhật thuộc tính sellerName của trò chơi với chính giá trị hiện tại (không thay đổi)
            ga.sellerName = ga.sellerName;

            // Đặt trạng thái của trò chơi thành "1" để đánh dấu là đã được khôi phục
            ga.status = "3";

            ga.downloadFile = ga.downloadFile;

            // Lưu các thay đổi vào cơ sở dữ liệu
            this._db.SaveChanges();

            // Chuyển hướng người dùng đến action "Management" của controller "Admin" sau khi khôi phục thành công
            return RedirectToAction("Management", "Admin");
        }

        public IActionResult changeseller(string id)
        {
            Profile profile = this._db.profile.Find(id);
            if (profile == null)
            {
                return NotFound();
            }
            profile.type = "2";
            this._db.SaveChanges();
            TempData["Message3"] = "Change account to seller successfully";
            return RedirectToAction("DeleteUser", "Admin", new { id = id });

        }
    }

}