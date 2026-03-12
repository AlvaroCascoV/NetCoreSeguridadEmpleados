using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using NetCoreSeguridadEmpleados.Data;
using NetCoreSeguridadEmpleados.Policies;
using NetCoreSeguridadEmpleados.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();
builder.Services.AddAuthentication
    (
        options =>
        {
            options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        }
    ).AddCookie
    (
        CookieAuthenticationDefaults.AuthenticationScheme,
        config =>
        {
            config.AccessDeniedPath = "/Managed/ErrorAcceso";
        }
    );

// Add services to the container.

builder.Services.AddControllersWithViews
    (options => options.EnableEndpointRouting = false).AddSessionStateTempDataProvider();

//las politicas se agregan con authorization
builder.Services.AddAuthorization
    (
        options =>
        {
            //debemos crear las policies que necesitemos para los roles
            options.AddPolicy("SOLOJEFES", policy => policy.RequireRole("PRESIDENTE", "DIRECTOR", "ANALISTA"));
            options.AddPolicy("AdminOnly", policy => policy.RequireClaim("Admin"));
            options.AddPolicy("SoloRicos", policy => policy.Requirements.Add(new OverSalarioRequirement()));
        }
    );

string conn = builder.Configuration.GetConnectionString("SQLHospital");
builder.Services.AddTransient<RepositoryEmpleados>();
builder.Services.AddDbContext<HospitalContext>(options => options.UseSqlServer(conn));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
//app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

//app.MapStaticAssets();

app.UseMvc(routes =>
{
    //si queremos una ruta con otro nombre en el parametro, hay que crear una ruta nueva
    //la default va siempre al final
    //routes.MapRoute(
    //    name: "defaultIdEmpleado",
    //    template: "{controller=Home}/{action=Index}/{idempleado?}");
    //routes.MapRoute(
    //    name: "otraRuta",
    //    template: "{controller=Home}/{action=Index}/{apellido?}/{oficio?}");
    routes.MapRoute(
        name: "default",
        template: "{controller=Home}/{action=Index}/{id?}");
});

//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/{id?}")
//    .WithStaticAssets();

app.Run();