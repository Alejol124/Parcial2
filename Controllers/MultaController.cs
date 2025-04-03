using AppSerWebParcial2.Clases;
using AppSerWebParcial2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AppSerWebParcial2.Controllers
{
    [RoutePrefix("api/Multas")]
    public class MultaController : ApiController
    {
        [Route("ConsultarImagenesPorPlaca")]
        [HttpGet]
        public IQueryable ConsultarMultasPorPlaca(string placa)
        {
            ClsMulta multa = new ClsMulta();
            return multa.ConsultarMultasPorPlaca(placa);
        }

        [Route("ConsultarPorPlaca")]
        [HttpGet]
        public Vehiculo Consultar(string placa)
        {
            ClsMulta multa = new ClsMulta();
            return multa.Consultar(placa);
        }

        [Route("InsertarFotoMulta")]
        [HttpPost]
        public string InsertarFotoMulta([FromBody] ClsMulta multa)
        {
            return multa.InsertarFotoMulta(multa.vehiculo, multa.infraccion);
        }
    }
}