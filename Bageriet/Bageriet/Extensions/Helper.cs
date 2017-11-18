using System.IO;
using System.Web;

namespace Bageriet.Extensions
{
    public class Helper
    {
        public class TextFormat
        {
            #region CropText
            public static string CropText(string text, int maxLength, bool doDots)
            {
                return (text.Length <= maxLength ? text : text.Substring(0, maxLength) + (doDots ? "..." : ""));
            }
            #endregion
        }

        public static void DeleteFile(string filename, string folderpath = "~/Content/Images/")
        {
            var file = HttpContext.Current.Server.MapPath(folderpath) + filename;

            if (!File.Exists(file)) return;
            var filestream = new FileStream(
                Path.Combine(HttpContext.Current.Server.MapPath(folderpath), filename),
                FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            filestream.Close();
            File.Delete(file);

        }

        public static string UploadFile(HttpPostedFileBase file, string filefront, string folderpath = "~/Content/Images/")
        {
            var Serverpath = System.Web.HttpContext.Current.Server.MapPath(folderpath);
            if (file != null && file.ContentLength > 0)
            {
                if (filefront.Length > 1)
                {
                    file.SaveAs(Path.Combine(Serverpath, filefront + "_" + file.FileName));
                    return filefront + "_" + file.FileName;
                }
                else
                {
                    file.SaveAs(Path.Combine(Serverpath, file.FileName));
                    return file.FileName;
                }
            }
            return string.Empty;
        }
    }
}