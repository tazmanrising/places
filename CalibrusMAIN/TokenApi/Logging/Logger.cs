using System;
using System.Diagnostics;
using NLog;

namespace Logging
{
    public static class Logger
    {
        #region Trace Methods
        public static void Trace(string message, params object[] args)
        {
            var logger = GetNLogLogger();
            logger.Trace(message, args);
        }

        public static void Trace(Exception exception, string message, params object[] args)
        {
            var logger = GetNLogLogger();
            logger.Trace(exception, message, args);
        }
        #endregion

        #region Debug Methods
        public static void Debug(string message, params object[] args)
        {
            var logger = GetNLogLogger();
            logger.Debug(message, args);
        }

        public static void Debug(Exception exception, string message, params object[] args)
        {
            var logger = GetNLogLogger();
            logger.Debug(exception, message, args);
        }
        #endregion

        #region Info Methods
        public static void Info(string message, params object[] args)
        {
            var logger = GetNLogLogger();
            logger.Info(message, args);
        }

        public static void Info(Exception exception, string message, params object[] args)
        {
            var logger = GetNLogLogger();
            logger.Info(exception, message, args);
        }
        #endregion

        #region Warn Methods
        public static void Warn(string message, params object[] args)
        {
            var logger = GetNLogLogger();
            logger.Warn(message, args);
        }

        public static void Warn(Exception exception, string message, params object[] args)
        {
            var logger = GetNLogLogger();
            logger.Warn(exception, message, args);
        }
        #endregion

        #region Error Methods
        public static void Error(string message, params object[] args)
        {
            var logger = GetNLogLogger();
            logger.Error(message, args);
        }

        public static void Error(Exception exception, string message, params object[] args)
        {
            var logger = GetNLogLogger();
            logger.Error(exception, message, args);
        }
        #endregion

        #region Fatal Methods
        public static void Fatal(string message, params object[] args)
        {
            var logger = GetNLogLogger();
            logger.Fatal(message, args);
        }

        public static void Fatal(Exception exception, string message, params object[] args)
        {
            var logger = GetNLogLogger();
            logger.Fatal(exception, message, args);
        }
        #endregion

        private static NLog.Logger GetNLogLogger()
        {
            return LogManager.GetLogger(GetClassFullName());
        }

        /// <summary>
        /// Gets the fully qualified name of the class invoking the LogManager, including the 
        /// namespace but not the assembly.    
        /// </summary>
        private static string GetClassFullName()
        {
            string className;
            Type declaringType;
            int framesToSkip = 3;

            do
            {
                var frame = new StackFrame(framesToSkip, false);
                var method = frame.GetMethod();
                declaringType = method.DeclaringType;
                if (declaringType == null)
                {
                    className = method.Name;
                    break;
                }

                framesToSkip++;
                className = declaringType.FullName;
            } while (declaringType.Module.Name.Equals("mscorlib.dll", StringComparison.OrdinalIgnoreCase));

            return className;
        }

        /// <summary>
        /// Need to somehow use the NLog.Web assembly so it is included when building the solution.
        /// Otherwise its dll isn't included and NLog.config will error.
        /// </summary>
        /// <returns></returns>
        internal static Type DoNotDelete()
        {
            return typeof(NLog.Web.LayoutRenderers.AspNetUserIdentityLayoutRenderer);
        }
    }
}
