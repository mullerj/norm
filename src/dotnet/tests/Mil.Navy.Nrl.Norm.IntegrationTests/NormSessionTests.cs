namespace Mil.Navy.Nrl.Norm.IntegrationTests
{
    public class NormSessionTests : IDisposable
    {
        private readonly NormInstance _normInstance;
        private NormSession _normSession;
        private bool _isInstanceDestroyed;
        private bool _isSessionDestroyed;
        private bool _isSenderStarted;
        private bool _isSenderStopped;

        /// <summary>
        /// Create a NORM session
        /// </summary>
        private NormSession CreateSession()
        {
            var sessionAddress = "224.1.2.3";
            var sessionPort = 6003;
            var localNodeId = NormNode.NORM_NODE_ANY;

            return _normInstance.CreateSession(sessionAddress, sessionPort, localNodeId);
        }

        public NormSessionTests() 
        {
            _normInstance = new NormInstance();
            _normSession = CreateSession();
            _isInstanceDestroyed = false;
            _isSessionDestroyed = false;
            _isSenderStarted = false;
            _isSenderStopped = false;
        }

        private void StartSender()
        {
            if (!_isSenderStarted)
            {
                _normSession.StartSender(1, 1024 * 1024, 1400, 64, 16);
                _isSenderStarted = true;
            }
        }

        private void StopSender()
        {
            if (_isSenderStarted && !_isSenderStopped)
            {
                _normSession.StopSender();
                _isSenderStopped = true;
            }
        }

        /// <summary>
        /// Destroys the NORM session
        /// </summary>
        private void DestroySession()
        {
            if (!_isSessionDestroyed)
            {
                StopSender();
                _normSession.DestroySession();
                _isSessionDestroyed = true;
            }
        }

        /// <summary>
        /// Destroy the NORM instance
        /// </summary>
        private void DestroyInstance()
        {
            if (!_isInstanceDestroyed)
            {
                DestroySession();
                _normInstance.DestroyInstance();
                _isInstanceDestroyed = true;
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
        /// Test for creating a NORM session
        /// </summary>
        [Fact]
        public void CreatesSession()
        {
            Assert.NotNull(_normSession);
        }

        /// <summary>
        /// Test for destroying a NORM session
        /// </summary>
        [Fact]
        public void DestroysSession()
        {
            DestroySession();
        }

        /// <summary>
        /// Test for starting a NORM sender
        /// </summary>
        [Fact]
        public void StartsSender()
        {
            StartSender();
        }

        /// <summary>
        /// Test for stopping a NORM sender
        /// </summary>
        [Fact]
        public void StopsSender()
        {
            StartSender();
            StopSender();
        }

        [Fact]
        public void EnqueuesFile()
        {
            StartSender();

            var fileContent = "Hello to the other norm node!!!!!!";
            var fileName = Guid.NewGuid().ToString();
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), fileName);
            File.WriteAllText(filePath, fileContent);

            try
            {
                var normFile = _normSession.FileEnqueue(filePath);
                Assert.NotNull(normFile);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                StopSender();
                File.Delete(filePath);
            }
        }
    }
}