using System.Linq;
using System.Net;
using System.Text.Json;

public class Program
{
    volatile static string filter = "*";


    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var app = builder.Build();

        app.Use(async (context, next) =>
        {
            if (context.Request.Path == "/favicon.ico")
                return;
            await next.Invoke();
        });

        app.Use(async (context, next) =>
        {
            if (context.Request.Path == "/list")
            {
                var files = Directory.GetFiles(app.Environment.WebRootPath, "*.svg")
                    .Select(x => Path.GetRelativePath(app.Environment.WebRootPath, x).Replace("\\", "/"))
                    .Where(x => !x.EndsWith("use_target.svg"))
                    .ToList();
                // var files = new string[] { "use.svg" };

                /*
                files.AddRange(
                    Directory.GetFiles(System.IO.Path.Combine(app.Environment.WebRootPath, "Svg12"), "*.svg", SearchOption.AllDirectories)
                    .Select(x => Path.GetRelativePath(app.Environment.WebRootPath, x).Replace("\\", "/"))
                );
                */

                //files = files.Where(x => x.EndsWith("script.svg")).ToList();

                context.Response.StatusCode = (int)HttpStatusCode.OK;
                await context.Response.WriteAsync(JsonSerializer.Serialize(files));
                return;
            }
            await next.Invoke();
        });

        app.Use(async (context, next) =>
        {
            if (context.Request.Path.StartsWithSegments("/--hint--"))
            {
                app.Logger.LogInformation("hint" + context.Request.Path.Value!["/--hint--/".Length..]);
                return;
            }
            await next.Invoke();
        });

        app.Use(async (context, next) =>
        {
            if (context.Request.Path.StartsWithSegments("/set-filter"))
            {
                filter = context.Request.Path.ToString()["/set-filter/".Length..];
                // if (filter != "")
                //    app.Logger.LogInformation("set filter : " + filter);
                return;
            }
            await next.Invoke();
        });

        app.Use(async (context, next) =>
        {
            if (filter != "*" && filter != context.Request.Path.ToString()[1..])
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                var error = "Not okej " + context.Request.Path;
                await context.Response.WriteAsync(error);
                app.Logger.LogError(error);
                return;
            }

            //if (filter != "*")
            //    app.Logger.LogInformation(context.Request.Path);

            await next.Invoke();
        });

        app.UseStaticFiles();
        
        app.Run();
    }
}