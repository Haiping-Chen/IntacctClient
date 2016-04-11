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
			JournalId	= Serializer.DeserializeXmlToString(data, "journalid");
			DateCreated = Serializer.DeserializeXmlToIntacctObject<IntacctDate>(data.Element("datecreated"));
			Description = Serializer.DeserializeXmlToString(data, "description");
			GlTransactionEntries = Serializer.DeserializeArrayOfChildIntacctObject<IntacctGeneralLedgerEntry>(data.Element("gltransactionentries"));
		}

		/// <summary>
		/// Adds a debit/credit pair of GL entries.
		/// </summary>
		/// <remarks>
		/// The pair should balance to zero. However, since transactions may come with different exchange rates and currencies,
		/// this is not explicitly validated.
		/// </remarks>
		public void AddEntryPair(IntacctGeneralLedgerEntry credit, IntacctGeneralLedgerEntry debit)
		{
			if (credit == null) throw new ArgumentNullException(nameof(credit));
			if (debit == null) throw new ArgumentNullException(nameof(debit));

			if (credit.Type != IntacctGeneralLedgerEntryType.Credit) throw new ArgumentException("Credit entry must be of type Credit", nameof(credit));
			if (debit.Type != IntacctGeneralLedgerEntryType.Debit) throw new ArgumentException("Debit entry must be of type Credit", nameof(debit));

			var entries = new List<IntacctGeneralLedgerEntry>();
			if (GlTransactionEntries != null && GlTransactionEntries.Length > 0)
			{
				entries.AddRange(GlTransactionEntries);
			}

			entries.Add(credit);
			entries.Add(debit);

			GlTransactionEntries = entries.ToArray();
		}

		internal override XObject[] ToXmlElements()
		{
			var elements = new List<XObject>();

			Serializer.SerializeStringToXml(JournalId, "journalid", elements);
			Serializer.SerializeChildIntacctObject(DateCreated, "datecreated", elements);
			Serializer.SerializeStringToXml(Description, "description", elements);

			if (GlTransactionEntries?.Length > 0)
			{
				elements.Add(Serializer.SerializeArrayOfChildIntacctObject(GlTransactionEntries, "gltransactionentries", "glentry"));
			}

			return elements.ToArray();
		}
	}
}
