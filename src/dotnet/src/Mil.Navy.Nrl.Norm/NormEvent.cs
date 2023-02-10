using Mil.Navy.Nrl.Norm.Enums;

namespace Mil.Navy.Nrl.Norm
{
    public class NormEvent
    {
        private NormEventType _type { get; }
        private long _sessionHandle { get; }
        private long _nodeHandle { get; }
        private long _objectHandle { get; }

        public NormEvent(NormEventType type, long sessionHandle, long nodeHandle, long objectHandle)
        {
            _type = type;
            _sessionHandle = sessionHandle;
            _nodeHandle = nodeHandle;
            _objectHandle = objectHandle;
        }
    }
}
