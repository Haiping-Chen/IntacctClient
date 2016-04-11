using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Intacct.Infrastructure;


namespace Intacct.Entities
{
    [IntacctName("customfield")]
    public class IntacctCustomField : IntacctObject
    {
        public string CustomFieldName           { get; set; }
        public string CustomFieldValue          { get; set; }

        public IntacctCustomField(XElement sourceData)
        {
            CustomFieldName = Util.DeserializeXmlToString(sourceData, "customfieldname");
            CustomFieldValue = Util.DeserializeXmlToString(sourceData, "customfieldvalue");
        }

        internal override XObject[] ToXmlElements()
        {
            var serializedElements = new List<XObject>();

            Util.SerializeStringToXml("customfieldname", CustomFieldName, serializedElements);
            Util.SerializeStringToXml("customfieldvalue", CustomFieldValue, serializedElements);
            return serializedElements.ToArray();
        }
    }
}
