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
    [Route("api/[controller]")]
    public class LineStyleController : Controller {
        private readonly ILineStyleService _lineStyleService;
        private readonly ILogger<LineStyleController> _logger;

        public LineStyleController(ILineStyleService lineStyleService, ILogger<LineStyleController> logger) {
            _lineStyleService = lineStyleService;
            _logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IEnumerable<OutLineStyle>> GetAll(long index = 0, long size = 10) {
            _logger.LogInformation(LoggingEvents.ListLineStyles, "Listing page {index} of LineStyles with size {size}", index, size);
            IEnumerable<LineStyle> lineStyles = await _lineStyleService.GetAllAsync(index, size);

            return lineStyles.Select(LineStyleExtensions.Out);
        }

        [HttpGet("{id}", Name = "GetLineStyle")]
        [AllowAnonymous]
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

        [HttpPost]
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

        [HttpPut("{id}")]
        [Authorize("Administrator")]
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

        [HttpDelete("{id}")]
        [Authorize("Administrator")]
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
