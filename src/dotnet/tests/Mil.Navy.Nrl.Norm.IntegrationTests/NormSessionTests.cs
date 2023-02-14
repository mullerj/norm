using Mil.Navy.Nrl.Norm.Enums;
using System.Text;

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

        private bool _isReceiverStarted;
        private bool _isReceiverStopped;

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
                _normSession.StartSender(1024 * 1024, 1400, 64, 16);
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

        private void StartReceiver()
        {
            if(!_isReceiverStarted)
            {
                //The appropriate bufferSpace to use is a function of expected network delay * bandwidth product and packet loss characteristics
                _normSession.StartReceiver(10*10);
                _isReceiverStarted = true;
            }
        }

        private void StopReceiver()
        {
            if(_isReceiverStarted && ! _isReceiverStopped)
            {
                _normSession.StopReceiver();
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

        private IEnumerable<NormEvent> GetEvents()
        {
            var normEvents = new List<NormEvent>();
            while (_normInstance.HasNextEvent(TimeSpan.FromMilliseconds(30)))
            {
                var normEvent = _normInstance.GetNextEvent(false);
                if (normEvent != null)
                {
                    normEvents.Add(normEvent);
                }
            }
            return normEvents;
        }

        private void WaitForEvents()
        {
            GetEvents();
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
                var expectedEventTypes = new List<NormEventType> { NormEventType.NORM_TX_OBJECT_SENT, NormEventType.NORM_TX_QUEUE_EMPTY};
                var actualEventTypes = GetEvents().Select(e => e.Type).ToList();
                Assert.Equal(expectedEventTypes, actualEventTypes);
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

        [Fact]
        public void SendsStream()
        {
            StartSender();

            var fileContent = "Hello to the other norm node!!!!!!";
            var data = Encoding.ASCII.GetBytes(fileContent);
            NormStream? normStream = null;

            try
            {
                var repairWindowSize = 1024 * 1024;
                normStream = _normSession.StreamOpen(repairWindowSize);
                using var dataStream = new MemoryStream(data);
                var readBuffer = new byte[data.Length];
                var length = dataStream.Read(readBuffer, 0, readBuffer.Length);

                var offset = 0;
                var expectedBytesWritten = data.Length;
                var actualBytesWritten = normStream.Write(readBuffer, offset, length);

                WaitForEvents();
                normStream.MarkEom();
                normStream.Flush();

                Assert.Equal(expectedBytesWritten, actualBytesWritten);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                normStream?.Close(true);
                StopSender();
            }
        }

        [Fact]
        public void StartsReceiver()
        {
            StartReceiver();
        }

        [Fact]
        public void StopsReceiver()
        {
            StartReceiver();
            StopReceiver();
        }

        [Fact]
        public void ReceivesFile()
        {
            _normSession.SetLoopback(true);
            StartSender();
            StartReceiver();

            //Set up cache directory
            var folderName = Guid.NewGuid().ToString();
            var cachePath = Path.Combine(Directory.GetCurrentDirectory(), folderName);
            Directory.CreateDirectory(cachePath);
            _normInstance.SetCacheDirectory(cachePath);

            //Set up file to send
            var fileName = Guid.NewGuid().ToString();
            var fileContent = "Hello to the other norm node!!!!!!";
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), fileName);
            File.WriteAllText(filePath, fileContent);

            try
            {
                //Enqueue file
                var normFile = _normSession.FileEnqueue(filePath);
                //Wait for events
                WaitForEvents();

                //Check that file exists
                var expectedFileCount = 1;
                var actualFiles = Directory.GetFiles(cachePath);
                var actualFileCount = actualFiles.Length;
                Assert.Equal(expectedFileCount, actualFileCount);

                //Check file content
                var actualContent = File.ReadAllText(actualFiles.First());
                Assert.Equal(fileContent, actualContent);

            }
            catch(Exception)
            {
                throw;
            }
            finally
            {
                StopSender();
                StopReceiver();
                File.Delete(filePath);
                Directory.Delete(cachePath, true);
            }
        }
    }
}