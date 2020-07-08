using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace File_Mgr
{
    class CPanel
    {
        public bool active { set; get; }
        int left { set; get; }
        int top { set; get; }
        int width { set; get; }
        int height { set; get; }

        ConsoleColor BackgroundColor = ConsoleColor.DarkBlue;
        ConsoleColor ForegroundColor = ConsoleColor.Gray;

        int numberVisibleLines;
        int selectedIdx = 0;
        int startIdx = 0;
        string path;
        List<CDirItem> dirItems;

        public CPanel(int left, int top, int width, int height, bool active)
        {
            this.active = active;
            this.left = left;
            this.top = top;
            this.width = width;
            this.height = height;
            numberVisibleLines = height - 8;
            path = @"C:\";
            RefreshListOfItems();
            Show();
        }

        public void selectionDown()
        {
            if (selectedIdx < dirItems.Count - 1)
            {
                selectedIdx++;
                if (selectedIdx-startIdx > numberVisibleLines-1)
                {
                    startIdx++;
                }
                Show();
            }
        }

        public void selectionUp()
        {
            if (selectedIdx > 0)
            {
                selectedIdx--;
                if (selectedIdx < startIdx)
                {
                    //startIdx--;
                    startIdx = selectedIdx;
                }
                Show();
            }
        }

        public void Show()
        {
            Console.CursorVisible = false;
            ShowFrame();
            ShowPath();
            ShowDirItems();
            ShowCurrentItemInfo();
            Console.CursorVisible = true;
        }

        private void ShowFrame()
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

            Console.BackgroundColor = BackgroundColor;
            Console.ForegroundColor = ForegroundColor;

            string top_line = left_top + new string(dbl_horiz, width - 2) + right_top;
            CCommon.ShowLine(left, top, top_line);

            for (int i = 0; i < height - 4; i++)
            {
                CCommon.ShowChar(left, top + i + 1, dbl_vert);
                CCommon.ShowChar(left + width - 1, top + i + 1, dbl_vert);
            }

            string bottom_line = left_bottom + new string(dbl_horiz, width - 2) + right_bottom;
            CCommon.ShowLine(left, height - 3, bottom_line);

            string mid_line = left_conj + new string(single_horiz, width - 2) + right_conj;
            CCommon.ShowLine(left, top + 2, mid_line);
            CCommon.ShowLine(left, height - 5, mid_line);
        }

        private void ShowPath()
        {
            string path_tmp = path;
            while (path_tmp.Length < width - 2) path_tmp += " ";
            CCommon.ShowLine(left + 1, top + 1, path_tmp);
        }

        private void ShowCurrentItemInfo()
        {
            CDirItem curItem = getCurrentItem();

            string info = curItem.getSizeInfo();
            info += " " + curItem.creation;// + " " + curItem.modification;

            info = info.PadRight(width - 2, ' ');
            CCommon.ShowLine(left + 1, height-4, info);
        }

        private void ShowDirItems()
        {
            int limit = Math.Min(dirItems.Count, numberVisibleLines);

            for (int i = 0; i < limit; i++)
            {
                CDirItem item = dirItems[startIdx+i];
                string fileName = item.shortName;
                //while (fileName.Length < width - 2) fileName += " ";
                fileName = fileName.PadRight(width - 2, ' ');
                if (fileName.Length> width - 2)
                {
                    fileName = fileName.Substring(0, width - 5) + "...";
                }

                ConsoleColor tmpColor = Console.ForegroundColor;
                Console.ForegroundColor = item.getForeColor();


                ConsoleColor backColor = ConsoleColor.Gray;//<--fiction
                if (startIdx + i == selectedIdx && active)
                {
                    backColor = CCommon.SetBackColor(ConsoleColor.Gray);
                }
                CCommon.ShowLine(left + 1, top + i + 3, fileName);
                if (startIdx + i == selectedIdx && active)
                {
                    CCommon.SetBackColor(backColor);
                }

                Console.ForegroundColor = tmpColor;
            }

            string spaces = "";
            while (spaces.Length < width - 2) spaces += " ";
            for (int i = limit; i < numberVisibleLines; i++)
            {
                CCommon.ShowLine(left + 1, top + i + 3, spaces);
            }
        }
        
        public void RefreshListOfItems()
        { 
            dirItems = new List<CDirItem>();

            string[] dirEntries = { };
            try
            {
                dirEntries = Directory.GetFileSystemEntries(path);
                //dirItems = dirEntries.ToList();

                foreach(string fullName in dirEntries){
                    dirItems.Add(CDirItem.createDirItem(fullName));
                }

                if (!CCommon.IsPathARootDirectory(path))
                {
                   // dirItems.Insert(0, "..");
                    dirItems.Insert(0, CDirItem.createDirItem(".."));
                }
            }
            catch
            {
                CLocalMenu localMenu = new CLocalMenu(left + 1, top + 1, 20, 4, true, true, 0,"Не могу отобразить", "   содержимое");
                localMenu.workAndSelect(Program.ScreenBufer);
                dirItems.Insert(0, CDirItem.createDirItem(".."));
            }

            selectedIdx = 0;
            startIdx = 0;
        }

        public void DoEnter()
        {
            string selectedFullPath = dirItems[selectedIdx].fullName;

            //Console.WriteLine(selectedPath);

            if (selectedFullPath == "..")
            {
                path = CCommon.DeleteLastPathSegment(path);
                RefreshListOfItems();
                Show();
                return;
            }

            if (CCommon.IsPathADirectory(selectedFullPath))
            {
                path = selectedFullPath;
                if (path[path.Length - 1] != '\\') path += "\\";
                RefreshListOfItems();
                Show();
                return;
            }

            if (CCommon.IsPathAFile(selectedFullPath))
            {
                Process.Start(selectedFullPath);
                //Process.Start("dir");
            }

        }

        public void SelectDrive()
        {
            //string[] drivesNames = { "C:\\", "E:\\", "G:\\"};
            string[] drivesNames = DriveInfo.GetDrives().Select(di=>di.Name).ToArray();
            CLocalMenu localMenu = new CLocalMenu(left+1, top+3, 15, 6, true, true, 0, drivesNames);
            int choice = localMenu.workAndSelect(Program.ScreenBufer);
            if (choice != -1)
            {
                path = drivesNames[choice];
                RefreshListOfItems();
                Show();
            }
        }

        public CDirItem getCurrentItem()
        {
            return dirItems[selectedIdx];
        }

        public string getPath()
        {
            return path;
        }

        public bool RemoveCurrentItem()
        {
            string[] options = { "Remove?", "Cancel" };
            CLocalMenu confirmationDialog = new CLocalMenu(left + width / 2 - 5, top + height / 2 - 3, 9, 4, true, true, 0, options);
            int answer = confirmationDialog.workAndSelect(Program.ScreenBufer);

            if (answer == 0)
            {
                string currentItem = getCurrentItem().fullName;

                if (CCommon.IsPathAFile(currentItem))
                {
                    try
                    {
                        File.Delete(currentItem);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        Program.ShowMessage(new[] { ex.Message });
                    }
                }

                if (CCommon.IsPathADirectory(currentItem) & currentItem != "..")
                {
                    try
                    {
                        Directory.Delete(currentItem, true);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        Program.ShowMessage(new[] { ex.Message });
                    }
                }

            }

            return false;
            //RefreshListOfItems();
            //ShowDirItems();
        }

        public void RefreshSizeForCurrentItem()
        {
            CDirItem curDirItem = getCurrentItem();
            curDirItem.getSize();
            ShowCurrentItemInfo();
        }
    }
}
