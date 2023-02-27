﻿namespace Mil.Navy.Nrl.Norm
{
    /// <summary>
    /// NormStream
    /// </summary>
    public class NormStream : NormObject
    {
        /// <summary>
        /// This boolean tells whether the transmit stream, specified by the streamHandle parameter,
        /// has buffer space available so that the application may successfully make a call to NormStreamWrite().
        /// </summary>
        public bool HasVacancy
        {
            get
            {
                return NormStreamHasVacancy(_handle);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public long ReadOffset
        {
            get
            {
                return NormStreamGetReadOffset(_handle);
            }
        }

        /// <summary>
        /// Internal constructor for NormStream
        /// </summary>
        /// <param name="handle">Used to identify the stream</param>
        /// <returns>Instance of a NormStream. </returns>
        internal NormStream(long handle) : base(handle)
        {
        }

        /// <summary>
        /// This function enqueues data for transmission within the NORM stream.
        /// </summary>
        /// <param name="buffer">The buffer parameter must be a pointer to the data to be enqueued.</param>
        /// <param name="offset">The offset indicated where in the buffer to start writing the data.
        /// Note: If the data is written in its entirety, offset should be set to 0.</param>
        /// <param name="length">The length parameter indicates the length of the data content.</param>
        /// <returns></returns>
        public int Write(byte[] buffer, int offset, int length)
        {
            var bytes = buffer.Skip(offset).Take(length).ToArray();
            return NormStreamWrite(_handle, bytes, length);
        }

        /// <summary>
        /// This function allows the application to indicate to the NORM protocol engine that the last data successfully written
        /// to the stream indicated by streamHandle corresponded to the end of an application-defined message boundary.
        /// </summary>
        public void MarkEom()
        {
            NormStreamMarkEom(_handle);
        }

        /// <summary>
        /// This function causes an immediate "flush" of the transmit stream.
        /// </summary>
        /// <param name="eom">The optional eom parameter, when set to true, allows the sender application to mark an end-of-message indication
        /// (see NormStreamMarkEom()) for the stream and initiate flushing in a single function call.</param>
        /// <param name="flushMode">The default stream "flush" operation invoked via
        /// NormStreamFlush() for flushMode equal to NORM_FLUSH_PASSIVE causes NORM to immediately transmit all
        /// enqueued data for the stream(subject to session transmit rate limits), even if this results in NORM_DATA messages
        /// with  "small"  payloads.If the  optional flushMode  parameter  is  set to  NORM_FLUSH_ACTIVE,  the application  can
        /// achieve reliable delivery of stream content up to the current write position in an even more proactive fashion.In
        /// this  case, the sender  additionally, actively transmits  NORM_CMD(FLUSH) messages  after any  enqueued stream
        /// content has  been sent.  This immediately  prompt receivers  for  repair requests  which reduces  latency of  reliable
        /// delivery, but at a cost of some additional messaging. Note any such "active" flush activity will be terminated upon
        /// the next subsequent write to the stream.If flushMode is set to NORM_FLUSH_NONE, this call has no effect other than
        /// the optional end-of-message marking described here. </param>
        public void Flush(bool eom, NormFlushMode flushMode)
        {
            NormStreamFlush(_handle, eom, flushMode);
        }

        /// <summary>
        /// This function causes an immediate "flush" of the transmit stream.
        /// </summary>
        /// <note>
        /// This is a default function which calls Flush(bool eom, NormFlushMode flushMode) override
        /// with eom set as false and flushMode set to NORM_FLUSH_PASSIVE
        /// </note>
        public void Flush()
        {
            Flush(false, NormFlushMode.NORM_FLUSH_PASSIVE);
        }

        /// <summary>
        /// This function halts transfer of the stream and releases any resources used unless the associated object
        /// has been explicitly retained by a call to NormObjectRetain().
        /// </summary>
        /// <param name="graceful">The optional graceful parameter, when
        /// set to a value of true, may be used by NORM senders to initiate "graceful" shutdown of a transmit stream.</param>
        public void Close(bool graceful)
        {
            NormStreamClose(_handle, graceful);
        }

        /// <summary>
        /// This function halts transfer of the stream and releases any resources used unless the associated object
        /// has been explicitly retained by a call to NormObjectRetain().
        /// </summary>
        /// <note>
        /// This is a default function which calls Close(bool graceful) override
        /// with graceful set as false.
        /// </note>
        public void Close()
        {
            Close(false);
        }

        /// <summary>
        /// This function can be used by the receiver application to read any available data from an incoming NORM stream.
        /// </summary>
        /// <param name="buffer">The buffer parameter must be a pointer to an array where the received
        /// data can be stored of a length as referenced by the length parameter. </param>
        /// <param name="offset">Indicates where in the buffer to start reading the data. 
        /// Note: To read the data in its entirety, begin at offset 0.</param>
        /// <param name="length">Expected length of data received</param>
        /// <returns></returns>
        public int Read(byte[] buffer, int offset, int length)
        {
            var bytes = new byte[length];

            if (!NormStreamRead(_handle, bytes, ref length))
            {
                return -1;
            }

            for (var i = 0; i < length; i++)
            {
                var bufferPosition = offset + i;
                if (bufferPosition < buffer.Length)
                {
                    buffer[bufferPosition] = bytes[i];
                } 
            }

            return length;
        }

        /// <summary>
        /// This function advances the read offset of the receive stream referenced by the streamHandle parameter to align
        /// with the next available message boundary
        /// </summary>
        /// <returns>
        /// This function returns a value of true when start-of-message is found. The next call to NormStreamRead() will
        /// retrieve data aligned with the message start. If no new message boundary is found in the buffered receive data for
        /// the stream, the function returns a value of false. In this case, the application should defer repeating a call to this
        /// function until a subsequent NORM_RX_OBJECT_UPDATE notification is posted.</returns>
        public bool SeekMsgStart()
        {
            return NormStreamSeekMsgStart(_handle);
        }

        /// <summary>
        /// This function controls how the NORM API behaves when the application attempts to enqueue new stream data for transmission
        /// when the associated stream's transmit buffer is fully occupied with data pending original or repair transmission.
        /// </summary>
        /// <param name="pushEnable"> By default (pushEnable = false), a call to NormStreamWrite() will return a zero value under this
        /// condition, indicating it was unable to enqueue the new data. However, if pushEnable is set to true for a given
        /// streamHandle, the NORM protocol engine will discard the oldest buffered stream data(even if it is pending repair
        /// transmission or has never been transmitted) as needed to enqueue the new data.</param>
        public void SetPushEnable(bool pushEnable)
        {
            NormStreamSetPushEnable(_handle, pushEnable);
        }

        /// <summary>
        /// This function sets "automated flushing" for the NORM transmit stream indicated by the streamHandle parameter.
        /// </summary>
        /// <param name="flushMode">Possible values for the flushMode parameter include NORM_FLUSH_NONE, NORM_FLUSH_PASSIVE, and NORM_FLUSH_ACTIVE.</param>
        public void SetAutoFlush(NormFlushMode flushMode)
        {
            NormStreamSetAutoFlush(_handle, flushMode);
        }
    }
}
