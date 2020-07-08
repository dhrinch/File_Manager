using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace File_Mgr
{
    class CScreenBufer
    {
        int M, N;
        CScreenCharacter[][] Matr;
        public CScreenBufer(int m, int n)
        {
            M = m;
            N = n;
            Matr = new CScreenCharacter[M][];
            for (int i = 0; i < M; i++)
            {
                Matr[i] = new CScreenCharacter[N];
            }

            for (int i = 0; i < M; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    Matr[i][j] = new CScreenCharacter(
                        ' ',
                        ConsoleColor.Black,
                        ConsoleColor.Black);
                }
            }
        }

        public CScreenCharacter this[int row, int col]
        {
            get { return Matr[row][col]; }
        }

        public char getSymbolAt(int row, int col)
        {
            return Matr[row][col].symbol;
        }

        public ConsoleColor getBackColorAt(int row, int col)
        {
            return Matr[row][col].backColor;
        }

        public ConsoleColor getForeColorAt(int row, int col)
        {
            return Matr[row][col].foreColor;
        }

        public void ShowLine(int left, int top, string line)
        {
            ConsoleColor backColor = Console.BackgroundColor;
            ConsoleColor foreColor = Console.ForegroundColor;
            int len = line.Length;
            for (int i = 0; i < len; i++)
            {
                try
                {
                    Matr[top][left + i] = new CScreenCharacter(line[i], foreColor, backColor);
                }
                catch
                {
                    Console.WriteLine(line);
                }
            }
        }

        public void ShowChar(int left, int top, char ch)
        {
            ConsoleColor backColor = Console.BackgroundColor;
            ConsoleColor foreColor = Console.ForegroundColor;

            //Matr[top][left].symbol = ch;
            //Matr[top][left].foreColor = foreColor;
            //Matr[top][left].backColor = backColor;
            Matr[top][left] = new CScreenCharacter(ch, foreColor, backColor);
        }

        public CScreenBufer getPart(int left, int top, int width, int height)
        {
            int m = height;
            int n = width;
            CScreenBufer screenBufer = new CScreenBufer(m, n);
            for (int i = 0; i < m; i++)
            {
                for (int j=0; j<n; j++)
                {
                    screenBufer.Matr[i][j] = (CScreenCharacter)Matr[top + i][left + j].Clone();
                    //screenBufer.Matr[i][j].symbol = Matr[top + i][left + j].symbol;
                    //screenBufer.Matr[i][j].backColor = Matr[top + i][left + j].backColor;
                    //screenBufer.Matr[i][j].foreColor = Matr[top + i][left + j].foreColor;
                }
            }

            return screenBufer;
        }

        public void setPart(int left, int top, CScreenBufer part)
        {
            int m = part.M;
            int n = part.N;
            for (int i=0; i<m; i++)
            {
                for (int j=0; j<n; j++)
                {
                    Matr[top + i][left + j] = (CScreenCharacter)part.Matr[i][j].Clone();
                }
            }

            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    CScreenCharacter curSymbol = (CScreenCharacter)part.Matr[i][j].Clone();
                    Console.BackgroundColor = curSymbol.backColor;
                    Console.ForegroundColor = curSymbol.foreColor;

                    Console.SetCursorPosition(left + j, top + i);
                    Console.Write(curSymbol.symbol);
                }
            }
        }
    }

    class CScreenCharacter: ICloneable
    {
        public char symbol;
        public ConsoleColor foreColor;
        public ConsoleColor backColor;

        public CScreenCharacter(char symbol, ConsoleColor foreColor, ConsoleColor backColor)
        {
            this.symbol = symbol;
            this.foreColor = foreColor;
            this.backColor = backColor;
        }

        public object Clone()
        {
            return new CScreenCharacter(symbol, foreColor, backColor);
        }
    }
}
