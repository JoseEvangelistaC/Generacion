﻿using Generacion.Models.Mantenimiento;

namespace Generacion.Application.Mantenimiento
{
    public interface IMantenimiento
    {
        Task<string> GuardarDatosMotoGenerador(List<MotoGenerador> listaMotoGeneradores);
        Task<string> GuardarDatosServ(List<MantenimientoServicios> detalleServicio);
    }


}
