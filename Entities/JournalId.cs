using System.Diagnostics.CodeAnalysis;

namespace Intacct.Entities
{
	/// <summary>
	/// A list of Intacct built-in journal IDs.
	/// </summary>
	[SuppressMessage("ReSharper", "UnusedMember.Global")]
	public static class JournalId
	{
		public const string AccountsPayable							= "APJ";
		public const string AccountsReceivable						= "ARJ";
		public const string CashDisbursements						= "CDJ";
		public const string CashReceipts							= "CRJ";
		public const string EmployeeExpensesDisbursementsJournal	= "EEDJ";
		public const string EmployeeExpensesJournal					= "EEJ";
		public const string GeneralJournal							= "GJ";
		public const string InterEntityPayablesJournal				= "IEPJ";
		public const string InterEntityReceivablesJournal			= "IERJ";
		public const string InventoryJournal						= "IJ";
		public const string OpeningBalanceJournal					= "OBJ";
		public const string PayrollJournal							= "PYRJ";
		public const string PurchaseJournal							= "PJ";
		public const string SalesJournal							= "SJ";
		public const string StatisticalJournal						= "STATJ";
	}
}
