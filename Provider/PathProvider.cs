namespace Hrms.Provider
{
    public static class PathProvider
    {
        public static string GetCurrentPath() { return Thread.GetDomain().BaseDirectory; }
        public static void CreateFile(string filePath) { if (!File.Exists(filePath)) { _ = File.Create(filePath); } }
        public static void DeleteFile(string filePath) { if (File.Exists(filePath)) { File.Delete(filePath); } }
        public static void DeleteDirectory(string folderPath) { if (Directory.Exists(folderPath)) { Directory.Delete(folderPath, true); } }
        public static void CreateDirectory(string folderPath) { if (!Directory.Exists(folderPath)) { _ = Directory.CreateDirectory(folderPath); } }
        public static void DeleteAndCreateDirectory(string folderPath) { if (!Directory.Exists(folderPath)) { _ = Directory.CreateDirectory(folderPath); } else { Directory.Delete(folderPath, true); _ = Directory.CreateDirectory(folderPath); } }
    }
}
