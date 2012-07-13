using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.IO;
using Telerik.Web.Mvc.UI;
using System.Configuration;

namespace BiblioContenidos_2.Models
{
    #region Models

    public class NetAdvImage
    {
        public string FileName { get; set; }
        public string Path { get; set; }
    }

    #endregion

    #region Services

    public class NetAdvDirectoryService
    {
        /// <summary>
        /// Gets the directory structure starting at the upload path
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns>An array of tree items</returns>
        public IEnumerable<TreeViewItemModel> GetDirectoryTree(HttpContextBase ctx)
        {
            return new[] { GetDirectoryRecursive(new DirectoryInfo(ctx.Server.MapPath(NetAdvImageSettings._uploadPath)), null, ctx) };
        }

        /// <summary>
        /// Builds the directory tree. A value of null for 'parentItem' denotes the root node.
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="parentItem"></param>
        /// <returns>A TreeViewItem structure</returns>
        public TreeViewItemModel GetDirectoryRecursive(DirectoryInfo directory, TreeViewItemModel parentItem, HttpContextBase ctx)
        {
            // If 'parentNode' is null, assume we're starting at the upload path
            string path = parentItem != null ?
                Path.Combine(parentItem.Value, directory.Name) :
                directory.FullName;

            // Get or initalize list in session
            if (ctx.Session[NetAdvImageSettings._netAdvImageTreeStateSessionKey] == null)
                ctx.Session[NetAdvImageSettings._netAdvImageTreeStateSessionKey] = new List<string>();
            List<string> expandedNodes = ctx.Session[NetAdvImageSettings._netAdvImageTreeStateSessionKey] as List<string>;

            // Create a new TreeViewItem
            TreeViewItemModel item = new TreeViewItemModel()
            {
                Text = directory.Name,
                Value = path,
                ImageUrl = "/Scripts/tiny_mce/plugins/netadvimage/img/folder-horizontal.gif",
                Enabled = true,
                Expanded = parentItem == null ?
                    true : // Expand the root node
                    expandedNodes.Contains(path) // Or... get expanded state from session
            };

            // Recurse through the current directory's sub-directories
            foreach (DirectoryInfo child in directory.GetDirectories())
            {
                TreeViewItemModel childNode = GetDirectoryRecursive(child, item, ctx);
                item.Items.Add(childNode);
            }

            return item;
        }

        /// <summary>
        /// Creates the upload directory if it does not already exist
        /// </summary>
        /// <param name="ctx"></param>
        public void CreateUploadDirectory(HttpContextBase ctx)
        {
            // Ensure upload directory exists
            if (!Directory.Exists(ctx.Server.MapPath(NetAdvImageSettings._uploadPath)))
                Directory.CreateDirectory(ctx.Server.MapPath(NetAdvImageSettings._uploadPath));
        }

        /// <summary>
        /// Moves a directory from one location to another
        /// </summary>
        /// <param name="path"></param>
        /// <param name="destinationPath"></param>
        /// <returns></returns>
        public string MoveDirectory(string path, string destinationPath)
        {
            try
            {
                // Move it!
                Directory.Move(path, Path.Combine(destinationPath, Path.GetFileName(path)));

                // Cleanup
                if (Directory.Exists(path))
                    Directory.Delete(path, true);

                // null == success
                return null;
            }
            catch (Exception ex)
            {
                // Return the error message to be alerted.
                return ex.Message;
            }
        }

        /// <summary>
        /// Deletes directory
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string DeleteDirectory(string path, HttpContextBase ctx)
        {
            try
            {
                if (path == ctx.Server.MapPath(NetAdvImageSettings._uploadPath))
                    return "You cannot delete the root folder.";

                // Delete it!
                Directory.Delete(path, true);
                return null;
            }
            catch (Exception ex)
            {
                // Return the error message to be alerted.
                return ex.Message;
            }
        }

        /// <summary>
        /// Inserts a new directory into the given physical path
        /// A unique name is applied to conflicting directory names
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string AddDirectory(string path)
        {
            try
            {
                // Ensure unique folder name
                int counter = 1;
                while (Directory.Exists(Path.Combine(path, "New Folder" + (counter > 1 ? String.Format(" ({0})", counter) : String.Empty))))
                    counter++;

                // Add directory
                Directory.CreateDirectory(
                    Path.Combine(path, "New Folder" + (counter > 1 ? String.Format(" ({0})", counter) : String.Empty))
                    );

                return null;
            }
            catch (Exception ex)
            {
                // Return the error message to be alerted.
                return ex.Message;
            }
        }

        /// <summary>
        /// Renames a directory
        /// </summary>
        /// <param name="path"></param>
        /// <param name="name"></param>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public string RenameDirectory(string path, string name, HttpContextBase ctx)
        {
            try
            {
                // Prevent renaming of root
                if (path.Equals(ctx.Server.MapPath(NetAdvImageSettings._uploadPath)))
                    throw new Exception("Cannot rename the root directory");

                // Trim extraneous slashes
                if (path.EndsWith(@"\"))
                    path = path.Substring(0, path.LastIndexOf(@"\"));

                name = Path.Combine(path.Substring(0, path.LastIndexOf(@"\") + 1), name.Trim());

                Directory.Move(path, Path.Combine(path, name));
                return null;
            }
            catch (Exception ex)
            {
                // Return the error message to be alerted.
                return ex.Message;
            }
        }
    }

    public class NetAdvImageService
    {
        /// <summary>
        /// Gets a list of top-level images within a given directory
        /// </summary>
        /// <param name="path"></param>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public IEnumerable<NetAdvImage> GetImages(string path, HttpContextBase ctx)
        {
            return

                from img in Directory.GetFiles(path)

                // Limit file types to images only
                where NetAdvImageSettings._allowedFileTypes.Any(ext => img.EndsWith(ext, StringComparison.OrdinalIgnoreCase))

                select new NetAdvImage
                {
                    FileName = Path.GetFileName(img),
                    // The image object will be used by the client, so we'll need a relative path instead of a physical path
                    Path = ctx.Request.Url.AbsoluteUri.Replace(ctx.Request.Url.PathAndQuery, String.Empty) +
                           (ctx.Request.ApplicationPath.Equals("/") ? String.Empty : ctx.Request.ApplicationPath) +
                           img.Replace(ctx.Server.MapPath("~/"), "/").Replace(@"\", "/")
                };
        }

        /// <summary>
        /// Deletes a image
        /// </summary>
        /// <param name="path"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public string DeleteImage(string path, string name)
        {
            try
            {
                string imgToDelete = Path.Combine(path, name);

                if (!File.Exists(imgToDelete))
                    throw new Exception("Image does not exist");

                // Delete it!
                File.Delete(imgToDelete);
                return null;
            }
            catch (Exception ex)
            {
                // Return the error message to be alerted.
                return ex.Message;
            }
        }
    }

    #endregion

    #region Service settings & helpers

    public class NetAdvImageSettings
    {
        public static string _netAdvImageTreeStateSessionKey = "NetAdvImageTreeState";

        public static string _uploadPath
        {
            get
            {
                return
                    ConfigurationManager.AppSettings["netAdvImage_UploadPath"] != null ?
                    ConfigurationManager.AppSettings["netAdvImage_UploadPath"] :
                    "~/Content/Uploads/";
            }
        }

        public static string[] _allowedFileTypes
        {
            get
            {
                return
                    ConfigurationManager.AppSettings["netAdvImage_AllowedFileTypes"] != null ?
                    ConfigurationManager.AppSettings["netAdvImage_AllowedFileTypes"].Replace(" ", String.Empty).Split(',') :
                    new string[] { ".gif", ".jpg", ".jpeg", ".png", ".bmp" };
            }
        }
    }

    #endregion
}