using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Net;
using Swashbuckle.AspNetCore.Filters;
using Compellio.DCAP.Web.RestApi.Models.V1;

namespace Compellio.DCAP.Web.RestApi.Controllers.V1
{
    public enum TARVersionError
    {
        NoSemicolonInId = 1,
        VersionIsNotANumber = 2,
    }

    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class TARController : ControllerBase
    {
        private readonly ILogger<TARController> logger;

        /// <summary>
        /// Initializes the <see cref="TAR"/> controller with a specific <paramref name="logger"/>, <paramref name="tarRepository"/> and <paramref name="userManager"/>.
        /// </summary>
        /// <param name="logger">Specifies the logger that has been injected, for logging all operations of the controller.</param>
        public TARController(ILogger<TARController> logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// Adds a new TAR to the Gateway.
        /// </summary>
        /// <param name="tarPayload">Specifies the off-chain information to be stored in JSON format.</param>
        /// <response code="201">Returns the newly created TAR.</response>
        /// <response code="401">The call was made with no authorization information.</response>
        /// <response code="400">The off-chain information is badly formatted (not in JSON format).</response>
        /// <response code="403">The Gateway user is not authorized to perform this action.</response>
        [HttpPost]
        [SwaggerRequestExample(typeof(JsonDocument), typeof(JsonDocumentDefault))]
        [ProducesResponseType(typeof(TAR), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.Forbidden)]
        public async Task<ActionResult<TAR>> Post([FromBody] JsonDocument tarPayload)
        {
            if (ModelState.IsValid == false)
            {
                return BadRequest();
            }

            //tarRepository.OperationApiKey = Request.Headers[ConfigureSwaggerOptions.ApiKeyName];
            //if (String.IsNullOrWhiteSpace(tarRepository.OperationApiKey))
            //{
            //    return Forbid();
            //}

            //var newPayload = new Model.TARPayload();
            //newPayload.ApiKey = tarRepository.OperationApiKey;
            ////newPayload.Data = tarPayload.RootElement.ToString();
            //newPayload.RawData = tarPayload.RootElement.ToString();
            //var tar = await tarRepository.AddOrUpdateAsync(null, newPayload);
            var tar = await TAR.AddOrUpdateAsync(null, new TAR() { Receipt = Guid.NewGuid().ToString(), Data = tarPayload });

            var tarFromModel = tar;
            if (tarFromModel.Id.Contains("compellio"))
            {
                tarFromModel.Id = String.Empty;
            }

            return CreatedAtAction(nameof(Post), tarFromModel);
        }

        /// <summary>
        /// Retrieves a list of TARs that meet the criteria specified in the filter.
        /// </summary>
        /// <param name="newTAR">Specifies the off-chain information to be stored in JSON format.</param>
        /// <response code="200">Returns the newly created TAR.</response>
        /// <response code="404">No TARs were found that meet the specified criteria in the filter.</response>
        /// <response code="400">The filter information is badly formatted (not in JSON format).</response>
        [HttpPost("find")]
        [ProducesResponseType(typeof(TAR), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<IList<TAR>>> Find([FromBody] IDictionary<string, string> filter)
        {
            if (ModelState.IsValid == false)
            {
                return BadRequest();
            }

            //tarRepository.OperationApiKey = Request.Headers[ConfigureSwaggerOptions.ApiKeyName];
            var tars = await TAR.FindAsync(filter);
            if (tars.Count == 0)
            {
                return NotFound();
            }

            var modelTars = tars;
            foreach (var modelTar in modelTars)
            {
                if (modelTar.Id.Contains("compellio"))
                {
                    modelTar.Id = String.Empty;
                }
            }

            return Ok(modelTars);
        }

        /// <summary>
        /// Retrieves the TAR that has a specific Gateway address.
        /// </summary>
        /// <param name="receiptID">Specifies the Gateway-specific address of the TAR to be fetched.</param>
        /// <response code="200">Returns the TAR with the specified Gateway address.</response>
        /// <response code="404">No TARs were found with the specified Gateway address.</response>
        [HttpGet("tarId/{receiptID:length(36)}")]
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

            var tarFromModel = tar;
            if (tarFromModel.Id.Contains("compellio"))
            {
                tarFromModel.Id = String.Empty;
            }

            return Ok(tarFromModel);
        }

        /// <summary>
        /// Retrieves the TAR that has a specific on-chain address.
        /// </summary>
        /// <param name="tarID">Specifies the on-chain address of the TAR to be fetched.</param>
        /// <response code="200">Returns the TAR with the specified on-chain address.</response>
        /// <response code="404">No TARs were found with the specified on-chain address.</response>
        [HttpGet("{tarID}", Name = "Get")]
        [ProducesResponseType(typeof(TAR), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<TAR>> GetByTokenId(string tarID)
        {
            //tarRepository.OperationApiKey = Request.Headers[ConfigureSwaggerOptions.ApiKeyName];
            var tar = await TAR.GetAsync(tarID);
            if (tar is null)
            {
                return NotFound();
            }

            return Ok(tar);
        }

        /// <summary>
        /// Retrieves the TAR that has a specific on-chain address and specific SDs.
        /// </summary>
        /// <param name="tarID">Specifies the on-chain address of the TAR to be fetched.</param>
        /// <response code="200">Returns the TAR with the specified on-chain address.</response>
        /// <response code="404">No TARs were found with the specified on-chain address.</response>
        [HttpPost("sd/{tarID}")]
        [ProducesResponseType(typeof(TAR), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<TAR>> GetByTokenIdWithSD(string tarID, [FromBody] IDictionary<string, string> saltHashDictionary)
        {
            //tarRepository.OperationApiKey = Request.Headers[ConfigureSwaggerOptions.ApiKeyName];
            var tar = await TAR.GetAsync(tarID);
            if (tar is null)
            {
                return NotFound();
            }

            return Ok(tar);
        }

        /// <summary>
        /// Retrieves the TAR that has a specific on-chain address and version number.
        /// </summary>
        /// <param name="tarVersionID">Specifies the on-chain address of the TAR to be fetched, a semicolon and the version number.</param>
        /// <response code="200">Returns the TAR with the specified on-chain address.</response>
        /// <response code="404">No TARs were found with the specified on-chain address.</response>
        /// <response code="400">The on-chain address and the version, is not separated by a semicolon or the version specified is not a number.</response>
        [HttpGet("version/{tarVersionID}")]
        [ProducesResponseType(typeof(TAR), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(TARVersionError), (int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<TAR>> GetByIdAndVersion(string tarVersionID)
        {
            var splitTARIDArray = tarVersionID.Split(':');
            if (splitTARIDArray.Length < 2)
            {
                return BadRequest(TARVersionError.NoSemicolonInId);
            }

            var tarVersionString = splitTARIDArray.Last();
            var tarVersion = 0;
            if (int.TryParse(tarVersionString, out tarVersion) == false)
            {
                return BadRequest(TARVersionError.VersionIsNotANumber);
            }

            //tarRepository.OperationApiKey = Request.Headers[ConfigureSwaggerOptions.ApiKeyName];
            var tarId = string.Join(":", splitTARIDArray.Take(splitTARIDArray.Length - 1));
            var tar = await TAR.GetByVersionAsync(tarId, tarVersion);
            if (tar is null)
            {
                return NotFound();
            }

            return Ok(tar);
        }

        /// <summary>
        /// Retrieves the raw TAR payload that has a specific on-chain address and version number.
        /// </summary>
        /// <param name="tarVersionID">Specifies the on-chain address of the TAR to be fetched, a semicolon and the version number.</param>
        /// <response code="200">Returns the TAR with the specified on-chain address.</response>
        /// <response code="404">No TARs were found with the specified on-chain address.</response>
        /// <response code="400">The on-chain address and the version, is not separated by a semicolon or the version specified is not a number.</response>
        [HttpGet("version/raw/{tarVersionID}")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(TARVersionError), (int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<string>> GetRawByIdAndVersion(string tarVersionID)
        {
            var splitTARIDArray = tarVersionID.Split(':');
            if (splitTARIDArray.Length < 2)
            {
                return BadRequest(TARVersionError.NoSemicolonInId);
            }

            var tarVersionString = splitTARIDArray.Last();
            var tarVersion = 0;
            if (int.TryParse(tarVersionString, out tarVersion) == false)
            {
                return BadRequest(TARVersionError.VersionIsNotANumber);
            }

            //tarRepository.OperationApiKey = Request.Headers[ConfigureSwaggerOptions.ApiKeyName];
            var tarId = string.Join(":", splitTARIDArray.Take(splitTARIDArray.Length - 1));
            var tar = await TAR.GetByVersionAsync(tarId, tarVersion);
            if (tar is null)
            {
                return NotFound();
            }

            return Ok(tar.Data.RootElement.ToString());
        }

        /// <summary>
        /// Retrieves the raw TAR payload that has a specific on-chain address and version number.
        /// </summary>
        /// <param name="tarVersionID">Specifies the on-chain address of the TAR to be fetched, a semicolon and the version number.</param>
        /// <response code="200">Returns the TAR with the specified on-chain address.</response>
        /// <response code="404">No TARs were found with the specified on-chain address.</response>
        /// <response code="400">The on-chain address and the version, is not separated by a semicolon or the version specified is not a number.</response>
        [HttpGet("version/canonicalized/{tarVersionID}")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(TARVersionError), (int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<string>> GetCanonicalizedByIdAndVersion(string tarVersionID)
        {
            var splitTARIDArray = tarVersionID.Split(':');
            if (splitTARIDArray.Length < 2)
            {
                return BadRequest(TARVersionError.NoSemicolonInId);
            }

            var tarVersionString = splitTARIDArray.Last();
            var tarVersion = 0;
            if (int.TryParse(tarVersionString, out tarVersion) == false)
            {
                return BadRequest(TARVersionError.VersionIsNotANumber);
            }

            //tarRepository.OperationApiKey = Request.Headers[ConfigureSwaggerOptions.ApiKeyName];
            var tarId = string.Join(":", splitTARIDArray.Take(splitTARIDArray.Length - 1));
            var tar = await TAR.GetByVersionAsync(tarId, tarVersion);
            if (tar is null)
            {
                return NotFound();
            }

            return Ok(tar.Data.RootElement.ToString());
        }

        /// <summary>
        /// Updates the off-chain information of a TAR with a specific on-chain address, with the information provided.
        /// </summary>
        /// <param name="id">Specifies the on-chain address of the TAR to be archived.</param>
        /// <param name="updatedPayload">Specifies the updated off-chain information to be stored in JSON format.</param>
        /// <response code="201">Returns the updated TAR.</response>
        /// <response code="401">The call was made with no authorization information.</response>
        /// <response code="400">The off-chain information is badly formatted (not in JSON format).</response>
        /// <response code="409">The TAR could not be fetched after its update.</response>
        /// <response code="403">The Gateway user is not authorized to perform this action.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(TAR), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.Conflict)]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.Forbidden)]
        public async Task<IActionResult> Update(string id, [FromBody] JsonDocument updatedPayload)
        {
            if (ModelState.IsValid == false)
            {
                return BadRequest();
            }

            //tarRepository.OperationApiKey = Request.Headers[ConfigureSwaggerOptions.ApiKeyName];
            //if (String.IsNullOrWhiteSpace(tarRepository.OperationApiKey))
            //{
            //    return Forbid();
            //}

            var tar = await TAR.GetAsync(id);
            if (tar is null)
            {
                return NotFound();
            }

            //var payload = new Model.TARPayload();
            //payload.ApiKey = tarRepository.OperationApiKey;
            //payload.RawData = updatedPayload.RootElement.ToString();
            ////payload.Data = updatedPayload.RootElement.ToString();
            tar = await TAR.AddOrUpdateAsync(id, new TAR() { Receipt = Guid.NewGuid().ToString(), Data = updatedPayload });
            //tar = await tarRepository.GetAsync(id);
            //if (tar is null)
            //{
            //    return Conflict("Cannot retrieve TAR after update");
            //}

            return CreatedAtAction(nameof(Update), tar);
        }

        /// <summary>
        /// Archives the TAR that has a specific on-chain address.
        /// </summary>
        /// <param name="id">Specifies the on-chain address of the TAR to be archived.</param>
        /// <response code="204">The archiving process was successful.</response>
        /// <response code="404">No TARs were found with the specified on-chain address, for archiving.</response>
        /// <response code="401">The call was made with no authorization information.</response>
        /// <response code="403">The Gateway user is not authorized to perform this action.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.Forbidden)]
        public async Task<IActionResult> Delete(string id)
        {
            //tarRepository.OperationApiKey = Request.Headers[ConfigureSwaggerOptions.ApiKeyName];
            //if (String.IsNullOrWhiteSpace(tarRepository.OperationApiKey))
            //{
            //    return Forbid();
            //}

            // TODO: Add user check logic and return Fobidden if user that deletes does not own the TAR
            var tar = await TAR.GetAsync(id);
            if (tar is null)
            {
                return NotFound();
            }

            await TAR.ArchiveAsync(id);

            return NoContent();
        }
    }
}
