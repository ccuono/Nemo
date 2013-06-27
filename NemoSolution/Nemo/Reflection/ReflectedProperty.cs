﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using Nemo.Attributes;
using Nemo.Fn;

namespace Nemo.Reflection
{
    public class ReflectedProperty
    {
        public ReflectedProperty(PropertyInfo property, int position = -1, bool readAttributes = true)
        {
            PropertyName = property.Name;
            PropertyType = property.PropertyType;
            IsPersistent = Maybe<bool>.Empty;
            IsSelectable = Maybe<bool>.Empty;
            IsSerializable = Maybe<bool>.Empty;
            IsSimpleList = Reflector.IsSimpleList(property.PropertyType);
            IsBusinessObject = Reflector.IsBusinessObject(property.PropertyType);
            Type elementType;
            IsBusinessObjectList = Reflector.IsBusinessObjectList(property.PropertyType, out elementType);
            ElementType = elementType;
            if (IsBusinessObjectList)
            {
                IsList = true;
                IsListInterface = property.PropertyType.GetGenericTypeDefinition() == typeof(IList<>);
            }
            else
            {
                IsList = Reflector.IsList(property.PropertyType);
                if (IsList)
                {
                    ElementType = Reflector.ExtractCollectionElementType(property.PropertyType);
                    IsListInterface = property.PropertyType.GetGenericTypeDefinition() == typeof(IList<>);
                }
            }
            IsSimpleType = Reflector.IsSimpleType(property.PropertyType);
            IsTypeUnion = Reflector.IsTypeUnion(property.PropertyType);
            IsTuple = Reflector.IsTuple(property.PropertyType);
            IsNullableType = Reflector.IsNullableType(property.PropertyType);
            CanWrite = property.CanWrite;
            CanRead = property.CanRead;
            Position = position;
            
            if (readAttributes)
            {
                if (IsListInterface)
                {
                    Sorted = property.GetCustomAttributes(typeof(SortedAttribute), false).Cast<SortedAttribute>().FirstOrDefault();
                    Distinct = property.GetCustomAttributes(typeof(DistinctAttribute), false).Cast<DistinctAttribute>().FirstOrDefault();
                }
                
                MappedColumnName = MapColumnAttribute.GetMappedColumnName(property);
                MappedPropertyName = MapPropertyAttribute.GetMappedPropertyName(property);
                
                var items = property.GetCustomAttributes(true).OfType<PropertyAttribute>();
                foreach (var item in items)
                {
                    if (item is PrimaryKeyAttribute)
                    {
                        IsPrimaryKey = true;
                        KeyPosition = ((PrimaryKeyAttribute)item).Position;
                    }
                    else if (item is Generate.UsingAttribute)
                    {
                        Generator = ((Generate.UsingAttribute)item).Type;
                    }
                    else if (item is Generate.NativeAttribute)
                    {
                        IsAutoGenerated = true;
                    }
                    else if (item is ReferencesAttribute)
                    {
                        Parent = ((ReferencesAttribute)item).Parent;
                        RefPosition = ((ReferencesAttribute)item).Position;
                    }
                    else if (item is CacheKeyAttribute)
                    {
                        IsCacheKey = true;
                    }
                    else if (item is ParameterAttribute)
                    {
                        ParameterName = ((ParameterAttribute)item).Name;
                        Direction = ((ParameterAttribute)item).Direction;
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

            if (!IsPersistent.HasValue)
            {
                IsPersistent = true;
            }

            if (!IsSelectable.HasValue)
            {
                IsSelectable = true;
            }

            if (!IsSerializable.HasValue)
            {
                IsSerializable = true;
            }
        }

        public bool IsSimpleList
        {
            get;
            private set;
        }

        public bool IsBusinessObjectList
        {
            get;
            private set;
        }

        public bool IsListInterface
        {
            get;
            private set;
        }

        public bool IsBusinessObject
        {
            get;
            private set;
        }

        public bool IsSimpleType
        {
            get;
            private set;
        }

        public bool IsTypeUnion
        {
            get;
            private set;
        }

        public bool IsTuple
        {
            get;
            private set;
        }

        public Maybe<bool> IsPersistent
        {
            get;
            internal set;
        }

        public Maybe<bool> IsSerializable
        {
            get;
            internal set;
        }

        public bool IsPrimaryKey
        {
            get;
            internal set;
        }

        public bool IsAutoGenerated
        {
            get;
            internal set;
        }

        public Type Generator
        {
            get;
            internal set;
        }

        public Type Parent
        {
            get;
            internal set;
        }

        public Maybe<bool> IsSelectable
        {
            get;
            internal set;
        }
        
        public bool IsCacheKey
        {
            get;
            internal set;
        }

        public string ParameterName
        {
            get;
            internal set;
        }

        public ParameterDirection Direction
        {
            get;
            internal set;
        }

        public string PropertyName
        {
            get;
            private set;
        }

        public Type PropertyType
        {
            get;
            private set;
        }

        public string MappedColumnName
        {
            get;
            internal set;
        }

        public string MappedPropertyName
        {
            get;
            internal set;
        }

        public bool CanWrite
        {
            get;
            private set;
        }

        public bool CanRead
        {
            get;
            private set;
        }

        public Type ElementType
        {
            get;
            private set;
        }

        public bool IsNullableType
        {
            get;
            private set;
        }

        public bool IsList
        {
            get;
            private set;
        }

        public SortedAttribute Sorted
        {
            get;
            internal set;
        }

        public DistinctAttribute Distinct
        {
            get;
            internal set;
        }

        public int Position
        {
            get;
            private set;
        }

        internal int KeyPosition
        {
            get;
            set;
        }

        internal int RefPosition
        {
            get;
            set;
        }
    }
}
