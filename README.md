# 🎓 Alumni Tracking System (Mezun Takip Sistemi)

[![.NET](https://img.shields.io/badge/.NET-8.0%2F9.0-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/C%23-239120?logo=csharp&logoColor=white)](https://learn.microsoft.com/dotnet/csharp/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-316192?logo=postgresql&logoColor=white)](https://www.postgresql.org/)
[![IDE](https://img.shields.io/badge/IDE-Google%20Antigravity-4285F4?logo=google&logoColor=white)](https://antigravity.google)
[![Methodology](https://img.shields.io/badge/Methodology-Vibe%20Coding-FF6F61)](https://github.com)

Üniversite mezunlarının kariyer yolculuklarını, çalıştıkları sektör ve şirketleri, edindikleri yetenekleri ve mezunlar arası iletişim ağını tek bir kurumsal çatı altında ilişkisel olarak takip eden modern web platformu.

---

## 📌 İçindekiler
- [Proje Konsepti ve Amacı](#-proje-konsepti-ve-amacı)
- [Teknoloji Yığını ve Tercih Nedenleri](#-teknoloji-yığını-ve-tercih-nedenleri)
- [Zayıf Yönler ve Risk Analizi](#-zayıf-yönler-ve-risk-analizi)
- [Geliştirme Metodolojisi](#-geliştirme-metodolojisi)
- [Önerilen Sistem Mimarisi](#-önerilen-sistem-mimarisi)
- [Kurulum ve Başlangıç](#-kurulum-ve-başlangıç)
- [Haftalık Yol Haritası ve Git İş Akışı](#-haftalık-yol-haritası-ve-git-iş-akışı)

---

## 🎯 Proje Konsepti ve Amacı

Üniversiteler ile mezunlar arasındaki bağların kopmaması, hem mevcut öğrencilerin kariyer rehberliği alabilmesi hem de üniversitenin mezun istihdam verilerini analiz edebilmesi açısından kritik öneme sahiptir.

**Alumni Tracking System**, mezunların:
- Kariyer süreçlerini (pozisyon, unvan, sektör değişiklikleri),
- Çalıştıkları şirketleri ve geçmiş tecrübelerini,
- Sahip oldukları teknik ve sosyal yetenekleri (skills),
- Mezunlar, öğrenciler ve akademisyenler arasındaki iletişim ve mentörlük ağlarını

ilişkisel bir veri tabanı omurgası üzerinde güvenilir, hızlı ve ölçeklenebilir şekilde takip etmeyi amaçlar.

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

## 💻 Kurulum ve Başlangıç

### Gereksinimler
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download) veya üzeri
- [PostgreSQL 15+](https://www.postgresql.org/download/)
- Git CLI

### Adımlar
```bash
# 1. Depoyu klonlayın
git clone <REPO_URL>
cd Alumni

# 2. Gerekli bağımlılıkları yükleyin
dotnet restore

# 3. Veritabanı bağlantı dizesini (appsettings.Development.json) yapılandırın
# 4. EF Core veritabanı migration'larını uygulayın
dotnet ef database update --project src/Infrastructure/Alumni.Persistence

# 5. Projeyi çalıştırın
dotnet run --project src/Presentation/Alumni.API
```

---

## 📅 Haftalık Yol Haritası

- [x] **Hafta 1:** Proje başlatma, mimari kararlar, Git deposu ve dokümantasyon kurulumu.
- [ ] **Hafta 2:** Veritabanı modelleme (PostgreSQL & EF Core Entities) ve Migration oluşturulması.
- [ ] **Hafta 3:** Mezun, Şirket ve Yetenek CRUD API servislerinin geliştirilmesi.
- [ ] **Hafta 4:** Kimlik doğrulama ve yetkilendirme (JWT & Role Based Access).
- [ ] **Hafta 5:** Arama, filtreleme ve mezun ilişkisel ağ sorgularının optimizasyonu.
- [ ] **Hafta 6:** Testler, dokümantasyon (Swagger/OpenAPI) ve canlıya alım hazırlıkları.
