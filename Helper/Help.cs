using System;
using System.Text.RegularExpressions;

namespace ProjectThoiTrang.Helper
{
    public static class Help
    {
        public static int page_size = 6;
        public static void CreateIfMiss(string path)
        {
            bool folderEx = Directory.Exists(path);
            if(!folderEx) {
            Directory.CreateDirectory(path);}
        }
        public static async Task<string> UploadFile(Microsoft.AspNetCore.Http.IFormFile file,string sDirec, string newname)
        {
            try
            {
                if(newname == null) { newname=file.FileName; }
                string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot","images",sDirec);
                CreateIfMiss(path);
                string pathFile = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", sDirec, newname);
                var supportedTypes = new[] { "jpg", "jpeg", "png", "gif" };
                var fileEx = System.IO.Path.GetExtension(file.FileName).Substring(1);
                if(!supportedTypes.Contains(fileEx.ToLower())) {
                    return null;
                }
                else
                {
                    using( var stream = new FileStream(pathFile, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                    return newname;
                }
            }catch (Exception ex)
            {
                return null;
            }
        }
        public static string SUrl(string url)
        {
            url = url.ToLower();
            url = Regex.Replace(url, @"[áàaaaaaaaaaaaaaaa]", "a");
            url = Regex.Replace(url, @"[éèęéẽêêêêêê]", "e");
            url = Regex.Replace(url, @"[ὁὁφὀδοοοοοοσάσσởỡ]", "ο");
            url = Regex.Replace(url, @"[íìiii]", "i");
            url = Regex.Replace(url, @"[ýyyiỹ]", "y");
            url = Regex.Replace(url, @"[úùyủũưứvựửữ]", "u");
            url = Regex.Replace(url, @"[đ]", "d");
            // Chỉ cho phép nhận: [0-9a-z-\s]
            url = Regex.Replace(url.Trim(), @"[^0-9a-z-\s]", "").Trim();
            // Xử lý nhiều hơn 1 khoảng trắng --> 1 kt
            url = Regex.Replace(url.Trim(), @"\s+", "-");
            // Thay khoảng trắng bằng -
            url = Regex.Replace(url, @"\s", "-");
            // Loại bỏ các trường hợp nhiều dấu - liên tiếp
            url = Regex.Replace(url, @"-+", "-");
            return url;
        }
        private static readonly Random random = new Random();
        public static int GenerateID()
        {
            // Tạo một số nguyên ngẫu nhiên làm ID
            return random.Next();
        }
    }
}
