var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<lw_Confirmation_of_received_telegram.Services.Connect_PostgreSQL>();

// 食事一覧の供給元。現在は仮実装（目次ハードコード＋肉付けは実DB）。
// 上司の meal_plan 相当テーブルができたら DbMealPlanProvider を作り、この1行を差し替える
builder.Services.AddSingleton<lw_Confirmation_of_received_telegram.Services.IMealPlanProvider,
    lw_Confirmation_of_received_telegram.Services.FakeMealPlanProvider>();

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
