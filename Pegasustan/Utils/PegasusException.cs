using System;

namespace Pegasustan.Utils
{
    public class PegasusException : Exception
    {
        public PegasusException()
        {
            
        }

        public PegasusException(string message) : base(message)
        {
            
        }

        public PegasusException(string message, Exception inner) : base(message, inner)
        {
            
        }
    }
}
