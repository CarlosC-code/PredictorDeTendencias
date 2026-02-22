using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos
{
    public class RocPointDto
    {
        public int T { get; set; }
        public DateTime Fecha { get; set; }
        public double Precio { get; set; }
        public double? RocPorciento { get; set; } // null => "n/a"

    }
}
