using Application.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Repositories
{
    public sealed class PredictionRepository
    {
        private PredictionRepository() { }

        public static PredictionRepository Instance { get; } = new();

        public PredictionType? TipoSeleccionado { get; set; }
    }
}
