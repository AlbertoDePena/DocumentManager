namespace DocumentManager.Helper
{
    using System;

    public static class ExtensionMethods
    {
        public static string ToFileSize(this long l)
        {
            return string.Format(new FileSizeFormatProvider(), "{0:fs}", l);
        }

        public static string ToMessage(this Exception ex)
        {
            var message = string.Format("{0} \n\n", ex.Message);

            if (ex.InnerException != null) message += ToMessage(ex.InnerException);

            return message;
        }
    }
}