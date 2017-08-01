using API.Interfaces.IServices;
using System;
using System.Collections.Generic;
using System.Text;
using IODomain.Input;
using IODomain.Output;
using System.Threading.Tasks;
using API.Domain;
using IODomain.Extensions;
using API.Services.Exceptions;
using API.Interfaces.IRepositories;
using System.Linq;
using API.Services.Utils;

namespace API.Services {
    class LineStyleService : ILineStyleService {
        private readonly ILineStyleRepository _lineStyleRepository;

        public LineStyleService(ILineStyleRepository lineStyleRepository) {
            _lineStyleRepository = lineStyleRepository;
        }

        public async Task<OutLineStyle> CreateAsync(InLineStyle inLineStyle) {
            if(inLineStyle == null) {
                throw new MissingInputException();
            }

            Validator<InLineStyle>.Valid(inLineStyle, GetCreateValidationConfigurations());

            LineStyle lineStyle = new LineStyle().In(inLineStyle);
            long id = await _lineStyleRepository.AddAsync(lineStyle);

            OutLineStyle outLineStyle = lineStyle.Out();
            outLineStyle.Id = id;

            return outLineStyle;
        }

        private static ValidatorConfiguration<InLineStyle> GetCreateValidationConfigurations() {
            return new ValidatorConfiguration<InLineStyle>()
                .NotNull("Color", i => i.Color);
        }

        public async Task DeleteAsync(long id) {
            LineStyle lineStyle = await _lineStyleRepository.FindAsync(id);//TODO replace to exists

            if(lineStyle == null) {
                throw new NotFoundException($"The Line Style with id {id} not exists");
            }

            await _lineStyleRepository.RemoveAsync(id);
        }

        public async Task<IEnumerable<OutLineStyle>> GetAllAsync(long index, long size) {
            IEnumerable<LineStyle> lineStyles = await _lineStyleRepository.GetAllAsync(index, size);

            return lineStyles.Select(LineStyleExtensions.Out);
        }

        public async Task<OutLineStyle> GetAsync(long id) {
            LineStyle lineStyle = await _lineStyleRepository.FindAsync(id);

            if(lineStyle == null) {
                throw new NotFoundException($"The Line Style with id {id} not exists");
            }

            return lineStyle.Out();
        }

        public async Task<OutLineStyle> UpdateAsync(long id, InLineStyle inLineStyle) {
            if(inLineStyle == null) {
                throw new MissingInputException();
            }

            Validator<InLineStyle>.Valid(inLineStyle, GetUpdateValidationConfigurations());

            if(inLineStyle.Id != id) {
                throw new InconsistentRequestException(
                    $"The id present on update is different of the expected. {Environment.NewLine}Expected: {id}{Environment.NewLine}Current: {inLineStyle.Id}"
                );
            }

            LineStyle lineStyle = await _lineStyleRepository.FindAsync(id);
            if(lineStyle == null) {
                throw new NotFoundException($"The Line Style with id {id} not exists");
            }

            lineStyle.In(inLineStyle);

            await _lineStyleRepository.UpdateAsync(lineStyle);
            return lineStyle.Out();
        }

        private ValidatorConfiguration<InLineStyle> GetUpdateValidationConfigurations() {
            return GetCreateValidationConfigurations()
                .NotNull("Id", i => i.Id);
        }
    }
}
