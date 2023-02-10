using System.Runtime.InteropServices;

namespace Mil.Navy.Nrl.Norm
{
    /// <summary>
    /// The native NORM API functions 
    /// </summary>
    public static class NormApi
    {
        public const string NORM_LIBRARY = "norm";

        /// <summary>
        /// This function creates an instance of a NORM protocol engine and is the necessary first step before any other API functions may be used.
        /// </summary>
        /// <param name="priorityBoost">The priorityBoost parameter, when set to a value of true, specifies that the NORM protocol engine thread be run with higher priority scheduling.</param>
        /// <returns>A value of NORM_INSTANCE_INVALID is returned upon failure.</returns>
        [DllImport(NORM_LIBRARY)]
        public static extern long NormCreateInstance(bool priorityBoost);

        /// <summary>
        /// The NormDestroyInstance() function immediately shuts down and destroys the NORM protocol engine instance referred to by the instanceHandle parameter.
        /// </summary>
        /// <param name="instanceHandle">The NORM protocol engine instance referred to by the instanceHandle parameter</param>
        [DllImport(NORM_LIBRARY)]
        public static extern void NormDestroyInstance(long instanceHandle);
    }
}
