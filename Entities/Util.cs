using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;


namespace Intacct.Entities
{
    internal static class Util
    {
        public static T[] DeserializeArrayOfChildIntacctObject<T>( XElement sourceElement )
            where T : IntacctObject
        {
            T[] deserializedObjects = null;

            if (sourceElement != null)
            {
                var childIntacctObjectList = new List<T>();
                T newChildObject = null;
                foreach (var currChildElement in sourceElement.Elements())
                {
                    newChildObject = (T) Activator.CreateInstance(typeof(T), new object[] { currChildElement });
                    childIntacctObjectList.Add(newChildObject);
                }
                deserializedObjects = childIntacctObjectList.ToArray();
            }
            return deserializedObjects;
        }

        /// <summary>
        /// Reads in the value of the specified child XML element.
        /// </summary>
        /// <param name="sourceElement">The source element that contains an element with the specified name</param>
        /// <param name="sourceChildElementName">Name of the source element within sourceElement.  There must be only one with this name.</param>
        /// <returns>The value of the specified XML element child of sourceElement, or NULL if none.</returns>
        public static string DeserializeXmlToString( XElement sourceElement, string sourceChildElementName )
        {
            string fieldValue = null;

            var sourceChildElement = sourceElement.Elements(XName.Get(sourceChildElementName));
            if ((sourceChildElement != null) && (sourceChildElement.Count() == 1))
                fieldValue = sourceChildElement.First().Value;
            return fieldValue;
        }

        /// <summary>
        /// Reads in the value of the specified child XML element into an IntacctObject
        /// </summary>
        /// <param name="sourceElement">The source element that contains an element with the specified name</param>
        /// <param name="sourceChildElementName">Name of the source element within sourceElement.  There must be only one with this name.</param>
        /// <returns>The IntacctObject deserialized from the element, or NULL if the specified child element does not exist</returns>
        public static T DeserializeXmlToIntacctObject<T>( XElement sourceElement, string sourceChildElementName )
            where T : IntacctObject
        {
            return DeserializeXmlToIntacctObject<T>(sourceElement.Element(sourceChildElementName));
        }

        /// <summary>
        /// Reads in the value of the specified child XML element into an IntacctObject
        /// </summary>
        /// <param name="sourceElement">The source element that contains an element with the specified name</param>
        /// <param name="sourceChildElementName">Name of the source element within sourceElement.  There must be only one with this name.</param>
        /// <returns>The IntacctObject deserialized from the element, or NULL if the specified child element does not exist</returns>
        public static T DeserializeXmlToIntacctObject<T>( XElement sourceElement )
            where T : IntacctObject
        {
            T deserializedObject = null;

            if (sourceElement != null)
                deserializedObject = (T) Activator.CreateInstance(typeof(T), new object[] { sourceElement });
            return deserializedObject;
        }

        public static void SerializeArrayOfChildIntacctObject( IntacctObject[] sourceObjects, string listElementName, string childElementName, List<XObject> targetElementList )
        {
            if ((sourceObjects != null) && (sourceObjects.Length > 0))
            {
                var listElement = new XElement(listElementName);
                XElement currChildElement = null;
                foreach (var currChildIntacctObject in sourceObjects)
                {
                    currChildElement = new XElement(childElementName);
                    currChildElement.Add(currChildIntacctObject.ToXmlElements());
                    listElement.Add(currChildElement);
                }
                targetElementList.Add(listElement);
            }
        }

        public static void SerializeChildIntacctObject( IntacctObject sourceObject, string elementName, List<XObject> targetElementList )
        {
            if (sourceObject != null)
            {
                var newElement = new XElement(elementName);
                newElement.Add(sourceObject.ToXmlElements());
                targetElementList.Add(newElement);
            }
        }

        public static void SerializeStringToXml( string fieldValue, string fieldName, List<XObject> targetElementList )
        {
            if (fieldValue != null)
                targetElementList.Add(new XElement(fieldName, fieldValue));
        }
    }
}
