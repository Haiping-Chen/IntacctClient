using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intacct.Operations;

namespace Intacct
{
	public interface IIntacctClient
	{
		Task<IIntacctSession> InitiateApiSession(IntacctUserCredential cred);
		Task<IIntacctSession> InitiateApiSession(IntacctUserCredential cred, CancellationToken token);
		Task<IIntacctServiceResponse> ExecuteOperations(ICollection<IIntacctOperation> operations, CancellationToken token);
	}
}