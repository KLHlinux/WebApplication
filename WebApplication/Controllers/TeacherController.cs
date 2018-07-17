using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    public class TeacherController : Controller
    {
        private ASP_testEntities2 db = new ASP_testEntities2();

        private static readonly int PAGE_SIZE = 4;

        private List<SelectListItem> GetProposalList()
        {
            return new List<SelectListItem>() {
              new SelectListItem{
                     Text = "提案",Value = "1"
              },new SelectListItem{
                     Text = "意见",Value = "2"
              }
            };
        }

        private List<SelectListItem> GetTypeList()
        {
            var proposals = db.Proposal_type.OrderBy(m => m.id).Select(m => m.proposal).Distinct();

            var items = new List<SelectListItem>();
            foreach (string proposal in proposals)
            {
                items.Add(new SelectListItem{
                    Text = proposal,
                    Value = proposal
                });
            }
            return items;
        }

        private List<SelectListItem> GetNameList()
        {
            var teachers = db.Teacher.OrderBy(T => T.id).Select(T => T.username).Distinct();

            var items = new List<SelectListItem>();
            foreach (string teacher in teachers){
                items.Add(new SelectListItem{
                    Text = teacher,
                    Value = teacher
                });
            }
            return items;
        }

        private int GetPageCount(int recordCount)
        {
            int pageCount = recordCount / PAGE_SIZE;
            if (recordCount % PAGE_SIZE != 0)
            {
                pageCount += 1;
            }
            return pageCount;
        }

        private List<Proposal> GetPagedDataSource(IQueryable<Proposal> proposal, int pageIndex, int recordCount)
        {
            var pageCount = GetPageCount(recordCount);
            if (pageIndex >= pageCount && pageCount >= 1)
            {
                pageIndex = pageCount - 1;
            }

            return proposal.OrderBy(p => p.id)
                .Skip(pageIndex * PAGE_SIZE)
                .Take(PAGE_SIZE).ToList();
        }

        //首页
        public ActionResult Index()
        {
            var id = ((Teacher)Session["teacher"]).id;
            var proposal = db.Proposal.Where(p => p.teacher_id == id);

            return View(proposal);
        }

        //待审提案
        public ActionResult Browse_F()
        {
            //var id = ((Teacher)Session["teacher"]).id;
            //var proposal = db.Proposal.Where(p => p.teacher_id == id & p.Execute.register==0);
            //return View(proposal);
            var id = ((Teacher)Session["teacher"]).id;
            var proposal = db.Proposal.Where(p => p.teacher_id == id & p.Execute.register == 0);
            proposal = proposal as IQueryable<Proposal>;
            var recordCount = proposal.Count();
            var pageCount = GetPageCount(recordCount);
            ViewBag.PageIndex = 0;
            ViewBag.PageCount = pageCount;
            ViewBag.TypeList = GetTypeList();
            return View(GetPagedDataSource(proposal, 0, recordCount));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Browse_F(string proposal, string Name, int PageIndex)
        {
            var id = ((Teacher)Session["teacher"]).id;
            var proposals = db.Proposal.Where(p => p.teacher_id == id & p.Execute.register == 0);
            proposals = proposals as IQueryable<Proposal>;
            if (!String.IsNullOrEmpty(Name)){
                proposals = proposals.Where(p => p.title == Name); //检索提案标题
            }
            if (!String.IsNullOrEmpty(proposal)){
                proposals = proposals.Where(p => p.Proposal_type.proposal == proposal);//检索提案类型
            }
            var recordCount = proposals.Count();
            var pageCount = GetPageCount(recordCount);
            if (PageIndex >= pageCount && pageCount >= 1){
                PageIndex = pageCount - 1;
            }
            proposals = proposals.OrderBy(p => p.id).Skip(PageIndex * PAGE_SIZE).Take(PAGE_SIZE);
            ViewBag.PageIndex = PageIndex;
            ViewBag.PageCount = pageCount;
            ViewBag.TypeList = GetTypeList();
            return View(proposals);
        }



        //取消提案
        public ActionResult Delete(int id)
        {
            Proposal proposal = db.Proposal.Find(id);
            db.Proposal.Remove(proposal);
            db.SaveChanges();
            return RedirectToAction("Browse_F");
        }

        //已审提案
        public ActionResult Browse_T()
        {
            //var id = ((Teacher)Session["teacher"]).id;
            //var proposal = db.Proposal.Where(p => p.teacher_id == id & p.Execute.register!=0);
            //return View(proposal);
            var id = ((Teacher)Session["teacher"]).id;
            var proposal = db.Proposal.Where(p => p.teacher_id == id & p.Execute.register != 0);

            proposal = proposal as IQueryable<Proposal>;
            var recordCount = proposal.Count();
            var pageCount = GetPageCount(recordCount);
            ViewBag.PageIndex = 0;
            ViewBag.PageCount = pageCount;

            ViewBag.TypeList = GetTypeList();

            return View(GetPagedDataSource(proposal, 0, recordCount));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Browse_T(string proposal, string Name, int PageIndex)
        {
            var id = ((Teacher)Session["teacher"]).id;
            var proposals = db.Proposal.Where(p => p.teacher_id == id & p.Execute.register != 0);
            proposals = proposals as IQueryable<Proposal>;
            if (!String.IsNullOrEmpty(Name))
            {
                proposals = proposals.Where(p => p.title == Name); //检索提案标题
            }
            if (!String.IsNullOrEmpty(proposal))
            {
                proposals = proposals.Where(p => p.Proposal_type.proposal == proposal);//检索提案类型
            }
            var recordCount = proposals.Count();
            var pageCount = GetPageCount(recordCount);
            if (PageIndex >= pageCount && pageCount >= 1)
            {
                PageIndex = pageCount - 1;
            }
            proposals = proposals.OrderBy(p => p.id).Skip(PageIndex * PAGE_SIZE).Take(PAGE_SIZE);
            ViewBag.PageIndex = PageIndex;
            ViewBag.PageCount = pageCount;
            ViewBag.TypeList = GetTypeList();
            return View(proposals);
        }

        //我的提案
        public ActionResult Browse()
        {
            var id = ((Teacher)Session["teacher"]).id;
            var proposal = db.Proposal.Where(p => p.teacher_id == id);

            proposal = proposal as IQueryable<Proposal>;
            var recordCount = proposal.Count();
            var pageCount = GetPageCount(recordCount);
            ViewBag.PageIndex = 0;
            ViewBag.PageCount = pageCount;

            ViewBag.TypeList = GetTypeList();

            return View(GetPagedDataSource(proposal, 0, recordCount));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Browse(string proposal, string Name, int PageIndex)
        {
            var id = ((Teacher)Session["teacher"]).id;
            var proposals = db.Proposal.Where(p => p.teacher_id == id);
            proposals = proposals as IQueryable<Proposal>;
            if (!String.IsNullOrEmpty(Name)){
                proposals = proposals.Where(p => p.title==Name) ; //检索提案标题
            }
            if (!String.IsNullOrEmpty(proposal)){
                proposals = proposals.Where(p => p.Proposal_type.proposal == proposal );//检索提案类型
            }
            var recordCount = proposals.Count();
            var pageCount = GetPageCount(recordCount);
            if (PageIndex >= pageCount && pageCount >= 1){
                PageIndex = pageCount - 1;
            }
            proposals = proposals.OrderBy(p => p.id).Skip(PageIndex * PAGE_SIZE).Take(PAGE_SIZE);
            ViewBag.PageIndex = PageIndex;
            ViewBag.PageCount = pageCount;
            ViewBag.TypeList = GetTypeList();
            return View(proposals);
        }

        //提交提案
        public ActionResult Create()
        {
            ViewBag.ProposalList = GetProposalList();
            ViewBag.NameList = GetNameList();
            return View();
        }

        [HttpPost]
        public ActionResult Create(Proposal proposal,string value1,string value2,string value3)
        {
            var id = ((Teacher)Session["teacher"]).id;
            proposal.teacher_id = id;
            db.Proposal.Add(proposal);
            db.SaveChanges();
            var j = db.Jointly.FirstOrDefault(J => J.proposal == proposal.id);
            string id1 = (db.Teacher.FirstOrDefault(t => t.username == value1)).id;
            string id2 = (db.Teacher.FirstOrDefault(t => t.username == value2)).id;
            string id3 = (db.Teacher.FirstOrDefault(t => t.username == value3)).id;
            j.teacher = id1 + ","+ id2 + ","+ id3;
            db.SaveChanges();
            ViewBag.NameList = GetNameList();
            return RedirectToAction("Index");
        }

        //关于网站
        public ActionResult About()
        {
            return View();
        }

        //教师信息
        public ActionResult Info()
        {
            var id = ((Teacher)Session["teacher"]).id;
            Teacher teacher = db.Teacher.Find(id);

            return View(teacher);
        }

        //提案内容
        public ActionResult Content(int id)
        {
            ViewData["jointly"] = "";
            Proposal proposal = db.Proposal.Find(id);
            var t = db.Jointly.FirstOrDefault(j => j.proposal == id);
            if (t!=null && t.teacher != null)
            {
                string a= t.teacher;
                string b = "";
                string s = "";
                string[] arrays = a.Split(',');
                for(int i = 0; i<arrays.Length; i++)
                {
                    s = arrays[i];
                    var teacher = db.Teacher.FirstOrDefault(T => T.id == s);
                    b = b + teacher.username + "  ";
                }
                ViewData["jointly"]=b;
            }
            return View(proposal);
        }

    }
}