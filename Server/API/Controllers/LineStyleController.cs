using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using IODomain.Output;
using IODomain.Input;
using IODomain.Extensions;
using API.Interfaces.IRepositories;
using API.Domain;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Controllers {
    [Route("api/[controller]")]
    public class LineStyleController : Controller {
        private readonly ILineStyleRepository _lineStyleRepository;

        public LineStyleController(ILineStyleRepository lineStyleRepository) {
            this._lineStyleRepository = lineStyleRepository;
        }

        [HttpGet]
        public async Task<IEnumerable<OutLineStyle>> GetAll() {
            IEnumerable<LineStyle> lineStyles = await _lineStyleRepository.GetAllAsync();

            return lineStyles.Select(LineStyleExtensions.Out);
        }

        [HttpGet("{id}", Name = "GetLineStyle")]
        public async Task<IActionResult> GetById(long id) {
            LineStyle lineStyle = await _lineStyleRepository.FindAsync(id);
            if(lineStyle == null) {
                return NotFound();
            }
            return new ObjectResult(lineStyle.Out());
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] InLineStyle inputLineStyle) {
            if(inputLineStyle == null) {
                return BadRequest();
            }

            LineStyle lineStyle = new LineStyle().In(inputLineStyle);
            long id = await _lineStyleRepository.AddAsync(lineStyle);

            inputLineStyle.Id = id;
            return CreatedAtRoute("GetLineStyle", new { id = id }, inputLineStyle);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, [FromBody] InLineStyle inputLineStyle) {
            if(inputLineStyle == null || inputLineStyle.Id != id) {
                return BadRequest();
            }

            LineStyle lineStyle = await _lineStyleRepository.FindAsync(id);
            if(lineStyle == null) {
                return NotFound();
            }

            lineStyle.In(inputLineStyle);

            await _lineStyleRepository.UpdateAsync(lineStyle);
            return new NoContentResult();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id) {
            LineStyle lineStyle = await _lineStyleRepository.FindAsync(id);
            if(lineStyle == null) {
                return NotFound();
            }

            await _lineStyleRepository.RemoveAsync(id);
            return new NoContentResult();
        }
    }
}
