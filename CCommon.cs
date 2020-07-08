using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace File_Mgr
{
    abstract class CCommon
    {
        static public void ShowLine(int left, int top, string line)
        {
            Console.SetCursorPosition(left, top);
            Console.Write(line);
            Program.ScreenBufer.ShowLine(left, top, line);
        }

        static public void ShowChar(int left, int top, char ch)
        {
            Console.SetCursorPosition(left, top);
            Console.Write(ch);
            Program.ScreenBufer.ShowChar(left, top, ch);
        }

        static public bool IsPathAFile(string path)
        {
            return File.Exists(path);
        }

        static public bool IsPathADirectory(string path)
        {
            return Directory.Exists(path);
        }

        static public bool IsPathARootDirectory(string path)
        {
            DirectoryInfo d = new DirectoryInfo(path);
            if (d.Parent == null) return true;
            return false;
        }

        static public ConsoleColor SetBackColor(ConsoleColor newColor)
        {
            ConsoleColor oldColor = Console.BackgroundColor;
            Console.BackgroundColor = newColor;
            return oldColor;
        }

        static public string DeleteLastPathSegment(string path)
        {
            int len = path.Length;
            if (path[len - 1] == '\\') path = path.Substring(0, len - 1);
            int lastSlashPos = path.LastIndexOf("\\");
            if (lastSlashPos < path.Length-1)
            {
                path = path.Remove(lastSlashPos+1);
            }
            if (lastSlashPos > 2)
            {
                path = path.Remove(lastSlashPos);
            }
            return path;
        }

        static public void ShowFrame(int left, int top, int width, int height)
        {
            char left_top = '\u2554';
            char dbl_horiz = '\u2550';
            char right_top = '\u2557';
            char dbl_vert = '\u2551';
            char right_bottom = '\u255D';
            char left_bottom = '\u255A';
            char left_conj = '\u255F';
            char single_horiz = '\u2500';
            char right_conj = '\u2562';

            string top_line = left_top + new string(dbl_horiz, width - 2) + right_top;
            CCommon.ShowLine(left, top, top_line);

            for (int i = 0; i < height-2; i++)
            {
                CCommon.ShowChar(left, top + i + 1, dbl_vert);
                CCommon.ShowChar(left + width - 1, top + i + 1, dbl_vert);
            }

            string bottom_line = left_bottom + new string(dbl_horiz, width - 2) + right_bottom;
            CCommon.ShowLine(left, top+height-1, bottom_line);
        }

        static public void CopyDirWithAllContent(string SourcePath, string DestinationPath)
        {
            Directory.CreateDirectory(DestinationPath);

            //Now Create all of the directories
            string[] dirs = Directory.GetDirectories(SourcePath, "*", SearchOption.AllDirectories);
            foreach (string dirPath in dirs)
            {
                string path = dirPath.Replace(SourcePath, DestinationPath);
                Directory.CreateDirectory(path);
            }

            //Copy all the files & Replaces any files with the same name
            var files = Directory.GetFiles(SourcePath, "*.*", SearchOption.AllDirectories);
            foreach (string oldPath in files)
            {
                string newPath = oldPath.Replace(SourcePath, DestinationPath);
                File.Copy(oldPath, newPath, true);
            }
        }

        static public long DirSize(DirectoryInfo d)
        {
            long size = 0;
            // Add file sizes.
            FileInfo[] fis = d.GetFiles();
            foreach (FileInfo fi in fis)
            {
                size += fi.Length;
            }
            // Add subdirectory sizes.
            DirectoryInfo[] dis = d.GetDirectories();
            foreach (DirectoryInfo di in dis)
            {
                size += DirSize(di);
            }
            return size;
        }

        static public string ExecCommand(string command)
        {
            Process p = new Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.Arguments = "/c " + command;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.Start();

            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();
            return output;
        }
    }
}
