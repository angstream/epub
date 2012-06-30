using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Ionic.Zip;
using System.Xml.Linq;
using System.ComponentModel;
using System.Diagnostics;

namespace EpubEditor.Model
{
    class ZipReader
    {
        private String path;

        public List<string> FileNameList { get; set; }

        public ZipReader(string filepath)
        {
            path = filepath;
            FileNameList = new List<string>();
        }

        

        public void ReadMemoryFile()
        {
            var ms = new MemoryStream();
            using (ZipFile zip = ZipFile.Read(path))
            {
                FileNameList = zip.EntryFileNames.Where(a => !a.EndsWith("/")).ToList();                          
            }              
        }

       

        private static string ReadMemoryStream(MemoryStream ms)
        {
            string Content = String.Empty;

            if (ms.Position > -1)
            {
                ms.Position = 0;

                using (var sr = new StreamReader(ms))
                {
                    Content = sr.ReadToEnd();
                }
            }
            return Content;
        }

        public string ReadZipEntry(string zipFileName)
        {
            using (ZipFile zip = ZipFile.Read(path))
            {
                var zipEntry = zip[zipFileName];
                var ms = ExtractZipEntry(zipEntry);
                return ReadMemoryStream(ms);
            }
        }
      
        
        private static MemoryStream ExtractZipEntry(ZipEntry entry)
        {
            var ms = new MemoryStream();
            entry.Extract(ms);
            return ms;
        }
      

        public static void UpdateZip(string filepath, EpubFile epub)
        {
            try
            {
                using (ZipFile zip = ZipFile.Read(filepath))
                {               
                    ZipEntry updatedZip = zip.UpdateEntry(epub.Name, epub.Content);
                    zip.Save();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
