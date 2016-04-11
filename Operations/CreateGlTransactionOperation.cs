using System;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Intacct.Entities;

namespace Intacct.Operations
{
	/// <summary>
	/// Creates an journal transaction - a group of journal entries.
	/// </summary>
	/// <remarks>
	/// This operation creates a journal transaction in ANY journal except statistics.  The name is purposefully specific to match the name in the Intacct XML API.
	/// 
	/// USAGE:
	/// 1.  Create a list of at least two IntacctGeneralLedgerEntry objects.
	/// 2.  Create a new IntacctGeneralLedgerTransaction object and add the IntacctGeneralLedgerEntry.  The Transaction must have at least two entry
	///     objects where the sum of all credits equals the sum of all debits.  Since this is often matched pairs of entries, the AddEntryPair method
	///     may be useful.
	///         var transaction = new IntacctGeneralLedgerTransaction(IntacctGeneralLedgerTransaction.JournalId_GeneralJournal, "Test of IT Intacct client", null);
	///         transaction.AddEntryPair(100.00M, "AccountNumber", "Note for line items", "SourceClass", "SourceCostCenter", "TargetClass", "TargetCostCenter");
	/// 3.  Create a CreateGlTransactionOperation for the transaction object.  This is typically combined with step 4.
	/// 4.  Execute the operation.
	///         var response = client.ExecuteOperations(new[] { new CreateGlTransactionOperation(session, transaction) }, CancellationToken.None).Result;
	/// 5.  Check the response.  A success is indicated when response.Success && ((OperationResults[0].Errors == null) || (OperationResults[0].Errors.Count == 0)).
	/// 
	/// Notes:
	/// 1.  The create API will fail with PL05000053 if entries do not sum to 0 (balanced debit and credit records).
	/// 2.  Required fields are amount, trtype, datecreated, glaccountno, classid, and costCenter.
	/// </remarks>
	[SuppressMessage("ReSharper", "UnusedMember.Global")]
	public class CreateGlTransactionOperation : IntacctAuthenticatedOperationBase<IntacctGeneralLedgerTransaction>
	{
		private readonly IntacctGeneralLedgerTransaction _transaction;
		private readonly IntacctDate _reverseDate;
		private readonly string _referenceno;
		private readonly string _sourceentity;
		private readonly IntacctCustomField[] _customfields;

		public CreateGlTransactionOperation(IIntacctSession parentSession, IntacctGeneralLedgerTransaction transaction, string referenceNumber = null, string sourceEntity = null, IntacctCustomField[] customFields = null, IntacctDate reverseDate = null) : base(parentSession, "create_gltransaction", "key")
		{
			// Validate parameters
			if (parentSession == null) throw new ArgumentNullException(nameof(parentSession));
			if (transaction == null) throw new ArgumentNullException(nameof(transaction));

			_transaction	= transaction;
			_reverseDate	= reverseDate;
			_referenceno	= referenceNumber;
			_sourceentity	= sourceEntity;
			_customfields	= customFields;
		}

		protected override XObject[] CreateFunctionContents()
		{
			var serializedElements = new List<XObject>();
			serializedElements.AddRange(_transaction.ToXmlElements());
			Serializer.SerializeChildIntacctObject(_reverseDate, "reversedate", serializedElements);
			Serializer.SerializeStringToXml(_referenceno, "referenceno", serializedElements);
			Serializer.SerializeStringToXml(_sourceentity, "sourceentity", serializedElements);

			if (_customfields?.Length > 0)
			{
				serializedElements.Add(Serializer.SerializeArrayOfChildIntacctObject(_customfields, "customfields", "customfield"));
			}
			return serializedElements.ToArray();
		}

		protected override IntacctOperationResult<IntacctGeneralLedgerTransaction> ProcessResponseData(XElement responseData)
		{
			var deserializedTransaction = new IntacctGeneralLedgerTransaction(responseData);
			return new IntacctOperationResult<IntacctGeneralLedgerTransaction>(deserializedTransaction);
		}
	}
}