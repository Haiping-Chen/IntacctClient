using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Linq;
using Intacct.Infrastructure;

namespace Intacct.Entities
{
	/// <summary>
	/// A line item inside a General Ledger Transaction that increases or decreases an individual account.
	/// </summary>
	[IntacctName("glentry")]
	[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
	[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
	[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
	[SuppressMessage("ReSharper", "UnusedMember.Global")]
	[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
	public class IntacctGeneralLedgerEntry : IntacctObject
	{
		public IntacctGeneralLedgerEntryType Type { get; set; }
		public decimal Amount { get; set; }
		public string GlAccountNo { get; set; }
		public string Document { get; set; }
		public IntacctDate DateCreated { get; set; }
		public string Memo { get; set; }
		public string LocationId { get; set; }
		public string CostCenter { get; set; }
		public string CustomerId { get; set; }
		public string VendorId { get; set; }
		public string EmployeeId { get; set; }
		public string ProjectId { get; set; }
		public string ItemId { get; set; }
		public string ClassId { get; set; }
		public IntacctCustomField[] CustomFields { get; set; }
		public IntacctDate ReconDate { get; set; }
		public string Currency { get; set; }
		public IntacctDate ExchangeRateDate { get; set; }
		public string ExchangeRateType { get; set; }
		public string ExchangeRate { get; set; }

		public IntacctGeneralLedgerEntry()
		{
			// Nothing needed
		}

		// Deserialize from XML
		public IntacctGeneralLedgerEntry(XElement data)
		{
			GlAccountNo			= Serializer.DeserializeXmlToString(data, "glaccountno");
			Document			= Serializer.DeserializeXmlToString(data, "document");
			DateCreated			= Serializer.DeserializeXmlToIntacctObject<IntacctDate>(data.Element("datecreated"));
			Memo				= Serializer.DeserializeXmlToString(data, "memo");
			LocationId			= Serializer.DeserializeXmlToString(data, "locationid");
			CostCenter			= Serializer.DeserializeXmlToString(data, "departmentid");
			CustomerId			= Serializer.DeserializeXmlToString(data, "customerid");
			VendorId			= Serializer.DeserializeXmlToString(data, "vendorid");
			EmployeeId			= Serializer.DeserializeXmlToString(data, "employeeid");
			ProjectId			= Serializer.DeserializeXmlToString(data, "projectid");
			ItemId				= Serializer.DeserializeXmlToString(data, "itemid");
			ClassId				= Serializer.DeserializeXmlToString(data, "classid");
			CustomFields		= Serializer.DeserializeArrayOfChildIntacctObject<IntacctCustomField>(data.Element("customfields"));
			ReconDate			= Serializer.DeserializeXmlToIntacctObject<IntacctDate>(data.Element("recon_date"));
			Currency			= Serializer.DeserializeXmlToString(data, "currency");
			ExchangeRateType	= Serializer.DeserializeXmlToString(data, "exchratetype");
			ExchangeRate		= Serializer.DeserializeXmlToString(data, "exchrate");

			var amountString = Serializer.DeserializeXmlToString(data, "amount");
			Amount = decimal.Parse(amountString);

			var entryTypeString = Serializer.DeserializeXmlToString(data, "trtype");
			Type = string.Equals(entryTypeString, "debit")
				       ? IntacctGeneralLedgerEntryType.Debit
				       : IntacctGeneralLedgerEntryType.Credit;
		}

		internal override XObject[] ToXmlElements()
		{
			var serializedElements = new List<XObject>();
			var amountString = $"{Amount:############.00}";

			// API requires lower case
			var trtypeString = Type == IntacctGeneralLedgerEntryType.Debit ? "debit" : "credit";

			Serializer.SerializeStringToXml(trtypeString, "trtype", serializedElements);
			Serializer.SerializeStringToXml(amountString, "amount", serializedElements);
			Serializer.SerializeStringToXml(GlAccountNo, "glaccountno", serializedElements);
			Serializer.SerializeStringToXml(Document, "document", serializedElements);
			Serializer.SerializeChildIntacctObject(DateCreated, "datecreated", serializedElements);
			Serializer.SerializeStringToXml(Memo, "memo", serializedElements);
			Serializer.SerializeStringToXml(LocationId, "locationid", serializedElements);
			Serializer.SerializeStringToXml(CostCenter, "departmentid", serializedElements);
			Serializer.SerializeStringToXml(CustomerId, "customerid", serializedElements);
			Serializer.SerializeStringToXml(VendorId, "vendorid", serializedElements);
			Serializer.SerializeStringToXml(EmployeeId, "employeeid", serializedElements);
			Serializer.SerializeStringToXml(ProjectId, "projectid", serializedElements);
			Serializer.SerializeStringToXml(ItemId, "itemid", serializedElements);
			Serializer.SerializeStringToXml(ClassId, "classid", serializedElements);

			if (CustomFields?.Length > 0)
			{
				serializedElements.Add(Serializer.SerializeArrayOfChildIntacctObject(CustomFields, "customfields", "customfield"));
			}

			Serializer.SerializeChildIntacctObject(ReconDate, "recon_date", serializedElements);
			Serializer.SerializeStringToXml(Currency, "currency", serializedElements);
			Serializer.SerializeChildIntacctObject(ExchangeRateDate, "exchratedate", serializedElements);
			Serializer.SerializeStringToXml(ExchangeRateType, "exchratetype", serializedElements);
			Serializer.SerializeStringToXml(ExchangeRate, "exchrate", serializedElements);

			return serializedElements.ToArray();
		}
	}
}
