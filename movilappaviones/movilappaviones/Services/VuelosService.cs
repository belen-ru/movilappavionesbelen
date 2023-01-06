using movilappaviones.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace movilappaviones.Services
{
    public class VuelosService
    {

        HttpClient cliente = new HttpClient
        {
            BaseAddress = new Uri("https://aerotec.sistemas19.com/")
        };

        public event Action<string> Error;
        void LanzarError(string mensaje)
        {
            Error?.Invoke(mensaje);
        }

        void LanzarErrorJson(string json)
        {
           string obj = JsonConvert.DeserializeObject<string>(json);
            if (obj != null)
            {
                Error?.Invoke(obj);
            }
        }

        public async Task<bool> Insert(Vuelo vuelo)
        {
            var json = JsonConvert.SerializeObject(vuelo);
            var response = await cliente.PostAsync("api/Aerolinea", new StringContent(json, Encoding.UTF8,
                "application/json"));

            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest) //BadRequest
            {
                var errores = await response.Content.ReadAsStringAsync();
                LanzarErrorJson(errores);
                return false;
            }
            return true;
        }

        public async Task<bool> Update(Vuelo vuelo)
        {
            var json = JsonConvert.SerializeObject(vuelo);
            var response = await cliente.PutAsync("api/Aerolinea", new StringContent(json, Encoding.UTF8,
                "application/json"));
            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest) //BadRequest
            {
                var errores = await response.Content.ReadAsStringAsync();
                LanzarErrorJson(errores);
                return false;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                LanzarError("No se encontro el registro del vuelo");
            }
            return true;
        }

        public async Task<bool> Delete(Vuelo vuelo)
        {
            var response = await cliente.DeleteAsync("api/Aerolinea" + vuelo.Id);
            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest) //BadRequest
            {
                var errores = await response.Content.ReadAsStringAsync();
                LanzarErrorJson(errores);
                return false;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                LanzarError("No se encontro el registro del vuelo");
            }
            return true;
        }

        public async Task<List<Vuelo>> GetVuelos()
        {
            List<Vuelo> listavuelos = new List<Vuelo>();
            var response = await cliente.GetAsync("api/Aerolinea");

            if (response.IsSuccessStatusCode) 
            {
                var json = await response.Content.ReadAsStringAsync();
                listavuelos = JsonConvert.DeserializeObject<List<Vuelo>>(json);
            }

            if (listavuelos != null)
            {
                return listavuelos;
            }
            else
            {
                return new List<Vuelo>();
            }
        }
    }
}
