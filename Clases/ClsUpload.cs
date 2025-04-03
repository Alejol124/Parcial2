using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace AppSerWebParcial2.Clases
{
    public class ClsUpload
    {
        public HttpRequestMessage request { get; set; }
        public string datos { get; set; }
        public string proceso { get; set; }

        //Proceso asincrono: Es el que no requiero una respuesta para poder continuar
        //Proceso sincrono: Es el que requiero una respuesta para poder continuar
        public async Task<HttpResponseMessage> GrabarArchivo(bool Actualizar)
        {
            string RptaError = "";
            if (!request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(System.Net.HttpStatusCode.UnsupportedMediaType);
            }
            //Acá crearemos la carpeta donde se guardará el archivo y asignamos la ruta
            string root = HttpContext.Current.Server.MapPath("~/Archivos");
            //Me permite gestionar toda la informacion que hay en el flujo de datos
            var provider = new MultipartFormDataStreamProvider(root);
            try
            {
                //Vamos a leer los datos
                await request.Content.ReadAsMultipartAsync(provider);
                List<string> Archivos = new List<string>();
                foreach (MultipartFileData file in provider.FileData)
                {
                    string fileName = file.Headers.ContentDisposition.FileName;
                    if (fileName.StartsWith("\"") && fileName.EndsWith("\""))
                    {
                        fileName = fileName.Trim('"');
                    }
                    if (fileName.Contains(@"/") || fileName.Contains(@"\"))
                    {
                        fileName = Path.GetFileName(fileName);
                    }
                    if (File.Exists(Path.Combine(root, fileName)))
                    {
                        if (Actualizar)
                        {
                            //Se borra el original
                            File.Delete(Path.Combine(root, fileName));
                            //Se crea el nuevo archivo con el mismo nombre
                            File.Move(file.LocalFileName, Path.Combine(root, fileName));
                            //No se debe agregar en la base de datos, porque ya existe
                        }
                        else
                        {
                            //No se pueden tener archivos con el mismo nombre. Es decir, las imágenes tienen que tener nombres únicos
                            //Si el archivo existe, se borra el archivo temporal que se subió
                            File.Delete(file.LocalFileName);
                            //Se da una respuesta de error
                            RptaError += "El archivo: " + fileName + " ya existe";
                            //return request.CreateErrorResponse(HttpStatusCode.Conflict, "El archivo ya existe");
                        }
                    }
                    else
                    {
                        Archivos.Add(fileName);
                        //Se renombra el archivo
                        File.Move(file.LocalFileName, Path.Combine(root, fileName));
                    }
                }
                if (Archivos.Count > 0)
                {
                    //Envía a grabar la información de las imágenes
                    string Respuesta = ProcesarArchivos(Archivos);
                    //Se da una respuesta de éxito
                    return request.CreateResponse(HttpStatusCode.OK, "Archivo subido con éxito");
                }
                else
                {
                    if (Actualizar)
                    {
                        return request.CreateResponse(HttpStatusCode.OK, "Archivo actualizado con éxito");
                    }
                    else
                    {
                        return request.CreateErrorResponse(HttpStatusCode.Conflict, "El(los) archivo(s) ya existe(n)");
                    }
                }
            }
            catch (Exception ex)
            {
                return request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Error al cargar el archivo: " + ex.Message);
            }
        }

        public HttpResponseMessage ConsultarArchivo(string nombreArchivo)
        {
            try
            {
                string ruta = HttpContext.Current.Server.MapPath("~/Archivos/");
                string archivo = Path.Combine(ruta, nombreArchivo);
                if (File.Exists(archivo))
                {
                    HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                    var stream = new FileStream(archivo, FileMode.Open, FileAccess.Read);
                    response.Content = new StreamContent(stream);
                    response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
                    response.Content.Headers.ContentDisposition.FileName = nombreArchivo;
                    response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
                    return response;
                }
                else
                {
                    return request.CreateErrorResponse(HttpStatusCode.NotFound, "El archivo no encontrado");
                }
            }
            catch (Exception ex)
            {
                return request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Error al cargar el archivo" + ex.Message);
            }
        }

        private string ProcesarArchivos(List<string> Archivos)
        {
            switch (proceso.ToUpper())
            {
                case "PRODUCTO":
                    ClsImagenes ImagenesProducto = new ClsImagenes();
                    ImagenesProducto.idInfraccion = datos;//Debe venir la información que se proceso en la BD, para nuestro caso el codigo del producto
                    ImagenesProducto.Archivos = Archivos;
                    return ImagenesProducto.GrabarImagenes();
                default:
                    return "Proceso no definido";
            }
        }

        public HttpResponseMessage EliminarArchivo(string nombreArchivo)
        {
            try
            {
                string ruta = HttpContext.Current.Server.MapPath("~/Archivos/");
                string archivo = Path.Combine(ruta, nombreArchivo);

                if (File.Exists(archivo))
                {
                    File.Delete(archivo);
                    return request.CreateResponse(HttpStatusCode.OK, "Archivo eliminado correctamente");
                }
                else
                {
                    return request.CreateErrorResponse(HttpStatusCode.NotFound, "El archivo no existe");
                }
            }
            catch (Exception ex)
            {
                return request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Error al eliminar el archivo: " + ex.Message);
            }
        }
    }
}