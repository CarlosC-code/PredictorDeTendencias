using Application.Dtos;
using Application.Enums;
using Application.Repositories;
using Application.Services;
using Application.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Reflection;

namespace PredictorDeTendencias.Controllers
{
    public class HomeController : Controller
    {
        private readonly IPredictionService _predictionService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(
            IPredictionService predictionService,
            ILogger<HomeController> logger)
        {
            _predictionService = predictionService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            var vm = new PredictorListViewModel();

            for (int i = 0; i < 20; i++)
            {
                vm.Datos.Add(new NewPredictorViewModel());
            }

            vm.ModoUsado = PredictionRepository.Instance.TipoSeleccionado;

            return View(vm);

        }


        public IActionResult Modos()
        {

            var tipo = PredictionRepository.Instance.TipoSeleccionado
                       ?? PredictionType.SmaCrossover; //Default a SMA si no hay nada

            //Persistimos el default tambien
            PredictionRepository.Instance.TipoSeleccionado = tipo;

            var vm = new ModosViewModel
            {
                //las opciones a selecionar: usan "SMA", "RL", "MOM"
                ModoSeleccionado = tipo switch
                {
                    PredictionType.SmaCrossover => "SMA",
                    PredictionType.Regression => "RL",
                    PredictionType.Momentum => "MOM",
                    _ => "SMA"
                }
            };

            return View(vm);

        }

        [HttpPost]
        public IActionResult Modo(ModosViewModel model)
        {


            if (string.IsNullOrEmpty(model.ModoSeleccionado))
                return RedirectToAction("Modos");

            PredictionType? tipo = model.ModoSeleccionado switch
            {
                "SMA" => PredictionType.SmaCrossover,
                "RL" => PredictionType.Regression,
                "MOM" => PredictionType.Momentum,
                _ => null
            };

            PredictionRepository.Instance.TipoSeleccionado = tipo;

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Calcular(PredictorListViewModel model)
        {

            // Recuperamos el modo 
            var tipo = PredictionRepository.Instance.TipoSeleccionado ?? model.ModoUsado;
            model.ModoUsado = tipo; 

            //Filtramos las filas completas 
            var datosDto = (model.Datos ?? new List<NewPredictorViewModel>())
                .Where(d => d.Fecha.HasValue && d.Valor.HasValue)
                .Select(d => new PredictionDto
                {
                    Fecha = d.Fecha!.Value,
                    Valor = d.Valor!.Value
                })
               
            
                .OrderByDescending(d => d.Fecha)
                .ToList();

            //Validamos que haya un modo
            if (tipo == null)
            {
                ModelState.AddModelError(string.Empty, "Debe seleccionar un modo.");
                return View("Index", model);
            }

            // Validamos minimo por modo
            var min = tipo.Value switch
            {
                PredictionType.SmaCrossover => 10, 
                PredictionType.Momentum => 2,  
                PredictionType.Regression => 1,
                _ => 1
            };

            if (datosDto.Count < min)
            {
                ModelState.AddModelError(string.Empty, $"Necesitas al menos {min} filas completas para {tipo}.");
                return View("Index", model);
            }

            try
            {
                var resultado = _predictionService.Calcular(datosDto, tipo.Value);
                model.Resultado = resultado;
                return View("Index", model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculando predicción");
                ModelState.AddModelError(string.Empty, "Ocurrio un error al calcular. Verifica los datos e intentalo de nuevo.");
                return View("Index", model);
            }

        }

    }
}
