namespace Mil.Navy.Nrl.Norm
{
    public class NormFile : NormObject
    {
        internal NormFile(long handle) : base(handle)
        {

        }

        public string Name
        {
            get
            {
                if (!NormApi.NormFileGetName(_handle, out string name, NormApi.FILENAME_MAX))
                {
                    throw new IOException("Failed to get file name");
                }
                return name;
            }
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
