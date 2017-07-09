using API.Interfaces.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebSockets.Operations {
    public class LineOperations {
        private readonly ILineRepository _lineRepository;

        public LineOperations(ILineRepository lineRepository) {
            _lineRepository = lineRepository;
        }

        //TODO implement
    }
}
