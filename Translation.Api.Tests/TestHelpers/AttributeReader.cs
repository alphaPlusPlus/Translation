using System;
using System.Linq;

namespace Translation.Api.Tests.TestHelpers
{
    public static class AttributeReader
    {
        public static T GetClassAttribute<T>(Type type)
        {
            return (T)type.GetCustomAttributes(typeof(T), true).SingleOrDefault();
        }

        public static T GetPropertyAttribute<T>(Type type, string propertyName)
        {
            var propertyInfo = type.GetProperty(propertyName);

            if (propertyInfo == null)
            {
                throw new ArgumentException(propertyName);
            }

            return (T)propertyInfo.GetCustomAttributes(typeof(T), true).SingleOrDefault();
        }

        public static T GetMethodAttribute<T>(Type type, string methodName, Type[] methodTypes = null)
        {
            var methodInfo = methodTypes == null
                ? type.GetMethod(methodName)
                : type.GetMethod(methodName, methodTypes);

            if (methodInfo == null)
            {
                throw new ArgumentException(nameof(methodName));
            }

            return (T)methodInfo.GetCustomAttributes(typeof(T), true).SingleOrDefault();
        }

        public static bool ClassHasAttribute(Type attributeType, Type classType)
        {
            return classType.IsDefined(attributeType, true);
        }

        public static bool PropertyHasAttribute(
            Type attributeType,
            Type classType,
            string propertyName)
        {
            var propertyInfo = classType.GetProperty(propertyName);

            if (propertyInfo == null)
            {
                throw new ArgumentException(propertyName);
            }

            return propertyInfo.IsDefined(attributeType, true);
        }

        public static bool MethodHasAttribute(
            Type attributeType,
            Type classType,
            string methodName,
            Type[] methodTypes = null)
        {
            var methodInfo = methodTypes == null
                ? classType.GetMethod(methodName)
                : classType.GetMethod(methodName, methodTypes);

            if (methodInfo == null)
            {
                throw new ArgumentException(nameof(methodName));
            }

            return methodInfo.IsDefined(attributeType, true);
        }
    }
}