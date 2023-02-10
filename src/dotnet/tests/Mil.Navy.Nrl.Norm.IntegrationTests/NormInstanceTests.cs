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
                _normInstance.DestroyInstance();
                _isDestroyed = true;
            }
        }

        /// <summary>
        /// Dispose destroys the NormInstance
        /// </summary>
        public void Dispose()
        {
            DestroyInstance();
        }

        /// <summary>
        /// Test for creating NormInstance
        /// </summary>
        [Fact]
        public void CreatesNormInstance()
        {
            Assert.NotNull(_normInstance);
        }

        /// <summary>
        /// Test for destroying NormInstance
        /// </summary>
        [Fact]
        public void DestroysNormInstance()
        {
            DestroyInstance();
        }
    }
}
