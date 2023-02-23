using Mil.Navy.Nrl.Norm;
public class NormInputStream : Stream
{
    private NormInstance _normInstance;
    private NormSession _normSession;
    private NormInputStream _normInputStream;
    private List<INormEventListener> _normEventListeners;
    private bool _closed;
    private object _closedLock;
    private bool _bufferIsEmpty;
    private bool _receivedEof;

    private NormStream _normStream;
    
    public bool IsClosed => _closed;

    public NormInputStream(string address, int port)
    {
        _normInstance = new NormInstance();

        _normSession = _normInstance.CreateSession(address, port, NormNode.NORM_NODE_ANY);       
        _normStream = null;
    
        _normEventListeners = new List<INormEventListener>();
    
        _closed = true;
        _closedLock = new Object();
        
        _bufferIsEmpty = true;
        _receivedEof  = false;
    }

    public void OpenDebugLog(string fileName)
    {
        if(fileName == null )
        {
            throw new IOException("File was name was not found.");
        }
        _normInstance.OpenDebugLog(fileName);
    }

    public void CloseDebugLog()
    {
        _normInstance.CloseDebugLog();
    }

    public void NormSetDebugLevel(int level)
    {
        //_normInstance.DebugLevel();
    }

    public void SetMessageTrace(bool messageTrace) 
    {
        _normSession.SetMessageTrace(messageTrace);
    }

    public void setMulticastInterface(String multicastInterface)
    {
        if(multicastInterface == null)
        {
            throw new IOException("");
        }
        _normSession.SetMulticastInterface(multicastInterface);
    }

    public void setEcnSupport(bool ecnEnable, bool ignoreLoss)
    {
        _normSession.SetEcnSupport(ecnEnable, ignoreLoss);
    }

    public void SetTtl(byte ttl)   
    {
        if(ttl == null)
        {
            throw new IOException("");
        }
        _normSession.SetTTL(ttl);
    }

    public void setTos(byte tos)
    {
        if(tos == null)
        {
            throw new IOException("");
        }
        _normSession.SetTOS(tos);
    }

    public void setSilentReceiver(bool silent, int maxDelay) 
    {
        _normSession.SetSilentReceiver(silent, maxDelay);
    }

    public void SetDefaultUnicastNack(bool defaultUnicastNack) 
    {
        _normSession.SetDefaultUnicastNack(defaultUnicastNack);
    }

    public  void SeekMsgStart() 
    {
        if (_normStream == null) 
        {
            throw new IOException(message: "Can only seek msg start after the stream is connected");
        }

        _normStream.SeekMsgStart();
    }


    public void AddNormEventListener(INormEventListener normEventListener) 
    {
        _normEventListeners.Add(normEventListener);
    }

    /// <param name="normEventListener">The INormEventListener to remove.</param>
    public void RemoveNormEventListener(INormEventListener normEventListener)
    {
        _normEventListeners.Remove(normEventListener);
    }

    private void FireNormEventOccured(NormEvent normEvent)
    {
        foreach (var normEventListener in _normEventListeners)
        {
            normEventListener.NormEventOccurred(normEvent);
        }
    }

    public void Open(long bufferSpace) 
    {
        if (!IsClosed) 
        {
            throw new IOException("Stream is already open");
        }

        _normSession.StartReceiver(bufferSpace);

        _closed = false;
    }

    private void ProcessEvent()
    {
        // Retrieve the next event
        var normEvent = _normInstance.GetNextEvent();

        // Check if the stream was closed
        if (IsClosed)
        {
            throw new IOException("Stream closed");
        }

        if (normEvent != null)
        {
            // Process the event
            var eventType = normEvent.Type;
            switch (eventType)
            {
                case NormEventType.NORM_TX_QUEUE_VACANCY:
                case NormEventType.NORM_TX_QUEUE_EMPTY:
                    var normObject = normEvent.Object;
                    if (normObject == null || !normObject.Equals(_normStream))
                    {
                        break;
                    }

                    // Signal that the buffer is not full
                    _bufferIsEmpty = false;
                    break;

                case NormEventType.NORM_TX_OBJECT_SENT:
                case NormEventType.NORM_TX_OBJECT_PURGED:
                    _normStream = null;
                    break;

                default:
                    break;
            }

            // Notify listeners of the norm event
            FireNormEventOccured(normEvent);
        }
    }

    public void Dispose()
    {
       
    }

    public override void Flush()
    {
        throw new NotImplementedException();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        throw new NotImplementedException();
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        throw new NotImplementedException();
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        throw new NotImplementedException();
    }

    public override void SetLength(long value)
    {
        throw new NotImplementedException();
    }

    public override bool CanRead => true;

    public override bool CanSeek => throw new NotImplementedException();

    public override bool CanWrite => false;

    public override long Length => throw new NotImplementedException();

    public override long Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }


}