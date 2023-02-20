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

        public NormEventType Type => _type;

        public NormSession? Session 
        { 
            get
            {
                if (_sessionHandle == NormApi.NORM_SESSION_INVALID)
                {
                    return null;
                }
                return NormSession.GetSession(_sessionHandle);
            } 
        }

        public NormNode? Node 
        { 
            get
            {
                if (_nodeHandle == 0)
                {
                    return null;
                }
                return new NormNode(_nodeHandle);
            } 
        }

        public NormObject? Object 
        { 
            get
            {
                NormObject? normObject = null;
                var normObjectType = NormApi.NormObjectGetType(_objectHandle);
                switch (normObjectType)
                {
                    case NormObjectType.NORM_OBJECT_DATA:
                        normObject = new NormData(_objectHandle);
                        break;
                    case NormObjectType.NORM_OBJECT_FILE:
                        normObject = new NormFile(_objectHandle);
                        break;
                    case NormObjectType.NORM_OBJECT_STREAM:
                        normObject= new NormStream(_objectHandle);
                        break;
                    case NormObjectType.NORM_OBJECT_NONE:
                    default:
                        break;
                }
                return normObject;
            } 
        }

        public override string ToString()
        {
            return $"NormEvent [type={_type}]";
        }
    }
}
