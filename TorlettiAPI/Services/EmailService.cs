using System.Net;
using System.Net.Mail;

namespace CatalogoAPI.Services;

public class EmailService(IConfiguration config)
{
    public async Task EnviarBienvenidaAsync(string emailDestino, string razonSocial)
    {
        using var client = CrearCliente();

        var mensaje = new MailMessage
        {
            From       = new MailAddress(config["Email:Remitente"]!, "Torletti Hidraulicos"),
            Subject    = "¡Tu cuenta fue aprobada!",
            IsBodyHtml = true,
            Body       = $"""
                <h1>Bienvenido, {razonSocial}</h1>
                <p>Tu solicitud de acceso fue aprobada.</p>
                <p>Ya podés ingresar a la web con tu email y contraseña.</p>
                """
        };
        mensaje.To.Add(emailDestino);
        await client.SendMailAsync(mensaje);
    }

    private SmtpClient CrearCliente() => new(config["Email:SmtpHost"]!, int.Parse(config["Email:SmtpPort"]!))
    {
        Credentials = new NetworkCredential(config["Email:Usuario"], config["Email:Password"]),
        EnableSsl   = true
    };
}
