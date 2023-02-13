using System.Xml.Linq;

namespace Mil.Navy.Nrl.Norm
{
    public class NormFile : NormObject
    {
        public string name
        {
            get
            {
                NormApi.NormFileGetName(_handle, name, Int32.MaxValue);
                return name;
            }
        }

        internal NormFile(long handle) : base(handle) 
        {
            
        }

        public void Rename(string filePath)
        {
            if(!NormApi.NormFileRename(_handle, filePath))
            {
                throw new IOException("Failed to rename file");
            }
        }
    }
}
