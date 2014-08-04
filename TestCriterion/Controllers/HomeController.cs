using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Services;
using System.Xml.Schema;
using CriterionMore;
using TestCriterion.CriterionModel;

namespace TestCriterion.Controllers
{
   public  class TestModel
    {
         public string Tes { get; set; }
    }
    public class HomeController : Controller
    {
        static readonly List<Body> List = new List<Body>() {                                               
            new Body { Radio=null,Price=500, MadeIn = null, CheckBox = null, Drop = 1, Name = "ion",       DateTimeBody = DateTime.Now .AddDays(100) },
            new Body { Radio=1,   Price=600, MadeIn = 1,    CheckBox = 1,    Drop = 1, Name = "ionson",    DateTimeBody = DateTime.Now .AddDays(50) },
            new Body { Radio=3,   Price=700, MadeIn =3,     CheckBox = 3,    Drop = 2, Name = "ionic",     DateTimeBody = DateTime.Now .AddDays(10) },
            new Body { Radio=2,   Price=800, MadeIn = 2,    CheckBox = 2,    Drop = 3, Name = "ionionson", DateTimeBody = DateTime.Now .AddDays(1) },
            new Body { Radio=2,   Price=900, MadeIn = 1,    CheckBox = 2,    Drop = 2, Name = "ionerr",    DateTimeBody = DateTime.Now .AddDays(100) },
            new Body { Radio=1,   Price=1000,MadeIn = 2,    CheckBox = 2,    Drop = 5, Name = "iontt",     DateTimeBody = DateTime.Now ,MyControl = "ion"}
        };


        [HttpGet]
        public ActionResult Index()
        {
            
            return View(new TestModel());
        }

        [HttpPost]
        public ActionResult Index(string id)
        {
            TestModel tm=new TestModel();
            var resultExpression = CriterionActivator.GetExpression<Body>();
            if (resultExpression != null)
            {
                 var testlist = List.Where(resultExpression.Compile()).ToList();
                tm.Tes = resultExpression + "<br/>___________COUNT_____________" +
                         testlist.Count();
            }
       
            return View(tm);
        }

       

        [WebMethod]
        public string Criterion(string form, string type)
        {
            if (string.IsNullOrWhiteSpace(type) || string.IsNullOrWhiteSpace(form))
            {
                return null;
            }
          
            var col = new FormCollection();
            form = form.Replace("%2C", ",");
            foreach (var s in form.Split('&'))
            {
                var res = s.Split('=');
                col.Add(res[0], res[1]);
            }

            var resultExpression = CriterionActivator.GetExpression<Body>(col);
            if (resultExpression == null) return null;
       
            var testlist= List.Where(resultExpression.Compile()).ToList();
           

            return resultExpression+"<br/>___________COUNT_____________" +testlist.Count();
        }

        [WebMethod]
        public string Auto(string id)
        {
            var liste = List.Where(a => a.Name.StartsWith(id)).Select(s=>s.Name).ToList();
            var json = new JavaScriptSerializer().Serialize(liste);
            return json;
        }


    }
}
