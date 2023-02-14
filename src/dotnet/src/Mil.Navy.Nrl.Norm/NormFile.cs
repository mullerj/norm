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
                var buffer = new char[NormApi.FILENAME_MAX];
                if (!NormApi.NormFileGetName(_handle, buffer, NormApi.FILENAME_MAX))
                {
                    throw new IOException("Failed to get file name");
                }
                buffer = buffer.Where(c => c != 0).ToArray();
                return new string(buffer);
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
