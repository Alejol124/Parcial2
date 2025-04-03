using AppSerWebParcial2.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Linq;
using System.Web;

namespace AppSerWebParcial2.Clases
{
    public class ClsMulta
    {
        private DBExamenEntities dbExamen = new DBExamenEntities();

        //Consultar FotoMultasPorPlaca
        public IQueryable ConsultarMultasPorPlaca(string placa)
        {
            return from V in dbExamen.Set<Vehiculo>()
                   join I in dbExamen.Set<Infraccion>()
                   on V.Placa equals I.PlacaVehiculo
                   join F in dbExamen.Set<FotoInfraccion>()
                   on I.idFotoMulta equals F.idInfraccion
                   where V.Placa == placa
                   select new
                   {
                       V.Placa,
                       V.TipoVehiculo,
                       V.Marca,
                       V.Color,
                       I.FechaInfraccion,
                       I.TipoInfraccion,
                       F.NombreFoto
                   };
        }

        //ConsultarPorPlaca
        public Vehiculo Consultar(string placa)
        {
            Vehiculo vehiculo = dbExamen.Vehiculoes.FirstOrDefault(v => v.Placa == placa);
            return vehiculo;
        }

        //GrabarFotoMulta
        public string InsertarFotoMulta(Vehiculo veh, Infraccion infra)
        {
            try
            {
                // Buscar el vehículo por la placa
                dbExamen.Vehiculoes.FirstOrDefault(v => v.Placa == veh.Placa);

                if (veh == null)
                {
                    dbExamen.Vehiculoes.Add(veh);
                    dbExamen.SaveChanges(); // Guardamos para obtener el ID del vehículo
                }

                // Registramos la infracción
                dbExamen.Infraccions.Add(infra);
                dbExamen.SaveChanges(); // Guardamos para obtener el ID de la infracción
            }
            catch (Exception ex)
            {

                return "Error al guardar la multa " + ex.ToString();
            }
          return "Multa registrada correctamente";
        }

    }
}