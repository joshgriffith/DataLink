using System;
using System.Collections;
using System.Reflection;
using DataLink.Core.Extensions;

namespace DataLink.Core.Utilities {
    public abstract class ObjectVisitor {

        private readonly bool _visitProperties;
        private readonly bool _visitFields;
        private readonly bool _visitObjects;
        private readonly bool _visitCollections;

        protected ObjectVisitor()
            : this(true, true, true, true) {
        }

        protected ObjectVisitor(bool visitProperties, bool visitFields, bool visitObjects, bool visitCollections) {
            _visitProperties = visitProperties;
            _visitFields = visitFields;
            _visitObjects = visitObjects;
            _visitCollections = visitCollections;
        }

        public void Visit(object input) {
            if (_visitProperties || _visitCollections) {
                foreach (var property in input.GetType().GetProperties()) {
                    VisitProperty(input, property);

                    if (!property.PropertyType.IsValueTypeOrString()) {
                        if (property.PropertyType.IsEnumerable()) {
                            if (_visitCollections) {
                                if (property.GetValue(input) is IEnumerable collection) {
                                    foreach (var item in collection) {
                                        //VisitCollectionItem(collection, item);
                                        Visit(item);
                                    }
                                }
                            }
                        }
                        else if(_visitObjects) {
                            var value = property.GetValue(input);

                            if (value != null) {
                                Visit(value);
                            }
                        }
                    }
                }
            }

            if (_visitFields) {
                foreach (var field in input.GetType().GetFields())
                    VisitField(input, field);
            }
        }
        
        //protected virtual void VisitCollectionItem(IEnumerable collection, object item) {
        //}

        protected virtual void VisitProperty(object target, PropertyInfo property) {
        }

        protected virtual void VisitField(object target, FieldInfo field) {
        }
    }
}