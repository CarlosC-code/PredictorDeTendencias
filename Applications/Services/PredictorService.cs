using Application.Dtos;
using Application.Enums;
using Application.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{

    public class PredictorService : IPredictionService
    {
        public PredictionResultDto Calcular(List<PredictionDto> datos, PredictionType tipo)
        {
            return tipo switch
            {
                PredictionType.SmaCrossover => MapearSma(datos),
                PredictionType.Regression => MapearRegression(datos),
                PredictionType.Momentum => MapearMomentum(datos),
                _ => throw new Exception("Tipo no soportado")
            };
        }

        
        // MAPEOS

        private PredictionResultDto MapearSma(List<PredictionDto> datos)
        {
            var sma = CalcularSma(datos);

            var tendencia = sma.SmaCorta > sma.SmaLarga ? "Alcista" : "Bajista";

            return new PredictionResultDto
            {
                Modo = "SMA Crossover",
                ResultadoFinal = sma.SmaCorta,

                DatosTecnicos = sma
            };
        }

        private PredictionResultDto MapearRegression(List<PredictionDto> datos)
        {

            var reg = CalcularRegression(datos);

            var tendenciaPorPendiente =
                reg.Pendiente > 0 ? "Alcista" :
                reg.Pendiente < 0 ? "Bajista" : "Lateral";

            if (tendenciaPorPendiente == "Lateral")
            {
                var ultimo = datos.OrderBy(d => d.Fecha).Last().Valor;
                tendenciaPorPendiente = reg.Prediccion > ultimo ? "Alcista" :
                                        reg.Prediccion < ultimo ? "Bajista" :
                                        "Lateral";
            }

            return new PredictionResultDto
            {
                Modo = "Regresión",
                ResultadoFinal = reg.Prediccion,
                Tendencia = tendenciaPorPendiente,  
                DatosTecnicos = reg
            };

        }

        private PredictionResultDto MapearMomentum(List<PredictionDto> datos)
        {
            var roc = CalcularRoc(datos, periodo: 5);

            return new PredictionResultDto
            {
                Modo = "Momentum (ROC)",
                ResultadoFinal = 0,
                DatosTecnicos = roc
            };
        }
        
        // CALCULOS
        
        private SmaCrossoverDto CalcularSma(List<PredictionDto> datos)
        {
            var ordenadosDesc = datos.OrderByDescending(d => d.Fecha).ToList();

            if (ordenadosDesc.Count < 20)
                throw new InvalidOperationException("Se requieren al menos 20 datos para SMA Crossover.");

            var smaCorta = ordenadosDesc.Take(5).Average(x => x.Valor);  
            var smaLarga = ordenadosDesc.Take(20).Average(x => x.Valor);  

            return new SmaCrossoverDto
            {
                SmaCorta = smaCorta,
                SmaLarga = smaLarga,
            };
        }

        private RegressionDto CalcularRegression(List<PredictionDto> datos)
        {
            var asc = datos.OrderBy(d => d.Fecha).ToList();
            int n = asc.Count;
            if (n < 2)
                throw new InvalidOperationException("Se requieren al menos 2 datos para Regresión Lineal.");

            double sumX = 0, sumY = 0, sumXY = 0, sumXX = 0;
            for (int i = 0; i < n; i++)
            {
                double x = i + 1;
                double y = asc[i].Valor;
                sumX += x;
                sumY += y;
                sumXY += x * y;
                sumXX += x * x;
            }

            double denom = n * sumXX - (sumX * sumX);
            if (denom == 0)
                throw new InvalidOperationException("No es posible ajustar la regresión con los datos proporcionados.");

            double m = (n * sumXY - (sumX * sumY)) / denom;      
            double b = (sumY - m * sumX) / n;                      

            double xFuturo = n + 1;
            double yPred = m * xFuturo + b;

            return new RegressionDto
            {
                Pendiente = m,
                Intercepto = b,
                Prediccion = yPred
            };
        }
        private RocResultDto CalcularRoc(List<PredictionDto> datos, int periodo)
        {
            var asc = datos.OrderBy(d => d.Fecha).ToList();
            int n = asc.Count;
            if (n < 1)
                throw new InvalidOperationException("No hay datos suficientes para calcular ROC.");

            var result = new RocResultDto { Periodo = periodo };

            for (int t = 0; t < n; t++)
            {
                var punto = new RocPointDto
                {
                    T = t,
                    Fecha = asc[t].Fecha,
                    Precio = asc[t].Valor,
                    RocPorciento = null
                };

                if (t >= periodo)
                {
                    double vt = asc[t].Valor;
                    double vt_n = asc[t - periodo].Valor;
                    if (vt_n != 0)
                    {
                        punto.RocPorciento = (vt / vt_n - 1.0) * 100.0;
                    }
                    else
                    {
                        punto.RocPorciento = null;
                    }
                }

                result.Puntos.Add(punto);
            }

            return result;
        }
    }

}
