using API.Interfaces.IRepositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace API.Services {
    public class FigureIdGenerator {//TODO Rever à luz da injeção de dependencias
        private long _id;

        private FigureIdGenerator(long initId) {
            _id = initId;
        }

        public long NewId() {
            return Interlocked.Increment(ref _id);
        }

        public static FigureIdGenerator Create(IFigureIdRepository figureIdRepository) {
            long initialId = figureIdRepository.GetMaxId();

            return new FigureIdGenerator(initialId);
        }
    }
}
