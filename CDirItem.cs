using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace File_Mgr
{
    public abstract class CDirItem
    {
        protected CDirItem(string fullName)
        {
            this.fullName = fullName;
            getInfo();
        }

        public string fullName { private set; get; }
        public string shortName { private set; get; }
        public string ext { private set; get; }

        protected long size;
        virtual public long getSize()
        {
            return size;
        }
        virtual public string getSizeInfo()
        {
            return size.ToString();
        }

        public DateTime creation { private set;  get; }
        public DateTime modification { private set;  get; }

        private void getInfo() {
            shortName = Path.GetFileName(fullName);
            ext = Path.GetExtension(fullName);
            creation = File.GetCreationTime(fullName);
            modification = File.GetLastWriteTime(fullName);
        }

        protected abstract void ObtainSize();

        static public CDirItem createDirItem(string fullName)
        {
            if (fullName == "..") {
                return new CFolderDirItem(fullName);
            }

            if (CCommon.IsPathAFile(fullName))
            {
                return new CFileDirItem(fullName);
            }

            if (CCommon.IsPathADirectory(fullName))
            {
                return new CFolderDirItem(fullName);
            }

            throw new Exception($"CDirItem.createDirItem({fullName})");
        }

        public abstract ConsoleColor getForeColor();
    }

    public class CFileDirItem : CDirItem
    {
        public CFileDirItem(string fullName): base(fullName)
        {
            ObtainSize();
        }

        public override ConsoleColor getForeColor()
        {
            return ConsoleColor.Cyan; 
        }

        protected override void ObtainSize()
        {
            FileInfo fileInfo = new FileInfo(fullName);
            size = fileInfo.Length;
        }
    }

    public class CFolderDirItem : CDirItem
    {
        public CFolderDirItem(string fullName) : base(fullName)
        {
        }

        public override long getSize()
        {
            if (size == 0)
            {
                ObtainSize();
            }
            return size;
        }

        protected override void ObtainSize()
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(fullName);
            size = CCommon.DirSize(directoryInfo);
        }

        public override string getSizeInfo()
        {
            if (size == 0) return "dir";
            return size.ToString();
        }

        public override ConsoleColor getForeColor()
        {
            return ConsoleColor.White;
        }
    }
}
