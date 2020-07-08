using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace File_Mgr
{
    class CLocalMenu
    {
        private readonly int left;
        private readonly int top;
        private int width;
        private int height;
        private readonly bool needFrame;
        private List<string> items;

        int selectedIdx = 0;
        int startIdx = 0;
        int firstItemIdx = 0;
        int clientHeight;
        int clientWidth;
        int maxLineWidth;

        public CLocalMenu(int left, int top, int width, int height, 
            bool needFrame, bool needFitFrameSize, int firstItemIdx, params string[] items)
        {
            this.left = left;
            this.top = top;
            this.width = width;
            this.height = height;
            this.needFrame = needFrame;
            this.items = items.ToList();
            clientHeight = height - 2;
            clientWidth = width - 2;
            this.firstItemIdx = firstItemIdx;
            selectedIdx = firstItemIdx;

            maxLineWidth = items.Select(s => s.Length).Max();
            
            if (needFitFrameSize)
            {
                if (clientHeight > items.Length)
                {
                    clientHeight = items.Length;
                    this.height = clientHeight + 2;
                }

                if (clientWidth > maxLineWidth)
                {
                    clientWidth = maxLineWidth;
                    this.width = clientWidth + 2;
                }
            }

            //Fit width all lines to clientWidth
            for (int i = 0; i < this.items.Count; i++)
            {
                if (this.items[i].Length < clientWidth)
                {
                    this.items[i] = this.items[i].PadRight(clientWidth, ' ');
                }
                if (this.items[i].Length > clientWidth)
                {
                    this.items[i] = this.items[i].Substring(0, clientWidth);
                }
            }
        }

        private void ShowItemsList()
        {
            for (int i = 0; i < clientHeight; i++)
            {
                Console.BackgroundColor = ConsoleColor.DarkBlue;
                Console.ForegroundColor = ConsoleColor.Gray;
                if (startIdx + i == selectedIdx)
                {
                    Console.BackgroundColor = ConsoleColor.Gray;
                    Console.ForegroundColor = ConsoleColor.Black;
                }

                Console.SetCursorPosition(left + 1, top + 1 + i);
                Console.Write(items[startIdx + i]);
            }
            Console.SetCursorPosition(left + 1, top + 1);
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        public int workAndSelect(CScreenBufer ScreenBufer)
        {
            CScreenBufer store = ScreenBufer.getPart(left, top, width, height);

            Console.CursorVisible = false;
            if (needFrame)
            {
                CCommon.ShowFrame(left, top, width, height);
            }

            ShowItemsList();

            while (true)
            {
                ConsoleKeyInfo consoleKeyInfo = Console.ReadKey();
                ConsoleKey consoleKey = consoleKeyInfo.Key;
                //ConsoleModifiers consoleModifiers = consoleKeyInfo.Modifiers;

                if (consoleKey == ConsoleKey.Escape) break;

                if (consoleKey == ConsoleKey.UpArrow)
                {
                    if (selectedIdx > firstItemIdx)
                    {
                        selectedIdx--;
                        if (selectedIdx < startIdx)
                        {
                            startIdx--;
                        }
                    }
                    ShowItemsList();
                }

                if (consoleKey == ConsoleKey.DownArrow)
                {
                    if (selectedIdx < items.Count - 1)
                    {
                        selectedIdx++;
                        if (selectedIdx - startIdx > clientHeight-1)
                        {
                            startIdx++;
                        }
                    }
                    ShowItemsList();
                }

                if (consoleKey == ConsoleKey.Enter)
                {
                    Console.CursorVisible = true;
                    ScreenBufer.setPart(left, top, store);
                    return selectedIdx;
                }
            }

            Console.CursorVisible = true;
            ScreenBufer.setPart(left, top, store);
            return -1;
        }
    }
}
