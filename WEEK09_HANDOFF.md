# 🤝 Alumni Tracking System — 9. Hafta Oturum Devir Özeti (Handoff)

> **Amaç:** Bu dosya, 9. hafta boyunca yapılan tüm geliştirme kararlarını, eklenen/değiştirilen dosyaları, güvenlik kurallarını ve henüz çalıştırılmamış canlı test adımlarını başka bir bilgisayardaki **Antigravity IDE** oturumuna eksiksiz aktarmak için hazırlanmıştır.  
> **Tarih:** 2026-11-25  
> **Teknoloji:** .NET 10 (ASP.NET Core MVC + Web API), Clean Architecture, PostgreSQL 16 (Docker), EF Core, BCrypt.Net-Next, JWT Bearer  
> **Proje Kök Dizini:** `Alumni/` (içinde `Alumni.slnx`, `docker-compose.yml`, `DATA_DESIGN.md`, `README.md` bulunur)

---

## 1. 📌 Tek Doğruluk Kaynakları (Single Source of Truth)
Yeni bir oturumda değişiklik yapmadan önce şu 4 dosya referans alınmalıdır:
- `DATA_DESIGN.md` (**v1.4**): Veritabanı şeması, `users.password_hash`, **Bölüm 4 RBAC Yetki Matrisi (`Who May Do What`)**, `401`/`403` sözleşmesi, `own`/`self` sahiplik kuralları.
- `README.md`: **`## Auth`** bölümü, güncel MVC/API rota tablosu ve Hafta 09 changelog.
- `brand.md` & `design.md`: **Akademik Lacivert (`#1E3A8A`) & Altın (`#D97706`)** tasarım sistemi (tüm yeni Razor görünümleri buna uyumludur).

---

## 2. ✅ Tamamlanan 8 Görev (9. Hafta — *ID Cards: Who Are You, and What May You Do*)

### Görev 1 — `DATA_DESIGN.md` Rol & Yetki Güncellemesi
- `DATA_DESIGN.md` v1.4'e yükseltildi.
- 3 temel aktör tanımlandı: **`guest`** (oturumsuz), **`Alumni` (`graduate`)** (varsayılan kayıtlı mezun), **`Admin`** (yönetici).
- `Who May Do What` matrisi, `password_hash` sütunu, `401` (*"I do not know who you are"*) vs `403` (*"I know who you are, and the answer is no"*), *"the rule is written once · the button is not the rule"* ve `own`/`self` kuralları eklendi.

### Görev 2 — Kayıt Ol (Register) Sayfası & BCrypt Şifre Güvenliği
- `BCrypt.Net-Next` (v4.2.0) paketi `Alumni.Persistence` projesine eklendi.
- `User.cs` içine `[JsonIgnore] public string PasswordHash` eklendi; `UserConfiguration.cs` içine `password_hash` (`VARCHAR(255) NOT NULL`) eşlendi.
- EF Core migration üretildi: `src/Infrastructure/Alumni.Persistence/Migrations/20261125070535_AddPasswordHashToUsers.cs`.
- `UserFactory.cs` ve `DatabaseSeeder.cs` güncellendi: Tüm seed kullanıcıları `workFactor: 12` tuzlanmış BCrypt hash alır (varsayılan seed şifresi: **`secret123`**); veritabanında boş `password_hash`'li eski kullanıcılar açılışta otomatik backfill edilir.
- `RegisterRequest`, `UserResponseDto` (`passwordHash` asla dönmez), `UserService.RegisterAsync` (rolü daima `Alumni` olarak kilitler; `"role": "Admin"` gelse bile yok sayar; opsiyonel mezun profili oluşturabilir), `POST /api/register`, `GET/POST /register` ve `Views/Auth/Register.cshtml` oluşturuldu.
- `ApiUserController` tüm kullanıcı yanıtlarında `UserResponseDto` kullanır (`GET /api/users -> no hash, ever`).

### Görev 3 — JWT Login, Logout ve `GET /api/me`
- `Microsoft.AspNetCore.Authentication.JwtBearer` (v10.0.15) paketi `Alumni.API` projesine eklendi.
- `appsettings.json` ve `.example.env` içine `Jwt` yapılandırması eklendi.
- `IJwtTokenService` & `JwtTokenService` oluşturuldu: HS256 imzalı JWT üretir; payload'da gizli veri olmadan yalnızca `sub`, `id`, `name`, `email`, `role` bulunur.
- `Program.cs` içinde `AddJwtBearer` yapılandırıldı: önce `Authorization: Bearer <token>` başlığını, yoksa tarayıcı `HttpOnly` çerezini (`alumni_auth_token`) okur.
- `UserService.AuthenticateAsync` (`BCrypt.Verify`, asla `==` değil), `POST /api/login` (`200 { token }` veya bilinmeyen e-posta/yanlış şifre için aynı `401 { "error": "invalid email or password" }`), `POST /api/logout`, `GET /api/me` (`200 { id, name, role }`), MVC `GET/POST /login`, `GET/POST /logout` ve `Views/Auth/Login.cshtml` eklendi.

### Görev 4 — Merkezi `AuthMiddleware` & Sahiplik (`own` / `self`) Kuralları
- `src/Presentation/Alumni.API/Middlewares/AuthMiddleware.cs` ve `ClaimsPrincipalExtensions.cs` oluşturuldu.
- `Program.cs` içinde `UseAuthentication()` sonrası, MVC model bağlama/validasyondan **önce** (`app.UseAuthMiddleware()`) konumlandırıldı → kart yoksa boş gövdeyle bile `400` değil **`401 { "error": "login required" }`** döner (*card before validation*).
- **Yetki Matrisi Kuralları:**
  - `see job postings` (`GET /api/jobs*`, `GET /jobs*`): `guest`, `Alumni`, `Admin` = **yes**
  - `see users` (`GET /api/users*`, `GET /users*`): `guest` = `401`/login, `Alumni`/`Admin` = **yes**
  - `sign up` (`POST /api/register`, `/register`, `POST /api/users`): `guest` & `Admin` = **yes**, oturumlu `Alumni` = **`403`**
  - `change a profile`: `Alumni` yalnızca kendi `{id}`'si (`own`, başkası için **`403`**), `Admin` = **yes**
  - `delete a user`: yalnızca `Admin` (`Alumni` = **`403`**)
  - `post a job`: `Alumni` yalnızca kendi `{id}`'si (`own`, başkası için **`403`**), `Admin` = **yes**
  - `apply to a job`: `Alumni` = **`self`** (gövdedeki `userId` yok sayılır, başvuran daima oturumdaki kullanıcıdır; kendi ilanına başvuru `400`), `Admin` = **`403`**
  - Duyuru yazma/silme & başvuru durum güncelleme: yalnızca `Admin`.

### Görev 5 — Admin Paneli (`/admin` & `/api/admin/*`)
- `AuthMiddleware` ile korundu: `Guest` → `/login` (veya API `401`), `Alumni` → **`403 Forbidden`**, `Admin` → tam erişim.
- `AdminController.cs`, `AdminDashboardViewModel.cs`, `Views/Admin/Index.cshtml` oluşturuldu: KPI kartları, kullanıcı oluşturma/silme (`Restrict`), rol atama (`Alumni` ↔ `Admin`), iş ilanı ekleme/silme (`Cascade`), başvuru durumu güncelleme/silme ve duyuru ekleme/aktif-pasif/silme.
- `ApiAdminController.cs`: `GET /api/admin/overview` ve `PATCH /api/admin/users/{id}/role`.

### Görev 6 — Rol Tabanlı Dinamik Navigasyon
- `NavigationMenuBuilder.cs` oluşturuldu; `_Layout.cshtml` ve `StaticHtmlPages.cs` üzerinden masaüstü, mobil (`.mobile-nav-bar`) ve footer menüleri tek merkezden dinamik üretilir:
  - **Guest:** `Ana Sayfa`, `İş İlanları`, `Duyurular`, `Hakkında` + `Giriş Yap` / `Kayıt Ol`
  - **Alumni:** `Ana Sayfa`, `Mezunlar`, `İş İlanları`, `Başvurularım`, `Profilim` (`/profile`), `Ayarlar` (`/settings`) + kullanıcı rozeti & `Çıkış Yap`
  - **Admin:** `Ana Sayfa`, `Mezunlar`, `İş İlanları`, `Başvurular`, `Profilim` (`/profile`), `Ayarlar` (`/settings`), altın vurgulu `Admin Panel` (`/admin`) + `Admin` rozeti & `Çıkış Yap`

### Görev 7 — Kullanıcıya Özel Sayfalar (`/profile` & `/settings`)
- `AccountSettingsDtos.cs`, `UserService` (`UpsertProfileAsync`, `UpdateOwnAccountAsync`, `ChangePasswordAsync`), `AccountController.cs`, `UserProfileViewModel.cs`, `Views/Account/Profile.cshtml`, `Views/Account/Settings.cshtml` oluşturuldu.
- `/profile`: Kendi `AlumniProfile` bilgilerini oluşturma/güncelleme (`LinkedInUrl` doğrulamalı), kendi ilanları ve başvuruları.
- `/settings`: Kendi ad/soyad/e-posta güncelleme (JWT çerezini otomatik yeniler; rol yükseltilemez) ve mevcut şifre doğrulamalı BCrypt şifre değiştirme.

### Görev 8 — `README.md` `## Auth` Bölümü
- `README.md` içine `## Auth` bölümü, tüm yeni rotalar ve Hafta 09 changelog eklendi.

---

## 3. 📂 Değişen / Eklenen Başlıca Dosyalar
- **Dokümantasyon:** `DATA_DESIGN.md`, `README.md`, `.example.env`
- **Domain & Application:**
  - `src/Core/Alumni.Domain/Entities/User.cs`
  - `src/Core/Alumni.Application/DTOs/Auth/RegisterRequest.cs`, `LoginRequest.cs`
  - `src/Core/Alumni.Application/DTOs/Users/UserResponseDto.cs`, `AccountSettingsDtos.cs`, `CreateUserRequest.cs`
  - `src/Core/Alumni.Application/Interfaces/IUserService.cs`, `IJwtTokenService.cs`
- **Persistence & Migrations:**
  - `src/Infrastructure/Alumni.Persistence/Alumni.Persistence.csproj` (`BCrypt.Net-Next`)
  - `src/Infrastructure/Alumni.Persistence/Configurations/UserConfiguration.cs`
  - `src/Infrastructure/Alumni.Persistence/Migrations/20261125070535_AddPasswordHashToUsers.cs`
  - `src/Infrastructure/Alumni.Persistence/Seed/Factories/UserFactory.cs`, `Seed/DatabaseSeeder.cs`
  - `src/Infrastructure/Alumni.Persistence/Services/UserService.cs`
- **API, Middleware & Views:**
  - `src/Presentation/Alumni.API/Alumni.API.csproj` (`Microsoft.AspNetCore.Authentication.JwtBearer`)
  - `src/Presentation/Alumni.API/Program.cs`, `appsettings.json`
  - `src/Presentation/Alumni.API/Middlewares/AuthMiddleware.cs`, `BouncerMiddleware.cs`
  - `src/Presentation/Alumni.API/Common/JwtTokenService.cs`, `ClaimsPrincipalExtensions.cs`, `NavigationMenuBuilder.cs`, `AdminDashboardViewModel.cs`, `UserProfileViewModel.cs`, `StaticHtmlPages.cs`
  - `src/Presentation/Alumni.API/Controllers/ApiAuthController.cs`, `AuthController.cs`, `AdminController.cs`, `ApiAdminController.cs`, `AccountController.cs`, `ApiUserController.cs`, `UserController.cs`, `ApiJobPostController.cs`, `JobPostController.cs`, `ApiApplicationController.cs`, `ApplicationController.cs`
  - `src/Presentation/Alumni.API/Views/Shared/_Layout.cshtml`
  - `src/Presentation/Alumni.API/Views/Auth/Register.cshtml`, `Login.cshtml`
  - `src/Presentation/Alumni.API/Views/Admin/Index.cshtml`
  - `src/Presentation/Alumni.API/Views/Account/Profile.cshtml`, `Settings.cshtml`
  - `src/Presentation/Alumni.API/Views/User/Details.cshtml`

---

## 4. ⏳ Mevcut Durum ve Yeni Bilgisayarda Yapılacak Testler
- **Derleme Durumu:** `dotnet build -c Release` tüm çözümde **0 hata** ile geçti.
- **Henüz Yapılmayan Canlı Testler:** Önceki makinede Docker Desktop Linux Engine çalışmadığı için PostgreSQL (`localhost:5432`) başlatılamadı ve canlı HTTP/tarayıcı testi ertelendi.
- **Yeni Bilgisayarda Sırasıyla Çalıştırılacaklar:**
  1. Veritabanı ve API'yi başlat (migration ve seed şifre hash'leri otomatik uygulanır):
     ```powershell
     cd Alumni
     docker compose up -d alumni-db
     dotnet run --project src/Presentation/Alumni.API
     ```
  2. `/register` üzerinden kendi hesabını oluştur (varsayılan rol: `Alumni`).
  3. Veritabanından kendine `Admin` rolü ata ve çıkış yapıp yeniden giriş yap:
     ```powershell
     docker exec -it alumni-db psql -U postgres -d alumnidb -c "UPDATE users SET role = 'Admin' WHERE email = 'SENIN_EPOSTAN';"
     ```
  4. `/admin` paneline girip kullanıcı/rol, ilan, başvuru ve duyuru verilerini manipüle et.
  5. Çıkış yap, normal bir `Alumni` hesabıyla (`ahmet.yilmaz@example.com` / `secret123`) giriş yap ve `/admin` adresine ulaşmaya çalış (**`403 Forbidden`** almalısın).
  6. `Guest`, `Alumni` ve `Admin` rollerinde dinamik navigasyonu, `/profile` ve `/settings` sayfalarını ve Postman'da `POST /api/login`, `GET /api/me`, `GET /api/users` (`no hash`), `POST /api/jobs/{id}/applications` (`userId` ignored) uç noktalarını doğrula.

---

## 5. 💬 Diğer Bilgisayardaki Antigravity IDE İçin Hazır Başlangıç Prompt'u
Diğer bilgisayarda projeyi açtığında yeni konuşmaya şu mesajla başlayabilirsin:

```text
@Alumni/WEEK09_HANDOFF.md, @Alumni/DATA_DESIGN.md ve @Alumni/README.md dosyalarını oku.
Önceki bilgisayarda 9. haftanın 8 görevini tamamladık ama Docker/PostgreSQL kapalı olduğu için canlı testleri yapamadık.
Buradan kaldığımız yerden devam edelim.
```
