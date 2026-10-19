# Kütüphane Yönetim Sistemi (v1)

Üniversite kütüphanesi personeli için kitap, üye, ödünç/iade ve basit raporlama otomasyonu.

Kaynak kılavuz: [docs/Kutuphane-Yonetim-Sistemi-v1-Kilavuz.pdf](docs/Kutuphane-Yonetim-Sistemi-v1-Kilavuz.pdf)

## Teknoloji yığını

| Alan | Tercih |
|------|--------|
| Dil / çatı | C#, .NET 8 (LTS), ASP.NET Core MVC |
| ORM | Entity Framework Core (Code First) |
| Veritabanı | SQL Server (Docker) |
| Kimlik | ASP.NET Core Identity (Admin, Librarian) |
| Doğrulama | FluentValidation |
| Loglama | Serilog |
| Test | xUnit + Moq |
| CSV | CsvHelper |

## Ön koşullar

- .NET SDK 9.x (hedef çerçeve: `net8.0`)
- Docker Desktop
- Git
- `dotnet-ef` aracı (`dotnet tool install -g dotnet-ef --version 8.*`)

## Hızlı başlangıç

```powershell
# SQL Server
Copy-Item .env.example .env   # gerekirse parolayı düzenleyin
docker compose up -d

# Derleme ve test
dotnet restore
dotnet build
dotnet test

# Migration (ilk kurulum veya şema güncellemesi)
dotnet ef database update --project src/Library.Data --startup-project src/Library.Web

# Web (Development ortamında migration + seed otomatik çalışır)
dotnet run --project src/Library.Web
```

Uygulama açılınca `/Account/Login` sayfasına yönlendirilirsiniz (giriş zorunlu).

Bağlantı dizesi (`appsettings.Development.json` / `ConnectionStrings:LibraryDb`):

```
Server=localhost,1433;Database=LibraryDB;User Id=sa;Password=<SA_PASSWORD>;TrustServerCertificate=True;Encrypt=True
```

`<SA_PASSWORD>`, `.env` içindeki `SA_PASSWORD` ile aynı olmalıdır (varsayılan: `.env.example`).

### Geliştirme hesapları (seed)

| Rol | E-posta | Parola |
|-----|---------|--------|
| Admin | `admin@library.local` | `Admin123!` |
| Librarian | `librarian@library.local` | `Librarian123!` |

- **Admin:** tüm menü + Ayarlar
- **Librarian:** operasyon menüsü; Ayarlar → erişim reddedilir

Kitap, üye, ödünç/iade, listeler ve raporlar hazırdır (FR-1…FR-6). Raporlar ekranından CSV indirilebilir (UTF-8). Excel dışa aktarma kapsam dışıdır.

### Faz 7 (test / güvenlik / cilalama)

- **Serilog:** konsol + `src/Library.Web/logs/library-*.log` (günlük dosya)
- **Hata:** Production’da `/Home/Error`; Development’ta developer exception page
- **Entegrasyon / eşzamanlılık testleri:** Docker SQL Server gerekir (`LibraryDB_Test`). İsteğe bağlı: `SA_PASSWORD`, `LIBRARY_TEST_DB`
- **Güvenlik özeti:** giriş zorunlu; Ayarlar yalnızca Admin; POST’larda CSRF; Identity parola politikası; FluentValidation; seed loglarında parola yok
- **Performans (10k kitap) benchmark’ı** bilinçli olarak esnetildi (paging + indeksler mevcut)

### Yeni migration oluşturma

```powershell
dotnet ef migrations add <Ad> --project src/Library.Data --startup-project src/Library.Web --output-dir Migrations
dotnet ef database update --project src/Library.Data --startup-project src/Library.Web
```

## Klasör yapısı

```
Library.sln
src/
  Library.Domain/      # Entity'ler, enum'lar, repository arayüzleri
  Library.Data/        # DbContext, repository'ler, migration'lar, seed
  Library.Business/    # Servisler, DTO'lar, doğrulayıcılar
  Library.Web/         # Controllers, Views, ViewModels
tests/
  Library.Tests/       # xUnit + Moq
docs/                  # Kılavuz, gereksinimler, diyagramlar, wireframe'ler
```

## Katkı

Dal ve commit kuralları: [docs/CONTRIBUTING.md](docs/CONTRIBUTING.md)

## Kapsam notu

v1: personel paneli, kitap/üye/ödünç, arama, basit raporlar.  
v2 listesi (şube, rezervasyon, ceza, üye self-servis vb.) kapsam dışıdır.
