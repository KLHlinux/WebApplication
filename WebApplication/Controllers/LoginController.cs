using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    public class LoginController : Controller
    {
        private ASP_testEntities2 db = new ASP_testEntities2();

        //登录
        public ActionResult Index()
        {
            ViewBag.LoginState = "登录前...";
            return View();
        }

        [HttpPost]
        public ActionResult Index(Teacher t, string administrators)
        {
            int type = 1;

            if (administrators == "administrators")
            {
                type = 2;
            }
            else if (administrators == "department")
            {
                type = 3;
            }

            //判断账号密码是否正确
            var teacher = db.Teacher.FirstOrDefault(l => l.account == t.account & l.password == t.password & l.type == type);

            if (teacher != null)
            {
                System.Web.HttpContext.Current.Session["teacher"] = teacher;
                if (type == 1)
                {
                    return RedirectToAction("Index", "Teacher");
                }
                else if(type==2)
                {
                    return RedirectToAction("Index", "Administrators");
                }
                else
                {
                    return RedirectToAction("Index", "Department");
                }
            }
            else
            {

            }
            return RedirectToAction("Index", "Login");
        }
    }

}