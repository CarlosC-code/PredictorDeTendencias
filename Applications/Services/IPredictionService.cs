using Application.Dtos;
using Application.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public interface IPredictionService
    {
        PredictionResultDto Calcular(List<PredictionDto> datos, PredictionType tipo);
    }
}
