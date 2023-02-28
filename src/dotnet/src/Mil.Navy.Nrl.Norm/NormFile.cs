namespace Mil.Navy.Nrl.Norm
{
    public class NormFile : NormObject
    {
        /// <summary>
        /// Constructor of NormFile
        /// </summary>
        /// <param name="handle">type is used to reference state kept for data transport objects being actively transmitted or received.</param>
        /// <returns>Returns instances of NormFile.</returns>
        internal NormFile(long handle) : base(handle)
        {

        }

        /// <summary>
        /// This function copies the name, as a NULL-terminated string, of the file object specified by the objectHandle
        /// parameter into the nameBuffer of length bufferLen bytes provided by the application.
        /// </summary>
        /// <exception cref="IOException">Thrown when failed to get file name.</exception>
        public string Name
        {
            get
            {
                var buffer = new char[FILENAME_MAX];
                if (!NormFileGetName(_handle, buffer, FILENAME_MAX))
                {
                    throw new IOException("Failed to get file name");
                }
                buffer = buffer.Where(c => c != 0).ToArray();
                return new string(buffer);
            }
        }

        /// <summary>
        /// This function renames the file used to store content for the NORM_OBJECT_FILE transport object specified by the objectHandle parameter.This allows receiver applications to rename (or move) received files as needed.NORM
        /// uses temporary file names for received files until the application explicitly renames the file.
        /// </summary>
        /// <exception cref="IOException">Thrown when failed to rename file.</exception>
        public void Rename(string filePath)
        {
            if(!NormFileRename(_handle, filePath))
            {
                throw new IOException("Failed to rename file");
            }
        }
    }
}
