using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Linq;
using Intacct.Infrastructure;

namespace Intacct.Entities
{
	[IntacctName("customfield")]
	[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
	[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
	[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
	public class IntacctCustomField : IntacctObject
	{
		public string Name { get; set; }
		public string Value { get; set; }

		public IntacctCustomField(XElement sourceData)
		{
			Name = Serializer.DeserializeXmlToString(sourceData, "customfieldname");
			Value = Serializer.DeserializeXmlToString(sourceData, "customfieldvalue");
		}

		internal override XObject[] ToXmlElements()
		{
			var serializedElements = new List<XObject>();

			Serializer.SerializeStringToXml("customfieldname", Name, serializedElements);
			Serializer.SerializeStringToXml("customfieldvalue", Value, serializedElements);

			return serializedElements.ToArray();
		}
	}
}
