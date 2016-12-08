using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.Entity;
using System.Net;
using System.Web.Mvc;
using System.Data.Entity.Infrastructure;
using WebApplication1.Models;


namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {




        // GET: HotelData

        documentDBEntities db = new documentDBEntities();

        public ActionResult Index()
        {
            var duplicateType = from h in db.duplicateDetails
                                orderby h.idDuplicateType
                                select new { text = h.duplicateDetails, value = h.idDuplicateType };

            ViewBag.duplicateType = duplicateType.ToList();

            return View();

        }
        public ActionResult HighLevelOfInfo()
        {
            var duplicateType = from h in db.duplicateDetails
                                orderby h.idDuplicateType
                                select new { text = h.duplicateDetails, value = h.idDuplicateType };

            ViewBag.duplicateType = duplicateType.ToList();

            return View();

        }




        public JsonResult GetDocumentData()
        {
            try
            {

                var documents = db.documents.Include(d => d.duplicateDetail);
                var collection = documents.Select(x => new

                {
                    idDoc = x.idDoc,
                    docName = x.docName,
                    docTypeID = x.docTypeID,
                    note = x.note,
                    confidenceScore=x.confidenceScore

                }).ToList();

                return Json(collection, JsonRequestBehavior.AllowGet);

            }
            catch (Exception e)
            {
                return Json(new { Success = false, Message = e.ToString() });

            }

        }



        [HttpPost]

        public JsonResult UpdateDocumentData(document model)
        {
            try
            {
                int Id = Convert.ToInt32(model.idDoc);
                var updateData = (from f in db.documents
                                  where Id == f.idDoc
                                  select f).FirstOrDefault();

                updateData.docName = model.docName;
                updateData.docTypeID = model.docTypeID;
                updateData.note = model.note;

                db.SaveChanges();
            }
            catch (Exception e)
            {
                return Json(new { Success = false, Message = e.ToString() });
            }

            return Json(model);
        }


        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            document doc = db.documents.Find(id);
            if (doc == null)
            {
                return HttpNotFound();
            }
            return View(doc);

        }
        // POST: Home/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            document doc = db.documents.Find(id);
            db.documents.Remove(doc);
            db.SaveChanges();
            return RedirectToAction("Index");

        }
    }

}

