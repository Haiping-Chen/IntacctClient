using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Intacct.Entities
{
	internal static class Util
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
		/// <param name="sourceElement">The source element that contains an element with the specified name</param>
		/// <param name="sourceChildElementName">Name of the source element within sourceElement.  There must be only one with this name.</param>
		/// <returns>The value of the specified XML element child of sourceElement, or NULL if none.</returns>
		public static string DeserializeXmlToString(XElement sourceElement, string sourceChildElementName)
		{
			if (sourceElement == null) throw new ArgumentNullException(nameof(sourceElement));
			if (string.IsNullOrWhiteSpace(sourceChildElementName)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(sourceChildElementName));

			var sourceChildElement = sourceElement.Elements(XName.Get(sourceChildElementName));

			var childElement = sourceChildElement as IList<XElement> ?? sourceChildElement.ToList();
			return childElement.Count == 1 ? childElement.First().Value : null;
		}

		/// <summary>
		/// Reads in the value of the specified child XML element into an IntacctObject
		/// </summary>
		/// <param name="sourceElement">The source element that contains an element with the specified name</param>
		/// <param name="sourceChildElementName">Name of the source element within sourceElement.  There must be only one with this name.</param>
		/// <returns>The IntacctObject deserialized from the element, or NULL if the specified child element does not exist</returns>
		public static T DeserializeXmlToIntacctObject<T>(XElement sourceElement, string sourceChildElementName) where T : IntacctObject
		{
			return DeserializeXmlToIntacctObject<T>(sourceElement.Element(sourceChildElementName));
		}

		/// <summary>
		/// Reads in the value of the specified child XML element into an IntacctObject
		/// </summary>
		/// <param name="sourceElement">The source element that contains an element with the specified name</param>
		/// <returns>The IntacctObject deserialized from the element, or NULL if the specified child element does not exist</returns>
		public static T DeserializeXmlToIntacctObject<T>(XElement sourceElement) where T : IntacctObject
		{
			if (sourceElement == null) return null;

			return (T) Activator.CreateInstance(typeof(T), sourceElement);
		}

		public static void SerializeArrayOfChildIntacctObject(IntacctObject[] sourceObjects, string listElementName, string childElementName, List<XObject> targetElementList)
		{
			if ((sourceObjects == null) || (sourceObjects.Length <= 0)) return;

			var listElement = new XElement(listElementName);
			foreach (var currChildIntacctObject in sourceObjects)
			{
				var currChildElement = new XElement(childElementName);
				currChildElement.Add(currChildIntacctObject.ToXmlElements());
				listElement.Add(currChildElement);
			}

			targetElementList.Add(listElement);
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
