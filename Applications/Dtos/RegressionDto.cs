using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos
{
    public class RegressionDto
    {
        public double Pendiente { get; set; }
        public double Intercepto { get; set; }
        public double Prediccion { get; set; }

        public string Tendencia { get; set; }

    }
}
