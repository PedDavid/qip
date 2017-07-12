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
        public IEnumerable<OutLineStyle> GetAll() {
            return _lineStyleRepository
                .GetAllAsync()
                .Select(LineStyleExtensions.Out);
        }

        [HttpGet("{id}", Name = "GetLineStyle")]
        public IActionResult GetById(long id) {
            LineStyle lineStyle = _lineStyleRepository.FindAsync(id);
            if(lineStyle == null) {
                return NotFound();
            }
            return new ObjectResult(lineStyle.Out());
        }

        [HttpPost]
        public IActionResult Create([FromBody] InLineStyle inputLineStyle) {
            if(inputLineStyle == null) {
                return BadRequest();
            }

            LineStyle lineStyle = new LineStyle().In(inputLineStyle);
            long id = _lineStyleRepository.AddAsync(lineStyle);

            inputLineStyle.Id = id;
            return CreatedAtRoute("GetLineStyle", new { id = id }, inputLineStyle);
        }

        [HttpPut("{id}")]
        public IActionResult Update(long id, [FromBody] InLineStyle inputLineStyle) {
            if(inputLineStyle == null || inputLineStyle.Id != id) {
                return BadRequest();
            }

            LineStyle lineStyle = _lineStyleRepository.FindAsync(id);
            if(lineStyle == null) {
                return NotFound();
            }

            lineStyle.In(inputLineStyle);

            _lineStyleRepository.UpdateAsync(lineStyle);
            return new NoContentResult();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(long id) {
            LineStyle lineStyle = _lineStyleRepository.FindAsync(id);
            if(lineStyle == null) {
                return NotFound();
            }

            _lineStyleRepository.RemoveAsync(id);
            return new NoContentResult();
        }
    }
}
