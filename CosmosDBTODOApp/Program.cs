using CosmosDBTODOApp.Services;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//Initialize the client based on the configure as signleton instance to injected through Dependency Injection.
builder.Services.AddSingleton<ICosmosService>(InitializeCosmosClientInstanceAsync(
    builder.Configuration.GetSection("CosmosDb")).GetAwaiter().GetResult());

builder.Services.AddCors(cors =>
{
    cors.AddPolicy("AllowOrigin", c => c.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});


// Creates a Cosmos DB database and a container with the specified partition key.
static async Task<CosmosService> InitializeCosmosClientInstanceAsync(IConfigurationSection configuration)
{
    string databaseName = configuration.GetSection("DatabaseName").Value;
    string containerName = configuration.GetSection("ContainerName").Value;
    string account = configuration.GetSection("Account").Value;
    string key = configuration.GetSection("Key").Value;
    CosmosClient client = new CosmosClient(account, key);
    
    DatabaseResponse database = await client.CreateDatabaseIfNotExistsAsync(databaseName);
    await database.Database.CreateContainerIfNotExistsAsync(containerName, "/id");

    CosmosService service = new CosmosService(client, databaseName, containerName);

    return service;
}


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

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Items}/{action=Index}/{id?}");

app.Run();
