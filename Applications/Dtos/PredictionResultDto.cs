using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos
{
    public class PredictionResultDto
    {
        public double ResultadoFinal { get; set; }
        public string Modo { get; set; }

        public string Tendencia { get; set; }

        public object DatosTecnicos { get; set; } = new object();

        
    }
}
