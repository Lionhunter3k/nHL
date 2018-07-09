using Microsoft.Extensions.Localization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace nHL.Web.Services
{
    public readonly struct GetOrFormatLocalizedString : IEnumerable<char>, IEnumerable, ICloneable, IComparable, IComparable<String>, IConvertible, IEquatable<String>
    {
        private readonly Func<string, object[], string> formatter;

        public GetOrFormatLocalizedString(LocalizedString localizedString, Func<string, object[], string> formatter) : this()
        {
            LocalizedString = localizedString;
            this.formatter = formatter;
        }

        public string this[params object[] args]
        {
            get
            {
                return this.Format(args);
            }
        }

        public LocalizedString LocalizedString { get; }

        public object Clone()
        {
            return LocalizedString.Value.Clone();
        }

        public int CompareTo(object obj)
        {
            return LocalizedString.Value.CompareTo(obj);
        }

        public int CompareTo(string other)
        {
            return LocalizedString.Value.CompareTo(other);
        }

        public bool Equals(string other)
        {
            return LocalizedString.Value.Equals(other);
        }

        public override bool Equals(object obj)
        {
            return LocalizedString.Value.Equals(obj);
        }

        public string Format(params object[] args)
        {
            return formatter(LocalizedString, args);
        }

        public IEnumerator<char> GetEnumerator()
        {
            return ((IEnumerable<char>)LocalizedString.Value).GetEnumerator();
        }

        public override int GetHashCode()
        {
            return LocalizedString.Value.GetHashCode();
        }

        public TypeCode GetTypeCode()
        {
            return LocalizedString.Value.GetTypeCode();
        }

        public bool ToBoolean(IFormatProvider provider)
        {
            return ((IConvertible)LocalizedString.Value).ToBoolean(provider);
        }

        public byte ToByte(IFormatProvider provider)
        {
            return ((IConvertible)LocalizedString.Value).ToByte(provider);
        }

        public char ToChar(IFormatProvider provider)
        {
            return ((IConvertible)LocalizedString.Value).ToChar(provider);
        }

        public DateTime ToDateTime(IFormatProvider provider)
        {
            return ((IConvertible)LocalizedString.Value).ToDateTime(provider);
        }

        public decimal ToDecimal(IFormatProvider provider)
        {
            return ((IConvertible)LocalizedString.Value).ToDecimal(provider);
        }

        public double ToDouble(IFormatProvider provider)
        {
            return ((IConvertible)LocalizedString.Value).ToDouble(provider);
        }

        public short ToInt16(IFormatProvider provider)
        {
            return ((IConvertible)LocalizedString.Value).ToInt16(provider);
        }

        public int ToInt32(IFormatProvider provider)
        {
            return ((IConvertible)LocalizedString.Value).ToInt32(provider);
        }

        public long ToInt64(IFormatProvider provider)
        {
            return ((IConvertible)LocalizedString.Value).ToInt64(provider);
        }

        public sbyte ToSByte(IFormatProvider provider)
        {
            return ((IConvertible)LocalizedString.Value).ToSByte(provider);
        }

        public float ToSingle(IFormatProvider provider)
        {
            return ((IConvertible)LocalizedString.Value).ToSingle(provider);
        }

        public string ToString(IFormatProvider provider)
        {
            return LocalizedString.Value.ToString(provider);
        }

        public override string ToString()
        {
            return LocalizedString.Value;
        }

        public object ToType(Type conversionType, IFormatProvider provider)
        {
            return ((IConvertible)LocalizedString.Value).ToType(conversionType, provider);
        }

        public ushort ToUInt16(IFormatProvider provider)
        {
            return ((IConvertible)LocalizedString.Value).ToUInt16(provider);
        }

        public uint ToUInt32(IFormatProvider provider)
        {
            return ((IConvertible)LocalizedString.Value).ToUInt32(provider);
        }

        public ulong ToUInt64(IFormatProvider provider)
        {
            return ((IConvertible)LocalizedString.Value).ToUInt64(provider);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<char>)LocalizedString.Value).GetEnumerator();
        }

        public static implicit operator string(GetOrFormatLocalizedString localizedString)
        {
            return localizedString.LocalizedString;
        }

        public static implicit operator LocalizedString(GetOrFormatLocalizedString localizedString)
        {
            return localizedString;
        }
    }
}
