using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace IniParser.Model
{
    public class Data
    {
        private readonly string _data;

        public Data(string data)
        {
            _data = data;
        }

        #region Export
        public static implicit operator Char(Data data)
        {
            try
            {
                return Convert.ToChar(data._data, CultureInfo.InvariantCulture);
            }
            catch
            {
                return '\0';
            }
        }

        public static implicit operator String(Data data)
        {
            return data._data;
        }

        public static implicit operator bool(Data data)
        {
            try
            {
                return Convert.ToBoolean(data._data, CultureInfo.InvariantCulture);
            }
            catch
            {
                return false;
            }
        }

        public static implicit operator Byte(Data data)
        {
            try
            {
                return Convert.ToByte(data._data, CultureInfo.InvariantCulture);
            }
            catch
            {
                return 0;
            }
        }

        public static implicit operator Int16(Data data)
        {
            try
            {
                return Convert.ToInt16(data._data, CultureInfo.InvariantCulture);
            }
            catch
            {
                return 0;
            }
        }

        public static implicit operator Int32(Data data)
        {
            try
            {
                return Convert.ToInt32(data._data, CultureInfo.InvariantCulture);
            }
            catch
            {
                return 0;
            }
        }

        public static implicit operator Int64(Data data)
        {
            try
            {
                return Convert.ToInt64(data._data, CultureInfo.InvariantCulture);
            }
            catch
            {
                return 0;
            }
        }

        public static implicit operator DateTime(Data data)
        {
            try
            {
                return Convert.ToDateTime(data._data, CultureInfo.InvariantCulture);
            }
            catch
            {
                return DateTime.UtcNow;
            }
        }

        public static implicit operator Single(Data data)
        {
            try
            {
                return Convert.ToSingle(data._data, CultureInfo.InvariantCulture);
            }
            catch
            {
                return 0;
            }
        }

        public static implicit operator Double(Data data)
        {
            try
            {
                return Convert.ToDouble(data._data, CultureInfo.InvariantCulture);
            }
            catch
            {
                return 0;
            }
        }

        public static implicit operator Decimal(Data data)
        {
            try
            {
                return Convert.ToDecimal(data._data, CultureInfo.InvariantCulture);
            }
            catch
            {
                return 0;
            }
        }
        #endregion

        #region Import
        public static implicit operator Data(Char data)
        {
            return new Data(data.ToString(CultureInfo.InvariantCulture));
        }

        public static implicit operator Data(String data)
        {
            return new Data(data);
        }

        public static implicit operator Data(bool data)
        {
            return new Data(data.ToString(CultureInfo.InvariantCulture));
        }

        public static implicit operator Data(Byte data)
        {
            return new Data(data.ToString(CultureInfo.InvariantCulture));
        }

        public static implicit operator Data(Int16 data)
        {
            return new Data(data.ToString(CultureInfo.InvariantCulture));
        }

        public static implicit operator Data(Int32 data)
        {
            return new Data(data.ToString(CultureInfo.InvariantCulture));
        }

        public static implicit operator Data(Int64 data)
        {
            return new Data(data.ToString(CultureInfo.InvariantCulture));
        }

        public static implicit operator Data(DateTime data)
        {
            return new Data(data.ToString(CultureInfo.InvariantCulture));
        }

        public static implicit operator Data(Single data)
        {
            return new Data(data.ToString(CultureInfo.InvariantCulture));
        }

        public static implicit operator Data(Double data)
        {
            return new Data(data.ToString(CultureInfo.InvariantCulture));
        }

        public static implicit operator Data(Decimal data)
        {
            return new Data(data.ToString(CultureInfo.InvariantCulture));
        }
        #endregion
    }
}
