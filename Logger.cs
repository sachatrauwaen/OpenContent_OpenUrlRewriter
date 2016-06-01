using System;
using System.Diagnostics;
using DotNetNuke.Instrumentation;

namespace Satrabel.OpenUrlRewriter.OpenContent
{
    /// <summary>
    /// Utility class containing several commonly used procedures by Stefan Kamphuis
    /// </summary>
    public static class Log
    {
        public static ILog Logger
        {
            get
            {
                return LoggerSource.Instance.GetLogger("OpenUrlRewriter_OpenContent");
            }
        }
       
    }
}