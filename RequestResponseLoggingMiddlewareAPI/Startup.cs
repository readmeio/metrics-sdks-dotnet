using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ReadmeDotnetCore;

namespace RequestResponseLoggingMiddlewareAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration; 
        }
        public IConfiguration Configuration { get; }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
        }
       
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.Use(async (context, next) =>
            {
                HttpRequest req = context.Request;

                //You can extract apikey from request header by key like authentication, x-api-key as
                // req.Headers["key"];

                //Or extract apikey from request body form or x-www-form-urlencoded by key as
                // req.Form["key"];

                context.Items["apiKey"] = req.Headers["key"];
                context.Items["label"] = "username / company name";
                context.Items["email"] = "email";
                await next();
            });
            app.UseMiddleware<RequestResponseLogger>();


            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //app.UseSwagger();
                //app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "RequestResponseLoggingMiddlewareAPI v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();         

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }

}
