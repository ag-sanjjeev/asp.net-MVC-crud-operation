using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Security;
using Crud_Operation.Models;

namespace Crud_Operation.Controllers.User
{
    public class UserController : Controller
    {
        // GET: User
        // Login page
        [AllowAnonymous]
        public ActionResult Login(string error = "", string success = "")
        {
            ViewBag.error = error;
            ViewBag.success = success;
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LoginUser(Crud_Operation.Models.User user)
        {
            string username = user.Username;
            string password = user.Password;
            if (String.IsNullOrEmpty(username) || String.IsNullOrWhiteSpace(username))
            {
                ViewData["error"] = "Please enter user name";
            }
            else
            {
                testdbsanEntities db = new testdbsanEntities();
                int count = db.Users.Where(u => u.Username == username && u.Activestatus == 1).Select(c => new { c.Hash, c.Salt }).Count();

                if (count > 0)
                {
                    var credentials = db.Users.Where(u => u.Username == username && u.Activestatus == 1).Select(c => new { c.Hash, c.Salt, c.UserId }).FirstOrDefault();

                    Session["UserId"] = credentials.UserId;
                    Session["UserName"] = username;
                    string hash = credentials.Hash;
                    string salt = credentials.Salt;
                    string saltedPassword = salt + password;
                    bool IsVerified = Crypto.VerifyHashedPassword(hash, saltedPassword);

                    if (IsVerified)
                    {
                        FormsAuthentication.SetAuthCookie(userName: hash, createPersistentCookie: false);                        
                        ViewData["success"] = "Login successfully";
                        return RedirectToAction("Index", "Home");
                    } else
                    {
                        ViewData["error"] = "Username or Password is incorrect";
                    }
                                        
                }
                else
                {
                    ViewData["error"] = username + " is not found";
                }
                
            }

            return RedirectToAction("Login", new { error = ViewData["error"] });
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }

        [AllowAnonymous]
        public ActionResult Registration(string error = "", string success = "")
        {
            ViewBag.error = error;
            ViewBag.success = success;
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RegisterUser(FormCollection form)
        {
            string username = form["Username"];
            string password = form["Password"];
            string confirmpassword = form["Confirmpassword"];

            if (String.IsNullOrEmpty(username) || String.IsNullOrWhiteSpace(username))
            {
                ViewData["error"] = "Please enter valid user name";
            }
            else if (String.IsNullOrEmpty(password) || String.IsNullOrWhiteSpace(password))
            {
                ViewData["error"] = "Please enter valid password";
            }
            else if (password != confirmpassword)
            {
                ViewData["error"] = "Please confirm password";
            }
            else
            {
                testdbsanEntities db = new testdbsanEntities();

                int count = db.Users.Where(m => m.Username == username).Select(u => u.Username).Count();

                if (count > 0)
                {
                    ViewData["error"] = username + " is already exist. please try different...";
                }
                else
                {
                    string salt = Crypto.GenerateSalt();
                    string saltpassword = salt + password;
                    string hashedPassword = Crypto.HashPassword(saltpassword);

                    Crud_Operation.Models.User user = new Crud_Operation.Models.User();
                    
                    user.Username = username;
                    user.Password = hashedPassword;
                    user.Salt = salt;
                    user.Hash = hashedPassword;
                    user.Activestatus = 1;
                    user.Created_at = System.DateTime.Now;
                    user.Modified_at = System.DateTime.Now;
                    user.IpAddress = "192.168.0.116";

                    db.Users.Add(user);
                    db.SaveChanges();

                    ViewData["Success"] = username + " is registered successfully";

                    return RedirectToAction("Login", new { success = ViewData["success"] });
                    
                }
            }


            return RedirectToAction("Registration", new { error = ViewData["error"] });
        }
        
        public ActionResult UserRole()
        {
            testdbsanEntities db = new testdbsanEntities();
            List<SelectListItem> MainRoles = new List<SelectListItem>();
            var MainRolesList = db.UserRoleTypes.Where(m => m.MainRoleId == 0).Select(m => new { m.RoleId, m.RoleName }).ToList();
            
            MainRoles.Add(new SelectListItem
            {
                Text = "Main Role",
                Value = "0"
            });
            foreach (var item in MainRolesList)
            {
                MainRoles.Add(new SelectListItem
                {
                    Text = item.RoleName,
                    Value = item.RoleId.ToString()
                });
            }

            ViewBag.MainRoles = MainRoles;

            return View();
        }

        public string[] UserDetails(string hash)
        {
            testdbsanEntities db = new testdbsanEntities();
            string[] user_details = new string[2];
            var credentials = db.Users.Where(u => u.Username == hash && u.Activestatus == 1).Select(c => new { c.UserId, c.Username }).FirstOrDefault();
            user_details[0] = Convert.ToString(credentials.UserId);
            user_details[1] = credentials.Username;

            return user_details;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UserRoleCreate(string mainRoleId, string roleName)
        {
            var resp = "";
            var respType = "";

            if (String.IsNullOrEmpty(mainRoleId))
            {
                resp = "Please choose Parant Role";
                respType = "error";
            }
            else if (String.IsNullOrEmpty(roleName))
            {
                resp = "Please choose Parant Role";
                respType = "error";
            }
            else
            {
                int mainroleid = Convert.ToInt32(mainRoleId);

                testdbsanEntities db = new testdbsanEntities();
                int count = db.UserRoleTypes.Where(m => m.MainRoleId == mainroleid && m.RoleName == roleName).Count();

                if (count > 0)
                {
                    resp = "Already exist";
                    respType = "error";
                }
                else
                {
                    count = db.UserRoleTypes.Where(m => m.RoleId == mainroleid && m.RoleName == roleName).Count();

                    if (count > 0)
                    {
                        resp = "Role Name can't be same as Main Role";
                        respType = "error";
                    } else
                    {

                        UserRoleType userRoleType = new UserRoleType();
                        userRoleType.MainRoleId = Convert.ToByte(mainroleid);
                        userRoleType.RoleName = roleName;

                        db.UserRoleTypes.Add(userRoleType);
                        db.SaveChanges();

                        resp = "Successfully created";
                        respType = "success";

                    }

                }
            }

            //string jsonresult = "{response: "+ resp +", responseType: " + respType + "}";
            var jsonresult = new { response = resp, responseType = respType};

            return Json(jsonresult, JsonRequestBehavior.AllowGet);
        }
   
    
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetAllUserRoleMain()
        {
            testdbsanEntities db = new testdbsanEntities();
            var options = db.UserRoleTypes.Where(m => m.MainRoleId == 0).Select(m => new { m.RoleId, m.RoleName }).ToList();

            var initialoption = new { text = "Main Role", value = 0 };

            var data = new { initialoption = initialoption, options = options };

            

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetUserRoleTypeList()
        {
            testdbsanEntities db = new testdbsanEntities();
            /*
             * select a.RoleId, 'Main Role' as MainRole, a.RoleName from[testdbsan].[dbo].[UserRoleTypes] a WHERE a.MainRoleId = 0
             *
             * UNION ALL 
             *
             * select a.RoleId, b.RoleName as MainRole, a.RoleName from[testdbsan].[dbo].[UserRoleTypes] a inner join[testdbsan].[dbo].[UserRoleTypes] b on a.MainRoleId = b.RoleId
             */
            
            var query = (from a in db.UserRoleTypes 
                         where a.MainRoleId == 0 
                         select new { 
                             RoleId = a.RoleId, 
                             MainRole = "Main Role", 
                             RoleName = a.RoleName 
                         }).Concat(
                         (from a in db.UserRoleTypes 
                          join b in db.UserRoleTypes on a.MainRoleId equals b.RoleId
                          select new
                          {
                             RoleId = a.RoleId,
                             MainRole = b.RoleName,
                             RoleName = a.RoleName
                          })
                         );

            var response = query.ToList();

            var data = new { response = response };

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        /* Http put, patch and delete verbose methods not woks due to web.config */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteUserRoleTypeList()
        {
            testdbsanEntities db = new testdbsanEntities();

            var all = from a in db.UserRoleTypes select a;
            db.UserRoleTypes.RemoveRange(all);
            db.SaveChanges();

            var data = new { response = "Successfully Deleted" };
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    
            
        public ActionResult UserPermission()
        {
            if (System.Web.HttpContext.Current.Session["UserName"].ToString().ToLower() == "admin")
            {
                return View();
            } 
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Not Allowed");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UserPermissionRoleTypeList()
        {
            testdbsanEntities db = new testdbsanEntities();

            /*
             *  select RoleName, RoleId, Levels, MainRoleId from (
             *   
             *  select a.RoleId as RoleId, a.RoleName as RoleName, a.MainRoleId as MainRoleId, 'level 1' as Levels from [testdbsan].[dbo].[UserRoleTypes] a WHERE a.MainRoleId =0 
             *
             *  UNION 
             *
             *  select c.RoleId as RoleId, c.RoleName as RoleName, c.MainRoleId as MainRoleId, 'level 2' as Levels from [testdbsan].[dbo].[UserRoleTypes] c WHERE c.MainRoleId IN (SELECT RoleId from [testdbsan].[dbo].[UserRoleTypes]) 
             *
             *  ) as Table1 ORDER BY RoleId, Levels
             *             
             */

            /*var userRoleList = (from c in db.UserRoleTypes select new { c.RoleId }).ToList();*/


            /*var query = (from a in db.UserRoleTypes
                         where a.MainRoleId == 0
                         select new {
                             RoleId = a.RoleId,
                             RoleName = a.RoleName,
                             MainRoleId = a.MainRoleId,
                             Levels = "level 1"
                         }).Concat(
                            (from b in db.UserRoleTypes
                             where userRoleList.Contains(b.RoleId)
                             select new
                             {
                                 RoleId = b.RoleId,
                                 RoleName = b.RoleName,
                                 MainRoleId = b.MainRoleId,
                                 Levels = "level 2"
                             }
                            )
                        );*/

            var result = db.UserRoleTypesListHeirarchy().ToList();


            var data = new { response = result };
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    
    
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UserPermissionCreate(FormCollection form)
        {



            var data = new { response = form };

            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
    
}
