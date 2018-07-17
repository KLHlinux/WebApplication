using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    public class DepartmentController : Controller
    {
        private ASP_testEntities2 db = new ASP_testEntities2();

        // GET: Department
        public ActionResult Index()
        {
            return View();
        }

        //管理员信息
        public ActionResult Info()
        {
            var id = ((Teacher)Session["teacher"]).id;
            Teacher teacher = db.Teacher.Find(id);

            return View(teacher);
        }

        //提案管理
        public ActionResult Manage()
        {
            var department = ((Teacher)Session["teacher"]).department;
            var result = db.Result.Where(r => r.Execute.department == department & r.acceptance==0);

            return View(result);
        }

        //审核
        public ActionResult Browse(int proposal)
        {
            Result result = db.Result.Find(proposal);
            return View(result);
        }

        [HttpPost]
        public ActionResult Browse(Result result)
        {
            db.Entry(result).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Manage");
        }

        //已处理的提案
        public ActionResult Manage_ok()
        {
            var department = ((Teacher)Session["teacher"]).department;
            var result = db.Result.Where(r => r.Execute.department == department & r.acceptance!= 0);

            return View(result);
        }

        //提案内容
        public ActionResult Content(int id)
        {
            ViewData["jointly"] = "";
            Proposal proposal = db.Proposal.Find(id);
            var t = db.Jointly.FirstOrDefault(j => j.proposal == id);
            if (t != null && t.teacher != null)
            {
                string a = t.teacher;
                string b = "";
                string s = "";
                string[] arrays = a.Split(',');
                for (int i = 0; i < arrays.Length; i++)
                {
                    s = arrays[i];
                    var teacher = db.Teacher.FirstOrDefault(T => T.id == s);
                    b = b + teacher.username + "  ";
                }
                ViewData["jointly"] = b;
            }
            return View(proposal);
        }

        //编写执行结果
        public ActionResult Write(int proposal)
        {
            Result result = db.Result.Find(proposal);
            return View(result);
        }

        [HttpPost]
        public ActionResult Write(Result result,string submit)
        {
            if (submit == "返回"){ }
            if (submit == "保存"){
                db.Entry(result).State = EntityState.Modified;
                db.Entry(result).Property("acceptance").IsModified = false;
                db.Entry(result).Property("opinion").IsModified = false;
                db.SaveChanges();
            } 
            return RedirectToAction("Manage_ok");
        }

        //受理
        public ActionResult AC(int proposal)
        {
            Result result = db.Result.Find(proposal);
            result.acceptance = 1;
            db.SaveChanges();
            return RedirectToAction("Manage");
        }
        //不受理
        public ActionResult nAC(int proposal)
        {
            Result result = db.Result.Find(proposal);
            result.acceptance = -1;
            db.SaveChanges();
            return RedirectToAction("Manage");
        }

    }
}