using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;
using System.Net.Sockets;

namespace Mil.Navy.Nrl.Norm
{
    /// <summary>
    /// An instance of a NORM protocol engine
    /// </summary>
    public class NormInstance
    {
        /// <summary>
        /// The _handle refers to the NORM protocol engine instance
        /// </summary>
        private long _handle;

        public NormInstance(bool priorityBoost)
        {
            CreateInstance(priorityBoost);
        }

        /// <summary>
        /// Default constructor for NormInstance
        /// </summary>
        public NormInstance() : this(false)
        {
        }

        /// <summary>
        /// This function creates an instance of a NORM protocol engine and is the necessary first step before any other API functions may be used.
        /// </summary>
        /// <param name="priorityBoost">The priorityBoost parameter, when set to a value of true, specifies that the NORM protocol engine thread be run with higher priority scheduling.</param>
        private void CreateInstance(bool priorityBoost)
        {
            var handle = NormCreateInstance(priorityBoost);
            _handle = handle;
        }

        /// <summary>
        /// The NormDestroyInstance() function immediately shuts down and destroys the NORM protocol engine instance referred to by the instanceHandle parameter.
        /// </summary>
        public void DestroyInstance()
        {
            NormDestroyInstance(_handle);
        }

        public NormSession CreateSession(string address, int port, long localNodeId)
        {
            var session = NormCreateSession(_handle, address, port, localNodeId);
            if (session == NORM_SESSION_INVALID)
            {
                throw new IOException("Failed to create session");
            }
            return new NormSession(session);
        }

        public bool HasNextEvent(int sec, int usec)
        {
            var totalMilliseconds = sec * 1000 + usec / 1000;
            var waitTime = TimeSpan.FromMilliseconds(totalMilliseconds);
            return HasNextEvent(waitTime);
        }

        public bool HasNextEvent(TimeSpan waitTime)
        {
            var normDescriptor = NormGetDescriptor(_handle);
            if (normDescriptor == NORM_DESCRIPTOR_INVALID)
            {
                return false;
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) 
            {
                using var eventWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
                eventWaitHandle.SafeWaitHandle = new SafeWaitHandle(new IntPtr(normDescriptor), false);
                return eventWaitHandle.WaitOne(waitTime);
            }
            var hasNextEvent = false;
            var timeout = DateTime.Now.Add(waitTime);
            while (!hasNextEvent && DateTime.Now <= timeout)
            {
                using var socketHandle = new SafeSocketHandle(new IntPtr(normDescriptor), false);
                using var socket = new Socket(socketHandle);
                hasNextEvent = socket.Available > 0;
            }  
            return hasNextEvent;
        }

        public NormEvent? GetNextEvent(bool waitForEvent)
        {
            bool success = NormGetNextEvent(_handle, out NormApi.NormEvent normEvent, waitForEvent);
            if (!success)
            {
                return null;
            }
            return new NormEvent(normEvent.Type, normEvent.Session, normEvent.Sender, normEvent.Object);
        }

        public NormEvent? GetNextEvent()
        {
            return GetNextEvent(true);
        }

        public void SetCacheDirectory(string cachePath)
        {
            if(!NormSetCacheDirectory(_handle, cachePath))
            {
                throw new IOException("Failed to set the cache directory");
            }
        }

        public void StopInstance()
        {
            NormStopInstance(_handle);
        }

        public bool RestartInstance()
        {
            return NormRestartInstance(_handle);
        }

        public bool SuspendInstance()
        {
            return NormSuspendInstance(_handle);
        }

        public void ResumeInstance()
        {
            NormResumeInstance(_handle);
        }

        public void OpenDebugLog(string fileName)
        {
            if (!NormOpenDebugLog(_handle, fileName))
            {
                throw new IOException("Failed to open debug log");
            }
        }

        public void CloseDebugLog()
        {
            NormCloseDebugLog(_handle);
        }

        public void OpenDebugPipe(string pipename)
        {
            if (!NormOpenDebugPipe(_handle, pipename))
            {
                throw new IOException("Failed to open debug pipe");
            }
        }

        public int DebugLevel 
        { 
            get => NormGetDebugLevel(); 
            set => NormSetDebugLevel(value); 
        }
    }
}
