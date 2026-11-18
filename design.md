---
# Machine-Readable Design Tokens (aligned with brand.md)
tokens:
  reference: "./brand.md"
  colors:
    primary: "#1E3A8A"
    primaryHover: "#1E40AF"
    primaryDark: "#172554"
    primaryTint: "#EFF6FF"
    accent: "#D97706"
    accentHover: "#B45309"
    accentTint: "#FEF3C7"
    background: "#F8FAFC"
    surface: "#FFFFFF"
    surfaceSubtle: "#F1F5F9"
    textPrimary: "#1E293B"
    textSecondary: "#475569"
    textMuted: "#64748B"
    border: "#E2E8F0"
    borderStrong: "#CBD5E1"
    success: "#059669"
    successTint: "#ECFDF5"
    warning: "#D97706"
    warningTint: "#FFFBEB"
    danger: "#DC2626"
    dangerTint: "#FEF2F2"
  typography:
    fontDisplay: "'Plus Jakarta Sans', 'Inter', system-ui, -apple-system, sans-serif"
    fontBody: "'Inter', system-ui, -apple-system, sans-serif"
    fontMono: "'JetBrains Mono', ui-monospace, SFMono-Regular, Menlo, monospace"
    baseSize: "16px"
  spacing:
    unit: "4px"
    scale: [4, 8, 12, 16, 20, 24, 32, 48, 64, 80, 96]
  radius:
    sm: "6px"
    md: "8px"
    lg: "12px"
    xl: "16px"
    pill: "9999px"
---

# 📐 Design System — Alumni Tracking System

> Bu doküman, [brand.md](./brand.md) içindeki **Akademik Lacivert & Altın** renk paleti temel alınarak; [VoltAgent/awesome-design-md](https://github.com/VoltAgent/awesome-design-md) standartlarından (özellikle **Stripe** tipografik netliği ve **Notion** yapısal ferahlığı) ilhamla oluşturulmuş tek bağlayıcı tasarım sistemidir.

---

## 1. Visual Theme & Atmosphere (Görsel Tema ve Atmosfer)

- **Atmosfer:** Güven veren, köklü akademik prestiji modern kurumsal SaaS sadeliğiyle birleştiren aydınlık ve odaklı bir çalışma alanı.
- **Yoğunluk (UI Density):** Dengeli ve okunabilir (*Airy Institutional*). Veri tabloları ve mezun dizinleri bilgi açısından zengin ama sıkışık olmayacak şekilde geniş kenar boşluklarıyla nefes alır.
- **Tasarım Felsefesi:**
  - **Stripe'tan İlham:** Keskin tipografik hiyerarşi, `1px` ince kenarlıklarla tanımlanan net yüzeyler, çok katmanlı yumuşak gölgeler ve istatistiklerde `tabular-nums` kullanımı.
  - **Notion'dan İlham:** Kırık beyaz (`#F8FAFC`) ferah tuval, abartısız `8px–12px` köşe geometrisi ve içerik odaklı sade kart düzeni.

---

## 2. Color Palette & Roles (Renk Rolleri — `brand.md` Referanslı)

Tüm renk seçimleri [brand.md](./brand.md) dosyasındaki çekirdek palete sadıktır:

| Token | Hex / Değer | Kullanım Rolü |
| :--- | :--- | :--- |
| `--bg-canvas` | `#F8FAFC` | Sayfa ana arka planı (Off-White) |
| `--bg-surface` | `#FFFFFF` | Kartlar, tablolar, formlar ve üst navigasyon çubuğu |
| `--bg-surface-subtle` | `#F1F5F9` | İkincil paneller, tablo başlıkları, hover arka planları |
| `--brand-navy` | `#1E3A8A` | Ana marka rengi, öne çıkan kart başlık bantları, aktif linkler |
| `--brand-navy-hover` | `#1E40AF` | Lacivert buton/link hover durumu |
| `--brand-navy-deep` | `#172554` | Hero vurgu şeritleri ve footer/koyu vurgu alanları |
| `--brand-navy-tint` | `#EFF6FF` | Akademik bilgi rozetleri ve ince vurgu kutuları |
| `--accent-gold` | `#D97706` | Birincil CTA butonları (*Bağlantı Kur*, *Başvur*, *Mezun Ara*) ve dikkat çekici rozetler |
| `--accent-gold-hover` | `#B45309` | Altın CTA hover durumu |
| `--accent-gold-tint` | `#FEF3C7` | Altın vurgulu rozet arka planları |
| `--text-primary` | `#1E293B` | Ana başlıklar ve gövde metinleri (Dark Slate) |
| `--text-secondary` | `#475569` | Kart alt başlıkları, özet paragraflar |
| `--text-muted` | `#64748B` | Yardımcı etiketler, tarih/konum meta verileri |
| `--border-subtle` | `#E2E8F0` | Kart, input ve tablo ayırıcı çizgileri (`1px solid`) |
| `--border-strong` | `#CBD5E1` | Odak/hover durumundaki input veya buton kenarlıkları |
| `--status-success` | `#059669` / `#ECFDF5` | Onaylı mezun (`Approved`), kabul edilen başvuru (`Accepted`) |
| `--status-warning` | `#D97706` / `#FFFBEB` | Beklemede (`Pending`, `Reviewing`, `Interview`) |
| `--status-danger` | `#DC2626` / `#FEF2F2` | Reddedilen (`Rejected`) veya silme uyarıları |

---

## 3. Typography Rules (Tipografi Kuralları)

### Yazı Tipi Ailesi
- **Display & Headings:** `'Plus Jakarta Sans', 'Inter', system-ui, -apple-system, sans-serif`
- **Body & UI:** `'Inter', system-ui, -apple-system, sans-serif`
- **Monospace (API rotaları, ID'ler, kod):** `'JetBrains Mono', ui-monospace, SFMono-Regular, monospace`

### Tipografik Ölçek (Type Scale)

| Seviye | Boyut (`rem` / `px`) | Ağırlık | Satır Yüksekliği | Harf Aralığı (`tracking`) | Kullanım |
| :--- | :--- | :--- | :--- | :--- | :--- |
| **Display (H1 - Hero)** | `2.75rem` – `3.25rem` (`44–52px`) | `800` | `1.12` | `-0.03em` | Ana sayfa Hero başlığı |
| **Page Title (H1 - Alt Sayfa)** | `2rem` (`32px`) | `700` | `1.2` | `-0.025em` | Alt sayfa ana başlıkları |
| **Section Heading (H2)** | `1.625rem` (`26px`) | `700` | `1.25` | `-0.02em` | Bölüm başlıkları |
| **Card Title (H3)** | `1.125rem` (`18px`) | `600` | `1.35` | `-0.01em` | Kart ve panel başlıkları |
| **Lead / Subtitle** | `1.125rem` (`18px`) | `400` | `1.6` | `0` | Hero alt açıklaması |
| **Body Default** | `1rem` (`16px`) | `400` | `1.6` | `0` | Genel metinler, form açıklamaları |
| **UI / Table / Nav** | `0.875rem` – `0.9375rem` (`14–15px`) | `500`–`600` | `1.45` | `0` | Menü linkleri, butonlar, tablo hücreleri |
| **Micro / Badge / Label** | `0.75rem` (`12px`) | `600` | `1.2` | `0.03em` | Rozetler, üst etiketler (*UPPERCASE*) |

- **Sayısal Hizalama:** Mezuniyet yılları, istatistik sayaçları ve ID sütunlarında `font-variant-numeric: tabular-nums;` uygulanır.

---

## 4. Layout & Spacing Principles (Yerleşim ve Boşluk Sistemi)

- **Temel Ölçek Birimi:** `4px` (`0.25rem`) katları kullanılır.
  - Bileşen içi sıkı boşluk: `8px` (`0.5rem`), `12px` (`0.75rem`)
  - Kart içi padding: `20px` – `24px` (`1.25rem` – `1.5rem`)
  - Grid kart aralığı (`gap`): `24px` (`1.5rem`)
  - Bölümler arası dikey boşluk: `64px` – `80px` (`4rem` – `5rem`)
- **Konteyner Genişliği:**
  - Standart sayfa konteyneri: `max-width: 1200px`, yatay padding `24px` (`1.5rem`), ortalanmış (`margin: 0 auto`).
  - Okuma / form odaklı dar konteyner: `max-width: 820px`.
- **Grid Sistemi:**
  - Hero bölümü: Masaüstünde `1.15fr 0.85fr` asimetrik 2 kolon (mobilde tek kolon).
  - Kart dizilimleri (Öne Çıkan Mezunlar, Modüller, İlanlar): `repeat(auto-fit, minmax(300px, 1fr))` veya `repeat(3, 1fr)`.

---

## 5. Component Stylings (Bileşen Kuralları)

### 5.1. Üst Navigasyon Çubuğu (Header / Navbar)
- **Yükseklik:** `72px`, yapışkan (`position: sticky; top: 0; z-index: 50;`).
- **Arka Plan:** `rgba(255, 255, 255, 0.92)` + `backdrop-filter: blur(10px)` + alt kenarda `1px solid #E2E8F0`.
- **Marka Logosu:** Sol tarafta akademik amblem/rozet + `#1E3A8A` koyu lacivert marka adı.
- **Navigasyon Linkleri:** `#475569` varsayılan, hover durumunda `#1E3A8A`, aktif sayfada alt çizgi veya `#EFF6FF` yumuşak arka plan kapsülü.
- **Sağ Aksiyon Alanı:** İkincil buton (örn. *API Dokümantasyonu* / *Giriş*) + Birincil Altın CTA (örn. *Mezun Ağına Katıl* / *İlan Yayınla*).

### 5.2. Butonlar (Buttons)
Köşe yuvarlaklığı: `8px` (Notion esintili net dikdörtgen form, aşırı yuvarlak kapsül butonlardan kaçınılır). Geçiş: `all 160ms ease`.

- **Primary CTA (Amber Gold):**
  - Arka plan `#D97706`, metin `#FFFFFF`, ağırlık `600`, padding `10px 20px`.
  - Hover: `#B45309` + hafif `translateY(-1px)` ve gölge.
- **Primary Brand (Navy Blue):**
  - Arka plan `#1E3A8A`, metin `#FFFFFF`, ağırlık `600`, padding `10px 20px`.
  - Hover: `#1E40AF`.
- **Secondary / Outline:**
  - Arka plan `#FFFFFF`, kenarlık `1px solid #CBD5E1`, metin `#1E293B`.
  - Hover: arka plan `#F8FAFC`, kenarlık `#1E3A8A`, metin `#1E3A8A`.
- **Ghost / Link Action:**
  - Arka plan transparan, metin `#1E3A8A`, hover arka plan `#EFF6FF`.

### 5.3. Kartlar (Cards)
1. **Standart İnce Kenarlıklı Kart (Standard Hairline Card):**
   - Arka plan `#FFFFFF`, kenarlık `1px solid #E2E8F0`, köşe `12px`, padding `24px`.
   - Hover: kenarlık `#CBD5E1` (veya `#1E3A8A`), ince gölge artışı ve `-2px` yükselme.
2. **Öne Çıkan Mezun Profil Kartı (Academic Navy Header Card):**
   - Üstte `#1E3A8A` lacivert başlık bandı (`padding: 14px 20px`), içinde dairesel avatar/monogram ve `#FFFFFF` mezun adı/unvanı.
   - Alt gövdede `#FFFFFF` zemin (`padding: 20px`), `#1E293B` bölüm/şirket bilgisi, `#64748B` mezuniyet yılı & sektör meta bilgisi ve tam genişlikli veya hizalı `#D97706` *Bağlantı Kur / Profili Gör* butonu.

### 5.4. Form Elemanları ve Arama Çubuğu (Inputs & Search Bar)
- **Arka Plan:** `#FFFFFF`, kenarlık `1px solid #CBD5E1`, köşe `10px`, yükseklik `44px–52px`, iç metin `#1E293B`, placeholder `#94A3B8`.
- **Focus Durumu:** Kenarlık `#1E3A8A`, dış hale `box-shadow: 0 0 0 3px rgba(30, 58, 138, 0.14);`.
- **Hero Arama Çubuğu:** Tek satır birleşik bileşen; sol tarafta ikon + arama inputu, sağ tarafta `#D97706` veya `#1E3A8A` aksiyon butonu.

### 5.5. Rozetler ve Etiketler (Badges & Pills)
- **Köşe:** `9999px` (sadece küçük rozet ve etiketlerde kullanılır), padding `4px 10px`, font `12px / 600`.
- **Akademik/Bölüm Etiketi:** Arka plan `#EFF6FF`, metin `#1E3A8A`, kenarlık `1px solid #DBEAFE`.
- **Altın Vurgu Etiketi:** Arka plan `#FEF3C7`, metin `#92400E`.
- **Durum Rozetleri:** Başarılı (`#ECFDF5` / `#059669`), Beklemede (`#FFFBEB` / `#B45309`), Hata/Red (`#FEF2F2` / `#DC2626`).

---

## 6. Depth & Elevation (Derinlik ve Gölge Hiyerarşisi)

Ağır koyu gölgeler yerine katmanlı ve düşük opaklıklı gölgeler kullanılır:

- **Level 0 (Flat):** `none` — Arka plan tuvali (`#F8FAFC`) ve gömülü bilgi kutuları.
- **Level 1 (Card Default):** `0 1px 2px rgba(30, 41, 59, 0.04), 0 1px 3px rgba(30, 41, 59, 0.03)` + `1px solid #E2E8F0`.
- **Level 2 (Hover / Raised Card):** `0 10px 25px -5px rgba(30, 58, 138, 0.08), 0 4px 10px -2px rgba(30, 41, 59, 0.04)`.
- **Level 3 (Sticky Header / Popover):** `0 4px 16px rgba(15, 23, 42, 0.06)`.

---

## 7. Do's and Don'ts (Tasarım Koruma Kuralları)

### ✅ Yapılması Gerekenler (Do's)
- Her sayfada `#F8FAFC` ana tuval üzerinde `#FFFFFF` kartlar kullanarak net kontrast sağla.
- Birincil dönüşüm butonlarında (CTA) tutarlı biçimde `#D97706` (Amber Gold), kurumsal navigasyon ve yapısal başlıklarda `#1E3A8A` (Navy Blue) kullan.
- Tüm veritabanı varlık alanlarını (`Users`, `JobPosts`, `Applications`, `Announcements`) temiz tablo veya kart hiyerarşisiyle sun.
- Her interaktif öğeye belirgin `:focus-visible` ve `:hover` durumları tanımla.

### ❌ Kaçınılması Gerekenler (Don'ts)
- Koyu neon/cyberpunk arka planlar, aşırı cam efekti (heavy dark glassmorphism) veya mor-pembe rastgele gradyanlar **kullanma**.
- Kartlarda ve büyük aksiyon butonlarında aşırı yuvarlak (`24px+` veya tam oval) formlar kullanma; yapısal `8px–12px` köşe disiplinini koru.
- Marka paleti dışında rastgele renkler (`#4F46E5` indigo veya neon cyan vb.) ekleme.
- Tek bir görünümde 2'den fazla baskın altın CTA butonu yan yana yerleştirme; görsel hiyerarşiyi bozma.

---

## 8. Responsive Behavior (Duyarlı Davranış)

- **`>= 1024px` (Desktop):** Tam yatay navigasyon menüsü, 2 sütunlu asimetrik Hero, 3 sütunlu kart gridleri (`repeat(3, 1fr)`).
- **`768px – 1023px` (Tablet):** Hero tek sütuna döner veya sıkılaşır; kart gridleri 2 sütuna düşer (`repeat(2, 1fr)`).
- **`< 768px` (Mobil):**
  - Navbar kompakt hale gelir (yatay kaydırılabilir/sarılabilir nav veya sade menü).
  - Tüm kart gridleri `1fr` tek sütun olarak istiflenir.
  - Hero başlığı `2rem` (`32px`) seviyesine ölçeklenir, arama çubuğu dikey veya kompakt hizaya geçer, dokunma hedefleri en az `44px` yükseklikte tutulur.

---

## 9. Agent Implementation Guide (6. Adım Uygulama Rehberi)

Sayfaları sıfırdan tasarlarken her sayfa şu ortak iskeleti ve kuralları izler:
1. Ortak CSS Token değişkenlerini (`:root`) birebir `design.md` YAML ve Bölüm 2'deki değerlerden türet.
2. Tüm sayfalarda tutarlı üst navigasyon (`/` Ana Sayfa, `/users` Mezunlar, `/jobs` İş İlanları, `/applications` Başvurular, `/announcements` Duyurular, `/about` Hakkında, `/api/swagger` API) ve kurumsal footer yapısını kullan.
3. Sayfa içeriklerini `brand.md` ve `design.md` dışında kalan eski inline stiller olmadan temiz, erişilebilir semantik HTML5 (`<header>`, `<main>`, `<section>`, `<article>`, `<footer>`) ile kur.
