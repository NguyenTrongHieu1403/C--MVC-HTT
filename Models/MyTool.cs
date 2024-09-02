namespace H2TSHOP2024.Models;


public class MyTool
{
    public static string UploadImageToFolder(IFormFile myfile, string folder)
    {
        try
        {
            // Xác định đường dẫn đến thư mục lưu ảnh
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", folder);

            // Tạo thư mục nếu chưa tồn tại
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            // Tạo tên file duy nhất
            var fileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(myfile.FileName);

            // Kết hợp đường dẫn thư mục và tên file
            var filePath = Path.Combine(folderPath, fileName);

            // Lưu file vào thư mục
            using (var newFile = new FileStream(filePath, FileMode.Create))
            {
                myfile.CopyTo(newFile);
            }

            // Trả về tên file đã được lưu
            return fileName;
        }
        catch (Exception ex)
        {
            return string.Empty;
        }
    }
    public static string GetUniqueFileName(string fileName, string folderName)
    {
        string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images", folderName);
        string uniqueFileName = fileName;

        // Kiểm tra xem file đã tồn tại chưa
        int count = 1;
        while (System.IO.File.Exists(Path.Combine(folderPath, uniqueFileName)))
        {
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            string extension = Path.GetExtension(fileName);
            uniqueFileName = $"{fileNameWithoutExtension}_{count}{extension}";
            count++;
        }

        return uniqueFileName;
    }

}