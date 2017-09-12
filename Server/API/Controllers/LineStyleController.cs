using API.Domain;
using API.Filters;
using API.Interfaces;
using API.Interfaces.IServices;
using IODomain.Extensions;
using IODomain.Input;
using IODomain.Output;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers {
    [ValidateModel]
    [ServicesExceptionFilter]
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class LineStyleController : Controller {
        private readonly ILineStyleService _lineStyleService;
        private readonly ILogger<LineStyleController> _logger;

        public LineStyleController(ILineStyleService lineStyleService, ILogger<LineStyleController> logger) {
            _lineStyleService = lineStyleService;
            _logger = logger;
        }

        /// <summary>
        /// Returns a List of Line Styles
        /// </summary>
        /// <param name="index">Number of the page</param>
        /// <param name="size">Number of Line Styles for page</param>
        /// <returns>Required List of Line Styles</returns>
        /// <response code="200">Returns the required list of Line Styles</response>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(IEnumerable<OutLineStyle>), 200)]
        public async Task<IEnumerable<OutLineStyle>> GetAll(long index = 0, long size = 10) {
            _logger.LogInformation(LoggingEvents.ListLineStyles, "Listing page {index} of LineStyles with size {size}", index, size);
            IEnumerable<LineStyle> lineStyles = await _lineStyleService.GetAllAsync(index, size);

            return lineStyles.Select(LineStyleExtensions.Out);
        }

        /// <summary>
        /// Returns a specific Line Style.
        /// </summary>
        /// <param name="id">Id of the Line Style to return</param>
        /// <returns>Required Line Style</returns>
        /// <response code="200">Returns the required Line Style</response>
        /// <response code="404">If the Line Style not exists</response>
        [HttpGet("{id}", Name = "GetLineStyle")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(OutLineStyle), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetById(long id) {
            _logger.LogInformation(LoggingEvents.GetLineStyle, "Getting LineStyle {ID}", id);
            LineStyle lineStyle = await _lineStyleService.GetAsync(id);

            if(lineStyle == null) {
                //$"The Line Style with id {id} not exists"
                _logger.LogWarning(LoggingEvents.GetLineStyleNotFound, "GetById({id}) NOT FOUND", id);
                return NotFound();
            }

            return Ok(lineStyle.Out());
        }

        /// <summary>
        /// Creates a Line Style.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/LineStyle
        ///     Content-Type: application/json
        /// 
        ///     {
        ///         "color":"red"
        ///     }
        ///
        /// </remarks>
        /// <param name="inputLineStyle">Information of the Line Style to create</param>
        /// <returns>A newly-created Line Style</returns>
        /// <response code="201">Returns the newly-created Line Style</response>
        /// <response code="400">If the inputLineStyle is null</response> 
        [HttpPost]
        [ProducesResponseType(typeof(OutLineStyle), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create([FromBody] InCreateLineStyle inputLineStyle) {
            if(inputLineStyle == null) {
                _logger.LogDebug(LoggingEvents.InsertLineStyleWithoutBody, "Create() WITHOUT BODY");
                return BadRequest();
            }

            LineStyle style = new LineStyle().In(inputLineStyle);

            await _lineStyleService.CreateAsync(style);
            _logger.LogInformation(LoggingEvents.InsertLineStyle, "LineStyle {ID} Created", style.Id);

            return CreatedAtRoute("GetLineStyle", new { id = style.Id }, style.Out());
        }

        /// <summary>
        /// Updates a specific LineStyle
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /api/LineStyle/0
        ///     Content-Type: application/json
        ///     Authorization: Bearer {ACCESS_TOKEN}
        /// 
        ///     {
        ///         "id": 0,
        ///         "color":"red"
        ///     }
        ///
        /// </remarks>
        /// <param name="id">Id of the Board to update</param>
        /// <param name="inLineStyle">Information of the Line Style to update</param>
        /// <response code="204">Line Style update succeeds</response>
        /// <response code="400">If there is inconsistent information or the inLineStyle is null</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user does not have authorization</response>
        /// <response code="404">If the Line Style not exists</response>
        [HttpPut("{id}")]
        [Authorize("Administrator")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(long id, [FromBody] InUpdateLineStyle inLineStyle) {
            if(inLineStyle == null) {
                _logger.LogDebug(LoggingEvents.UpdateLineStyleWithoutBody, "Update() WITHOUT BODY");
                return BadRequest();
            }

            if(inLineStyle.Id != id) {
                //$"The id present on update is different of the expected. {Environment.NewLine}Expected: {id}{Environment.NewLine}Current: {inLineStyle.Id}"
                _logger.LogDebug(LoggingEvents.UpdateLineStyleWrongId, "Update({ID}) WRONG ID {wrongId}", id, inLineStyle.Id);
                return BadRequest();
            }

            LineStyle lineStyle = await _lineStyleService.GetAsync(id);
            if(lineStyle == null) {
                //$"The Line Style with id {id} not exists"
                _logger.LogWarning(LoggingEvents.UpdateLineStyleNotFound, "Update({ID}) NOT FOUND", id);
                return NotFound();
            }

            lineStyle.In(inLineStyle);

            await _lineStyleService.UpdateAsync(lineStyle);
            _logger.LogInformation(LoggingEvents.UpdateLineStyle, "LineStyle {ID} Updated", lineStyle.Id);

            return NoContent();
        }

        /// <summary>
        /// Deletes a specific LineStyle
        /// </summary>
        /// <param name="id">Id of the Line Style to delete</param>
        /// <response code="204">Line Style deletion succeeds</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user does not have authorization</response>
        /// <response code="404">If the Line Style not exists</response>
        [HttpDelete("{id}")]
        [Authorize("Administrator")]
        [ProducesResponseType(204)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(long id) {
            if(!await _lineStyleService.ExistsAsync(id)) {
                //$"The Line Style with id {id} not exists"
                _logger.LogWarning(LoggingEvents.DeleteLineStyleNotFound, "Delete({ID}) NOT FOUND", id);
                return NotFound();
            }

            await _lineStyleService.DeleteAsync(id);
            _logger.LogInformation(LoggingEvents.DeleteLineStyle, "LineStyle {ID} Deleted", id);

            return NoContent();
        }
    }
}
