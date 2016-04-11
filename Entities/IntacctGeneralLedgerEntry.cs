using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Intacct.Infrastructure;


namespace Intacct.Entities
{
    /// <summary>
    /// A line item inside a General Ledger Transaction that increases or decreases an individual account.
    /// </summary>
    [IntacctName("glentry")]
    public class IntacctGeneralLedgerEntry : IntacctObject
    {
        public enum EntryType
        {
            Credit,
            Debit
        }

        public EntryType        TrType                  { get; set; }
        public decimal          Amount                  { get; set; }
        public string           GlAccountNo             { get; set; }
        public string           Document                { get; set; }
        public IntacctDate      DateCreated             { get; set; }
        public string           Memo                    { get; set; }
        public string           LocationId              { get; set; }
        public string           CostCenter              { get; set; }
        public string           CustomerId              { get; set; }
        public string           VendorId                { get; set; }
        public string           EmployeeId              { get; set; }
        public string           ProjectId               { get; set; }
        public string           ItemId                  { get; set; }
        public string           ClassId                 { get; set; }
        public IntacctCustomField[]     CustomFields    { get; set; }
        public IntacctDate      ReconDate               { get; set; }
        public string           Currency                { get; set; }
        public IntacctDate      ExchRateDate            { get; set; }
        public string           ExchRateType            { get; set; }
        public string           ExchRate                { get; set; }

        public IntacctGeneralLedgerEntry()
        {
            // Nothing needed
        }

        // Deserialize from XML
        public IntacctGeneralLedgerEntry(XElement data)
        {
            string          amountString;
            string          trtypeString;

            trtypeString        = Util.DeserializeXmlToString(data, "trtype");
            amountString        = Util.DeserializeXmlToString(data, "amount");
            GlAccountNo         = Util.DeserializeXmlToString(data, "glaccountno");
            Document            = Util.DeserializeXmlToString(data, "document");
            DateCreated         = Util.DeserializeXmlToIntacctObject<IntacctDate>(data.Element("datecreated"));
            Memo                = Util.DeserializeXmlToString(data, "memo");
            LocationId          = Util.DeserializeXmlToString(data, "locationid");
            CostCenter          = Util.DeserializeXmlToString(data, "departmentid");
            CustomerId          = Util.DeserializeXmlToString(data, "customerid");
            VendorId            = Util.DeserializeXmlToString(data, "vendorid");
            EmployeeId          = Util.DeserializeXmlToString(data, "employeeid");
            ProjectId           = Util.DeserializeXmlToString(data, "projectid");
            ItemId              = Util.DeserializeXmlToString(data, "itemid");
            ClassId             = Util.DeserializeXmlToString(data, "classid");
            CustomFields        = Util.DeserializeArrayOfChildIntacctObject<IntacctCustomField>(data.Element("customfields"));
            ReconDate           = Util.DeserializeXmlToIntacctObject<IntacctDate>(data.Element("recon_date"));
            Currency            = Util.DeserializeXmlToString(data, "currency");
            ExchRateType        = Util.DeserializeXmlToString(data, "exchratetype");
            ExchRate            = Util.DeserializeXmlToString(data, "exchrate");

            Amount = decimal.Parse(amountString);
            if (string.Equals(trtypeString, "debit"))
                TrType = EntryType.Debit;
            else
                TrType = EntryType.Credit;
        }

        internal override XObject[] ToXmlElements()
        {
            var     serializedElements      = new List<XObject>();
            string  amountString            = string.Format("{0:############.00}", Amount);
            string  trtypeString;

            // API requires lower case
            if (TrType == EntryType.Debit)
                trtypeString = "debit";
            else
                trtypeString = "credit";

            Util.SerializeStringToXml(trtypeString,         "trtype", serializedElements);
            Util.SerializeStringToXml(amountString,         "amount", serializedElements);
            Util.SerializeStringToXml(GlAccountNo,          "glaccountno", serializedElements);
            Util.SerializeStringToXml(Document,             "document", serializedElements);
            Util.SerializeChildIntacctObject(DateCreated,   "datecreated", serializedElements);
            Util.SerializeStringToXml(Memo,                 "memo", serializedElements);
            Util.SerializeStringToXml(LocationId,           "locationid", serializedElements);
            Util.SerializeStringToXml(CostCenter,           "departmentid", serializedElements);
            Util.SerializeStringToXml(CustomerId,           "customerid", serializedElements);
            Util.SerializeStringToXml(VendorId,             "vendorid", serializedElements);
            Util.SerializeStringToXml(EmployeeId,           "employeeid", serializedElements);
            Util.SerializeStringToXml(ProjectId,            "projectid", serializedElements);
            Util.SerializeStringToXml(ItemId,               "itemid", serializedElements);
            Util.SerializeStringToXml(ClassId,              "classid", serializedElements);
            Util.SerializeArrayOfChildIntacctObject(CustomFields, "customfields", "customfield", serializedElements);
            Util.SerializeChildIntacctObject(ReconDate,     "recon_date", serializedElements);
            Util.SerializeStringToXml(Currency,             "currency", serializedElements);
            Util.SerializeChildIntacctObject(ExchRateDate,  "exchratedate", serializedElements);
            Util.SerializeStringToXml(ExchRateType,         "exchratetype", serializedElements);
            Util.SerializeStringToXml(ExchRate,             "exchrate", serializedElements);
            return serializedElements.ToArray();
        }
    }
}
