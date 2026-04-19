var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddMemoryCache(); // store data in RAM
builder.Services.AddOutputCache();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

//app.UseResponseCaching();
//app.UseOutputCache();

app.Run();

//```
//builder.Services.AddMemoryCache();
//✔️ NO attribute
//✔️ NO middleware
// —> for maually and caching a single or some part of datacaching
//——————————————

//Client side caching 

//—>  no service needed
//just use
//app.UseResponseCaching();
// in Program.cs

//use annotation in controller 

// [ResponseCache(Duration = 60)]  // 60 sec cache
//————————————

//server Side caching
//var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddOutputCache();

//var app = builder.Build();

//app.UseOutputCache();


//Example with minimal APIs:

//app.MapGet("/data", () =>
//{
//    return DateTime.Now.ToString();
//}).CacheOutput();

//Example with controller:

//[OutputCache(Duration = 60)]
//public IActionResult Get()
//{
//    return Ok(DateTime.Now.ToString());
//}
//```

//They both are Time based Caching, not Change based caching