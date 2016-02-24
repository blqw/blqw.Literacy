using System;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using blqw.Reflection;
using blqw.ReflectionComponent;

namespace blqw.Reflection
{
    class Component
    {
        public Component()
        {
            MEFPart.Import(typeof(Component));
        }

        [Import("GetConverter")]
        public static readonly IFormatterConverter Converter = new FormatterConverter();

        /// <summary> 用于数据转换的输出插件
        /// </summary>
        [Import("Convert")]
        public static readonly Func<object, Type, bool, object> Convert = 
            delegate(object value, Type type, bool throwError)
            {
                if (type == null)
                {
                    throw new ArgumentNullException("type");
                }
                var str = value as string;
                if (str == null)
                {
                    if (value == null)
                    {
                        return null;
                    }
                    try
                    {
                        return System.Convert.ChangeType(value, type);
                    }
                    catch
                    {
                        if (throwError)
                        {
                            throw;
                        }
                        return null;
                    }
                }
                if (type == typeof(Guid))
                {
                    Guid g;
                    if (Guid.TryParse(str, out g))
                    {
                        return g;
                    }
                }
                else if (type == typeof(Uri))
                {
                    Uri u;
                    if (Uri.TryCreate(str, UriKind.RelativeOrAbsolute, out u))
                    {
                        return u;
                    }
                }
                else if (type == typeof(TimeSpan))
                {
                    TimeSpan t;
                    if (TimeSpan.TryParse(str, out t))
                    {
                        return t;
                    }
                }
                else if (type == typeof(Type))
                {
                    return Type.GetType(str, false, true);
                }
                else
                {
                    try
                    {
                        return System.Convert.ChangeType(value, type);
                    }
                    catch
                    {
                        if (throwError)
                        {
                            throw;
                        }
                        return null;
                    }
                }
                if (throwError)
                {
                    throw new InvalidCastException("字符串: " + str + " 转为类型:" + Component.Convert(type, typeof(string), false) + " 失败");
                }
                return null;
            };
    }
}
