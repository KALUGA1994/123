using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Linq.Dynamic;

namespace Exp001.Controllers
{
    public class GetUsersController : Controller
    {
        // GET: GetUsers
        public ActionResult Index()
        {
            return View();
        }

        public string CurrentLangCode { get; protected set; }
        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            if (requestContext.HttpContext.Request.Url != null)
            {
                var HostName = requestContext.HttpContext.Request.Url.Authority;
            }

            if (requestContext.RouteData.Values["lang"] != null && requestContext.RouteData.Values["lang"] as string != "null")
            {
                CurrentLangCode = requestContext.RouteData.Values["lang"] as string;
                var ci = new CultureInfo(CurrentLangCode);
                Thread.CurrentThread.CurrentUICulture = ci;
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(ci.Name);
            }
            base.Initialize(requestContext);
        }

        UsersEntities ue = new UsersEntities();
       

        public JsonResult getRoles()
        {
            var json = ue.Roles.Select(ue=>ue.Roles1);
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        public JsonResult getStatuses()
        {
            var json = ue.Statuses.Select(ue=>ue.Status);
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Get(int page = 1, int itemsPerPage = 3, string sortBy = "Roles1", bool reverse = false, string fieldname = null, string filtervalue = null, string firstdate = "", string seconddate = "")
        {

            var fieldName = fieldname;
            //database
            var result = ue.Users.Select(ue => new { ue.Roles1.Roles1, ue.Statuses1.Status, ue.EMail, ue.Name, ue.Surname, ue.dataRozh }).AsQueryable();
            //sort
            if (sortBy != null)
            {
                result = result.OrderBy(sortBy + (reverse ? " descending" : ""));
            }
            //filter OrderBy(sortBy + (reverse ? " descending" : ""));
            if (filtervalue == "")
            {
                filtervalue = null;
            }
                switch (fieldName)
                {
                    case "Roles":
                        if (filtervalue != null)
                        { result = result.Where(x => x.Roles1.ToLower().Contains(filtervalue.ToLower())); }
                        break;
                    case "Status":
                        if (filtervalue != null)
                        { result = result.Where(x => x.Status.ToLower().Contains(filtervalue.ToLower())); }
                        break;
                    case "EMail":
                        if (filtervalue != null)
                        { result = result.Where(x => x.EMail.ToLower().Contains(filtervalue.ToLower())); }
                        break;
                    case "Name":
                        if (filtervalue != null)
                        { result = result.Where(x => x.Name.ToLower().Contains(filtervalue)); }
                        break;
                    case "Surname":
                        if (filtervalue != null)
                        { result = result.Where(x => x.Surname.ToLower().Contains(filtervalue)); }
                        break;
                    case "dataRozh":
                        if ((firstdate != "" && seconddate != "") && (firstdate != seconddate))
                        {
                            result = result.Where(x => x.dataRozh > Convert.ToDateTime(firstdate) && x.dataRozh < Convert.ToDateTime(seconddate));
                        }
                        if ((firstdate == seconddate) && (firstdate != "" && seconddate != ""))
                        {
                            result = result.Where(x => x.dataRozh == Convert.ToDateTime(firstdate));
                        }
                        break;
                }              
            //paging
            var resultPaged = result.Skip((page - 1) * itemsPerPage).Take(itemsPerPage);
            //json
            var json = new
            {
                count = result.Count(),
                data = resultPaged.Select(e => new {
                    e.Roles1,
                    e.Status,
                    e.EMail,
                    e.Name,
                    e.Surname,
                    e.dataRozh
                })
            };
            return Json(json, JsonRequestBehavior.AllowGet);
        }
    }
}