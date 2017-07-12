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
    public class PointStyleController : Controller {
        private readonly IPointStyleRepository _pointStyleRepository;

        public PointStyleController(IPointStyleRepository pointStyleRepository) {
            this._pointStyleRepository = pointStyleRepository;
        }

        [HttpGet]
        public IEnumerable<OutPointStyle> GetAll() {
            return _pointStyleRepository
                .GetAllAsync()
                .Select(PointStyleExtensions.Out);
        }

        [HttpGet("{id}", Name = "GetPointStyle")]
        public IActionResult GetById(long id) {
            PointStyle pointStyle = _pointStyleRepository.FindAsync(id);
            if(pointStyle == null) {
                return NotFound();
            }
            return new ObjectResult(pointStyle.Out());
        }

        [HttpPost]
        public IActionResult Create([FromBody] InPointStyle inputPointStyle) {
            if(inputPointStyle == null) {
                return BadRequest();
            }

            PointStyle pointStyle = new PointStyle().In(inputPointStyle);
            long id = _pointStyleRepository.AddAsync(pointStyle);

            inputPointStyle.Id = id;
            return CreatedAtRoute("GetPointStyle", new { id = id }, inputPointStyle);
        }

        [HttpPut("{id}")]
        public IActionResult Update(long id, [FromBody] InPointStyle inputPointStyle) {
            if(inputPointStyle == null || inputPointStyle.Id != id) {
                return BadRequest();
            }

            PointStyle pointStyle = _pointStyleRepository.FindAsync(id);
            if(pointStyle == null) {
                return NotFound();
            }

            pointStyle.In(inputPointStyle);

            _pointStyleRepository.UpdateAsync(pointStyle);
            return new NoContentResult();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(long id) {
            PointStyle pointStyle = _pointStyleRepository.FindAsync(id);
            if(pointStyle == null) {
                return NotFound();
            }

            _pointStyleRepository.RemoveAsync(id);
            return new NoContentResult();
        }
    }
}
