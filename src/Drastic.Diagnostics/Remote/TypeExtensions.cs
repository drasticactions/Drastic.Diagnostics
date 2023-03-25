﻿using Drastic.Diagnostics.Representations.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Drastic.Diagnostics.Remote
{
    static class TypeExtensions
    {
        public static Type? GetPublicType(this Type? objType)
        {
            if (objType == null)
            {
                return null;
            }

            if (objType.IsPublic)
                return objType;
            return GetPublicType(objType.BaseType);
        }

        /// <summary>
        /// Get a C#-compatible type name string. For example, "My.Generic+Inner`1[System.String]" becomes
        /// "My.Generic.Inner<System.String>".
        /// </summary>
        public static string GetCSharpTypeName(this Type type)
        {
            var buffer = new StringWriter();
            var writer = new CSharpWriter(buffer) { WriteLanguageKeywords = true };
            writer.VisitTypeSpec(TypeSpec.Create(type));
            return buffer.ToString();
        }

        public static void InvokeDefaultCtor(this Type type, object target)
        {
            type.GetConstructor(
                BindingFlags.NonPublic |
                    BindingFlags.Public |
                    BindingFlags.CreateInstance |
                    BindingFlags.Instance,
                null, new Type[0], null)?.Invoke(target, null);
        }
    }

    static class TypeHelper
    {
        public static string GetCSharpTypeName(string type)
        {
            var buffer = new StringWriter();
            new CSharpWriter(buffer).VisitTypeSpec(TypeSpec.Parse(type));
            return buffer.ToString();
        }
    }
}
