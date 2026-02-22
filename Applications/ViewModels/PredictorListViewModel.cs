using Application.Dtos;
using Application.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels
{
    public class PredictorListViewModel
    {
      
        public List<NewPredictorViewModel> Datos { get; set; } = new();

        public PredictionResultDto Resultado { get; set; }

        public PredictionType? ModoUsado { get; set; }


    }
}
