using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace File_Mgr
{
    enum Mode { ePanels, eBuffer}

    class Program
    {
        static CPanel panel1;
        static CPanel panel2;
        static CPanel activePanel;
        static CPanel passivePanel;
        const int width = 80;
        const int height = 40;
        public static CScreenBufer ScreenBufer;
        static string cmdLine = "";
        static List<string> buffer = new List<string>();
        static Mode mode = Mode.ePanels;

        static void Main(string[] args)
        {
            buffer.Clear();
            ConfigureMainWindow();
            ScreenBufer = new CScreenBufer(height, width);
            panel1 = new CPanel(0, 0, width/2, height, true);
            activePanel = panel1;
            panel2 = new CPanel(width/2, 0, width/2, height, false);
            passivePanel = panel2;

            ShowHelpLine();
            Console.SetCursorPosition(0, height - 2);

            while (true)
            {
                ShowCommandLine();
  
                ConsoleKeyInfo consoleKeyInfo = Console.ReadKey();
                ConsoleKey consoleKey = consoleKeyInfo.Key;
                ConsoleModifiers consoleModifiers = consoleKeyInfo.Modifiers;

                if (consoleKey == ConsoleKey.Escape)
                {
                    break;
                }
                else if (consoleKey == ConsoleKey.Tab)
                {
                    SwitchPanels();
                    Refresh();
                }
                else if (consoleKey == ConsoleKey.Spacebar & cmdLine.Length == 0)
                {
                    activePanel.RefreshSizeForCurrentItem();
                }
                else if (consoleKey == ConsoleKey.UpArrow)
                {
                    activePanel.selectionUp();
                }
                else if (consoleKey == ConsoleKey.DownArrow)
                {
                    activePanel.selectionDown();
                }
                else if (consoleKey == ConsoleKey.Enter)
                {
                    if (cmdLine.Length > 0)
                    {
                        string tmpPath = Directory.GetCurrentDirectory();//save
                        Directory.SetCurrentDirectory(activePanel.getPath());
                        string answer = CCommon.ExecCommand(cmdLine);
                        string[] answerLines = answer.Split(new[] { '\n' });
                        buffer.AddRange(answerLines);
                        Directory.SetCurrentDirectory(tmpPath); //restore
                        cmdLine = "";
                    }
                    else
                    {
                        activePanel.DoEnter();
                    }
                }
                else if (consoleKey == ConsoleKey.F3)
                {
                    activePanel.DoEnter();
                }
                else if (consoleKey == ConsoleKey.F4 && consoleModifiers == ConsoleModifiers.Shift)
                {
                    string path = activePanel.getPath();
                    Console.Write(" Enter name for new file: ");
                    string fileName = Console.ReadLine();

                    try
                    {
                        string fullPath = path + fileName;
                        FileStream fileStream = File.Create(fullPath);
                        fileStream.Close();
                        Refresh();

                        Process.Start(fullPath);
                    }
                    catch
                    {
                        string[] lines =
                        {
                            "   Can't create   ",
                            "and edit such file"
                        };
                        CLocalMenu msg = new CLocalMenu(width / 2 - 10, height / 2 - 2, 20, 4, true, true, 2, lines);
                        msg.workAndSelect(ScreenBufer);
                    }
                }
                else if (consoleKey == ConsoleKey.F4)
                {
                    activePanel.DoEnter();
                }
                else if (consoleKey == ConsoleKey.F1 && consoleModifiers==ConsoleModifiers.Alt)
                {
                    panel1.SelectDrive();
                }
                else if (consoleKey == ConsoleKey.F1)
                {
                    string[] helpLiles = {
                        "OurSuperFileCommander",
                        "Version 1.0",
                        "august 2019."
                    };

                    int width = helpLiles.Select(line => line.Length).Max() + 2;
                    if (width > 60) width = 60;

                    int height = helpLiles.Length + 2;
                    if (height > 30) height = 30;

                    CLocalMenu helpMenu = new CLocalMenu(10, 5, width, height, true, true, height, helpLiles);
                    helpMenu.workAndSelect(ScreenBufer);
                }
                else if (consoleKey == ConsoleKey.F2 && consoleModifiers == ConsoleModifiers.Alt)
                {
                    panel2.SelectDrive();
                }
                else if (consoleKey == ConsoleKey.F5)
                {
                    DoCopy();
                }
                else if (consoleKey == ConsoleKey.F6)
                {
                    if (DoCopy())
                    {
                        if (activePanel.RemoveCurrentItem())
                        {
                            Refresh();
                        }
                    }
                }
                else if (consoleKey == ConsoleKey.F7)
                {
                    Console.Write("Enter new folder name: ");
                    string newFolderName = Console.ReadLine();
                    try
                    {
                        string currentPath = activePanel.getPath();
                        string newFolderFullName = currentPath + newFolderName;
                        Directory.CreateDirectory(newFolderFullName);
                    }
                    catch
                    {
                        string[] message = { "Can't create new folder", "Press any key." };
                        ShowMessage(message);
                    }
                    Refresh();
                }
                else if (consoleKey == ConsoleKey.F8)
                {
                    activePanel.RemoveCurrentItem();
                    Refresh();
                }
                else if (consoleKey == ConsoleKey.Backspace)
                {
                    int len = cmdLine.Length;
                    if (len > 0)
                    {
                        cmdLine = cmdLine.Substring(0, len - 1);
                    }
                }
                else if (consoleKey == ConsoleKey.O && consoleModifiers == ConsoleModifiers.Control)
                {
                    if (mode == Mode.ePanels)
                    {
                        mode = Mode.eBuffer;
                    }
                    else
                    {
                        mode = Mode.ePanels;
                    }
                }
                else
                {
                    char c = consoleKeyInfo.KeyChar;
                    cmdLine += c;
                }

                if (mode == Mode.eBuffer) {
                    ShowBuffer();
                }
                if (mode == Mode.ePanels)
                {
                    panel1.Show();
                    panel2.Show();
                }
            }
        }

        static private void ShowBuffer()
        {
            ConsoleColor tmp = Console.BackgroundColor;
            Console.BackgroundColor = ConsoleColor.Black;

            int row = 0;
            int N = height-2;
            if (buffer.Count < N)
            {
                string lineOfSpaces = new string(' ', width);
                int n = N - buffer.Count;
                for (int i = 0; i < n; i++)
                {
                    CCommon.ShowLine(0, row++, lineOfSpaces);
                }
            }

            int rest = N - row;
            int startIdx = buffer.Count - rest;
            for (int i=0; i<rest; i++)
            {
                string line = buffer[startIdx + i];
                int len = line.Length;
                if (len > 0 && line[len - 1] == '\r')
                {
                    line = line.Substring(0, len - 1);
                }
                line = line.PadRight(width, ' ');
                CCommon.ShowLine(0, row++, line);
            }

            Console.BackgroundColor = tmp;
        }

        static private void Refresh()
        {
            panel1.RefreshListOfItems();
            panel2.RefreshListOfItems();
            panel1.Show();
            panel2.Show();
            ShowCommandLine();
            ShowHelpLine();
            Console.SetCursorPosition(0, height - 2);
        }

        static private void SwitchPanels()
        {
            if (panel1.active)
            {
                panel1.active = false;
                panel2.active = true;
                activePanel = panel2;
                passivePanel = panel1;
            }
            else
            {
                panel1.active = true;
                panel2.active = false;
                activePanel = panel1;
                passivePanel = panel2;
            }
        }

        static private void ShowCommandLine()
        {
            string lineOfSpaces = new string(' ', width);
            ConsoleColor tmp = Console.BackgroundColor;
            Console.BackgroundColor = ConsoleColor.Black;
            CCommon.ShowLine(0, height - 2, lineOfSpaces);

            string line = activePanel.getPath()+">"+cmdLine;
            CCommon.ShowLine(0, height - 2, line);
            Console.BackgroundColor = tmp;
        }

        static private void ConfigureMainWindow()
        {
            Console.WindowHeight = height;
            Console.WindowWidth = width;
            Console.BufferHeight = height;
            Console.BufferWidth = width;
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.ForegroundColor = ConsoleColor.Gray;
            
            Console.Clear();
            Console.Title = "My Super File Manager";
        }

        static private bool DoCopy()
        {
            string srcPath = activePanel.getCurrentItem().fullName;
            if (srcPath == "..") return false;
            string dstPath = passivePanel.getPath();

            if (CCommon.IsPathAFile(srcPath))
            {
                try
                {
                    CopyFile(srcPath, dstPath);
                }
                catch {
                    return false;
                }
            }

            if (CCommon.IsPathADirectory(srcPath))
            {
                string dirName = Path.GetFileName(srcPath);
                dstPath += dirName;

                srcPath += "\\";
                dstPath += "\\";
                try
                {
                    CCommon.CopyDirWithAllContent(srcPath, dstPath);
                }
                catch {
                    return false;
                }
            }

            passivePanel.RefreshListOfItems();
            passivePanel.Show();

            return true;
        }

        static private bool CopyFile(string srcPath, string dstPath)
        {
            string fileName = dstPath + Path.GetFileName(srcPath);
            if (File.Exists(fileName))
            {
                string[] answers = { "Make your choice", "Replace", "Cancel" };
                CLocalMenu menu = new CLocalMenu(width / 2 - 9, height / 2 - 3, 18, 5, true, true, 1, answers);
                int answerIdx = menu.workAndSelect(ScreenBufer);
                //activePanel.Show();
                //passivePanel.Show();
                if (answerIdx != 1) return false;
            }

            try
            {
                File.Copy(srcPath, fileName, true);
            }
            catch
            {
                string[] answers = { "Can't to copy the file", "     to this place." };
                CLocalMenu menu = new CLocalMenu(width / 2 - 12, height / 2 - 3, 24, 4, true, true, 0, answers);
                menu.workAndSelect(ScreenBufer);
                //activePanel.Show();
                //passivePanel.Show();
                return false;
            }
            //passivePanel.RefreshListOfItems();
            //passivePanel.Show();
            return true;
        }

        static private void ShowHelpLine()
        {
            string helpLine = "F1-Help  F2-UserMenu  F3-View  F4-Edit  F5-Copy  F6-Move  F7-MkDir  D8-Delete";
            CCommon.ShowLine(0, height - 1, helpLine);
        }

        static public void ShowMessage(string[] messageLines)
        {
            int maxLen = messageLines.Select(s => s.Length).Max();
            int count = messageLines.Length;
            CLocalMenu messageDlg = new CLocalMenu(width / 2 - maxLen / 2 - 1, height / 2 - count / 2 - 1, maxLen + 2, count + 2, true, true, 0, messageLines);
            messageDlg.workAndSelect(ScreenBufer);
        }
    }
}
