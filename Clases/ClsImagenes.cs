using AppSerWebParcial2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppSerWebParcial2.Clases
{
    public class ClsImagenes
    {
        private DBExamenEntities dbExamen = new DBExamenEntities();
        public string idInfraccion { get; set; }
        public List<string> Archivos { get; set; }

        public string GrabarImagenes()
        {
            try
            {
                if (Archivos.Count > 0)
                {
                    foreach (string Archivo in Archivos)
                    {
                        FotoInfraccion imagen = new FotoInfraccion();
                        imagen.idInfraccion = Convert.ToInt32(idInfraccion);
                        imagen.NombreFoto = Archivo;
                        dbExamen.FotoInfraccions.Add(imagen);
                    }
                    dbExamen.SaveChanges();
                    return "Imagenes guardadas correctamente";
                }
                else
                {
                    return "No hay imagenes para guardar";
                }
            }
            catch (Exception ex)
            {
                return "Error al guardar las imagenes: " + ex.Message;
            }
        }
    }
}