var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

// Root route: GET / returns HTML landing page
app.MapGet("/", () => 
{
    var html = """
    <!DOCTYPE html>
    <html lang="tr">
    <head>
        <meta charset="UTF-8">
        <meta name="viewport" content="width=device-width, initial-scale=1.0">
        <title>Alumni Network</title>
        <style>
            :root {
                --primary: #4F46E5;
                --primary-hover: #4338CA;
                --bg: #F9FAFB;
                --text: #111827;
                --text-light: #6B7280;
                --card-bg: #FFFFFF;
                --border: #E5E7EB;
            }
            body {
                font-family: 'Segoe UI', system-ui, -apple-system, sans-serif;
                margin: 0;
                background-color: var(--bg);
                color: var(--text);
                line-height: 1.5;
            }
            header {
                background: var(--card-bg);
                padding: 1rem 2rem;
                display: flex;
                justify-content: space-between;
                align-items: center;
                border-bottom: 1px solid var(--border);
                box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.05);
            }
            .logo {
                font-size: 1.5rem;
                font-weight: bold;
                color: var(--primary);
                text-decoration: none;
            }
            .auth-buttons button {
                margin-left: 0.5rem;
                padding: 0.5rem 1rem;
                border-radius: 0.375rem;
                font-weight: 500;
                cursor: pointer;
                border: 1px solid transparent;
                transition: all 0.2s;
            }
            .btn-login {
                background: white;
                color: var(--text);
                border-color: var(--border) !important;
            }
            .btn-login:hover {
                background: var(--bg);
            }
            .btn-register {
                background: var(--primary);
                color: white;
            }
            .btn-register:hover {
                background: var(--primary-hover);
            }
            .hero {
                text-align: center;
                padding: 5rem 1rem;
                background: linear-gradient(135deg, #EEF2FF 0%, #E0E7FF 100%);
            }
            .hero h1 {
                font-size: 2.75rem;
                margin-bottom: 1rem;
                color: var(--text);
            }
            .hero p {
                font-size: 1.125rem;
                color: var(--text-light);
                max-width: 600px;
                margin: 0 auto 2.5rem auto;
            }
            .search-container {
                display: flex;
                max-width: 600px;
                margin: 0 auto;
                box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1);
                border-radius: 0.5rem;
                overflow: hidden;
            }
            .search-container input {
                flex: 1;
                padding: 1rem 1.25rem;
                border: none;
                outline: none;
                font-size: 1rem;
            }
            .search-container button {
                padding: 0 1.5rem;
                background: var(--primary);
                color: white;
                border: none;
                font-size: 1rem;
                font-weight: 600;
                cursor: pointer;
                transition: background 0.2s;
            }
            .search-container button:hover {
                background: var(--primary-hover);
            }
            .preview-section {
                max-width: 1000px;
                margin: 4rem auto;
                padding: 0 1rem;
                margin-bottom: 5rem;
            }
            .preview-section h2 {
                text-align: center;
                margin-bottom: 2.5rem;
                font-size: 2rem;
            }
            .cards {
                display: grid;
                grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
                gap: 1.5rem;
            }
            .card {
                background: var(--card-bg);
                border-radius: 0.75rem;
                padding: 1.5rem;
                box-shadow: 0 1px 3px 0 rgba(0, 0, 0, 0.1);
                border: 1px solid var(--border);
                transition: transform 0.2s, box-shadow 0.2s;
            }
            .card:hover {
                transform: translateY(-5px);
                box-shadow: 0 10px 15px -3px rgba(0, 0, 0, 0.1);
            }
            .card h3 {
                margin: 0 0 0.25rem 0;
                font-size: 1.25rem;
            }
            .card .dept {
                color: var(--text-light);
                font-size: 0.875rem;
                margin-bottom: 1.25rem;
            }
            .card .job {
                font-weight: 500;
            }
            .tag {
                display: inline-block;
                background: #EEF2FF;
                color: var(--primary);
                padding: 0.25rem 0.75rem;
                border-radius: 9999px;
                font-size: 0.75rem;
                font-weight: 600;
                margin-top: 1rem;
            }
        </style>
    </head>
    <body>
        <header>
            <a href="#" class="logo">🎓 Alumni Network</a>
            <div class="auth-buttons">
                <button class="btn-login">Giriş Yap</button>
                <button class="btn-register">Kayıt Ol</button>
            </div>
        </header>
        
        <main>
            <section class="hero">
                <h1>Mezun Ağına Hoş Geldiniz</h1>
                <p>Üniversitemiz mezunlarıyla iletişime geçin, kariyer fırsatlarını keşfedin.</p>
                <div class="search-container">
                    <input type="text" placeholder="İsim, şirket veya mezuniyet yılı ile mezun ara...">
                    <button>Ara</button>
                </div>
            </section>

            <section class="preview-section">
                <h2>Öne Çıkan Mezunlarımız</h2>
                <div class="cards">
                    <div class="card">
                        <h3>Ahmet Yılmaz</h3>
                        <div class="dept">Bilgisayar Mühendisliği</div>
                        <div class="job">Senior Software Engineer @ TechCorp</div>
                        <span class="tag">2019 Mezunu</span>
                    </div>
                    <div class="card">
                        <h3>Ayşe Demir</h3>
                        <div class="dept">Endüstri Mühendisliği</div>
                        <div class="job">Product Manager @ InnovateAŞ</div>
                        <span class="tag">2021 Mezunu</span>
                    </div>
                    <div class="card">
                        <h3>Can Kaya</h3>
                        <div class="dept">İşletme</div>
                        <div class="job">Financial Analyst @ GlobalBank</div>
                        <span class="tag">2018 Mezunu</span>
                    </div>
                </div>
            </section>
        </main>
    </body>
    </html>
    """;
    
    return Results.Content(html, "text/html");
});

// Hello route: GET /hello returns "Hello,World!"
app.MapGet("/hello", () => "Hello,World!");

// Dynamic hello route: GET /hello/{name} returns "Hello,{name}!"
app.MapGet("/hello/{name}", (string name) => $"Hello,{name}!");

// Sum route: GET /sum/{number1:double}/{number2:double} returns sum
app.MapGet("/sum/{number1:double}/{number2:double}", (double number1, double number2) =>
{
    double sum = number1 + number2;
    return sum;
});

// About route: GET /about returns HTML about page
app.MapGet("/about", () => 
{
    var html = """
    <!DOCTYPE html>
    <html lang="tr">
    <head>
        <meta charset="UTF-8">
        <meta name="viewport" content="width=device-width, initial-scale=1.0">
        <title>Hakkında - Alumni Network</title>
        <style>
            :root {
                --primary: #4F46E5;
                --primary-hover: #4338CA;
                --bg: #F9FAFB;
                --text: #111827;
                --text-light: #6B7280;
                --card-bg: #FFFFFF;
                --border: #E5E7EB;
            }
            body {
                font-family: 'Segoe UI', system-ui, -apple-system, sans-serif;
                margin: 0;
                background-color: var(--bg);
                color: var(--text);
                line-height: 1.5;
            }
            header {
                background: var(--card-bg);
                padding: 1rem 2rem;
                display: flex;
                justify-content: space-between;
                align-items: center;
                border-bottom: 1px solid var(--border);
                box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.05);
            }
            .logo {
                font-size: 1.5rem;
                font-weight: bold;
                color: var(--primary);
                text-decoration: none;
            }
            .nav-link {
                color: var(--text);
                text-decoration: none;
                font-weight: 500;
                transition: color 0.2s;
            }
            .nav-link:hover {
                color: var(--primary);
            }
            .hero {
                text-align: center;
                padding: 4rem 1rem;
                background: linear-gradient(135deg, #EEF2FF 0%, #E0E7FF 100%);
            }
            .hero h1 {
                font-size: 2.5rem;
                margin-bottom: 1rem;
                color: var(--text);
            }
            .hero p {
                font-size: 1.125rem;
                color: var(--text-light);
                max-width: 700px;
                margin: 0 auto;
            }
            .features-section {
                max-width: 1000px;
                margin: 4rem auto;
                padding: 0 1rem;
            }
            .cards {
                display: grid;
                grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
                gap: 1.5rem;
            }
            .card {
                background: var(--card-bg);
                border-radius: 0.75rem;
                padding: 2rem 1.5rem;
                box-shadow: 0 1px 3px 0 rgba(0, 0, 0, 0.1);
                border: 1px solid var(--border);
                text-align: center;
                transition: transform 0.2s, box-shadow 0.2s;
            }
            .card:hover {
                transform: translateY(-5px);
                box-shadow: 0 10px 15px -3px rgba(0, 0, 0, 0.1);
            }
            .card-icon {
                font-size: 2.5rem;
                margin-bottom: 1rem;
            }
            .card h3 {
                margin: 0 0 1rem 0;
                font-size: 1.25rem;
                color: var(--primary);
            }
            .card p {
                color: var(--text-light);
                font-size: 0.95rem;
                margin: 0;
            }
            .contact-section {
                max-width: 800px;
                margin: 4rem auto 5rem auto;
                padding: 2rem;
                background: var(--card-bg);
                border-radius: 0.75rem;
                box-shadow: 0 1px 3px 0 rgba(0, 0, 0, 0.1);
                border: 1px solid var(--border);
                text-align: center;
            }
            .contact-section h2 {
                margin-top: 0;
            }
            .contact-section p {
                color: var(--text-light);
                margin-bottom: 0;
            }
            .email-link {
                color: var(--primary);
                font-weight: 600;
                text-decoration: none;
            }
            .email-link:hover {
                text-decoration: underline;
            }
        </style>
    </head>
    <body>
        <header>
            <a href="/" class="logo">🎓 Alumni Network</a>
            <nav>
                <a href="/" class="nav-link">Ana Sayfa</a>
            </nav>
        </header>
        
        <main>
            <section class="hero">
                <h1>Mezun Takip Platformu Hakkında</h1>
                <p>Üniversitemiz mezunları ile öğrencileri arasındaki bağı güçlendirmeyi, kariyer yolculuklarını görünür kılmayı ve köklü bir iletişim ağı inşa etmeyi amaçlıyoruz.</p>
            </section>

            <section class="features-section">
                <div class="cards">
                    <div class="card">
                        <div class="card-icon">🤝</div>
                        <h3>Mezun İletişim Ağı</h3>
                        <p>Yıllara ve bölümlere göre mezunların güncel iletişim ve şirket bilgilerine erişim sağlayarak ağınızı genişletin.</p>
                    </div>
                    <div class="card">
                        <div class="card-icon">🚀</div>
                        <h3>Kariyer ve Mentorluk</h3>
                        <p>Yeni mezun ve öğrencilere yönelik staj, iş fırsatları ve sektör rehberliği imkanlarıyla kariyerinize yön verin.</p>
                    </div>
                    <div class="card">
                        <div class="card-icon">🏢</div>
                        <h3>Üniversite & Sanayi Köprüsü</h3>
                        <p>Mezunların çalıştığı kurumlarla üniversite arasındaki iş birliklerini artırarak akademi ve sektörü buluşturuyoruz.</p>
                    </div>
                </div>
            </section>

            <section class="contact-section">
                <h2>İletişim</h2>
                <p>Kariyer Merkezi & Mezunlar Ofisi ile iletişime geçmek için: <br> 
                <a href="mailto:alumni@ogr.iu.edu.tr" class="email-link">alumni@ogr.iu.edu.tr</a></p>
            </section>
        </main>
    </body>
    </html>
    """;
    
    return Results.Content(html, "text/html");
});

app.MapControllers();

app.Run();
