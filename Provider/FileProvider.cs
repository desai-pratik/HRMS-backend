namespace Hrms.Provider
{
    public class FileProvider
    {
        public static class ContentTypes
        {
            public const string Pdf = "application/pdf";
            public const string General = "application/octet-stream";

        }

        public static class FileExtensions
        {
            public const string Pdf = ".pdf";
          
        }

        public static string ContentType(string fileName)
        {
            string contentType = ContentTypes.General;
            switch (Path.GetExtension(fileName))
            {
              
                case FileExtensions.Pdf: { contentType = ContentTypes.Pdf; break; }
              
                default: { break; }
            }
            return contentType;
        }

      
        public static async Task<string> ReadFileToPath(IFormFile file, string uploadPath)
        {
            // Constructing file name with current date and time and original file name
            string fileName = $"{Guid.NewGuid()}-{DateTime.Now:dd-MM-yyyy_HH-mm-ss}{Path.GetExtension(file.FileName)}".Replace(" ", "");

            // Ensure the directory exists, or create it if it doesn't
            if (!Directory.Exists(uploadPath))
            {
                PathProvider.CreateDirectory(uploadPath);
            }

            // Write the file to the specified path with the generated filename
            if (file.Length > 0)
            {
                using (FileStream filestream = new(Path.Combine(uploadPath, file.FileName), FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    await file.CopyToAsync(filestream);
                }
            }

            return fileName; // Return the generated filename
        }

        public static void CopyTempFileToPath(string copyFileName, string copyToPath, string newFileName)
        {
            PathProvider.CreateDirectory(copyToPath);
            File.Copy(Path.Combine(ConfigProvider.Provider.BaseDirectory, ConfigProvider.Settings.TempFolderName, copyFileName), Path.Combine(copyToPath, newFileName));
        }
    }
}
