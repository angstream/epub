using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.IsolatedStorage;
using System.IO;
using System.Diagnostics;

namespace EpubEditor.Model
{
    public class Storage
    {
        const string fileName = "folder.txt";

        public static bool StorageExists()
        {
           IsolatedStorageFile isoStore = 
                            IsolatedStorageFile.GetUserStoreForAssembly();
           
           string [] isofileNames= isoStore.GetFileNames(fileName);

           return (isofileNames.Count() > 0);           
        }

        public static void CreateFile()
        {
            
            IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForAssembly();
            // Create the isolated storage file in the assembly we just grabbed
            IsolatedStorageFileStream isoFile = new
                    IsolatedStorageFileStream(fileName, FileMode.Create, isoStore);            

        }
        
        

        private static string[] SplitByComma(string str)
        {
            Char[] separ = new Char[] { ',' };
            return str.Split(separ);
        }

    

        public static string GetLastFolder()
        {
            string contents = ReadFile();
            string[] arr= contents.Split(new string[] { System.Environment.NewLine },
                StringSplitOptions.RemoveEmptyEntries);

            if (arr.Length > 0)
                return arr[arr.Length - 1];
            else
                return String.Empty;
        }

        public static void AddData(string data)
        {            
            IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForAssembly();

            string contents = ReadContents(isoStore);

            // Create or open the isolated storage file in the assembly we just grabbed
            IsolatedStorageFileStream isoFile = new
                    IsolatedStorageFileStream(fileName, FileMode.OpenOrCreate, isoStore);

            StreamWriter sw = new StreamWriter(isoFile);
            if (string.IsNullOrEmpty(contents))
            {
                sw.WriteLine(data);
            }
            else if (!contents.Contains(data))
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(contents);
                sb.Append(data);
                sb.Append(Environment.NewLine);
                sw.Write(sb.ToString());
            }
            else
            {
                contents = contents.Replace(data, "");
              
                StringBuilder sb = new StringBuilder();
                sb.Append(contents);
                sb.Append(data);
                sb.Append(Environment.NewLine);
                sw.Write(sb.ToString());
            }
                        
            //// Close the file
            sw.Close();
        }

        private static string ReadFile()
        {
            // Get the store isolated by the assembly
            IsolatedStorageFile isoStore =
            IsolatedStorageFile.GetUserStoreForAssembly();
            // Open the isolated storage file in the assembly we just grabbed
            IsolatedStorageFileStream isoFile = new
                    IsolatedStorageFileStream(fileName, FileMode.Open, isoStore);
            // Create a StreamReader using the isolated storage file
            StreamReader sr = new StreamReader(isoFile);
            // Read a line of text from the file
            string fileContents = sr.ReadToEnd();
            // Close the file
            sr.Close();

            return fileContents;
        }

        private static string ReadContents(IsolatedStorageFile isoStore)
        {            
            // Open the isolated storage file in the assembly we just grabbed
            IsolatedStorageFileStream isoFile = new
                        IsolatedStorageFileStream(fileName, FileMode.Open, isoStore);
            // Create a StreamReader using the isolated storage file
            StreamReader sr = new StreamReader(isoFile);
            // Read a line of text from the file
            string fileContents = sr.ReadToEnd();
            // Close the file
            sr.Close();

            return fileContents;
        }
    }
}
