var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<lw_Confirmation_of_received_telegram.Services.Connect_PostgreSQL>();

// 食事一覧の供給元。本物＝DbMealPlanProvider（目次を lw_meal_plan から取得。SELECTのみ）。
// lw_meal_plan が無い環境で動かすときは FakeMealPlanProvider（目次ハードコード）に差し替える
builder.Services.AddSingleton<lw_Confirmation_of_received_telegram.Services.IMealPlanProvider,
    lw_Confirmation_of_received_telegram.Services.DbMealPlanProvider>();

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
    pattern: "{controller=OrderLog}/{action=Index}/{id?}");

app.Run();
