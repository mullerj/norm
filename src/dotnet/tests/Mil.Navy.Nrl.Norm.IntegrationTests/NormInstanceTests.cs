namespace Mil.Navy.Nrl.Norm.IntegrationTests
{
    /// <summary>
    /// Tests for NORM instance
    /// </summary>
    public class NormInstanceTests : IDisposable
    {
        /// <summary>
        /// The NORM instance
        /// </summary>
        private readonly NormInstance _normInstance;
        private NormSession? _normSession;
        /// <summary>
        /// Determines if the NORM instance has been destroyed
        /// </summary>
        private bool _isDestroyed;

        /// <summary>
        /// Default constructor for NORM instance tests
        /// </summary>
        /// <remarks>
        /// Creates the NORM instance.
        /// Initializes _isDestroyed to false.
        /// </remarks>
        public NormInstanceTests()
        {
            _normInstance = new NormInstance();
            _isDestroyed = false;
        }

        /// <summary>
        /// Destroy the NORM instance
        /// </summary>
        private void DestroyInstance()
        {
            if (!_isDestroyed) 
            {
                _normSession?.DestroySession();
                _normInstance.DestroyInstance();
                _isDestroyed = true;
            }
        }

        /// <summary>
        /// Dispose destroys the NORM instance
        /// </summary>
        public void Dispose()
        {
            DestroyInstance();
        }

        /// <summary>
        /// Test for creating a NORM instance
        /// </summary>
        [Fact]
        public void CreatesNormInstance()
        {
            Assert.NotNull(_normInstance);
        }

        /// <summary>
        /// Test for destroying a NORM instance
        /// </summary>
        [Fact]
        public void DestroysNormInstance()
        {
            DestroyInstance();
        }

        /// <summary>
        /// Test for creating a NORM session
        /// </summary>
        [Fact]
        public void CreatesSession()
        {
            var sessionAddress = "224.1.2.3";
            var sessionPort = 6003;
            var localNodeId = NormNode.NORM_NODE_ANY;

            _normSession = _normInstance.CreateSession(sessionAddress, sessionPort, localNodeId);
            Assert.NotNull(_normSession);
        }

        /// <summary>
        /// Test for throwing an exception when attempting to create a NORM session with an invalid session address
        /// </summary>
        [Fact]
        public void CreateSessionThrowsExceptionForInvalidSessionAddress()
        {
            var sessionAddress = "999.999.999.999";
            var sessionPort = 6003;
            var localNodeId = NormNode.NORM_NODE_ANY;

            Assert.Throws<IOException>(() => _normInstance.CreateSession(sessionAddress, sessionPort, localNodeId));
        }

        [Fact]
        public void StopInstance()
        {
            var sessionAddress = "224.1.2.3";
            var sessionPort = 6003;
            var localNodeId = NormNode.NORM_NODE_ANY;

            _normSession = _normInstance.CreateSession(sessionAddress, sessionPort, localNodeId);
        
            _normInstance.StopInstance();
        }

        [Fact]
        public void RestartInstance()
        {
            var sessionAddress = "224.1.2.3";
            var sessionPort = 6003;
            var localNodeId = NormNode.NORM_NODE_ANY;
            var expected = true;
            _normSession = _normInstance.CreateSession(sessionAddress, sessionPort, localNodeId);
        
            _normInstance.StopInstance();

          var actual = _normInstance.RestartInstance();
         Assert.Equal(expected,actual);
        }

         [Fact]
        public void SuspendInstance()
        {
            var sessionAddress = "224.1.2.3";
            var sessionPort = 6003;
            var localNodeId = NormNode.NORM_NODE_ANY;
            var expected = true;
            _normSession = _normInstance.CreateSession(sessionAddress, sessionPort, localNodeId);
        
            _normInstance.StopInstance();

          var actual = _normInstance.SuspendInstance();
         Assert.Equal(expected,actual);
        }
    }
}
