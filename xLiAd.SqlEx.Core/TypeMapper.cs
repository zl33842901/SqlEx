﻿using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace xLiAd.SqlEx.Core
{
    /// <summary>
    /// TypeMapper Interface
    /// </summary>
    public interface ITypeMapper
    {
        MemberInfo FindMember(MemberInfo[] properties, DbDataInfo dataInfo);
        MethodInfo FindConvertMethod(Type csharpType, Type dbType);
        DbDataInfo FindConstructorParameter(DbDataInfo[] dataInfos, ParameterInfo parameterInfo);
        ConstructorInfo FindConstructor(Type csharpType);
    }
    /// <summary>
    /// Default TypeMapper
    /// </summary>
    public class TypeMapper : ITypeMapper
    {
        /// <summary>
        /// Find parametric constructors.
        /// If there is no default constructor, the constructor with the most parameters is returned.
        /// </summary>
        public ConstructorInfo FindConstructor(Type csharpType)
        {
            var constructor = csharpType.GetConstructor(Type.EmptyTypes);
            if (constructor == null)
            {
                var constructors = csharpType.GetConstructors();
                constructor = constructors.Where(a => a.GetParameters().Length == constructors.Max(s => s.GetParameters().Length)).FirstOrDefault();
            }
            return constructor;
        }
        /// <summary>
        /// Returns field information based on parameter information
        /// </summary>
        public DbDataInfo FindConstructorParameter(DbDataInfo[] dataInfos, ParameterInfo parameterInfo)
        {
            foreach (var item in dataInfos)
            {
                if (item.DataName.Equals(parameterInfo.Name, StringComparison.OrdinalIgnoreCase))
                {
                    return item;
                }
                else if (SqlMapper.MatchNamesWithUnderscores && item.DataName.Replace("_", "").Equals(parameterInfo.Name, StringComparison.OrdinalIgnoreCase))
                {
                    return item;
                }
            }
            return null;
        }
        /// <summary>
        /// Returns attribute information based on field information
        /// </summary>
        public MemberInfo FindMember(MemberInfo[] properties, DbDataInfo dataInfo)
        {
            foreach (var item in properties)
            {
                if (item.Name.Equals(dataInfo.DataName, StringComparison.OrdinalIgnoreCase))
                {
                    return item;
                }
                else if (SqlMapper.MatchNamesWithUnderscores && item.Name.Equals(dataInfo.DataName.Replace("_", ""), StringComparison.OrdinalIgnoreCase))
                {
                    return item;
                }
            }
            return null;
        }
        /// <summary>
        /// Return type conversion function.
        /// </summary>
        public MethodInfo FindConvertMethod(Type csharpType, Type dbType)
        {
            if (GetUnderlyingType(dbType) == typeof(bool) || GetUnderlyingType(csharpType) == typeof(bool))
            {
                return !IsNullableType(csharpType) ? DataConvertMethod.ToBooleanMethod : DataConvertMethod.ToBooleanNullableMethod;
            }
            if (GetUnderlyingType(csharpType).IsEnum)
            {
                return !IsNullableType(csharpType) ? DataConvertMethod.ToEnumMethod.MakeGenericMethod(csharpType) : DataConvertMethod.ToEnumNullableMethod.MakeGenericMethod(GetUnderlyingType(csharpType));
            }
            if (GetUnderlyingType(dbType) == typeof(char) || GetUnderlyingType(csharpType) == typeof(char))
            {
                return !IsNullableType(csharpType) ? DataConvertMethod.ToCharMethod : DataConvertMethod.ToCharNullableMethod;
            }
            if (GetUnderlyingType(dbType) == typeof(string) && (csharpType == typeof(string)))
            {
                return DataConvertMethod.ToStringMethod;
            }
            if (GetUnderlyingType(dbType) == typeof(Guid) || GetUnderlyingType(csharpType) == typeof(Guid))
            {
                return !IsNullableType(csharpType) ? DataConvertMethod.ToGuidMethod : DataConvertMethod.ToGuidNullableMethod;
            }
            if (GetUnderlyingType(dbType) == typeof(DateTime) || GetUnderlyingType(csharpType) == typeof(DateTime))
            {
                return !IsNullableType(csharpType) ? DataConvertMethod.ToDateTimeMethod : DataConvertMethod.ToDateTimeNullableMethod;
            }
            if (GetUnderlyingType(dbType) == typeof(byte) || GetUnderlyingType(dbType) == typeof(sbyte) || GetUnderlyingType(csharpType) == typeof(byte) || GetUnderlyingType(csharpType) == typeof(sbyte))
            {
                return !IsNullableType(csharpType) ? DataConvertMethod.ToByteMethod : DataConvertMethod.ToByteNullableMethod;
            }
            if (GetUnderlyingType(dbType) == typeof(short) || GetUnderlyingType(dbType) == typeof(ushort) || GetUnderlyingType(csharpType) == typeof(short) || GetUnderlyingType(csharpType) == typeof(ushort))
            {
                return !IsNullableType(csharpType) ? DataConvertMethod.ToIn16Method : DataConvertMethod.ToIn16NullableMethod;
            }
            if (GetUnderlyingType(dbType) == typeof(int) || GetUnderlyingType(dbType) == typeof(uint) || GetUnderlyingType(csharpType) == typeof(int) || GetUnderlyingType(csharpType) == typeof(uint))
            {
                return !IsNullableType(csharpType) ? DataConvertMethod.ToIn32Method : DataConvertMethod.ToIn32NullableMethod;
            }
            if (GetUnderlyingType(dbType) == typeof(long) || GetUnderlyingType(dbType) == typeof(long) || GetUnderlyingType(csharpType) == typeof(long) || GetUnderlyingType(csharpType) == typeof(ulong))
            {
                return !IsNullableType(csharpType) ? DataConvertMethod.ToIn64Method : DataConvertMethod.ToIn64NullableMethod;
            }
            if (GetUnderlyingType(dbType) == typeof(float) || GetUnderlyingType(csharpType) == typeof(float))
            {
                return !IsNullableType(csharpType) ? DataConvertMethod.ToFloatMethod : DataConvertMethod.ToFloatNullableMethod;
            }
            if (GetUnderlyingType(dbType) == typeof(double) || GetUnderlyingType(csharpType) == typeof(double))
            {
                return !IsNullableType(csharpType) ? DataConvertMethod.ToDoubleMethod : DataConvertMethod.ToDoubleNullableMethod;
            }
            if (GetUnderlyingType(dbType) == typeof(decimal) || GetUnderlyingType(csharpType) == typeof(decimal))
            {
                return !IsNullableType(csharpType) ? DataConvertMethod.ToDecimalMethod : DataConvertMethod.ToDecimalNullableMethod;
            }
            return !IsNullableType(csharpType) ? DataConvertMethod.ToObjectMethod.MakeGenericMethod(csharpType) : DataConvertMethod.ToObjectNullableMethod.MakeGenericMethod(Nullable.GetUnderlyingType(GetUnderlyingType(csharpType)));
        }
        private Type GetUnderlyingType(Type type)
        {
            var underlyingType = Nullable.GetUnderlyingType(type);
            return underlyingType ?? type;
        }
        private bool IsNullableType(Type type)
        {
            return Nullable.GetUnderlyingType(type) != null;
        }
    }
    public static class DataConvertMethod
    {
        #region Method Field
        public static MethodInfo ToObjectMethod = typeof(DataConvertMethod).GetMethod(nameof(DataConvertMethod.ConvertToObject));
        public static MethodInfo ToByteMethod = typeof(DataConvertMethod).GetMethod(nameof(DataConvertMethod.ConvertToByte));
        public static MethodInfo ToIn16Method = typeof(DataConvertMethod).GetMethod(nameof(DataConvertMethod.ConvertToInt16));
        public static MethodInfo ToIn32Method = typeof(DataConvertMethod).GetMethod(nameof(DataConvertMethod.ConvertToInt32));
        public static MethodInfo ToIn64Method = typeof(DataConvertMethod).GetMethod(nameof(DataConvertMethod.ConvertToInt64));
        public static MethodInfo ToFloatMethod = typeof(DataConvertMethod).GetMethod(nameof(DataConvertMethod.ConvertToFloat));
        public static MethodInfo ToDoubleMethod = typeof(DataConvertMethod).GetMethod(nameof(DataConvertMethod.ConvertToDouble));
        public static MethodInfo ToDecimalMethod = typeof(DataConvertMethod).GetMethod(nameof(DataConvertMethod.ConvertToDecimal));
        public static MethodInfo ToBooleanMethod = typeof(DataConvertMethod).GetMethod(nameof(DataConvertMethod.ConvertToBoolean));
        public static MethodInfo ToCharMethod = typeof(DataConvertMethod).GetMethod(nameof(DataConvertMethod.ConvertToChar));
        public static MethodInfo ToStringMethod = typeof(DataConvertMethod).GetMethod(nameof(DataConvertMethod.ConvertToString));
        public static MethodInfo ToDateTimeMethod = typeof(DataConvertMethod).GetMethod(nameof(DataConvertMethod.ConvertToDateTime));
        public static MethodInfo ToEnumMethod = typeof(DataConvertMethod).GetMethod(nameof(DataConvertMethod.ConvertToEnum));
        public static MethodInfo ToGuidMethod = typeof(DataConvertMethod).GetMethod(nameof(DataConvertMethod.ConvertToGuid));
        #endregion

        #region NullableMethod Field
        public static MethodInfo ToObjectNullableMethod = typeof(DataConvertMethod).GetMethod(nameof(DataConvertMethod.ConvertObjectNullable));
        public static MethodInfo ToByteNullableMethod = typeof(DataConvertMethod).GetMethod(nameof(DataConvertMethod.ConvertToInt16Nullable));
        public static MethodInfo ToIn16NullableMethod = typeof(DataConvertMethod).GetMethod(nameof(DataConvertMethod.ConvertToInt16Nullable));
        public static MethodInfo ToIn32NullableMethod = typeof(DataConvertMethod).GetMethod(nameof(DataConvertMethod.ConvertToInt32Nullable));
        public static MethodInfo ToIn64NullableMethod = typeof(DataConvertMethod).GetMethod(nameof(DataConvertMethod.ConvertToInt64Nullable));
        public static MethodInfo ToFloatNullableMethod = typeof(DataConvertMethod).GetMethod(nameof(DataConvertMethod.ConvertToFloatNullable));
        public static MethodInfo ToDoubleNullableMethod = typeof(DataConvertMethod).GetMethod(nameof(DataConvertMethod.ConvertToDoubleNullable));
        public static MethodInfo ToBooleanNullableMethod = typeof(DataConvertMethod).GetMethod(nameof(DataConvertMethod.ConvertToBooleanNullable));
        public static MethodInfo ToDecimalNullableMethod = typeof(DataConvertMethod).GetMethod(nameof(DataConvertMethod.ConvertToDecimalNullable));
        public static MethodInfo ToCharNullableMethod = typeof(DataConvertMethod).GetMethod(nameof(DataConvertMethod.ConvertToCharNullable));
        public static MethodInfo ToDateTimeNullableMethod = typeof(DataConvertMethod).GetMethod(nameof(DataConvertMethod.ConvertToDateTimeNullable));
        public static MethodInfo ToEnumNullableMethod = typeof(DataConvertMethod).GetMethod(nameof(DataConvertMethod.ConvertToEnumNullable));
        public static MethodInfo ToGuidNullableMethod = typeof(DataConvertMethod).GetMethod(nameof(DataConvertMethod.ConvertToGuidNullable));
        #endregion

        #region Define Convert
        public static T ConvertToObject<T>(IDataRecord dr, int i)
        {
            if (dr.IsDBNull(i))
            {
                return default;
            }
            var data = dr.GetValue(i);
            return (T)Convert.ChangeType(data, typeof(T));
        }
        public static byte ConvertToByte(IDataRecord dr, int i)
        {
            if (dr.IsDBNull(i))
            {
                return default;
            }
            var result = dr.GetByte(i);
            return result;
        }
        public static short ConvertToInt16(IDataRecord dr, int i)
        {
            if (dr.IsDBNull(i))
            {
                return default;
            }
            var result = dr.GetInt16(i);
            return result;
        }
        public static int ConvertToInt32(IDataRecord dr, int i)
        {
            if (dr.IsDBNull(i))
            {
                return default;
            }
            return dr.GetInt32(i);
        }
        public static long ConvertToInt64(IDataRecord dr, int i)
        {
            if (dr.IsDBNull(i))
            {
                return default;
            }
            return dr.GetInt64(i);
        }
        public static float ConvertToFloat(IDataRecord dr, int i)
        {
            if (dr.IsDBNull(i))
            {
                return default;
            }
            var result = dr.GetFloat(i);
            return result;
        }
        public static double ConvertToDouble(IDataRecord dr, int i)
        {
            if (dr.IsDBNull(i))
            {
                return default;
            }
            var result = dr.GetDouble(i);
            return result;
        }
        public static bool ConvertToBoolean(IDataRecord dr, int i)
        {
            if (dr.IsDBNull(i))
            {
                return default;
            }
            if (dr.GetFieldType(i) == typeof(bool))
            {
                var result = dr.GetBoolean(i);
                return result;
            }
            else
            {
                var result = dr.GetValue(i);
                return Convert.ToBoolean(result);
            }
        }
        public static decimal ConvertToDecimal(IDataRecord dr, int i)
        {
            if (dr.IsDBNull(i))
            {
                return default;
            }
            var result = dr.GetDecimal(i);
            return result;
        }
        public static char ConvertToChar(this IDataRecord dr, int i)
        {
            if (dr.IsDBNull(i))
            {
                return default;
            }
            var result = dr.GetChar(i);
            return result;
        }
        public static string ConvertToString(IDataRecord dr, int i)
        {
            if (dr.IsDBNull(i))
            {
                return default;
            }
            var result = dr.GetString(i);
            return result;
        }
        public static DateTime ConvertToDateTime(IDataRecord dr, int i)
        {
            if (dr.IsDBNull(i))
            {
                return default;
            }
            var result = dr.GetDateTime(i);
            return result;
        }
        public static T ConvertToEnum<T>(IDataRecord dr, int i) where T : struct
        {
            if (dr.IsDBNull(i))
            {
                return default;
            }
            var value = dr.GetValue(i);
            if (Enum.TryParse(value.ToString(), out T result)) return result;
            return default;
        }
        public static Guid ConvertToGuid(IDataRecord dr, int i)
        {
            if (dr.IsDBNull(i))
            {
                return default;
            }
            var result = dr.GetGuid(i);
            return result;
        }
        #endregion

        #region Define Nullable Convert
        public static T ConvertObjectNullable<T>(IDataRecord dr, int i)
        {
            if (dr.IsDBNull(i))
            {
                return default;
            }
            var data = dr.GetValue(i);
            return (T)Convert.ChangeType(data, typeof(T));
        }
        public static byte? ConvertToByteNullable(IDataRecord dr, int i)
        {
            if (dr.IsDBNull(i))
            {
                return default;
            }
            var result = dr.GetByte(i);
            return result;
        }
        public static short? ConvertToInt16Nullable(IDataRecord dr, int i)
        {
            if (dr.IsDBNull(i))
            {
                return default;
            }
            var result = dr.GetInt16(i);
            return result;
        }
        public static int? ConvertToInt32Nullable(IDataRecord dr, int i)
        {
            if (dr.IsDBNull(i))
            {
                return default;
            }
            var result = dr.GetInt32(i);
            return result;
        }
        public static long? ConvertToInt64Nullable(IDataRecord dr, int i)
        {
            if (dr.IsDBNull(i))
            {
                return default;
            }
            var result = dr.GetInt64(i);
            return result;
        }
        public static float? ConvertToFloatNullable(IDataRecord dr, int i)
        {
            if (dr.IsDBNull(i))
            {
                return default;
            }
            var result = dr.GetFloat(i);
            return result;
        }
        public static double? ConvertToDoubleNullable(IDataRecord dr, int i)
        {
            if (dr.IsDBNull(i))
            {
                return default;
            }
            var result = dr.GetDouble(i);
            return result;
        }
        public static bool? ConvertToBooleanNullable(IDataRecord dr, int i)
        {
            if (dr.IsDBNull(i))
            {
                return default;
            }
            if (dr.GetFieldType(i) == typeof(bool))
            {
                var result = dr.GetBoolean(i);
                return result;
            }
            else
            {
                var result = dr.GetValue(i);
                return Convert.ToBoolean(result);
            }
        }
        public static decimal? ConvertToDecimalNullable(IDataRecord dr, int i)
        {
            if (dr.IsDBNull(i))
            {
                return default;
            }
            var result = dr.GetDecimal(i);
            return result;
        }
        public static char? ConvertToCharNullable(IDataRecord dr, int i)
        {
            if (dr.IsDBNull(i))
            {
                return default;
            }
            var result = dr.GetChar(i);
            return result;
        }
        public static DateTime? ConvertToDateTimeNullable(IDataRecord dr, int i)
        {
            if (dr.IsDBNull(i))
            {
                return default;
            }
            var result = dr.GetDateTime(i);
            return result;
        }
        public static T? ConvertToEnumNullable<T>(IDataRecord dr, int i) where T : struct
        {
            if (dr.IsDBNull(i))
            {
                return default;
            }
            var value = dr.GetValue(i);
            if (Enum.TryParse(value.ToString(), out T result)) return result;
            return default;
        }
        public static Guid? ConvertToGuidNullable(IDataRecord dr, int i)
        {
            if (dr.IsDBNull(i))
            {
                return default;
            }
            var result = dr.GetGuid(i);
            return result;
        }
        #endregion
    }
}
