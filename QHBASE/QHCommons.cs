using System;

namespace QHBASE
{
    public static class QHCommons
    {
        public static string GenAutoId()
        {
            return Guid.NewGuid().ToString("N").Substring(0, 6).ToLower();
        }


    }
}
