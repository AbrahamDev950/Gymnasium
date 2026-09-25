namespace Gym.Application.Features.Socio.Services;

public interface IAlmacenadorArchivos
{
    Task BorrarArchivoAsync(string ruta, string contenedor);
    Task <string> Almacenar(string contenedor, IFormFile archivo);

    async Task<string> Editar(string contenedor, IFormFile archivo, string ruta)
    {
        await BorrarArchivoAsync(ruta, contenedor);
        return await Almacenar(contenedor, archivo);
    }
}