namespace Mil.Navy.Nrl.Norm
{
    public class NormFile : NormObject
    {
        internal NormFile(long handle) : base(handle)
        {

        }

        /// <summary>
        /// This function copies the name, as a NULL-terminated string, of the file object specified by the objectHandle
        /// parameter into the nameBuffer of length bufferLen bytes provided by the application.
        /// </summary>
        /// <returns>
        /// This function returns true upon success and false upon failure. Possible failure conditions include the objectHandle
        /// does not refer to an object of type NORM_OBJECT_FILE.
        /// </returns>
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
        /// uses temporary file names for received files until the application explicitly renames the file.For example, sender
        /// applications may choose to use the NORM_INFO content associated with a file object to provide name and/or typing
        /// information to receivers.
        /// </summary>
        public void Rename(string filePath)
        {
            if(!NormFileRename(_handle, filePath))
            {
                throw new IOException("Failed to rename file");
            }
        }
    }
}
