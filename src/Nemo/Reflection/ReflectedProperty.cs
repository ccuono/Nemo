﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using Nemo.Attributes;
using Nemo.Attributes.Converters;
using Nemo.Fn;

namespace Nemo.Reflection
{
    public class ReflectedProperty
    {
        public ReflectedProperty(PropertyInfo property, int position = -1, bool readAttributes = true)
        {
            PropertyName = property.Name;
            PropertyType = property.PropertyType;
            IsPersistent = true;
            IsSelectable = true;
            IsSerializable = true;
            IsBinary = property.PropertyType == typeof(byte[]);
            IsSimpleList = !IsBinary && Reflector.IsSimpleList(property.PropertyType);
            IsDataEntity = Reflector.IsDataEntity(property.PropertyType);
            
            Type elementType;
            IsDataEntityList = Reflector.IsDataEntityList(property.PropertyType, out elementType);
            ElementType = elementType;
            IsPolymorphicList = IsDataEntityList && elementType != null && elementType.IsAbstract && !elementType.IsInterface;

            if (IsDataEntityList)
            {
                IsList = true;
                IsListInterface = property.PropertyType.GetGenericTypeDefinition() == typeof(IList<>);
            }
            else
            {
                IsList = !IsBinary && Reflector.IsList(property.PropertyType);
                if (IsList)
                {
                    ElementType = Reflector.GetElementType(property.PropertyType);
                    IsListInterface = property.PropertyType.GetGenericTypeDefinition() == typeof(IList<>);
                }
            }

            IsSimpleType = !IsBinary && Reflector.IsSimpleType(property.PropertyType);
            IsNullableType = Reflector.IsNullableType(property.PropertyType);
            CanWrite = property.CanWrite;
            CanRead = property.CanRead;
            Position = position;
            IsObject = IsDataEntity || (property.PropertyType.IsClass && !IsSimpleType && !IsSimpleList && !IsBinary && !IsList);
            IsObjectList = IsDataEntityList || (IsList && ElementType != null && ElementType.IsClass && !Reflector.IsSimpleType(ElementType) && !Reflector.IsSimpleList(ElementType));

            Converter = null;

            MappedColumnName = property.Name;

            if (readAttributes)
            {
                if (IsListInterface)
                {
                    Sorted = property.GetCustomAttributes(typeof(SortedAttribute), false).Cast<SortedAttribute>().FirstOrDefault();
                    Distinct = property.GetCustomAttributes(typeof(DistinctAttribute), false).Cast<DistinctAttribute>().FirstOrDefault();
                }

                MappedColumnName = MapColumnAttribute.GetMappedColumnName(property);
                MappedPropertyName = MapPropertyAttribute.GetMappedPropertyName(property);

                var typeConverter = TypeConverterAttribute.GetTypeConverter(property);
                if (typeConverter != null)
                {
                    AddConverter(typeConverter.TypeConverterType);
                }

                var items = property.GetCustomAttributes(true).OfType<PropertyAttribute>();
                foreach (var item in items)
                {
                    var primaryKeyAttribute = item as PrimaryKeyAttribute;
                    if (primaryKeyAttribute != null)
                    {
                        IsPrimaryKey = true;
                        KeyPosition = primaryKeyAttribute.Position;
                    }
                    else
                    {
                        var generategAttribute = item as Generate.UsingAttribute;
                        if (generategAttribute != null)
                        {
                            Generator = generategAttribute.Type;
                        }
                        else if (item is Generate.NativeAttribute)
                        {
                            IsAutoGenerated = true;
                        }
                        else
                        {
                            var referencesAttribute = item as ReferencesAttribute;
                            if (referencesAttribute != null)
                            {
                                Parent = referencesAttribute.Parent;
                                RefPosition = referencesAttribute.Position;
                            }
                            else
                            {
                                var parameterAttribute = item as ParameterAttribute;
                                if (parameterAttribute != null)
                                {
                                    ParameterName = parameterAttribute.Name;
                                    Direction = parameterAttribute.Direction;
                                }
                                else if (item is DoNotPersistAttribute)
                                {
                                    IsPersistent = false;
                                }
                                else if (item is DoNotSelectAttribute)
                                {
                                    IsSelectable = false;
                                }
                                else if (item is DoNotSerializeAttribute)
                                {
                                    IsSerializable = false;
                                }
                            }
                        }
                    }
                }
            }
        }

        public bool IsSimpleList { get; private set; }

        public bool IsDataEntityList { get; private set; }

        public bool IsListInterface { get; private set; }

        public bool IsDataEntity { get; private set; }

        public bool IsSimpleType { get; private set; }

        public bool IsPersistent { get; internal set; }

        public bool IsSerializable { get; internal set; }

        public bool IsPrimaryKey { get; internal set; }

        public bool IsAutoGenerated { get; internal set; }

        public Type Generator { get; internal set; }

        public Type Parent { get; internal set; }

        public bool IsSelectable { get; internal set; }

        public string ParameterName { get; internal set; }

        public ParameterDirection Direction { get; internal set; }

        public string PropertyName { get; private set; }

        public Type PropertyType { get; private set; }

        public string MappedColumnName { get; internal set; }

        public string MappedPropertyName { get; internal set; }

        public bool CanWrite { get; private set; }

        public bool CanRead { get; private set; }

        public Type ElementType { get; private set; }

        public bool IsNullableType { get; private set; }

        public bool IsList { get; private set; }

        public bool IsObject { get; private set; }

        public bool IsObjectList { get; private set; }

        public SortedAttribute Sorted { get; internal set; }

        public DistinctAttribute Distinct { get; internal set; }

        public int Position { get; private set; }

        public int KeyPosition { get; internal set; }

        public int RefPosition { get; internal set; }

        internal bool IsBinary { get; set; }

        public bool IsPolymorphicList { get; private set; }

        internal Type Converter { get; private set; }

        internal void AddConverter(Type typeConverter)
        {
            Converter = Converter == null ? typeConverter : TypeConverterAttribute.ComposeConverters(Converter, typeConverter);
        }

        public override string ToString()
        {
            return $"{MappedPropertyName} => {MappedColumnName}";
        }
    }
}
