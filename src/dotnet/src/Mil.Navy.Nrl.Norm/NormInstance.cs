using Microsoft.Win32.SafeHandles;
using System.Diagnostics.CodeAnalysis;
using System.IO;
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

        /// <summary>
        /// Default constructor for NormInstance
        /// </summary>
        public NormInstance()
        {
            CreateInstance(false);
        }

        /// <summary>
        /// This function creates an instance of a NORM protocol engine and is the necessary first step before any other API functions may be used.
        /// </summary>
        /// <param name="priorityBoost">The priorityBoost parameter, when set to a value of true, specifies that the NORM protocol engine thread be run with higher priority scheduling.</param>
        private void CreateInstance(bool priorityBoost)
        {
            var handle = NormApi.NormCreateInstance(priorityBoost);
            _handle = handle;
        }

        /// <summary>
        /// The NormDestroyInstance() function immediately shuts down and destroys the NORM protocol engine instance referred to by the instanceHandle parameter.
        /// </summary>
        public void DestroyInstance()
        {
            NormApi.NormDestroyInstance(_handle);
        }

        public NormSession CreateSession(string address, int port, long localNodeId)
        {
            var session = NormApi.NormCreateSession(_handle, address, port, localNodeId);
            if (session == NormApi.NORM_SESSION_INVALID)
            {
                throw new IOException("Failed to create session");
            }
            return new NormSession(session);
        }

        public bool HasNextEvent(TimeSpan waitTime)
        {
            var normDescriptor = NormApi.NormGetDescriptor(_handle);
            if (normDescriptor == NormApi.NORM_DESCRIPTOR_INVALID)
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
                using var socket =new Socket(socketHandle);
                hasNextEvent = socket.Available > 0;
               
            }  
            return hasNextEvent;
        }

        public NormEvent? GetNextEvent(bool waitForEvent)
        {
            bool success = NormApi.NormGetNextEvent(_handle, out NormApi.NormEvent normEvent, waitForEvent);
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
    }
}
