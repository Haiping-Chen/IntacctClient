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
        /// <summary>
        /// The ID of the journal to post this to.  See the JournalId class for a list of the built-in journals.
        /// </summary>
        public string                           JournalId               { get; set; }
        public IntacctDate                      DateCreated             { get; set; }
        public string                           Description             { get; set; }
        public IntacctGeneralLedgerEntry[]      GlTransactionEntries    { get; set; }

        /// <summary>
        /// Adds a debit/credit pair of GL entries for the same amount, datecreated = today, with the specified values, crediting source and debiting target
        /// </summary>
        /// <param name="newEntryAmount"></param>
        /// <param name="newEntryAccountNo"></param>
        /// <param name="newEntryMemo"></param>
        /// <param name="newClassId"></param>
        /// <param name="newCostCenter"></param>
        public void AddEntryPair(decimal newEntryAmount, string newEntryAccountNo, string newEntryMemo, string newEntrySourceClassId, string newEntrySourceCostCenter, string newEntryTargetClassId, string newEntryTargetCostCenter)
        {
            var newSourceEntry  = new IntacctGeneralLedgerEntry();
            var newTargetEntry  = new IntacctGeneralLedgerEntry();
            var newEntryList    = new List<IntacctGeneralLedgerEntry>();

            newSourceEntry.Amount       = newEntryAmount;
            newSourceEntry.TrType       = IntacctGeneralLedgerEntry.EntryType.Credit;
            newSourceEntry.DateCreated  = new IntacctDate(DateTime.Now);
            newSourceEntry.Memo         = newEntryMemo;
            newSourceEntry.GlAccountNo  = newEntryAccountNo;
            newSourceEntry.ClassId      = newEntrySourceClassId;
            newSourceEntry.CostCenter   = newEntrySourceCostCenter;

            newTargetEntry.Amount       = newSourceEntry.Amount;
            newTargetEntry.TrType       = IntacctGeneralLedgerEntry.EntryType.Debit;
            newTargetEntry.DateCreated  = newSourceEntry.DateCreated;
            newTargetEntry.Memo         = newSourceEntry.Memo;
            newTargetEntry.GlAccountNo  = newSourceEntry.GlAccountNo;
            newTargetEntry.ClassId      = newEntryTargetClassId;
            newTargetEntry.CostCenter   = newEntryTargetCostCenter;

            if ((GlTransactionEntries != null) && (GlTransactionEntries.Length > 0))
                newEntryList.AddRange(GlTransactionEntries);
            newEntryList.Add(newSourceEntry);
            newEntryList.Add(newTargetEntry);
            GlTransactionEntries = newEntryList.ToArray();
        }

        public IntacctGeneralLedgerTransaction(string newTargetJournalId, string newDescription, IntacctGeneralLedgerEntry[] newEntries)
        {
            JournalId = newTargetJournalId;
            Description = newDescription;
            DateCreated = new IntacctDate(DateTime.Now);
            GlTransactionEntries = newEntries;
        }

        public IntacctGeneralLedgerTransaction(string newTargetJournalId, string newDescription, IntacctGeneralLedgerEntry[] newEntries, DateTime newCreatedDate)
        {
            JournalId = newTargetJournalId;
            Description = newDescription;
            DateCreated = new IntacctDate(newCreatedDate);
            GlTransactionEntries = newEntries;
        }

        public IntacctGeneralLedgerTransaction()
        {
            // Nothing needed
        }

        public IntacctGeneralLedgerTransaction(XElement data)
        {
            JournalId               = Util.DeserializeXmlToString(data, "journalid");
            DateCreated             = Util.DeserializeXmlToIntacctObject<IntacctDate>(data.Element("datecreated"));
            Description             = Util.DeserializeXmlToString(data, "description");
            GlTransactionEntries    = Util.DeserializeArrayOfChildIntacctObject<IntacctGeneralLedgerEntry>(data.Element("gltransactionentries"));
        }

        internal override XObject[] ToXmlElements()
        {
            var elements = new List<XObject>();

            Util.SerializeStringToXml(JournalId, "journalid", elements);
            Util.SerializeChildIntacctObject(DateCreated, "datecreated", elements);
            Util.SerializeStringToXml(Description, "description", elements);
            Util.SerializeArrayOfChildIntacctObject(GlTransactionEntries, "gltransactionentries", "glentry", elements);
            return elements.ToArray();
        }
    }
}
