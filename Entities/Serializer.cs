using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Intacct.Entities
{
	internal static class Serializer
	{
		public static T[] DeserializeArrayOfChildIntacctObject<T>(XElement sourceElement) where T : IntacctObject
		{
			return sourceElement?.Elements()
			                     .Select(currChildElement => (T) Activator.CreateInstance(typeof(T), currChildElement))
			                     .ToArray();
		}

		/// <summary>
		/// Reads in the value of the specified child XML element.
		/// </summary>
		/// <param name="element">The source element that contains an element with the specified name</param>
		/// <param name="childElementName">Name of the source element within element. There must be only one with this name.</param>
		/// <returns>The value of the specified XML element child of element, or NULL if none.</returns>
		public static string DeserializeXmlToString(XElement element, string childElementName)
		{
			if (element == null) throw new ArgumentNullException(nameof(element));
			if (string.IsNullOrWhiteSpace(childElementName)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(childElementName));

			var sourceChildElement = element.Elements(XName.Get(childElementName));

			var childElement = sourceChildElement as IList<XElement> ?? sourceChildElement.ToList();
			return childElement.Count == 1 ? childElement.First().Value : null;
		}

		/// <summary>
		/// Reads in the value of the specified child XML element into an IntacctObject
		/// </summary>
		/// <param name="element">The source element that contains an element with the specified name</param>
		/// <param name="childElementName">Name of the source element within element.  There must be only one with this name.</param>
		/// <returns>The IntacctObject deserialized from the element, or NULL if the specified child element does not exist</returns>
		public static T DeserializeXmlToIntacctObject<T>(XElement element, string childElementName) where T : IntacctObject
		{
			return DeserializeXmlToIntacctObject<T>(element.Element(childElementName));
		}

		/// <summary>
		/// Reads in the value of the specified child XML element into an IntacctObject
		/// </summary>
		/// <param name="element">The source element that contains an element with the specified name</param>
		/// <returns>The IntacctObject deserialized from the element, or NULL if the specified child element does not exist</returns>
		public static T DeserializeXmlToIntacctObject<T>(XElement element) where T : IntacctObject
		{
			if (element == null) return null;

			return (T) Activator.CreateInstance(typeof(T), element);
		}

		public static XElement SerializeArrayOfChildIntacctObject(IEnumerable<IntacctObject> sourceObjects, string listElementName, string childElementName)
		{
			var listElement = new XElement(listElementName);
			foreach (var currChildIntacctObject in sourceObjects)
			{
				var currChildElement = new XElement(childElementName);
				currChildElement.Add(currChildIntacctObject.ToXmlElements());
				listElement.Add(currChildElement);
			}

			return listElement;
		}

		public static void SerializeChildIntacctObject(IntacctObject sourceObject, string elementName, List<XObject> targetElementList)
		{
			if (sourceObject == null) return;

			var newElement = new XElement(elementName);
			newElement.Add(sourceObject.ToXmlElements());

			targetElementList.Add(newElement);
		}

		public static void SerializeStringToXml(string fieldValue, string fieldName, List<XObject> targetElementList)
		{
			if (fieldValue == null) return;

			targetElementList.Add(new XElement(fieldName, fieldValue));
		}
	}
}
