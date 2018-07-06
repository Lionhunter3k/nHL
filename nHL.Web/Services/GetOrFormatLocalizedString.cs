using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace nHL.Web.Services
{
    public readonly struct GetOrFormatLocalizedString : IEnumerable<char>, IEnumerable, ICloneable, IComparable, IComparable<String>, IConvertible, IEquatable<String>
    {
        private readonly Func<string, object[], string> formatter;

        public GetOrFormatLocalizedString(string value, Func<string, object[], string> formatter) : this()
        {
            Value = value;
            this.formatter = formatter;
        }

        public string this[params object[] args]
        {
            get
            {
                return this.Format(args);
            }
        }

        public string Value { get; }

        public object Clone()
        {
            return Value.Clone();
        }

        public int CompareTo(object obj)
        {
            return Value.CompareTo(obj);
        }

        public int CompareTo(string other)
        {
            return Value.CompareTo(other);
        }

        public bool Equals(string other)
        {
            return Value.Equals(other);
        }

        public override bool Equals(object obj)
        {
            return Value.Equals(obj);
        }

        public string Format(params object[] args)
        {
            return formatter(Value, args);
        }

        public IEnumerator<char> GetEnumerator()
        {
            return ((IEnumerable<char>)Value).GetEnumerator();
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public TypeCode GetTypeCode()
        {
            return Value.GetTypeCode();
        }

        public bool ToBoolean(IFormatProvider provider)
        {
            return ((IConvertible)Value).ToBoolean(provider);
        }

        public byte ToByte(IFormatProvider provider)
        {
            return ((IConvertible)Value).ToByte(provider);
        }

        public char ToChar(IFormatProvider provider)
        {
            return ((IConvertible)Value).ToChar(provider);
        }

        public DateTime ToDateTime(IFormatProvider provider)
        {
            return ((IConvertible)Value).ToDateTime(provider);
        }

        public decimal ToDecimal(IFormatProvider provider)
        {
            return ((IConvertible)Value).ToDecimal(provider);
        }

        public double ToDouble(IFormatProvider provider)
        {
            return ((IConvertible)Value).ToDouble(provider);
        }

        public short ToInt16(IFormatProvider provider)
        {
            return ((IConvertible)Value).ToInt16(provider);
        }

        public int ToInt32(IFormatProvider provider)
        {
            return ((IConvertible)Value).ToInt32(provider);
        }

        public long ToInt64(IFormatProvider provider)
        {
            return ((IConvertible)Value).ToInt64(provider);
        }

        public sbyte ToSByte(IFormatProvider provider)
        {
            return ((IConvertible)Value).ToSByte(provider);
        }

        public float ToSingle(IFormatProvider provider)
        {
            return ((IConvertible)Value).ToSingle(provider);
        }

        public string ToString(IFormatProvider provider)
        {
            return Value.ToString(provider);
        }

        public override string ToString()
        {
            return Value;
        }

        public object ToType(Type conversionType, IFormatProvider provider)
        {
            return ((IConvertible)Value).ToType(conversionType, provider);
        }

        public ushort ToUInt16(IFormatProvider provider)
        {
            return ((IConvertible)Value).ToUInt16(provider);
        }

        public uint ToUInt32(IFormatProvider provider)
        {
            return ((IConvertible)Value).ToUInt32(provider);
        }

        public ulong ToUInt64(IFormatProvider provider)
        {
            return ((IConvertible)Value).ToUInt64(provider);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<char>)Value).GetEnumerator();
        }

        public static implicit operator string(GetOrFormatLocalizedString localizedString)
        {
            return localizedString.Value;
        }
    }
}
