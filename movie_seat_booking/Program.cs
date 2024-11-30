using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using movie_seat_booking.Models;
using movie_seat_booking.Services;
using Stripe;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("conn")));

                                    

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();
// Optional: Add token providers for things like password reset

//// Add Application Services (if you have them)
//builder.Services.AddScoped<IService, Service>();

//builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

//// Register IEmailSender as a transient service
//builder.Services.AddTransient<IEmailSender, EmailSender>();
//builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
//builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.AddSingleton<StripeClient>(new StripeClient(builder.Configuration["Stripe:SecretKey"]));

builder.Services.AddSingleton<SmsService>();

builder.Services.AddControllersWithViews();

var app = builder.Build();
// Call CreateRolesAsync during startup
await CreateRolesAsync(app);
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
static async Task CreateRolesAsync(WebApplication app)
{
    using (var scope = app.Services.CreateScope())
    {
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        string[] roleNames = { "Admin", "Customer" };
        foreach (var roleName in roleNames)
        {
            var roleExist = await roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }
    }
}

/// This method will create roles and default users at app startup
//static async Task CreateRolesAsync(WebApplication app)
//{
//    // Create a new scope to resolve scoped services
//    using (var scope = app.Services.CreateScope())
//    {
//        // Now, resolve the scoped services within the scope
//        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
//        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

//        // Create roles if they don't exist
//        string[] roleNames = { "Admin", "Customer" };
//        foreach (var roleName in roleNames)
//        {
//            var roleExist = await roleManager.RoleExistsAsync(roleName);
//            if (!roleExist)
//            {
//                await roleManager.CreateAsync(new IdentityRole(roleName));
//            }
//        }

//        // Create default admin user if it doesn't exist
//        var defaultAdmin = await userManager.FindByEmailAsync("admin@example.com");
//        if (defaultAdmin == null)
//        {
//            var admin = new ApplicationUser
//            {
//                UserName = "admin@example.com",
//                Email = "admin@example.com",
//                FullName = "Admin User",
//                Age = 30,
//                Sex = "Male"
//            };
//            var result = await userManager.CreateAsync(admin, "Admin1234!");  // Set a strong password here
//            if (result.Succeeded)
//            {
//                await userManager.AddToRoleAsync(admin, "Admin");
//            }
//        }

//        // Optionally, create a default customer if you want
//        // You can add a default customer if required
//    }
//}
