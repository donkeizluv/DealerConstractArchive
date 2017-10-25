using System;
using System.Linq;

namespace DealerContractArchive.Helper
{
    public static class EnviromentHelper
    {
        public static string ScanFolder = "UploadedScans";
        public static string DocumentFolder = "PrintDocument";
        public static string RootPath { get; set; }
        public static string ScanFilePathMaker(string fileName, int dealerId)
        {
            return $"{dealerId}_{GetNowSalt()}_{fileName}";
        }
        public static string GetScanfileFullPath(string fileName)
        {
            return $"{RootPath}\\{ScanFolder}\\{fileName}";
        }
        public static string GetDocumentFullPath(string fileName)
        {
            return $"{RootPath}\\{DocumentFolder}\\{fileName}";
        }
        public static string GetNowSalt()
        {
            return DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss");
        }
    }
}
