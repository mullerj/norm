namespace Mil.Navy.Nrl.Norm
{
    public class NormInputStream
    {
        private NormInstance _normInstance;
        private NormSession _normSession;
        private NormInputStream _normInputStream;
        private List<INormEventListener> normEventListener;
        private bool _closed;
        private  object _closedLock;
        private bool _bufferIsEmpty;
        private bool _receivedEof;

        private NormStream _normStream;

        public NormInputStream(string address, int port)
        {
            _normInstance = new NormInstance();

            _normSession = _normInstance.CreateSession(address,port,NormNode.NORM_NODE_ANY);       
            _normStream = null;
       
            normEventListener = new List<INormEventListener>();
       
            _closed = true;
            _closedLock = new Object();
           
            _bufferIsEmpty = true;
            _receivedEof  = false;
        }
    }
}