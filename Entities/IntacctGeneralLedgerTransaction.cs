using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
	[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
	[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
	[SuppressMessage("ReSharper", "UnusedMember.Global")]
	public class IntacctGeneralLedgerTransaction : IntacctObject
	{
		/// <summary>
		/// The ID of the journal to post this to.  See the JournalId class for a list of the built-in journals.
		/// </summary>
		public string		JournalId							{ get; set; }
		public IntacctDate	DateCreated							{ get; set; }
		public string		Description							{ get; set; }
		public IntacctGeneralLedgerEntry[] GlTransactionEntries { get; set; }

		public IntacctGeneralLedgerTransaction()
		{
			// Nothing needed
		}

		public IntacctGeneralLedgerTransaction(string targetJournalId, string description, IntacctGeneralLedgerEntry[] entries) : this(targetJournalId, description, entries, DateTime.Now)
		{

		}

		public IntacctGeneralLedgerTransaction(string targetJournalId, string description, IntacctGeneralLedgerEntry[] entries, DateTime dateCreated)
		{
			JournalId	= targetJournalId;
			Description = description;
			DateCreated = new IntacctDate(dateCreated);
			GlTransactionEntries = entries;
		}

		public IntacctGeneralLedgerTransaction(XElement data)
		{
			JournalId	= Util.DeserializeXmlToString(data, "journalid");
			DateCreated = Util.DeserializeXmlToIntacctObject<IntacctDate>(data.Element("datecreated"));
			Description = Util.DeserializeXmlToString(data, "description");
			GlTransactionEntries = Util.DeserializeArrayOfChildIntacctObject<IntacctGeneralLedgerEntry>(data.Element("gltransactionentries"));
		}

		/// <summary>
		/// Adds a debit/credit pair of GL entries for the same amount, datecreated = today, with the specified values, crediting source and debiting target
		/// </summary>
		/// <param name="newEntryAmount"></param>
		/// <param name="newEntryAccountNo"></param>
		/// <param name="newEntryMemo"></param>
		/// <param name="newEntrySourceClassId"></param>
		/// <param name="newEntrySourceCostCenter"></param>
		/// <param name="newEntryTargetClassId"></param>
		/// <param name="newEntryTargetCostCenter"></param>
		public void AddEntryPair(decimal newEntryAmount, string newEntryAccountNo, string newEntryMemo, string newEntrySourceClassId, string newEntrySourceCostCenter, string newEntryTargetClassId, string newEntryTargetCostCenter)
		{
			var sourceEntry = new IntacctGeneralLedgerEntry
				                  {
					                  Amount = newEntryAmount,
					                  Type = IntacctGeneralLedgerEntry.EntryType.Credit,
					                  DateCreated = new IntacctDate(DateTime.Now),
					                  Memo = newEntryMemo,
					                  GlAccountNo = newEntryAccountNo,
					                  ClassId = newEntrySourceClassId,
					                  CostCenter = newEntrySourceCostCenter
				                  };

			var targetEntry = new IntacctGeneralLedgerEntry
				                  {
					                  Amount = sourceEntry.Amount,
					                  Type = IntacctGeneralLedgerEntry.EntryType.Debit,
					                  DateCreated = sourceEntry.DateCreated,
					                  Memo = sourceEntry.Memo,
					                  GlAccountNo = sourceEntry.GlAccountNo,
					                  ClassId = newEntryTargetClassId,
					                  CostCenter = newEntryTargetCostCenter
				                  };

			var newEntryList = new List<IntacctGeneralLedgerEntry>();
			if ((GlTransactionEntries != null) && (GlTransactionEntries.Length > 0))
			{
				newEntryList.AddRange(GlTransactionEntries);
			}

			newEntryList.Add(sourceEntry);
			newEntryList.Add(targetEntry);

			GlTransactionEntries = newEntryList.ToArray();
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
