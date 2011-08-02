using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Isam.Esent.Interop;
using Microsoft.Isam.Esent.Interop.Vista;

namespace Meowth.Esentery.Querying
{
    /// <summary> All byte converters </summary>
    /// <remarks> Переписать на LCG </remarks>
    static class Converters
    {
        /// <summary> Checks type for validity in usage </summary>
        public static bool IsValidEsentType(Type type)
        {
            return s_validEsentTypes.Contains(type);
        }

        public static Type GetCLRType(JET_coltyp jct)
        {
            throw new NotImplementedException();
        }

        /// <summary> Converts value to sequence of bytes </summary>
        public static byte[] Convert<T>(T value)
        {
            return GetConverter<T>()(value);
        }
        
        /// <summary> Returns suitable converter </summary>
        public static Func<T, byte[]> GetConverter<T>()
        {
            if (s_convertersToByteArray.ContainsKey(typeof(T)))
                return x => s_convertersToByteArray[typeof(T)](x);

            throw new NotSupportedException("Dont' know how to convert this type to byte sequence");
        }

        /// <summary> All types known to be valid Esent types </summary>
        private static readonly HashSet<Type> s_validEsentTypes
           = new HashSet<Type>
                  {
                      typeof(char),
                      typeof(string),

                      typeof(int),
                      typeof(short),
                      typeof(sbyte),
                      typeof(long),
                      
                      typeof(uint),
                      typeof(ushort),
                      typeof(byte),
                      typeof(ulong),
                      
                      typeof(double),
                      typeof(float),
                      
                      typeof(bool),
                      typeof(DateTime),
                      typeof(Guid)
                  };

        private static readonly Dictionary<JET_coltyp, Type> s_convertersJetTypeToClrType
            = new Dictionary<JET_coltyp, Type>
                  {
                      {JET_coltyp.Bit, typeof(bool)},
                      {JET_coltyp.Currency, typeof(decimal)},
                      {JET_coltyp.DateTime, typeof(DateTime)},
                      {JET_coltyp.IEEEDouble, typeof(double)},
                      {JET_coltyp.IEEESingle, typeof(float)},
                      {JET_coltyp.Long, typeof(long)},
                      {JET_coltyp.LongBinary, typeof(byte[])},
                      {JET_coltyp.LongText, typeof(string)},
                      {JET_coltyp.Short, typeof(short)},
                      {JET_coltyp.Text, typeof(string)},
                      {JET_coltyp.UnsignedByte, typeof(byte)},
                      {JET_coltyp.Binary, typeof(byte[])},

                  };

        private static readonly Dictionary<Type, JET_coltyp> s_convertersClrTypeTo
            = new Dictionary<Type, JET_coltyp>
                  {
                      { typeof(bool), JET_coltyp.Bit },
                      { typeof(decimal), JET_coltyp.Currency },
                      { typeof(DateTime), JET_coltyp.DateTime },
                      { typeof(double), JET_coltyp.IEEEDouble },
                      { typeof(float), JET_coltyp.IEEESingle },
                      { typeof(long), JET_coltyp.Long },
                      //{ typeof(byte[]), JET_coltyp.LongBinary },
                      { typeof(string), JET_coltyp.LongText },
                      { typeof(short), JET_coltyp.Short },
                      { typeof(byte), JET_coltyp.UnsignedByte },
                      //{ typeof(byte[]), JET_coltyp.Binary},
                  };

        /// <summary> Table of converter functions </summary>
        private static readonly Dictionary<Type, Func<object, byte[]>> s_convertersToByteArray
            = new Dictionary<Type, Func<object, byte[]>>
                  {
                      { typeof (char), x => BitConverter.GetBytes((char) x) },
                      { typeof(string), x => ((string)x).ToCharArray().SelectMany(BitConverter.GetBytes).ToArray()},
                      { typeof(int), x => BitConverter.GetBytes((int)x) },
                      { typeof(short), x => BitConverter.GetBytes((short)x) },
                      { typeof(sbyte), x => BitConverter.GetBytes((sbyte)x) },
                      { typeof(long), x => BitConverter.GetBytes((long)x) },
                      { typeof(uint), x => BitConverter.GetBytes((uint)x) },
                      { typeof(ushort), x => BitConverter.GetBytes((ushort)x) },
                      { typeof(byte), x => BitConverter.GetBytes((byte)x) },
                      { typeof(ulong), x => BitConverter.GetBytes((ulong)x) },

                      { typeof(double), x => BitConverter.GetBytes((double)x) },
                      { typeof(float), x => BitConverter.GetBytes((float)x) },

                      { typeof(bool), x => BitConverter.GetBytes((bool)x) },
                      { typeof(DateTime), x => BitConverter.GetBytes(((DateTime)x).ToOADate()) },
                      { typeof(Guid), x => ((Guid)x).ToByteArray()}
                  };
    }
}