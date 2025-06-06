using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Net;
using Swashbuckle.AspNetCore.Filters;
using Compellio.DCAP.Web.RestApi.Models.V1;

namespace Compellio.DCAP.Web.RestApi.Controllers.V1
{
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/TAR/receipt")]
    public class TARReceiptController : ControllerBase
    {
        private readonly ILogger<TARReceiptController> logger;

        /// <summary>
        /// Initializes the <see cref="TAR"/> controller with a specific <paramref name="logger"/>, <paramref name="tarRepository"/> and <paramref name="userManager"/>.
        /// </summary>
        /// <param name="logger">Specifies the logger that has been injected, for logging all operations of the controller.</param>
        public TARReceiptController(ILogger<TARReceiptController> logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// Retrieves the TAR that has a specific Gateway address (receipt).
        /// </summary>
        /// <param name="receiptID">Specifies the Gateway-specific address of the TAR to be fetched.</param>
        /// <response code="200">Returns the TAR with the specified Gateway address.</response>
        /// <response code="404">No TARs were found with the specified Gateway address.</response>
        [HttpGet("{receiptID:length(36)}")]
        [ProducesResponseType(typeof(TAR), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<TAR>> Get(string receiptID)
        {
            //tarRepository.OperationApiKey = Request.Headers[ConfigureSwaggerOptions.ApiKeyName];
            var tar = await TAR.GetByReceiptIdAsync(receiptID);
            if (tar is null)
            {
                return NotFound();
            }

            return Ok(tar);
        }

        /// <summary>
        /// Retrieves the TAR that has a specific Gateway address (receipt) and specific SDs.
        /// </summary>
        /// <param name="receiptID">Specifies the Gateway address (receipt) of the TAR to be fetched.</param>
        /// <response code="200">Returns the TAR with the specified Gateway address.</response>
        /// <response code="404">No TARs were found with the specified Gateway address.</response>
        [HttpPost("sd/{receiptID:length(36)}")]
        [ProducesResponseType(typeof(TAR), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<TAR>> GetByTokenIdWithSD(string receiptID, [FromBody] IDictionary<string, string> saltHashDictionary)
        {
            //tarRepository.OperationApiKey = Request.Headers[ConfigureSwaggerOptions.ApiKeyName];
            var tar = await TAR.GetByReceiptIdAsync(receiptID);
            if (tar is null)
            {
                return NotFound();
            }

            return Ok(tar);
        }

        /// <summary>
        /// Retrieves the raw TAR payload that has a specific Gateway address (receipt).
        /// </summary>
        /// <param name="receiptID">Specifies the Gateway address (receipt) of the TAR to be fetched.</param>
        /// <response code="200">Returns the TAR with the specified Gateway address.</response>
        /// <response code="404">No TARs were found with the specified Gateway address.</response>
        [HttpGet("raw/{receiptID:length(36)}")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<string>> GetRawByReceiptId(string receiptID)
        {
            //tarRepository.OperationApiKey = Request.Headers[ConfigureSwaggerOptions.ApiKeyName];
            var tar = await TAR.GetByReceiptIdAsync(receiptID);
            if (tar is null)
            {
                return NotFound();
            }

            return Ok(tar.Data.RootElement.ToString());
        }

        /// <summary>
        /// Retrieves the raw TAR payload that has a specific Gateway address (receipt).
        /// </summary>
        /// <param name="receiptID">Specifies the Gateway address (receipt) of the TAR to be fetched.</param>
        /// <response code="200">Returns the TAR with the specified Gateway address.</response>
        /// <response code="404">No TARs were found with the specified Gateway address.</response>
        [HttpGet("canonicalized/{receiptID:length(36)}")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<string>> GetCanonicalizedByReceiptId(string receiptID)
        {
            //tarRepository.OperationApiKey = Request.Headers[ConfigureSwaggerOptions.ApiKeyName];
            var tar = await TAR.GetByReceiptIdAsync(receiptID);
            if (tar is null)
            {
                return NotFound();
            }

            return Ok(tar.Data.RootElement.ToString());
        }

    }
}
