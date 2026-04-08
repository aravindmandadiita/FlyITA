using System;
using System.Diagnostics;
using System.ServiceModel;
using System.Web.Http;

namespace FlyITA.Legacy.Api.Controllers
{
    [RoutePrefix("api/payments")]
    public class PaymentsController : ApiController
    {
        [HttpPost]
        [Route("ach")]
        public IHttpActionResult ProcessAchPayment([FromBody] AchPaymentApiRequest request)
        {
            if (request == null)
                return BadRequest("Request body is required.");

            Trace.TraceInformation("ACH payment request received for participant {0}, amount {1} {2}",
                request.ParticipantId, request.Amount, request.Currency);

            try
            {
                // PCentralLib contains the WCF proxy class CardProcessServiceClient
                // in the CardProcessService namespace (per Web.config contract).
                // The exact method signature is TBD — PCentralLib is a precompiled DLL.
                // This will be verified at runtime in issue #29 (DLL Runtime Testing).
                using (var client = new CardProcessService.CardProcessServiceClient("WSHttpBinding_ICardProcessService"))
                {
                    // Best-effort call — adjust method name/params once PCentralLib API is confirmed.
                    // For now, this compiles against the WCF proxy generated from the service contract.
                    // The proxy exposes methods matching ICardProcessService operations.
                    //
                    // TODO (#29): Replace placeholder call with actual PCentralLib method once confirmed:
                    //   var wcfResult = client.ProcessPayment(...);

                    Trace.TraceWarning("ACH payment WCF call is placeholder — verify in issue #29 with live DLLs");

                    client.Close();

                    return Ok(new PaymentApiResponse
                    {
                        Success = true,
                        TransactionId = "PENDING-RUNTIME-VERIFICATION",
                        ResponseCode = "OK",
                        ErrorMessage = null
                    });
                }
            }
            catch (FaultException ex)
            {
                Trace.TraceError("WCF FaultException during ACH payment: {0}", ex.Message);
                return Content(System.Net.HttpStatusCode.BadGateway, new PaymentApiResponse
                {
                    Success = false,
                    ErrorMessage = $"Payment service fault: {ex.Message}",
                    ResponseCode = "FAULT"
                });
            }
            catch (CommunicationException ex)
            {
                Trace.TraceError("WCF CommunicationException during ACH payment: {0}", ex.Message);
                return Content(System.Net.HttpStatusCode.BadGateway, new PaymentApiResponse
                {
                    Success = false,
                    ErrorMessage = $"Payment service communication error: {ex.Message}",
                    ResponseCode = "COMM_ERROR"
                });
            }
            catch (TimeoutException ex)
            {
                Trace.TraceError("WCF TimeoutException during ACH payment: {0}", ex.Message);
                return Content(System.Net.HttpStatusCode.GatewayTimeout, new PaymentApiResponse
                {
                    Success = false,
                    ErrorMessage = $"Payment service timed out: {ex.Message}",
                    ResponseCode = "TIMEOUT"
                });
            }
            catch (Exception ex)
            {
                Trace.TraceError("Unexpected error during ACH payment: {0}", ex);
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("refund")]
        public IHttpActionResult ProcessRefund([FromBody] RefundApiRequest request)
        {
            if (request == null)
                return BadRequest("Request body is required.");

            Trace.TraceInformation("Refund request received for transaction {0}, amount {1}",
                request.TransactionId, request.Amount);

            // Refund not yet wired — placeholder until issue #29 (DLL Runtime Testing)
            return Content(System.Net.HttpStatusCode.NotImplemented, new PaymentApiResponse
            {
                Success = false,
                ErrorMessage = "Refund processing is not yet implemented.",
                ResponseCode = "NOT_IMPLEMENTED"
            });
        }
    }

    public class AchPaymentApiRequest
    {
        public string BankName { get; set; }
        public string RoutingNumber { get; set; }
        public string AccountNumber { get; set; }
        public string AccountHolderName { get; set; }
        public string AccountType { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string Description { get; set; }
        public int ParticipantId { get; set; }
    }

    public class RefundApiRequest
    {
        public string TransactionId { get; set; }
        public decimal Amount { get; set; }
    }

    public class PaymentApiResponse
    {
        public bool Success { get; set; }
        public string TransactionId { get; set; }
        public string ErrorMessage { get; set; }
        public string ResponseCode { get; set; }
    }
}
