using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    public class AdministratorsController : Controller
    {
        private ASP_testEntities2 db = new ASP_testEntities2();

        private List<SelectListItem> GetNameList()
        {
            var names = db.Department.OrderBy(d => d.id).Select(d => d.name).Distinct();

            var items = new List<SelectListItem>();
            foreach (string name in names)
            {
                items.Add(new SelectListItem
                {
                    Text = name,
                    Value = name
                });
            }
            return items;
        }

        public static SelectList RegList()
        {
            List<SelectListItem> items = new List<SelectListItem>()
            {
                new SelectListItem(){Text="是", Value="1"},
                new SelectListItem(){Text="否", Value="-1"},
            };

            SelectList regList = new SelectList(items, "Value", "Text", 2);
            return regList;
        }

        // 管理员首页
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

        //已处理的提案
        public ActionResult Browse_ok()
        {
            var execute = db.Execute.Where(e => e.register != 0);

            return View(execute);
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

        //审核提案
        public ActionResult Manage()
        {
            var execute = db.Execute.Where(e => e.register==0);

            return View(execute);
        }

        //审核
        public ActionResult Edit(int proposal)
        {
            ViewData["jointly"] = "";
            var t = db.Jointly.FirstOrDefault(j => j.proposal == proposal);
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

            Execute execute = db.Execute.Find(proposal);
            ViewData["Reg"] = RegList().AsEnumerable();
            ViewBag.NameList = GetNameList(); 
            return View(execute);
        }

        [HttpPost]
        public ActionResult Edit(Execute execute,string submit,string department)
        {
            if (submit=="保存"){
                db.Entry(execute).State = EntityState.Modified;
                if (department!="")
                {
                    Department dep = db.Department.FirstOrDefault(d => d.name == department);
                    execute.department = dep.id;
                }
                db.SaveChanges();
            }
            if (submit=="返回"){
                
            }
            ViewBag.NameList = GetNameList();
            return RedirectToAction("Manage");  
        }

    }
}