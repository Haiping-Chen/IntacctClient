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
	public class IntacctGeneralLedgerEntry : IntacctObject
	{
		public enum EntryType
		{
			Credit,
			Debit
		}

		public EntryType Type { get; set; }
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
			GlAccountNo			= Util.DeserializeXmlToString(data, "glaccountno");
			Document			= Util.DeserializeXmlToString(data, "document");
			DateCreated			= Util.DeserializeXmlToIntacctObject<IntacctDate>(data.Element("datecreated"));
			Memo				= Util.DeserializeXmlToString(data, "memo");
			LocationId			= Util.DeserializeXmlToString(data, "locationid");
			CostCenter			= Util.DeserializeXmlToString(data, "departmentid");
			CustomerId			= Util.DeserializeXmlToString(data, "customerid");
			VendorId			= Util.DeserializeXmlToString(data, "vendorid");
			EmployeeId			= Util.DeserializeXmlToString(data, "employeeid");
			ProjectId			= Util.DeserializeXmlToString(data, "projectid");
			ItemId				= Util.DeserializeXmlToString(data, "itemid");
			ClassId				= Util.DeserializeXmlToString(data, "classid");
			CustomFields		= Util.DeserializeArrayOfChildIntacctObject<IntacctCustomField>(data.Element("customfields"));
			ReconDate			= Util.DeserializeXmlToIntacctObject<IntacctDate>(data.Element("recon_date"));
			Currency			= Util.DeserializeXmlToString(data, "currency");
			ExchangeRateType	= Util.DeserializeXmlToString(data, "exchratetype");
			ExchangeRate		= Util.DeserializeXmlToString(data, "exchrate");

			var amountString = Util.DeserializeXmlToString(data, "amount");
			Amount = decimal.Parse(amountString);

			var entryTypeString = Util.DeserializeXmlToString(data, "trtype");
			Type = string.Equals(entryTypeString, "debit")
				       ? EntryType.Debit
				       : EntryType.Credit;
		}

		internal override XObject[] ToXmlElements()
		{
			var serializedElements = new List<XObject>();
			var amountString = $"{Amount:############.00}";

			// API requires lower case
			var trtypeString = Type == EntryType.Debit ? "debit" : "credit";

			Util.SerializeStringToXml(trtypeString, "trtype", serializedElements);
			Util.SerializeStringToXml(amountString, "amount", serializedElements);
			Util.SerializeStringToXml(GlAccountNo, "glaccountno", serializedElements);
			Util.SerializeStringToXml(Document, "document", serializedElements);
			Util.SerializeChildIntacctObject(DateCreated, "datecreated", serializedElements);
			Util.SerializeStringToXml(Memo, "memo", serializedElements);
			Util.SerializeStringToXml(LocationId, "locationid", serializedElements);
			Util.SerializeStringToXml(CostCenter, "departmentid", serializedElements);
			Util.SerializeStringToXml(CustomerId, "customerid", serializedElements);
			Util.SerializeStringToXml(VendorId, "vendorid", serializedElements);
			Util.SerializeStringToXml(EmployeeId, "employeeid", serializedElements);
			Util.SerializeStringToXml(ProjectId, "projectid", serializedElements);
			Util.SerializeStringToXml(ItemId, "itemid", serializedElements);
			Util.SerializeStringToXml(ClassId, "classid", serializedElements);
			Util.SerializeArrayOfChildIntacctObject(CustomFields, "customfields", "customfield", serializedElements);
			Util.SerializeChildIntacctObject(ReconDate, "recon_date", serializedElements);
			Util.SerializeStringToXml(Currency, "currency", serializedElements);
			Util.SerializeChildIntacctObject(ExchangeRateDate, "exchratedate", serializedElements);
			Util.SerializeStringToXml(ExchangeRateType, "exchratetype", serializedElements);
			Util.SerializeStringToXml(ExchangeRate, "exchrate", serializedElements);

			return serializedElements.ToArray();
		}
	}
}
