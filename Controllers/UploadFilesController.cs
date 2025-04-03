using AppSerWebParcial2.Clases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace ServiciosProyecto.Controllers
{
    [RoutePrefix("api/UploadFiles")]
    public class UploadFilesController : ApiController
    {
        [HttpPost]
        public async Task<HttpResponseMessage> GrabarArchivo(HttpRequestMessage Request, string datos, string proceso)
        {
            ClsUpload UploadFiles = new ClsUpload();
            UploadFiles.request = Request;
            UploadFiles.datos = datos;
            UploadFiles.proceso = proceso;
            return await UploadFiles.GrabarArchivo(false);
        }

        [HttpGet]
        public HttpResponseMessage Get(string nombreImagen)
        {
            ClsUpload upload = new ClsUpload();
            return upload.ConsultarArchivo(nombreImagen);
        }

        [HttpPut]
        public async Task<HttpResponseMessage> ActualizarArchivo(HttpRequestMessage Request, string datos, string proceso)
        {
            ClsUpload UploadFiles = new ClsUpload();
            UploadFiles.request = Request;
            UploadFiles.datos = datos;
            UploadFiles.proceso = proceso;
            return await UploadFiles.GrabarArchivo(true);
        }
    }
}