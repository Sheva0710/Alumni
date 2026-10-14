# 🎓 Alumni Tracking System (Mezun Takip Sistemi)

[![.NET](https://img.shields.io/badge/.NET-8.0%2F9.0-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/C%23-239120?logo=csharp&logoColor=white)](https://learn.microsoft.com/dotnet/csharp/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-316192?logo=postgresql&logoColor=white)](https://www.postgresql.org/)
[![IDE](https://img.shields.io/badge/IDE-Google%20Antigravity-4285F4?logo=google&logoColor=white)](https://antigravity.google)
[![Methodology](https://img.shields.io/badge/Methodology-Vibe%20Coding-FF6F61)](https://github.com)

Üniversite mezunlarının kariyer yolculuklarını, çalıştıkları sektör ve şirketleri, edindikleri yetenekleri ve mezunlar arası iletişim ağını tek bir kurumsal çatı altında ilişkisel olarak takip eden modern web platformu.

---

## 📌 İçindekiler
- [Proje Kapsamı ve Temel Modüller](#-proje-kapsamı-ve-temel-modüller)
- [Kritik İş Kuralları (Business Rules)](#-kritik-iş-kuralları-business-rules)
- [Kapsam Dışı (Out of Scope)](#-kapsam-dışı-out-of-scope)
- [Teknoloji Yığını ve Tercih Nedenleri](#-teknoloji-yığını-ve-tercih-nedenleri)
- [Zayıf Yönler ve Risk Analizi](#-zayıf-yönler-ve-risk-analizi)
- [Geliştirme Metodolojisi](#-geliştirme-metodolojisi)
- [Önerilen Sistem Mimarisi](#-önerilen-sistem-mimarisi)
- [Proje Dizin Yapısı ve Dağıtılmış MVC Mimarisi](#-proje-dizin-yapısı-ve-dağıtılmış-mvc-mimarisi)
- [Veritabanı ve Veri Tasarımı (Data Design)](#-veritabanı-ve-veri-tasarımı-data-design)
- [Mevcut Uç Noktalar (Aktif Rotalar)](#-mevcut-uç-noktalar-aktif-rotalar)
- [Kurulum ve Başlangıç](#-kurulum-ve-başlangıç)
- [Geliştirme Süreci (Changelog)](#-geliştirme-süreci-changelog)

---

## 🎯 Proje Kapsamı ve Temel Modüller
Alumni Tracking System, mezunların kariyer süreçlerini, çalıştıkları şirketleri, yeteneklerini (skills) ve iletişim ağlarını takip eden kurumsal bir backend platformudur. Temel modüller:
1. **Mezun Profil ve Özgeçmiş Modülü:** Mezuniyet yılı, fakülte, bölüm, iletişim ve ağ profilleri.
2. **Kariyer ve İstihdam Geçmişi Modülü:** Çalışılan kurumlar, unvanlar, sektör bilgisi ve yetenek eşleştirmeleri.
3. **Doğrulama ve Yetkilendirme Mekanizması:** Yönetici / Kariyer Merkezi onay iş akışı (`Status: Pending, Approved, Rejected`).
4. **İlişkisel Arama ve Filtreleme:** Bölüm, şirket, unvan ve yetenek kriterlerine göre dinamik arama.
5. **İstihdam Analitiği ve İstatistik Raporlama Modülü:** Bölüm bazlı istihdam oranı, ortalama işe giriş süresi ve sektörel dağılım analizleri.

## 📋 Kritik İş Kuralları (Business Rules)
* **Doğrulanmış Profil Kuralı:** Yalnızca yönetici tarafından onaylanmış (`Status = Approved`) profiller istatistiklerde ve genel aramalarda listelenir.
* **Aktif İstihdam Bütünlüğü:** Bir mezunun birden fazla geçmiş iş deneyimi olabilir ancak aynı anda sadece bir adet güncel/aktif (`IsCurrent = true`) iş kaydı tutulabilir.
* **İlişkisel Normalizasyon:** Şirket, bölüm ve sektör tanımları serbest metin değil, veri bütünlüğü için ilişkisel tablolar üzerinden yönetilir.

## 🚫 Kapsam Dışı (Out of Scope)
* Canlı anlık mesajlaşma (Real-time Chat), üçüncü parti ödeme altyapıları ve mobil bildirim mekanizmaları ilk faz kapsamı dışındadır.

---

## 🛠 Teknoloji Yığını ve Tercih Nedenleri

| Katman | Teknoloji | Temel Tercih Nedenleri |
| :--- | :--- | :--- |
| **Backend** | **C# (ASP.NET Core)** | • **Katı Tip Güvenliği:** Derleme zamanı hata denetimi ve güçlü tip sistemi.<br>• **Kurumsal Mimari Standartları:** Clean Architecture, Repository Pattern ve Dependency Injection desteği.<br>• **Entity Framework Core:** Güçlü ORM yetenekleri, LINQ sorgu desteği ve gelişmiş migration yönetimi.<br>• **Yüksek API Performansı:** Kestrel web sunucusu ile yüksek eşzamanlı istek işleme kapasitesi. |
| **Veritabanı** | **PostgreSQL** | • **ACID Uyumluluğu:** Mezun, şirket, yetenek ve iletişim kayıtlarında tam veri güvenilirliği ve işlem tutarlılığı.<br>• **İlişkisel Bütünlük:** Foreign Key kısıtlamaları ile karmaşık kurumsal veri modellerinin hatasız yönetimi.<br>• **JSONB Desteği:** Mezunların dinamik profil alanları, sosyal medya linkleri veya ek özgeçmiş detayları için esnek doküman saklama imkânı. |

---

## ⚠️ Zayıf Yönler ve Risk Analizi

Her mimari kararın beraberinde getirdiği ödünleşimler (trade-offs) bulunur. Proje sürecinde dikkat edilmesi gereken başlıca riskler:

### 1. C# / ASP.NET Core
- **Yüksek Boilerplate (Hazır Kod) İhtiyacı:** Katı mimari katmanları (Controllers, DTOs, Entities, Repositories, Mappings) başlangıçta geliştirme hızını yavaşlatabilir.
- **Katı Mimari ve Öğrenme Eğrisi:** Hızlı prototiplemeye kıyasla kurumsal kalıplara bağlı kalma zorunluluğu esnekliği sınırlayabilir.

### 2. PostgreSQL
- **Katı Migration Bağımlılığı:** Veri tabanı şemasındaki değişiklikler Entity Framework Core migration'ları üzerinden titizlikle yönetilmelidir. Uyumsuz şema güncellemeleri canlı ortamlarda veri tutarsızlığı veya kesinti riski doğurabilir.
- **İlişkisel Karmaşıklık:** Çoktan-çoğa (Many-to-Many) mezun-yetenek ve mezun-şirket ilişkilerinin indeksleme ve sorgu optimizasyonu (N+1 query problemi vb.) dikkatle ele alınmalıdır.

---

## 🚀 Geliştirme Metodolojisi

Proje, modern yazılım geliştirme pratiklerini yapay zekâ destekli araçlarla birleştiren **Vibe Coding** felsefesiyle yürütülmektedir:

- **Google Antigravity IDE ile Vibe Coding:**
  - Akıllı kod asistanı, test senaryosu üretimi ve anlık mimari doğrulama.
  - Hızlı prototipleme, refactoring ve temiz kod standartlarının sürekli denetimi.
- **Haftalık Git Commit ve Branch Takibi:**
  - `main` dalı daima kararlı ve test edilmiş sürümleri barındırır.
  - Özellik geliştirmeleri için `feature/<ozellik-adi>` veya `feat/<ozellik-adi>` dalları kullanılır.
  - Commit mesajları **Conventional Commits** (`feat:`, `fix:`, `docs:`, `refactor:`, `test:`) standardına uygun yazılır.

---

## 📐 Önerilen Sistem Mimarisi

```
Alumni Tracking System
 ├── src/
 │    ├── Core/
 │    │    ├── Alumni.Domain/         # Varlıklar (Entities), Enums, Arayüzler
 │    │    └── Alumni.Application/    # Servisler, CQRS/Handlers, DTOs, Validations
 │    ├── Infrastructure/
 │    │    ├── Alumni.Persistence/    # EF Core, PostgreSQL DbContext, Migrations
 │    │    └── Alumni.Infrastructure/ # E-posta, Dosya Yönetimi, Harici Entegrasyonlar
 │    └── Presentation/
 │         └── Alumni.API/            # ASP.NET Core Web API, Controllers, Middleware
 └── tests/
      ├── Alumni.UnitTests/
      └── Alumni.IntegrationTests/
```

---

## 🏗 Proje Dizin Yapısı ve Dağıtılmış MVC Mimarisi

Projemiz tek bir projeye sıkıştırılmış basit bir MVC (Model-View-Controller) mimarisi değildir. Kurumsal ihtiyaçlara cevap verebilmesi adına **Clean Architecture** standartlarına göre katmanlara ayrılmış bir MVC (**Distributed MVC**) modeli benimsemiştir. Geleneksel MVC bileşenleri, sorumlulukların ayrılığı (Separation of Concerns) prensibi gereği farklı katmanlara dağıtılmıştır.

```text
Alumni/
├── src/
│   ├── Core/
│   │   ├── Alumni.Domain/           -> [MODEL] Ana iş varlıkları (Entities - User.cs, AlumniProfile.cs, JobPost.cs, Announcement.cs)
│   │   └── Alumni.Application/      -> [MODEL] DTOs, CRUD Servis Arayüzleri (IUserService, IAnnouncementService)
│   ├── Infrastructure/
│   │   ├── Alumni.Infrastructure/   -> [ALTYAPI] Dış servis entegrasyonları
│   │   └── Alumni.Persistence/      -> [ALTYAPI] Veritabanı (PostgreSQL, DbContext, Migrations, Seed & Factories, CRUD Servisleri)
│   └── Presentation/
│       └── Alumni.API/
│           ├── Controllers/         -> [CONTROLLER] MVC (UserController, AnnouncementController) ve API (ApiUserController, ApiAnnouncementController)
│           ├── Views/               -> [VIEW] Razor View arayüzleri (User: Index, Details, Edit | Announcement: Index, Details, Edit)
│           ├── Program.cs           -> Uygulama başlangıç yapılandırması, DI ve Middleware
│           └── Dockerfile           -> API Konteyner inşası
├── docker-compose.yml               -> Proje orkestrasyonu
└── README.md
```

- **Model (M):** Monolitik bir yapı yerine ikiye ayrılmıştır. `Alumni.Domain` veritabanı şemasını ve iş varlıklarını (Entity - `User.cs`, `AlumniProfile.cs`, `JobPost.cs`, `Announcement.cs`) tutarken, `Alumni.Application` veri transfer objelerini (DTO) ve CRUD servis arayüzlerini (`IUserService`, `IAnnouncementService`) yönetir. Veritabanı işlemleri `Alumni.Persistence` altındaki DbContext ve servisler üzerinden yürütülür.
- **View (V):** İki farklı sunum katmanından oluşur:
  1. **Razor HTML Views (`Views/`):** Sunucu taraflı (SSR) çalışan kullanıcı yönetim arayüzleri (`Views/User/`) ve duyuru yönetim arayüzleri (`Views/Announcement/`).
  2. **API & Dokümantasyon Arayüzü:** `/api/users` ve `/api/announcements` üzerinden dönülen JSON veri çıktıları ve `/api/swagger` (Scalar) arayüzü.
- **Controller (C):** `src/Presentation/Alumni.API/Controllers/` dizini altında hem MVC hem de API controller'ları ile yönetilir:
  1. `UserController` & `AnnouncementController`: Razor View sayfalarını ve form işlemlerini yöneten MVC controller'ları (`/users`, `/announcements`).
  2. `ApiUserController` & `ApiAnnouncementController`: RESTful JSON API isteklerini yöneten API controller'ları (`/api/users`, `/api/announcements`).

---

## 📊 Veritabanı ve Veri Tasarımı (Data Design)

Mezun Takip Sistemi'nin ilişkisel veri mimarisi, PostgreSQL 16 veritabanı motoru ve Entity Framework Core ORM altyapısı temel alınarak modellenmiştir. Sistemdeki tüm tablolar, varlık modelleri (Entities), kardinaliteler, kısıtlamalar, indeksler ve silme davranışları [DATA_DESIGN.md](./DATA_DESIGN.md) dokümanında detaylandırılmıştır.

> 🤖 **AI / Antigravity Assistant Directive (Data Single Source of Truth):**
> All entity models, DTOs, migrations, and database schema modifications MUST strictly reference and adhere to [DATA_DESIGN.md](./DATA_DESIGN.md). Whenever a database table, entity, property, or relationship is added, updated, or deleted in the codebase, the AI assistant MUST keep `DATA_DESIGN.md` synchronized and updated as the single source of truth.

---

## 🌐 Dokümantasyon / API Uç Noktaları

* **API Swagger / OpenAPI dokümantasyonuna `/api/swagger` üzerinden erişilebilir.**
* **Dinamik Dokümantasyon İlkesi:** Projeye eklenen tüm API rotaları, `EndpointsApiExplorer` ve Swagger altyapısı sayesinde herhangi bir manuel müdahaleye gerek kalmadan `/api/swagger` arayüzüne otomatik olarak yansıtılmaktadır.

Sistemde halihazırda çalışan HTTP uç noktaları:
| HTTP Metodu | Uç Nokta (Route) | Açıklama |
|---|---|---|
| `GET` | `/` | Web Vitrini / Ana Sayfa Prototipi |
| `GET` | `/about` | Platform Misyonu ve Hakkında Sayfası |
| `GET` | `/hello` | Temel Çalışma Doğrulama Testi |
| `GET` | `/hello/{name}` | Dinamik Route Parametresi Testi |
| `GET` | `/sum/{n1}/{n2}` | Sayısal Hesaplama ve Parametre Doğrulama Testi |
| `GET` | `/api/health` | API Sağlık Durumu |
| `GET` | `/users` | Kullanıcı Listeleme ve Ekleme Formu (MVC Razor View) |
| `POST` | `/users` | Yeni Kullanıcı Ekleme (MVC Form POST) |
| `GET` | `/users/details/{id}` | Kullanıcı Detay Görüntüleme (MVC View) |
| `GET` | `/users/edit/{id}` | Kullanıcı Düzenleme Formu (MVC View) |
| `POST` | `/users/edit/{id}` | Kullanıcı Bilgilerini Güncelleme (MVC Form POST) |
| `POST` | `/users/delete/{id}` | Kullanıcı Kaydını Silme (MVC Form POST) |
| `GET` | `/announcements` | Duyuru Listeleme ve Ekleme Formu (MVC Razor View) |
| `POST` | `/announcements` | Yeni Duyuru Ekleme (MVC Form POST) |
| `GET` | `/announcements/details/{id}` | Duyuru Detay Görüntüleme (MVC View) |
| `GET` | `/announcements/edit/{id}` | Duyuru Düzenleme Formu (MVC View) |
| `POST` | `/announcements/edit/{id}` | Duyuru Güncelleme (MVC Form POST) |
| `POST` | `/announcements/delete/{id}` | Duyuru Silme (MVC Form POST) |
| `GET` | `/api/users` | Tüm Kullanıcıları Listeleme (JSON API) |
| `GET` | `/api/users/{id}` | ID'ye Göre Tekil Kullanıcı Getirme (JSON API) |
| `POST` | `/api/users` | Yeni Kullanıcı Ekleme (JSON API) |
| `PUT` | `/api/users/{id}` | Kullanıcı Tam Güncelleme (JSON API) |
| `PATCH`| `/api/users/{id}` | Kullanıcı Kısmi Güncelleme (JSON API) |
| `DELETE`| `/api/users/{id}`| Kullanıcı Silme (JSON API) |
| `GET` | `/api/announcements` | Tüm Duyuruları Listeleme (JSON API) |
| `GET` | `/api/announcements/{id}` | ID'ye Göre Tekil Duyuru Getirme (JSON API) |
| `POST` | `/api/announcements` | Yeni Duyuru Ekleme (JSON API) |
| `PUT` | `/api/announcements/{id}` | Duyuru Güncelleme (JSON API) |
| `DELETE`| `/api/announcements/{id}` | Duyuru Silme (JSON API) |

---

## 🚀 Kurulum ve Başlangıç

### 1. Ortam Değişkenleri (Environment Variables)
Proje kök dizinindeki şablon dosyayı kopyalayarak `.env` dosyasını oluşturun (`.env` hassas verileri korumak adına `.gitignore` ile korunmaktadır):
```bash
# Linux / macOS:
cp .example.env .env

# Windows PowerShell:
Copy-Item .example.env .env
```

### 2. Yerel Ortamda Çalıştırma (.NET CLI)
1. API projesinin dizinine gidin:
   `cd src/Presentation/Alumni.API`
2. Uygulamayı başlatın:
   `dotnet run`

### 3. Docker Compose ile Çalıştırma
Veritabanı (PostgreSQL 16) ve API servislerini tek bir komutla ayağa kaldırmak için kök dizinde çalıştırın:
```bash
docker compose up -d
```

---

## 📅 Geliştirme Süreci (Changelog)
### Hafta 1: Altyapı ve Planlama
* Proje vizyonu, teknoloji tercihleri (C# & PostgreSQL) ve zayıf yön/kısıt analizleri belirlendi.
* Clean Architecture katmanlı proje iskeleti kuruldu ve mimari altyapı dokümante edildi.
### Hafta 2: Minimal API ve Temel Yönlendirmeler
* Minimal API altyapısı üzerinde temel HTTP GET yönlendirmeleri yapılandırıldı.
* Dinamik route parametresi alımı (`/hello/{name}`) ve hesaplama mekanizması (`/sum/{n1}/{n2}`) geliştirildi.
* Sistemin erken aşama vitrinini sunan **Ana Sayfa (`/`)** ve **Hakkında (`/about`)** arayüzleri yayına alındı.
### Hafta 03: In-Memory User CRUD ve API Dokümantasyonu
* In-memory User CRUD mimarisi kuruldu (GET, POST, PUT, PATCH, DELETE `/api/users` rotaları).
* PUT (tam güncelleme) ile PATCH (kısmi güncelleme) uç noktaları standartlara uygun ayrıldı.
* Sistem sağlık kontrolü için `GET /api/health` uç noktası eklendi.
* API dokümantasyonu Scalar ile `/api/swagger` adresinde yapılandırıldı.
* API isteklerinin testi için Postman çalışma alanı ve koleksiyonu projeye dahil edildi.
### Hafta 04: MVC Mimarisi, İkili Controller Yapısı ve Razor Views
* **Dağıtılmış MVC (Distributed MVC) Mimarisi:** Clean Architecture katmanları MVC sorumluluklarıyla eşleştirildi ve dokümante edildi.
* **Veritabanı Bağımsız User Modeli ve In-Memory CRUD:** `User` varlığı ile `UserStore` veri havuzu üzerinde thread-safe `GetAll`, `GetById`, `Add`, `Update`, `Patch`, `Delete` operasyonları kuruldu.
* **İkili Controller Mimarisi:**
  * `UserController`: `/users` rotası üzerinden Razor View sayfalarını ve form işlemlerini yöneten MVC controller'ı.
  * `ApiUserController`: `/api/users` rotası üzerinden JSON tabanlı RESTful CRUD işlemlerini yürüten API controller'ı.
* **Razor View Katmanı (`Views/User`):**
  * `Index.cshtml`: Kayıtlı kullanıcıların listesi ve yeni kullanıcı ekleme formu (`GET /users`, `POST /users`).
  * `Details.cshtml`: Seçilen kullanıcının ayrıntılarını görüntüleme sayfası (`GET /users/details/{id}`).
  * `Edit.cshtml`: Kullanıcı bilgilerini güncelleme formu (`GET /users/edit/{id}`, `POST /users/edit/{id}`).
  * Kullanıcı silme iş akışı (`POST /users/delete/{id}`).
* **Hafta 04 Ödevi (Announcement Modülü):**
  * `Announcement` varlığı oluşturuldu (`Title`, `Content`, `Author`, `CreatedAt`, `IsActive`).
  * Yönetim için `IAnnouncementService` interface'i ve in-memory `AnnouncementService` tanımlandı (veritabanı henüz bağlanmadı).
  * `AnnouncementController` (MVC - `/announcements`) ve `ApiAnnouncementController` (API - `/api/announcements`) controller'ları kuruldu.
  * `Views/Announcement` altında `Index.cshtml`, `Details.cshtml`, `Edit.cshtml` sayfaları ile tam CRUD arayüzü sağlandı.
  * Scalar/OpenAPI dokümantasyonuna "Announcements" grubu eklendi.
### Hafta 05: Database & ORM — Veri Tasarımı ve Migrations
* PostgreSQL 16 (`docker-compose` persistent volume: `postgres_data`) ve Entity Framework Core uyumlu veri mimarisi şartnamesi hazırlandı.
* `Users`, `AlumniProfiles` (1-to-1) ve `JobPosts` (1-to-N) varlık modelleri, ilişkileri, kısıtları ve silme davranışları (Cascade / Set Null) [DATA_DESIGN.md](./DATA_DESIGN.md) dosyasında eksiksiz dokümante edildi.
* Antigravity AI asistanı için veri modelini tek ve bağlayıcı kaynak (Single Source of Truth) kabul eden kural direktifi README.md ve DATA_DESIGN.md içerisine entegre edildi.
* Güvenli ortam değişkenleri yönetimi için `.example.env` şablonu oluşturuldu; yerel geliştirme için `.env` dosyası hazırlanarak `.gitignore` ile korumaya alındı ve `docker-compose.yml` entegrasyonu sağlandı.
* EF Core Code-First altyapısı `Alumni.Persistence` katmanında kuruldu; `InitialDataDesign` migration'ı oluşturularak PostgreSQL veritabanına uygulandı (`users`, `alumni_profiles`, `job_posts` ve `announcements` tabloları üretildi).
* Varlık oluşturma süreçlerini standardize eden **Factory sınıfları** (`UserFactory`, `AlumniProfileFactory`, `JobPostFactory`, `AnnouncementFactory`) ve ilk açılışta veritabanını otomatik tohumlayan **Idempotent DatabaseSeeder** mimarisi entegre edildi.
* CRUD servis arayüzleri (`IUserService`, `IAnnouncementService`) üzerinden in-memory yapılardan PostgreSQL veritabanı destekli servislere (`UserService`, `AnnouncementService`) geçiş yapıldı; MVC (`UserController`, `AnnouncementController`) ve REST API (`ApiUserController`, `ApiAnnouncementController`) controller'ları Dependency Injection ile veritabanına bağlandı.





