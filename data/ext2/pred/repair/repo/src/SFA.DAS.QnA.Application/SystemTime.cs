using System;

namespace SFA.DAS.QnA.Application
{
    public class SystemTime
    {
        public static Func<DateTime> UtcNow = () => DateTime.UtcNow;
    }
}