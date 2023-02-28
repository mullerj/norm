namespace Mil.Navy.Nrl.Norm
{
    public class NormEvent
    {
        private NormEventType _type { get; }
        private long _sessionHandle { get; }
        private long _nodeHandle { get; }
        private long _objectHandle { get; }

        /// <summary>
        /// The NormEvent type is a structure used to describe significant NORM protocol events.
        /// </summary>
        public NormEvent(NormEventType type, long sessionHandle, long nodeHandle, long objectHandle)
        {
            _type = type;
            _sessionHandle = sessionHandle;
            _nodeHandle = nodeHandle;
            _objectHandle = objectHandle;
        }


        /// <summary>
        /// The NormEvent type is a structure used to describe significant NORM protocol events.
        /// </summary>
        public NormEventType Type => _type;

        /// <summary>
        /// Type is used to reference NORM transport sessions which have been created using the NormCreateSession() API call.
        /// </summary>
        public NormSession? Session 
        { 
            get
            {
                if (_sessionHandle == NORM_SESSION_INVALID)
                {
                    return null;
                }
                return NormSession.GetSession(_sessionHandle);
            } 
        }

        /// <summary>
        /// type is used to reference state kept by the NORM implementation with respect to other participants within a NormSession.
        /// </summary>
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

        /// <summary>
        /// This function can be used to determine the object type ((NORM_OBJECT_DAT, NORM_OBJECT_FILE, or NORM_OBJECT_STREAM) for the NORM transport object identified by the objectHandle parameter.
        /// </summary>
        public NormObject? Object 
        { 
            get
            {
                NormObject? normObject = null;
                var normObjectType = NormObjectGetType(_objectHandle);
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
