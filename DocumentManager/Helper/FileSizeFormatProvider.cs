namespace DocumentManager.Helper
{
    using System;

    public class FileSizeFormatProvider : IFormatProvider, ICustomFormatter
    {
        private const string FileSizeFormat = "fs";
        private const decimal OneKiloByte = 1024M;
        private const decimal OneMegaByte = OneKiloByte*1024M;
        private const decimal OneGigaByte = OneMegaByte*1024M;

        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            if (format == null || !format.StartsWith(FileSizeFormat)) return DefaultFormat(format, arg, formatProvider);

            if (arg is string) return DefaultFormat(format, arg, formatProvider);

            decimal size;

            try
            {
                size = Convert.ToDecimal(arg);
            }
            catch (InvalidCastException)
            {
                return DefaultFormat(format, arg, formatProvider);
            }

            string suffix;

            if (size > OneGigaByte)
            {
                size /= OneGigaByte;
                suffix = "GB";
            }
            else if (size > OneMegaByte)
            {
                size /= OneMegaByte;
                suffix = "MB";
            }
            else if (size > OneKiloByte)
            {
                size /= OneKiloByte;
                suffix = "KB";
            }
            else
            {
                suffix = " B";
            }

            var precision = format.Substring(2);
            if (string.IsNullOrEmpty(precision)) precision = "2";
            return string.Format("{0:N" + precision + "} {1}", size, suffix);
        }

        public object GetFormat(Type formatType)
        {
            return formatType == typeof (ICustomFormatter) ? this : null;
        }

        private static string DefaultFormat(string format, object arg, IFormatProvider formatProvider)
        {
            var formattableArg = arg as IFormattable;

            return formattableArg != null ? formattableArg.ToString(format, formatProvider) : arg.ToString();
        }
    }
}