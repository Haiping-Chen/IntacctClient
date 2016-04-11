using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Intacct.Infrastructure;


namespace Intacct.Entities
{
    /// <summary>
    /// A list of Intacct built-in journal IDs.
    /// </summary>
    public static class JournalId
    {
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
    }
}
