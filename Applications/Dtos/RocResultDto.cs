using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos
{
    public class RocResultDto
    {

        public int Periodo { get; set; } = 5; // fijo
        public List<RocPointDto> Puntos { get; set; } = new();


    }
}
