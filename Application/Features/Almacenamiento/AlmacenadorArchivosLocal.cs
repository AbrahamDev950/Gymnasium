using Gym.Application.Features.Socio.Services;

namespace Gym.Application.Features.Almacenamiento;

public class AlmacenadorArchivosLocal : IAlmacenadorArchivos
{
    private readonly IWebHostEnvironment env;
    private readonly IHttpContextAccessor accessor;

    public AlmacenadorArchivosLocal(IWebHostEnvironment env, IHttpContextAccessor accessor)
    {
        this.env = env;
        this.accessor = accessor;
    }
    public Task BorrarArchivoAsync(string ruta, string contenedor)
    {
        if(string.IsNullOrEmpty(ruta))
        {
            return Task.CompletedTask;
        }
        var nombreArchivo = Path.GetFileName(ruta);
        var directorioArchivo = Path.Combine(env.WebRootPath, contenedor, nombreArchivo);
        if (File.Exists(directorioArchivo))
        {
            File.Delete(directorioArchivo);
        }
        return Task.CompletedTask;
    }

    public async Task<string> Almacenar(string contenedor, IFormFile archivo)
    {
        var extension = Path.GetExtension(archivo.FileName);
        var nombreArchivo = $"{Guid.NewGuid()}{extension}";
        string folder = Path.Combine(env.WebRootPath, contenedor);
        if (!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
        }
        var rutaCompleta = Path.Combine(folder, nombreArchivo);
        using (var ms = new MemoryStream())
        {
            await archivo.CopyToAsync(ms);
            var contenido = ms.ToArray();
            await File.WriteAllBytesAsync(rutaCompleta, contenido);
        }
        var request = accessor.HttpContext!.Request;
        var url = $"{request.Scheme}://{request.Host}/{contenedor}/{nombreArchivo}";
        return url;
    }
}