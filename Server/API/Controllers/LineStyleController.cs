using API.Domain;
using API.Filters;
using API.Interfaces.IServices;
using IODomain.Extensions;
using IODomain.Input;
using IODomain.Output;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        public LineStyleController(ILineStyleService lineStyleService) {
            _lineStyleService = lineStyleService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IEnumerable<OutLineStyle>> GetAll(long index = 0, long size = 10) {
            IEnumerable<LineStyle> lineStyles = await _lineStyleService.GetAllAsync(index, size);

            return lineStyles.Select((Func<LineStyle, OutLineStyle>)LineStyleExtensions.Out);
        }

        [HttpGet("{id}", Name = "GetLineStyle")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(long id) {
            LineStyle lineStyle = await _lineStyleService.GetAsync(id);

            if(lineStyle == null) {
                //$"The Line Style with id {id} not exists"
                return NotFound();
            }

            return Ok(lineStyle.Out());
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] InCreateLineStyle inputLineStyle) {
            if(inputLineStyle == null) {
                return BadRequest();
            }

            LineStyle style = new LineStyle().In(inputLineStyle);

            await _lineStyleService.CreateAsync(style);

            return CreatedAtRoute("GetLineStyle", new { id = style.Id }, style.Out());
        }

        [HttpPut("{id}")]
        [Authorize("Administrator")]
        public async Task<IActionResult> Update(long id, [FromBody] InUpdateLineStyle inLineStyle) {
            if(inLineStyle == null) {
                return BadRequest();
            }

            if(inLineStyle.Id != id) {
                //$"The id present on update is different of the expected. {Environment.NewLine}Expected: {id}{Environment.NewLine}Current: {inLineStyle.Id}"
                return BadRequest();
            }

            LineStyle lineStyle = await _lineStyleService.GetAsync(id);
            if(lineStyle == null) {
                //$"The Line Style with id {id} not exists"
                return NotFound();
            }

            lineStyle.In(inLineStyle);

            await _lineStyleService.UpdateAsync(lineStyle);

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize("Administrator")]
        public async Task<IActionResult> Delete(long id) {
            if(!await _lineStyleService.ExistsAsync(id)) {
                //$"The Line Style with id {id} not exists"
                return NotFound();
            }

            await _lineStyleService.DeleteAsync(id);

            return NoContent();
        }
    }
}
