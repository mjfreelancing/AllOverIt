using System.Reflection;

namespace AllOverIt.Extensions
{
    /// <summary>Provides a variety of extension methods for <see cref="TypeInfo"/> types.</summary>
    public static class TypeInfoExtensions
    {
        /// <summary>Gets all <see cref="PropertyInfo"/> (property metadata) for a given <see cref="TypeInfo"/>.</summary>
        /// <param name="typeInfo">The <see cref="TypeInfo"/> to obtain all property metadata.</param>
        /// <param name="declaredOnly">If true, the metadata of properties in the declared class as well as base class(es) are returned
        /// (if a property is overriden then only the base class <see cref="PropertyInfo"/> is returned).
        /// If false, only property metadata of the declared type is returned.</param>
        /// <returns>The property metadata, as <see cref="PropertyInfo"/>, of a provided <see cref="TypeInfo"/>.</returns>
        /// <remarks>When class inheritance is involved, this method returns the first property found, starting at the type represented
        /// by <paramref name="typeInfo"/>.</remarks>
        public static IEnumerable<PropertyInfo> GetPropertyInfo(this TypeInfo typeInfo, bool declaredOnly = false)
        {
            var propInfoList = new List<PropertyInfo>();

            GetPropertyInfo(typeInfo, declaredOnly, propInfoList);

            return propInfoList;
        }

        /// <summary>Gets the <see cref="PropertyInfo"/> (property metadata) for a given public or protected property on a <see cref="TypeInfo"/>.</summary>
        /// <param name="typeInfo">The <see cref="TypeInfo"/> to obtain the property metadata from.</param>
        /// <param name="propertyName">The name of the property to obtain metadata for.</param>
        /// <returns>The property metadata, as <see cref="PropertyInfo"/>, of a specified property on the provided <paramref name="typeInfo"/>.</returns>
        /// <remarks>When class inheritance is involved, this method returns the first property found, starting at the type represented
        /// by <paramref name="typeInfo"/>. If the property is overriden, this means the base class <see cref="PropertyInfo"/> will not be
        /// returned. If you require the base class <see cref="PropertyInfo"/> then use the <see cref="GetPropertyInfo(TypeInfo,bool)"/>
        /// method.</remarks>
        public static PropertyInfo GetPropertyInfo(this TypeInfo typeInfo, string propertyName)
        {
            var propertyInfo = typeInfo.GetDeclaredProperty(propertyName);

            if (propertyInfo == null && typeInfo.BaseType != null)
            {
                var baseTypeInfo = typeInfo.BaseType.GetTypeInfo();
                propertyInfo = GetPropertyInfo(baseTypeInfo, propertyName);
            }

            return propertyInfo;
        }

        /// <summary>Gets all <see cref="FieldInfo"/> (field metadata) for a given <see cref="TypeInfo"/>.</summary>
        /// <param name="typeInfo">The <see cref="TypeInfo"/> to obtain all field metadata.</param>
        /// <param name="declaredOnly">If true, the metadata of properties in the declared class as well as base class(es) are returned
        /// (if a field is overriden then only the base class <see cref="FieldInfo"/> is returned).
        /// If false, only field metadata of the declared type is returned.</param>
        /// <returns>The field metadata, as <see cref="FieldInfo"/>, of a provided <see cref="TypeInfo"/>.</returns>
        /// <remarks>When class inheritance is involved, this method returns the first field found, starting at the type represented
        /// by <paramref name="typeInfo"/>.</remarks>
        public static IEnumerable<FieldInfo> GetFieldInfo(this TypeInfo typeInfo, bool declaredOnly = false)
        {
            var fieldInfoList = new List<FieldInfo>();

            GetFieldInfo(typeInfo, declaredOnly, fieldInfoList);

            return fieldInfoList;
        }

        /// <summary>Gets the <see cref="FieldInfo"/> (field metadata) for a given public or protected field on a <see cref="TypeInfo"/>.</summary>
        /// <param name="typeInfo">The <see cref="TypeInfo"/> to obtain the field metadata from.</param>
        /// <param name="fieldName">The name of the field to obtain metadata for.</param>
        /// <returns>The field metadata, as <see cref="FieldInfo"/>, of a specified field on the provided <paramref name="typeInfo"/>.</returns>
        /// <remarks>When class inheritance is involved, this method returns the first field found, starting at the type represented
        /// by <paramref name="typeInfo"/>. If the field is overriden, this means the base class <see cref="FieldInfo"/> will not be
        /// returned. If you require the base class <see cref="FieldInfo"/> then use the <see cref="GetFieldInfo(TypeInfo,bool)"/>
        /// method.</remarks>
        public static FieldInfo GetFieldInfo(this TypeInfo typeInfo, string fieldName)
        {
            var propertyInfo = typeInfo.GetDeclaredField(fieldName);

            if (propertyInfo == null && typeInfo.BaseType != null)
            {
                var baseTypeInfo = typeInfo.BaseType.GetTypeInfo();
                propertyInfo = GetFieldInfo(baseTypeInfo, fieldName);
            }

            return propertyInfo;
        }

        private static void GetPropertyInfo(TypeInfo typeInfo, bool declaredOnly, ICollection<PropertyInfo> propInfoList)
        {
            if (!declaredOnly && typeInfo.BaseType != null)
            {
                var baseTypeInfo = typeInfo.BaseType.GetTypeInfo();
                GetPropertyInfo(baseTypeInfo, false, propInfoList);
            }

            foreach (var declaredProperty in typeInfo.DeclaredProperties)
            {
                if (propInfoList.All(prop => prop.Name != declaredProperty.Name))
                {
                    propInfoList.Add(declaredProperty);
                }
            }
        }

        private static void GetFieldInfo(TypeInfo typeInfo, bool declaredOnly, ICollection<FieldInfo> fieldInfoList)
        {
            if (!declaredOnly && typeInfo.BaseType != null)
            {
                var baseTypeInfo = typeInfo.BaseType.GetTypeInfo();
                GetFieldInfo(baseTypeInfo, false, fieldInfoList);
            }

            foreach (var declaredField in typeInfo.DeclaredFields)
            {
                if (fieldInfoList.All(field => field.Name != declaredField.Name))
                {
                    fieldInfoList.Add(declaredField);
                }
            }
        }
    }
}