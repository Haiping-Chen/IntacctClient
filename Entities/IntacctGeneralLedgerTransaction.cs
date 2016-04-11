using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Intacct.Infrastructure;


namespace Intacct.Entities
{
    /// <summary>
    /// A journal transaction, a group of journal entries that accomplish a specific purpose.
    /// </summary>
    /// <remarks>
    /// NOTES:
    /// 1.  The journal ID must be a recognized ID.  Use the JournalId constants for built-in journals.
    /// 2.  The sum of the amount fields for all credit entries must equal that for all debit entries or the API will refuse to create the transaction.
    /// 3.  The XML objects used to retrieve journal entries do not match those used to create journal entries.
    /// 4.  Documentation to create journal entries is listed under GL Batch, not GL Journal or Journal.  See https://developer.intacct.com/wiki/gl-batch-development-functions.
    /// 5.  The API terminology is slightly different from some standard accounting terminology.  In accounting, a journal entry is a group of line
    ///     items that debit/credit different accounts summing to zero.  In the API, a journal entry is the individual line item and contains is
    ///     called a transaction, although it is documented under the batch functions.
    /// </remarks>
    [IntacctName("gltransaction")]
    public class IntacctGeneralLedgerTransaction : IntacctObject
    {
        #region Public Properties
        public string                           journalid               { get; set; }
        public IntacctDate                      datecreated             { get; set; }
        public string                           description             { get; set; }
        public IntacctGeneralLedgerEntry[]      gltransactionentries    { get; set; }
        #endregion #region Public Properties

        #region Public Methods
        /// <summary>
        /// Adds a debit/credit pair of GL entries for the same amount, datecreated = today, with the specified values, crediting source and debiting target
        /// </summary>
        /// <param name="newAmount"></param>
        /// <param name="newAccountNo"></param>
        /// <param name="newMemo"></param>
        /// <param name="newClassId"></param>
        /// <param name="newCostCenter"></param>
        public void AddEntryPair( decimal newAmount, string newAccountNo, string newMemo, string newSourceClassId, string newSourceCostCenter, string newTargetClassId, string newTargetCostCenter )
        {
            var newSourceEntry  = new IntacctGeneralLedgerEntry();
            var newTargetEntry  = new IntacctGeneralLedgerEntry();
            var newEntryList    = new List<IntacctGeneralLedgerEntry>();

            newSourceEntry.amount       = newAmount;
            newSourceEntry.trtype       = IntacctGeneralLedgerEntry.EntryType.Credit;
            newSourceEntry.datecreated  = new IntacctDate(DateTime.Now);
            newSourceEntry.memo         = newMemo;
            newSourceEntry.glaccountno  = newAccountNo;
            newSourceEntry.classid      = newSourceClassId;
            newSourceEntry.costcenter   = newSourceCostCenter;

            newTargetEntry.amount       = newSourceEntry.amount;
            newTargetEntry.trtype       = IntacctGeneralLedgerEntry.EntryType.Debit;
            newTargetEntry.datecreated  = newSourceEntry.datecreated;
            newTargetEntry.memo         = newSourceEntry.memo;
            newTargetEntry.glaccountno  = newSourceEntry.glaccountno;
            newTargetEntry.classid      = newTargetClassId;
            newTargetEntry.costcenter   = newTargetCostCenter;

            if ((gltransactionentries != null) && (gltransactionentries.Length > 0))
                newEntryList.AddRange(gltransactionentries);
            newEntryList.Add(newSourceEntry);
            newEntryList.Add(newTargetEntry);
            gltransactionentries = newEntryList.ToArray();
        }
        #endregion Public Methods

        #region Constructor-Dispose
        public IntacctGeneralLedgerTransaction( string newTargetJournalId, string newDescription, IntacctGeneralLedgerEntry[] newEntries )
        {
            journalid = newTargetJournalId;
            description = newDescription;
            datecreated = new IntacctDate(DateTime.Now);
            gltransactionentries = newEntries;
        }

        public IntacctGeneralLedgerTransaction( string newTargetJournalId, string newDescription, IntacctGeneralLedgerEntry[] newEntries, DateTime newCreatedDate )
        {
            journalid = newTargetJournalId;
            description = newDescription;
            datecreated = new IntacctDate(newCreatedDate);
            gltransactionentries = newEntries;
        }

        public IntacctGeneralLedgerTransaction( )
        {
            // Nothing needed
        }
        #endregion Constructor-Dispose

        #region IntacctObject
        // Deserialize from XML
        public IntacctGeneralLedgerTransaction( XElement data )
        {
            journalid               = Util.DeserializeXmlToString(data, "journalid");
            datecreated             = Util.DeserializeXmlToIntacctObject<IntacctDate>(data.Element("datecreated"));
            description             = Util.DeserializeXmlToString(data, "description");
            gltransactionentries    = Util.DeserializeArrayOfChildIntacctObject<IntacctGeneralLedgerEntry>(data.Element("gltransactionentries"));
        }

        // Serialize to XML
        internal override XObject[] ToXmlElements()
        {
            var elements = new List<XObject>();

            Util.SerializeStringToXml(journalid, "journalid", elements);
            Util.SerializeChildIntacctObject(datecreated, "datecreated", elements);
            Util.SerializeStringToXml(description, "description", elements);
            Util.SerializeArrayOfChildIntacctObject(gltransactionentries, "gltransactionentries", "glentry", elements);
            return elements.ToArray();
        }
        #endregion IntacctObject

        #region Public Constants
        public const string         JournalId_AccountsPayable                           = "APJ";
        public const string         JournalId_AccountsReceivable                        = "ARJ";
        public const string         JournalId_CashDisbursements                         = "CDJ";
        public const string         JournalId_CashReceipts                              = "CRJ";
        public const string         JournalId_EmployeeExpensesDisbursementsJournal      = "EEDJ";
        public const string         JournalId_EmployeeExpensesJournal                   = "EEJ";
        public const string         JournalId_GeneralJournal                            = "GJ";
        public const string         JournalId_InterEntityPayablesJournal                = "IEPJ";
        public const string         JournalId_InterEntityReceivablesJournal             = "IERJ";
        public const string         JournalId_InventoryJournal                          = "IJ";
        public const string         JournalId_OpeningBalanceJournal                     = "OBJ";
        public const string         JournalId_PayrollJournal                            = "PYRJ";
        public const string         JournalId_PurchaseJournal                           = "PJ";
        public const string         JournalId_SalesJournal                              = "SJ";
        public const string         JournalId_StatisticalJournal                        = "STATJ";
        #endregion Public Constants
    }

    [IntacctName("glentry")]
    public class IntacctGeneralLedgerEntry : IntacctObject
    {
        #region Public Types
        public enum EntryType
        {
            Credit,
            Debit
        }
        #endregion Public Types

        #region Public Properties
        public EntryType        trtype                  { get; set; }
        public decimal          amount                  { get; set; }
        public string           glaccountno             { get; set; }
        public string           document                { get; set; }
        public IntacctDate      datecreated             { get; set; }
        public string           memo                    { get; set; }
        public string           locationid              { get; set; }
        public string           costcenter              { get; set; }
        public string           customerid              { get; set; }
        public string           vendorid                { get; set; }
        public string           employeeid              { get; set; }
        public string           projectid               { get; set; }
        public string           itemid                  { get; set; }
        public string           classid                 { get; set; }
        public IntacctCustomField[]     customfields    { get; set; }
        public IntacctDate      recon_date              { get; set; }
        public string           currency                { get; set; }
        public IntacctDate      exchratedate            { get; set; }
        public string           exchratetype            { get; set; }
        public string           exchrate                { get; set; }
        #endregion Public Properties

        #region IntacctObject
        public IntacctGeneralLedgerEntry()
        {
            // Nothing needed
        }

        // Deserialize from XML
        public IntacctGeneralLedgerEntry( XElement data )
        {
            string          amountString;
            string          trtypeString;

            trtypeString        = Util.DeserializeXmlToString(data, "trtype");
            amountString        = Util.DeserializeXmlToString(data, "amount");
            glaccountno         = Util.DeserializeXmlToString(data, "glaccountno");
            document            = Util.DeserializeXmlToString(data, "document");
            datecreated         = Util.DeserializeXmlToIntacctObject<IntacctDate>(data.Element("datecreated"));
            memo                = Util.DeserializeXmlToString(data, "memo");
            locationid          = Util.DeserializeXmlToString(data, "locationid");
            costcenter        = Util.DeserializeXmlToString(data, "departmentid");
            customerid          = Util.DeserializeXmlToString(data, "customerid");
            vendorid            = Util.DeserializeXmlToString(data, "vendorid");
            employeeid          = Util.DeserializeXmlToString(data, "employeeid");
            projectid           = Util.DeserializeXmlToString(data, "projectid");
            itemid              = Util.DeserializeXmlToString(data, "itemid");
            classid             = Util.DeserializeXmlToString(data, "classid");
            customfields        = Util.DeserializeArrayOfChildIntacctObject<IntacctCustomField>(data.Element("customfields"));
            recon_date          = Util.DeserializeXmlToIntacctObject<IntacctDate>(data.Element("recon_date"));
            currency            = Util.DeserializeXmlToString(data, "currency");
            exchratetype        = Util.DeserializeXmlToString(data, "exchratetype");
            exchrate            = Util.DeserializeXmlToString(data, "exchrate");

            amount = decimal.Parse(amountString);
            if (string.Equals(trtypeString, "debit"))
                trtype = EntryType.Debit;
            else
                trtype = EntryType.Credit;
        }

        internal override XObject[] ToXmlElements()
        {
            var     serializedElements      = new List<XObject>();
            string  amountString            = string.Format("{0:############.00}", amount);
            string  trtypeString;

            // API requires lower case
            if (trtype == EntryType.Debit)
                trtypeString = "debit";
            else
                trtypeString = "credit";

            Util.SerializeStringToXml(trtypeString,         "trtype", serializedElements);
            Util.SerializeStringToXml(amountString,         "amount", serializedElements);
            Util.SerializeStringToXml(glaccountno,          "glaccountno", serializedElements);
            Util.SerializeStringToXml(document,             "document", serializedElements);
            Util.SerializeChildIntacctObject(datecreated,   "datecreated", serializedElements);
            Util.SerializeStringToXml(memo,                 "memo", serializedElements);
            Util.SerializeStringToXml(locationid,           "locationid", serializedElements);
            Util.SerializeStringToXml(costcenter,         "departmentid", serializedElements);
            Util.SerializeStringToXml(customerid,           "customerid", serializedElements);
            Util.SerializeStringToXml(vendorid,             "vendorid", serializedElements);
            Util.SerializeStringToXml(employeeid,           "employeeid", serializedElements);
            Util.SerializeStringToXml(projectid,            "projectid", serializedElements);
            Util.SerializeStringToXml(itemid,               "itemid", serializedElements);
            Util.SerializeStringToXml(classid,              "classid", serializedElements);
            Util.SerializeArrayOfChildIntacctObject(customfields, "customfields", "customfield", serializedElements);
            Util.SerializeChildIntacctObject(recon_date,    "recon_date", serializedElements);
            Util.SerializeStringToXml(currency,             "currency", serializedElements);
            Util.SerializeChildIntacctObject(exchratedate,  "exchratedate", serializedElements);
            Util.SerializeStringToXml(exchratetype,         "exchratetype", serializedElements);
            Util.SerializeStringToXml(exchrate,             "exchrate", serializedElements);
            return serializedElements.ToArray();
        }
        #endregion IntacctObject
    }

    [IntacctName("customfield")]
    public class IntacctCustomField : IntacctObject
    {
        #region Public Properties
        public string customfieldname           { get; set; }
        public string customfieldvalue          { get; set; }
        #endregion Public Properties

        #region IntacctObject
        public IntacctCustomField( XElement sourceData )
        {
            customfieldname = Util.DeserializeXmlToString(sourceData, "customfieldname");
            customfieldvalue = Util.DeserializeXmlToString(sourceData, "customfieldvalue");
        }

        internal override XObject[] ToXmlElements()
        {
            var serializedElements = new List<XObject>();

            Util.SerializeStringToXml("customfieldname", customfieldname, serializedElements);
            Util.SerializeStringToXml("customfieldvalue", customfieldvalue, serializedElements);
            return serializedElements.ToArray();
        }
        #endregion IntacctObject
    }
}
