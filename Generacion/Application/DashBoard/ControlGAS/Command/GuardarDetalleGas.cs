﻿using Generacion.Models;
using Generacion.Views.ControlGAS;
using MediatR;

namespace Generacion.Application.DashBoard.ControlGAS.Command
{
    public class GuardarDetalleGas : IRequest<Respuesta<string>>
    {
        public DetalleConsumoGas detalleConsumoGas { get; set; }
    }

    public class GuardarDetalleGasHandler : IRequestHandler<GuardarDetalleGas, Respuesta<string>>
    {

        private readonly IRegistroControlGas _registroControlGas;
        public GuardarDetalleGasHandler(IRegistroControlGas registroControlGas)
        {
            _registroControlGas = registroControlGas;

        }
        public async Task<Respuesta<string>> Handle(GuardarDetalleGas request, CancellationToken cancellationToken)
        {
            Respuesta<string> respuesta = new Respuesta<string>();

            respuesta = await _registroControlGas.RegistrarDetalleControl(request.detalleConsumoGas);

            return respuesta;
        }
    }
}
