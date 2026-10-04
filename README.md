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

## 🌐 Dokümantasyon / API Uç Noktaları

* **API Swagger / OpenAPI dokümantasyonuna `/api/swagger` üzerinden erişilebilir.**
* **Dinamik Dokümantasyon İlkesi:** Projeye ilerleyen haftalarda eklenecek tüm yeni Minimal API rotaları, `EndpointsApiExplorer` ve Swagger altyapısı sayesinde herhangi bir manuel müdahaleye gerek kalmadan `/api/swagger` arayüzüne otomatik olarak yansıtılacaktır.

Sistemde halihazırda çalışan HTTP uç noktaları:
| HTTP Metodu | Uç Nokta (Route) | Açıklama |
|---|---|---|
| `GET` | `/` | Web Vitrini / Ana Sayfa Prototipi |
| `GET` | `/about` | Platform Misyonu ve Hakkında Sayfası |
| `GET` | `/hello` | Temel Çalışma Doğrulama Testi |
| `GET` | `/hello/{name}` | Dinamik Route Parametresi Testi |
| `GET` | `/sum/{n1}/{n2}` | Sayısal Hesaplama ve Parametre Doğrulama Testi |
| `GET` | `/api/health` | API Sağlık Durumu |
| `GET` | `/api/users` | Tüm Kullanıcıları Listeleme |
| `GET` | `/api/users/{id}` | ID'ye Göre Tekil Kullanıcı Getirme |
| `POST` | `/api/users` | Yeni Kullanıcı Ekleme |
| `PUT` | `/api/users/{id}` | Kullanıcı Tam Güncelleme |
| `PATCH`| `/api/users/{id}` | Kullanıcı Kısmi Güncelleme |
| `DELETE`| `/api/users/{id}`| Kullanıcı Silme |


---

## 🚀 Kurulum ve Başlangıç
Projeyi yerel ortamda çalıştırmak için:
1. Depoyu klonlayıp API projesinin dizinine gidin:
   `cd src/Presentation/Alumni.API`
2. Uygulamayı başlatın:
   `dotnet run`
3. Proje, nihai aşamada tüm veritabanı ve servisleriyle birlikte tek bir komutla ayağa kalkacak şekilde hedeflenmektedir:
   `docker compose up`

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

