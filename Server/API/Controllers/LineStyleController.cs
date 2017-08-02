using API.Filters;
using API.Interfaces.IServices;
using IODomain.Input;
using IODomain.Output;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Controllers {
    [ServicesExceptionFilter]
    [Route("api/[controller]")]
    public class LineStyleController : Controller {
        private readonly ILineStyleService _lineStyleService;

        public LineStyleController(ILineStyleService lineStyleService) {
            _lineStyleService = lineStyleService;
        }

        [HttpGet]
        public Task<IEnumerable<OutLineStyle>> GetAll(long index = 0, long size = 10) {
            return _lineStyleService.GetAllAsync(index, size);
        }

        [HttpGet("{id}", Name = "GetLineStyle")]
        public Task<OutLineStyle> GetById(long id) {
            return _lineStyleService.GetAsync(id);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] InLineStyle inputLineStyle) {
            OutLineStyle lineStyle = await _lineStyleService.CreateAsync(inputLineStyle);
            return CreatedAtRoute("GetLineStyle", new { id = lineStyle.Id }, lineStyle);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, [FromBody] InLineStyle inputLineStyle) {
            await _lineStyleService.UpdateAsync(id, inputLineStyle);
            return new NoContentResult();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id) {
            await _lineStyleService.DeleteAsync(id);
            return new NoContentResult();
        }
    }
}
