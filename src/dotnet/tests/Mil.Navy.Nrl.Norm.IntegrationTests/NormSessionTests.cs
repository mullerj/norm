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
                var expectedEventTypes = new List<NormEventType> { NormEventType.NORM_TX_OBJECT_SENT, NormEventType.NORM_TX_QUEUE_EMPTY };
                var actualEventTypes = GetEvents().Select(e => e.Type).ToList();
                Assert.Equal(expectedEventTypes, actualEventTypes);
                var expectedFileName = filePath;
                var actualFileName = normFile.Name;
                Assert.Equal(expectedFileName, actualFileName);
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
                var normEvents = GetEvents();

                var expectedNormEventType = NormEventType.NORM_RX_OBJECT_COMPLETED;
                Assert.Contains(expectedNormEventType, normEvents.Select(e => e.Type));
                var normObjectEvent = normEvents.First(e => e.Type == expectedNormEventType);

                var receivedNormFile = Assert.IsType<NormFile>(normObjectEvent.Object);
                var expectedFileName = receivedNormFile.Name;

                //Check that file exists
                var expectedFileCount = 1;
                var actualFiles = Directory.GetFiles(cachePath);
                var actualFileCount = actualFiles.Length;
                Assert.Equal(expectedFileCount, actualFileCount);

                //Check file content
                var actualFileName = actualFiles.First();
                Assert.Equal(expectedFileName, actualFileName);
                var actualContent = File.ReadAllText(actualFileName);
                Assert.Equal(fileContent, actualContent);

            }
            catch (Exception)
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

        [Fact]
        public void EnqueuesData()
        {
            StartSender();
            //Create data to write to the stream
            UnicodeEncoding unicodeEncoding = new UnicodeEncoding();
            byte[] data = unicodeEncoding.GetBytes("Data string to be transmitted");

            try
            {
                _normSession.DataEnqueue(data, 0, data.Length);
                var expectedEventTypes = new List<NormEventType> { NormEventType.NORM_TX_OBJECT_SENT, NormEventType.NORM_TX_QUEUE_EMPTY };
                var actualEventTypes = GetEvents().Select(e => e.Type).ToList();
                Assert.Equal(expectedEventTypes, actualEventTypes);
            }
            catch(Exception)
            {
                throw;
            }
            finally
            {
                StopSender();
            }
        }

        [Fact]
        public void ReceivesData()
        {
            _normSession.SetLoopback(true);
            StartSender();
            StartReceiver();

            //Set up cache directory
            var folderName = Guid.NewGuid().ToString();
            var cachePath = Path.Combine(Directory.GetCurrentDirectory(), folderName);
            Directory.CreateDirectory(cachePath);
            _normInstance.SetCacheDirectory(cachePath);

            //Create data to be sent
            var expectedContent = "Data string to be transmitted";
            var data = Encoding.ASCII.GetBytes(expectedContent);

            try
            {
                var normData = _normSession.DataEnqueue(data, 0, data.Length);
                var expectedEventTypes = new List<NormEventType> 
                { 
                    NormEventType.NORM_REMOTE_SENDER_NEW,
                    NormEventType.NORM_REMOTE_SENDER_ACTIVE,
                    NormEventType.NORM_TX_OBJECT_SENT,
                    NormEventType.NORM_TX_QUEUE_EMPTY,
                    NormEventType.NORM_RX_OBJECT_NEW,
                    NormEventType.NORM_RX_OBJECT_UPDATED,
                    NormEventType.NORM_RX_OBJECT_COMPLETED
                };
                var actualEventTypes = GetEvents().Select(e => e.Type).ToList();
                Assert.Equivalent(expectedEventTypes, actualEventTypes);

                var actualContent = Encoding.Default.GetString(normData.Data);
                Assert.Equal(expectedContent, actualContent);
            }
            catch(Exception)
            {
                throw;
            }
            finally
            {
                StopSender();
                StopReceiver();
                Directory.Delete(cachePath, true);
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

                var expectedBytesWritten = data.Length;
                var actualBytesWritten = normStream.Write(data, data.Length);

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
        public void ReceivesStream()
        {
            _normSession.SetLoopback(true);
            StartSender();
            StartReceiver();

            //Set up cache directory
            var folderName = Guid.NewGuid().ToString();
            var cachePath = Path.Combine(Directory.GetCurrentDirectory(), folderName);
            Directory.CreateDirectory(cachePath);
            _normInstance.SetCacheDirectory(cachePath);

            var fileContent = "Hello to the other norm node!!!!!!";
            var data = Encoding.ASCII.GetBytes(fileContent);
            NormStream? normStream = null;

            try
            {
                var repairWindowSize = 1024 * 1024;
                normStream = _normSession.StreamOpen(repairWindowSize);

                var expectedBytesWritten = data.Length;
                normStream.Write(data, data.Length);

                normStream.MarkEom();
                normStream.Flush();
                var normEvents = GetEvents();
                var expectedNormEventType = NormEventType.NORM_RX_OBJECT_UPDATED;
                Assert.Contains(expectedNormEventType, normEvents.Select(e => e.Type));
                var normObjectEvent = normEvents.First(e => e.Type == expectedNormEventType);

                var receivedNormStream = Assert.IsType<NormStream>(normObjectEvent.Object);
                var numRead = 0;
                var receiveBuffer = new byte[65536];
                while ((numRead = receivedNormStream.Read(receiveBuffer, receiveBuffer.Length)) > 0)
                {
                    if (numRead != -1)
                    {
                        var receivedData = receiveBuffer.Take(numRead).ToArray();
                        var receivedContent = Encoding.ASCII.GetString(receivedData);
                        Assert.Equal(fileContent, receivedContent);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                normStream?.Close(true);
                StopSender();
                StopReceiver();
                Directory.Delete(cachePath, true);
            }
        }
    }
}