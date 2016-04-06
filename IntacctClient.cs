using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Intacct.Operations;

namespace Intacct
{
	public class IntacctClient : IIntacctClient
	{
		private readonly Uri _apiUri;
		private readonly NetworkCredential _serviceCredential;

		public IntacctClient(Uri apiUri, NetworkCredential serviceCredential)
		{
			_apiUri = apiUri;
			_serviceCredential = serviceCredential;
		}

		public async Task<IIntacctSession> InitiateApiSession(IntacctUserCredential cred)
		{
			return await InitiateApiSession(cred, CancellationToken.None);
		}

		public async Task<IIntacctSession> InitiateApiSession(IntacctUserCredential cred, CancellationToken token)
		{
			var operation = new GetApiSessionOperation(cred);

			// construct and execute request
			var operations = new[] { operation };
			var response = await ExecuteOperations(operations, token);

			// expecting a single operation result (more or less is a problem)
			var result = response.OperationResults.OfType<IntacctOperationResult<IntacctSession>>().SingleOrDefault();
			if (result != null) return result.Value;

			// let's see what the result is
			var badResult = response.OperationResults.FirstOrDefault();
			if (badResult == null) throw new Exception("Unable to initiate API session, and no results were captured.");

			var authError = badResult as IntacctOperationAuthFailedResult;
			if (authError != null) throw new Exception($"Unable to initiate API session, authentication failed. Service error: {authError.Errors.Select(e => e.ToString()).Aggregate((curr, next) => curr + " " + next)}");

			throw new Exception($"Unable to initiate API session, unexpected result type {badResult.GetType().Name}.");
		}

		public async Task<IIntacctServiceResponse> ExecuteOperations(ICollection<IIntacctOperation> operations, CancellationToken token)
		{
			using (var requestStream = new MemoryStream())
			{
				ConstructRequestBody(requestStream, Guid.NewGuid().ToString(), operations);

				// execute request
				var response = await ExecuteRequest(requestStream, operations, token, _apiUri);
				if (!response.Success)
				{
					throw new IntacctServiceException(response);
				}

				return response;
			}
		}

		private void ConstructRequestBody(Stream stream, string requestId, IEnumerable<IIntacctOperation> operations)
		{
			var doc = new XDocument();

			var request = new XElement("request", GetControlElement(_serviceCredential, requestId));
			request.Add(operations.Select(op => op.GetOperationElement()));

			doc.Add(request);

			using (var writer = XmlWriter.Create(stream, new XmlWriterSettings { Encoding = Encoding.UTF8, CloseOutput = false }))
			{
				doc.Save(writer);
				writer.Flush();
			}

			// rewind stream
			stream.Seek(0, SeekOrigin.Begin);
		}

		private static XElement GetControlElement(NetworkCredential serviceCredential, string requestId)
		{
			var control = new XElement("control",
			                           new XElement("senderid", serviceCredential.UserName),
			                           new XElement("password", serviceCredential.Password),
			                           new XElement("controlid", requestId),
			                           new XElement("uniqueid", "false"),
			                           new XElement("dtdversion", "3.0"));
			return control;
		}

		private async Task<IIntacctServiceResponse> ExecuteRequest(Stream requestStream, ICollection<IIntacctOperation> operations, CancellationToken token, Uri uri = null)
		{
			uri = uri ?? _apiUri;
			
			using (var client = new HttpClient())
			{
				// set up the content for the request
				var content = new StreamContent(requestStream);
				content.Headers.ContentType = new MediaTypeHeaderValue("application/x-intacct-xml-request");

				// execute request and throw if response is not a success code
				var response = await client.PostAsync(uri, content, token);
				response.EnsureSuccessStatusCode();

				// process response
				using (var responseStream = await response.Content.ReadAsStreamAsync())
				{
					var doc = XDocument.Load(responseStream);

					return ResponseParser.Parse(doc, operations);
				}
			}
		}
	}
}
