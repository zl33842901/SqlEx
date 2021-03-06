﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using xLiAd.SqlEx.Core.Core.Dialect;

namespace xLiAd.SqlEx.Core.Helper
{
    internal static class TypeHelper
    {
        public static Type FindIEnumerable(Type seqType)
        {
            if (seqType == null || seqType == typeof(string))
                return null;
            if (seqType.IsArray)
                return typeof(IEnumerable<>).MakeGenericType(seqType.GetElementType());
            if (seqType.IsGenericType)
            {
                foreach (var arg in seqType.GetGenericArguments())
                {
                    var ienum = typeof(IEnumerable<>).MakeGenericType(arg);
                    if (ienum.IsAssignableFrom(seqType))
                    {
                        return ienum;
                    }
                }
            }
            Type[] ifaces = seqType.GetInterfaces();
            if (ifaces.Length > 0)
            {
                foreach (Type iface in ifaces)
                {
                    Type ienum = FindIEnumerable(iface);
                    if (ienum != null) return ienum;
                }
            }
            if (seqType.BaseType != null && seqType.BaseType != typeof(object))
            {
                return FindIEnumerable(seqType.BaseType);
            }
            return null;
        }

        public static bool IsNullableType(Type type)
        {
            return type != null && type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        public static bool IsNullAssignable(Type type)
        {
            return !type.IsValueType || IsNullableType(type);
        }

        public static Type GetNonNullableType(Type type)
        {
            if (IsNullableType(type))
            {
                return type.GetGenericArguments()[0];
            }
            return type;
        }

        public static string GetColumnAttributeName(this PropertyInfo propertyInfo, ISqlDialect dialect = null)
        {
            var rst = propertyInfo.GetCustomAttribute<ColumnAttribute>()?.Name ?? propertyInfo.Name;
            if(dialect != null)
                rst = dialect.ParseColumnName(rst);
            if (Attribute.IsDefined(propertyInfo, typeof(System.ComponentModel.DataAnnotations.TimestampAttribute)))
            {
                return $"CONVERT(BIGINT, {rst})";
            }
            else
            {
                return rst;
            }
        }

        public static string GetTableAttributeName(this Type type, out TableAttribute att)
        {
            att = type.GetCustomAttribute<TableAttribute>();
            return att?.Name ?? type.Name;
        }
    }

}
