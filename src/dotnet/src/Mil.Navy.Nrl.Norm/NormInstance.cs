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

         /// <summary>
        /// This function creates a NORM protocol session (NormSession) using the address (multicast or unicast) and port
        /// parameters provided.While session state is allocated and initialized, active session participation does not begin
        /// until a call is made to NormStartSender() and/or NormStartReceiver() to join the specified multicast group
        /// (if applicable) and start protocol operation.
        /// </summary>
        /// <param name="instanceHandle">Valid NormInstanceHandle previously obtained with a call to NormCreateInstance() </param>
        /// <param name="sessionAddress">Specified address determines the destination of NORM messages sent </param>
        /// <param name="sessionPort">Valid, unused port number corresponding to the desired NORM session address. </param>
        /// <param name="localNodeId">Identifies the application's presence in the NormSession </param>
        /// <returns></return
        public NormSession CreateSession(string address, int port, long localNodeId)
        {
            var session = NormCreateSession(_handle, address, port, localNodeId);
            if (session == NORM_SESSION_INVALID)
            {
                throw new IOException("Failed to create session");
            }
            return new NormSession(session);
        }

        ///
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

         /// <summary>
        /// This function retrieves the next available NORM protocol event from the protocol engine.
        /// </summary>
        /// <param name="instanceHandle"The instanceHandle parameter specifies the applicable NORM protocol engine. </param>
        /// <param name="theEvent"> the theEvent parameter must be a valid pointer to a NormEvent structure capable of receiving the NORM event information. </param>
        /// <param name="waitForEvent">waitForEvent specifies whether the call to this function is blocking or not, if "waitForEvent" is false, this is a non-blocking call. </param>
        /// <returns>The function returns true when a NormEvent is successfully retrieved, and false otherwise. Note that a return value of false does not indicate an error or signify end of NORM operation.</returns>
        public NormEvent? GetNextEvent(bool waitForEvent)
        {
            bool success = NormGetNextEvent(_handle, out NormApi.NormEvent normEvent, waitForEvent);
            if (!success)
            {
                return null;
            }
            return new NormEvent(normEvent.Type, normEvent.Session, normEvent.Sender, normEvent.Object);
        }

         /// <summary>
        /// This function retrieves the next available NORM protocol event from the protocol engine.
        /// </summary>
        /// <param name="instanceHandle"The instanceHandle parameter specifies the applicable NORM protocol engine. </param>
        /// <param name="theEvent"> the theEvent parameter must be a valid pointer to a NormEvent structure capable of receiving the NORM event information. </param>
        /// <param name="waitForEvent">waitForEvent specifies whether the call to this function is blocking or not, if "waitForEvent" is false, this is a non-blocking call. </param>
        /// <returns>The function returns true when a NormEvent is successfully retrieved, and false otherwise. Note that a return value of false does not indicate an error or signify end of NORM operation.</returns>
        public NormEvent? GetNextEvent()
        {
            return GetNextEvent(true);
        }

         /// <summary>
        /// This function sets the directory path used by receivers to cache newly-received NORM_OBJECT_FILE content.
        /// </summary>
        /// <param name="instanceHandle">The instanceHandle parameter specifies the NORM protocol engine instance (all NormSessions associated with that instanceHandle share the same cache path). </param>
        /// <param name="cachePath">the cachePath is a string specifying a valid (and writable) directory path. </param>
        /// <returns>The function returns true on success and false on failure. The failure conditions are that the indicated directory does not exist or the process does not have permissions to write. </returns>
        public void SetCacheDirectory(string cachePath)
        {
            if(!NormSetCacheDirectory(_handle, cachePath))
            {
                throw new IOException("Failed to set the cache directory");
            }
        }

         /// <summary>
        /// The NormStopInstance() this function immediately stops the NORM protocol engine thread corresponding to the given instanceHandle parameter.
        /// </summary>
        /// <param name="instanceHandle">The NORM protocol engine instance referred to by the instanceHandle parameter. </param>
        public void StopInstance()
        {
            NormStopInstance(_handle);
        }

      /// <summary>
        /// The NormRestartInstance() this function creates and starts an operating system thread to resume NORM protocol engine operation for the given instanceHandle that was previously stopped by a call to NormStopInstance().
        /// </summary>
        /// <param name="instanceHandle">The NORM protocol engine instance referred to by the instanceHandle parameter. </param>
        /// <returns>Boolean as to the success of the instance restart. </return>
        public bool RestartInstance()
        {
            return NormRestartInstance(_handle);
        }
        
      /// <summary>
        /// The NormSuspendInstance() immediately suspends the NORM protocol engine thread corresponding to the given instanceHandle parameter
        /// </summary>
        /// <param name="instanceHandle">The NORM protocol engine instance referred to by the instanceHandle parameter. </param>
        /// <returns>Boolean as to the success of the instance suspension. </returns>
        public bool SuspendInstance()
        {
            return NormSuspendInstance(_handle);
        }

         /// <summary>
        /// This function allows NORM debug output to be directed to a file instead of the default STDERR.
        /// </summary>
        /// <param name="instanceHandle">Used to identify application in the NormSession. </param>
        /// <param name="path">Full path and name of the debug log. </param>
        /// <returns>The function returns true on success. If the specified file cannot be opened a value of false is returned. </returns>
        public void ResumeInstance()
        {
            NormResumeInstance(_handle);
        }

         /// <summary>
        /// This function allows NORM debug output to be directed to a file instead of the default STDERR.
        /// </summary>
        /// <param name="instanceHandle">Used to identify application in the NormSession. </param>
        /// <param name="path">Full path and name of the debug log. </param>
        /// <returns>The function returns true on success. If the specified file cannot be opened a value of false is returned. </returns>
        public void OpenDebugLog(string fileName)
        {
            if (!NormOpenDebugLog(_handle, fileName))
            {
                throw new IOException("Failed to open debug log");
            }
        }

          /// <summary>
        /// This function disables NORM debug output to be directed to a file instead of the default STDERR.
        /// </summary>
        /// <param name="instanceHandle">Used to identify application in the NormSession. </param>
        /// <returns>The function returns true on success. </return>
        public void CloseDebugLog()
        {
            NormCloseDebugLog(_handle);
        }

        /// <summary>
        /// This function allows NORM debug output to be directed to a file instead of the default STDERR.
        /// </summary>
        /// <param name="instanceHandle">Used to identify application in the NormSession. </param>
        /// <param name="path">Full path and name of the debug log. </param>
        /// <returns>The function returns true on success. If the specified file cannot be opened a value of false is returned. </returns>
        public void OpenDebugPipe(string pipename)
        {
            if (!NormOpenDebugPipe(_handle, pipename))
            {
                throw new IOException("Failed to open debug pipe");
            }
        }

          /// <summary>
        /// Returns the currently set debug level.
        /// Returns the current debug level
        /// </summary>
        /// <returns>Returns the currently set debug level. </returns>
        public int DebugLevel 
        { 
            get => NormGetDebugLevel(); 
            set => NormSetDebugLevel(value); 
        }
    }
}
