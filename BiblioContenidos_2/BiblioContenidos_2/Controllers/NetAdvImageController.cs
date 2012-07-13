using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.IO;
using Telerik.Web.Mvc.UI;
using BiblioContenidos_2.Models;


namespace BiblioContenidos_2.Controllers
{
    public class NetAdvImageController : Controller
    {
        // TODO: Do we need interfaces and dependency injection? (probably not since we're working with the file system)
        NetAdvImageService imageService = new NetAdvImageService();
        NetAdvDirectoryService directoryService = new NetAdvDirectoryService();

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Index(string path)
        {
            string filePath = String.Empty;

            try
            {
                int length = 4096;
                int bytesRead = 0;
                Byte[] buffer = new Byte[length];

                // This works with Chrome/FF/Safari
                // get the name from qqfile url parameter here

                if (String.IsNullOrEmpty(Request["qqfile"]))
                {
                    // IE
                    filePath = Path.Combine(path, System.IO.Path.GetFileName(Request.Files[0].FileName));
                }
                else
                {
                    // Webkit, Mozilla
                    filePath = Path.Combine(path, Request["qqfile"]);
                }

                try
                {
                    using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        do
                        {
                            bytesRead = Request.InputStream.Read(buffer, 0, length);
                            fileStream.Write(buffer, 0, bytesRead);
                        }
                        while (bytesRead > 0);
                    }
                }
                catch (UnauthorizedAccessException ex)
                {
                    // log error hinting to set the write permission of ASPNET or the identity accessing the code
                    return Json(new { success = false, message = ex.Message }, "application/json");
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, "application/json");
            }

            return Json(new { success = true }, "application/json");
        }

        [HttpPost]
        public JsonResult _GetImages(string path)
        {
            return new JsonResult { Data = imageService.GetImages(path, HttpContext) };
        }

        [HttpPost]
        public JsonResult _DeleteImage(string path, string name)
        {
            return new JsonResult { Data = imageService.DeleteImage(path, name) };
        }

        [HttpPost]
        public JsonResult _MoveDirectory(string path, string destinationPath)
        {
            return new JsonResult { Data = directoryService.MoveDirectory(path, destinationPath) };
        }

        [HttpPost]
        public JsonResult _DeleteDirectory(string path)
        {
            return new JsonResult { Data = directoryService.DeleteDirectory(path, HttpContext) };
        }

        [HttpPost]
        public JsonResult _AddDirectory(string path)
        {
            // If no destination folder was selected, add new folder to root upload path
            if (String.IsNullOrEmpty(path))
                path = Server.MapPath(NetAdvImageSettings._uploadPath);

            _ExpandDirectoryState(path);
            return new JsonResult { Data = directoryService.AddDirectory(path) };
        }

        [HttpPost]
        public JsonResult _GetDirectories()
        {
            // Ensure the upload directory exists
            directoryService.CreateUploadDirectory(HttpContext);

            return new JsonResult { Data = directoryService.GetDirectoryTree(HttpContext) };
        }

        [HttpPost]
        public JsonResult _RenameDirectory(string path, string name)
        {
            return new JsonResult { Data = directoryService.RenameDirectory(path, name, HttpContext) };
        }

        [HttpPost]
        public ActionResult _ExpandDirectoryState(string value)
        {
            // Get or initalize list in session
            if (Session[NetAdvImageSettings._netAdvImageTreeStateSessionKey] == null)
                Session[NetAdvImageSettings._netAdvImageTreeStateSessionKey] = new List<string>();
            List<string> expandedNodes = Session[NetAdvImageSettings._netAdvImageTreeStateSessionKey] as List<string>;

            // Persist the expanded state of the directory in session
            if (!expandedNodes.Contains(value))
            {
                expandedNodes.Add(value);
                Session[NetAdvImageSettings._netAdvImageTreeStateSessionKey] = expandedNodes;
            }

            return new EmptyResult();
        }

        [HttpPost]
        public ActionResult _CollapseDirectoryState(string value)
        {
            // Get or initalize list in session
            if (Session[NetAdvImageSettings._netAdvImageTreeStateSessionKey] == null)
                Session[NetAdvImageSettings._netAdvImageTreeStateSessionKey] = new List<string>();
            List<string> expandedNodes = Session[NetAdvImageSettings._netAdvImageTreeStateSessionKey] as List<string>;

            // Persist the collapsed state of the directory in session
            if (expandedNodes.Contains(value))
            {
                expandedNodes.Remove(value);
                Session[NetAdvImageSettings._netAdvImageTreeStateSessionKey] = expandedNodes;
            }

            return new EmptyResult();
        }

    }
}
