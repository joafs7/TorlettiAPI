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
            From       = new MailAddress(config["Email:Remitente"]!, "Torletti"),
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

    public async Task EnviarCreacionAdminAsync(string emailDestino, string nombreCompleto)
    {
        using var client = CrearCliente();

        var mensaje = new MailMessage
        {
            From = new MailAddress(config["Email:Remitente"]!, "Torletti"),
            Subject = "Cuenta de administrador creada",
            IsBodyHtml = true,
            Body = $"""
            <h1>Hola, {nombreCompleto}</h1>
            <p>Se ha creado tu cuenta de administrador en Torletti.</p>
            <p>Puedes acceder al panel desde <a href="https://localhost:44328">la aplicación</a> usando tu email.</p>
            <p>Si necesitás cambiar la contraseña, contactá al superadmin.</p>
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
