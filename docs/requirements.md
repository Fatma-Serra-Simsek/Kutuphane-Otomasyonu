# Gereksinimler – Kütüphane Yönetim Sistemi (v1)

Kaynak: `Kutuphane-Yonetim-Sistemi-v1-Kilavuz.pdf` (bölüm 1, 3, 7, 8).  
Kapsam bilinçli olarak v1 ile sınırlıdır.

---

## 1. Kapsam

### 1.1 Fonksiyonel gereksinimler

#### FR-1 Ana panel
- FR-1.1 Toplam kitap sayısı gösterimi
- FR-1.2 Toplam üye sayısı gösterimi
- FR-1.3 Ödünçteki kitap sayısı gösterimi
- FR-1.4 Gecikmiş kitap sayısı gösterimi
- FR-1.5 Son ödünç işlemleri listesi
- FR-1.6 Gecikmişlere kısayol

#### FR-2 Kitap yönetimi
- FR-2.1 Kitap ekleme
- FR-2.2 Kitap düzenleme
- FR-2.3 Kitap silme (pasife alma / soft delete)
- FR-2.4 Alanlar: ad (Title), yazar, ISBN, yayınevi, yayın yılı, kategori, açıklama
- FR-2.5 Toplam kopya sayısı (hesaplanan)
- FR-2.6 Kopya barkodu ile kopya ekleme
- FR-2.7 Listeleme ve arama
- FR-2.8 Müsait / ödünçte durumu (rozetsel gösterim)

#### FR-3 Üye yönetimi
- FR-3.1 Üye ekleme
- FR-3.2 Üye düzenleme
- FR-3.3 Üye silme (pasife alma)
- FR-3.4 Alanlar: ad soyad, e-posta, telefon, üye numarası, kayıt tarihi
- FR-3.5 Aktif / pasif durumu
- FR-3.6 Üye arama
- FR-3.7 Üyenin ödünç aldığı kitapları görüntüleme (mevcut ve geçmiş)

#### FR-4 Ödünç ve iade
- FR-4.1 Üye seçme
- FR-4.2 Kitap / kopya seçme (barkod)
- FR-4.3 Ödünç tarihi kaydı
- FR-4.4 Otomatik teslim tarihi (`DueDate = LoanDate + LoanPeriodDays`)
- FR-4.5 İade alma
- FR-4.6 İade tarihini kaydetme
- FR-4.7 Süresi geçmişleri belirleme
- FR-4.8 Aynı kopyanın tekrar verilmesini engelleme

#### FR-5 Arama ve filtreleme
- FR-5.1 Kitap adı ile arama
- FR-5.2 Yazar ile arama
- FR-5.3 Kategori ile filtreleme
- FR-5.4 Üye adı ile arama
- FR-5.5 Ödünçteki kitaplar listesi
- FR-5.6 Gecikmiş kitaplar listesi

#### FR-6 Basit raporlama
- FR-6.1 En çok ödünç alınan kitaplar
- FR-6.2 En çok kitap alan üyeler
- FR-6.3 Ödünçteki kitaplar
- FR-6.4 Gecikmiş kitaplar
- FR-6.5 Tarih aralığına göre ödünç işlemleri
- FR-6.6 CSV / Excel çıktısı (isteğe bağlı; v1’de CSV zorunlu, Excel zaman kalırsa)

### 1.2 Aktörler

| Aktör | Yetki |
|-------|--------|
| Yönetici (Admin) | Tüm işlemler, personel hesapları, ayarlar (ödünç süresi vb.) |
| Kütüphaneci (Librarian) | Kitap, üye, ödünç ve iade işlemleri, raporlar |

v1’de üye sisteme giriş yapmaz; tüm işlemleri personel yürütür.

### 1.3 Fonksiyonel olmayan gereksinimler

| Alan | Gereksinim |
|------|------------|
| Güvenlik | Parolalar hash’li (ASP.NET Core Identity); rol bazlı yetkilendirme; CSRF ve XSS korumaları |
| Veri bütünlüğü | Ödünç işlemleri transaction içinde; veritabanında benzersiz kısıtlar |
| Performans | Liste ekranlarında sayfalama; arama alanlarında indeks |
| Kullanılabilirlik | Barkod alanı odaklı hızlı işlem akışı (barkod okuyucu klavye gibi davranır) |
| Bakım | Katmanlı yapı, bağımlılık enjeksiyonu, birim testler |

### 1.4 v2 listesi (şimdilik dışarıda)

Şube yönetimi, rezervasyon, ceza yönetimi, üyelik türleri ve limitleri, üye self-servis hesabı, bağış ve satın alma süreçleri, envanter sayımı, toplu içe aktarma, bildirimler (e-posta / SMS), tatil günü politikası, ayıklama süreci, yedekleme izleme.

---

## 2. İş kuralları

### 2.1 Temel ilke (PDF 3.1)

Ödünç verilen şey kitabın kendisi değil, fiziksel bir kopyasıdır.  
`Book` (eser) ile `BookCopy` (fiziksel nüsha) ayrıdır; `Loan` kaydı `BookCopyId`’ye bağlanır.

### 2.2 Ödünç verme – `LoanService.BorrowBook` (PDF 3.2)

1. Üyenin aktif olduğunu kontrol et  
2. Seçilen kopyanın `Available` olduğunu kontrol et  
3. Transaction başlat  
4. Aktif ödünç kaydı (`Loan`) oluştur  
5. Teslim tarihini hesapla: `DueDate = LoanDate + LoanPeriodDays` (ayardan)  
6. Kopya durumunu `Loaned` yap  
7. Kaydet ve transaction’ı tamamla  
8. İşlem sonucunu kullanıcıya göster  

Sonuç kodları: `Success`, `MemberInactive`, `CopyNotAvailable`, `CopyNotFound` (+ kararlar bölümü: `MemberNotFound`).

### 2.3 İade – `LoanService.ReturnBook` (PDF 3.3)

1. Kopyanın aktif ödünç kaydını bul (`ReturnDate == null`)  
2. İade tarihini kaydet  
3. Kopya durumunu `Available` yap  
4. Transaction’ı tamamla  
5. İşlem sonucunu göster (gecikme varsa gün sayısı ile birlikte)  

Aktif ödünç yoksa: `NoActiveLoan`.

### 2.4 Hesaplanan durumlar (PDF 3.4)

| Kavram | Formül |
|--------|--------|
| Müsait kopya sayısı | `Status == Available` olan `BookCopy` sayısı (saklı sayaç yok) |
| Toplam kopya sayısı | Kitaba bağlı, silinmemiş `BookCopy` sayısı |
| Gecikmiş ödünç | `ReturnDate == null && DueDate < bugün` |
| Ödünçteki kitap sayısı | `ReturnDate == null` olan `Loan` sayısı |

### 2.5 Çift ödünç engeli – iki katman (PDF 3.5)

1. **Servis:** Kontrol ve yazma tek transaction içinde  
2. **Veritabanı:** `Loans` üzerinde filtrelenmiş benzersiz indeks  
   `UX_Loans_ActiveCopy` ON `BookCopyId` WHERE `ReturnDate IS NULL`

### 2.6 Silme kuralları (PDF 3.6)

- Ödünç geçmişi olan kitap, kopya veya üye fiziksel silinmez; pasife alınır  
- Aktif ödüncü olan üye pasife alınamaz (veya uyarı verilir)  
- Ödünçteki kopya silinemez  

### 2.7 Kopya durum geçişleri (PDF 7.3)

| Mevcut durum | Geçerli sonraki durumlar |
|--------------|--------------------------|
| Available | Loaned, Lost, Damaged |
| Loaned | Available (iade), Lost |
| Damaged | Available (onarım sonrası), Lost |
| Lost | Available (bulunursa) |

Geçişler serviste kontrol edilir; durum alanı serbest metin gibi kullanılmaz.

---

## 3. Veri kuralları (PDF 7)

### 3.1 Tablolar (özet)

| Tablo | Önemli sütunlar | Notlar |
|-------|-----------------|--------|
| Books | Id, Title, ISBN, PublishYear, Description, CategoryId, PublisherId, IsDeleted | ISBN için indeks |
| BookCopies | Id, Barcode, Status, BookId, IsDeleted | Barcode benzersiz |
| Authors | Id, FullName | |
| BookAuthors | BookId, AuthorId | Bileşik birincil anahtar |
| Publishers | Id, Name | |
| Categories | Id, Name | |
| Members | Id, MemberNumber, FullName, Email, Phone, RegisteredAt, IsActive | MemberNumber benzersiz |
| Loans | Id, LoanDate, DueDate, ReturnDate, MemberId, BookCopyId, StaffId | Filtrelenmiş benzersiz indeks |
| Staff / Identity | Kullanıcı, rol, parola hash’i | ASP.NET Core Identity |
| Settings | Key, Value | Örn. `LoanPeriodDays = 14` |

### 3.2 Kritik kısıtlar

```sql
CREATE UNIQUE INDEX UX_Loans_ActiveCopy
ON Loans (BookCopyId)
WHERE ReturnDate IS NULL;
```

- `BookCopies.Barcode`: benzersiz  
- `Members.MemberNumber`: benzersiz  
- Yabancı anahtarlarda silme davranışı **Restrict** (zincirleme silme yok)  
- Arama sütunlarında (`Title`, `FullName`) indeks  
- Varsayılan ayar: `LoanPeriodDays = 14`

---

## 4. Kararlar (Faz 1 netleştirmeleri)

PDF’te açık bırakılan veya uygulama için netleştirilmesi gereken maddeler:

| # | Karar | Gerekçe |
|---|--------|---------|
| K1 | `Staff` saf Domain entity’sidir. `ApplicationUser : IdentityUser` Data katmanındadır ve `StaffId` ile bağlanır. `Loan.StaffId` → `Staff`. | Domain Identity’ye bağımlı olmaz (PDF 2.1). |
| K2 | Soft delete: `Book.IsDeleted`, `BookCopy.IsDeleted`, `Member.IsActive`. Staff pasife alma Identity `LockoutEnd` ile. | PDF Class diyagramı alanlarıyla uyumlu. |
| K3 | `LoanDate` / `DueDate` gün hassasiyetinde (saat 00:00). Gecikme: `DueDate < DateTime.Today`. | PDF 3.4 ve Object senaryosu. |
| K4 | Sayfalama varsayılanı 20 kayıt; arama büyük/küçük harf duyarsız. | PDF NFR performans / kullanılabilirlik. |
| K5 | `Lost` / `Damaged` geçişleri kitap detay ekranından, PDF 7.3 matrisine göre. | PDF 7.3 + FR-2. |
| K6 | Ödünç ekranında personel = oturum açan kullanıcı (`StaffId` otomatik). | PDF aktör modeli. |
| K7 | Sonuç kodu kümesi: `Success`, `MemberNotFound`, `MemberInactive`, `CopyNotFound`, `CopyNotAvailable`, `NoActiveLoan`. | PDF 3.2 kodları + eksik durumlar. |

---

## 5. İzlenebilirlik (FR → ekran → servis)

| FR | Ekran(lar) | Servis metodu (hedef) |
|----|------------|------------------------|
| FR-1 | Ana panel (`/`) | `DashboardService.GetSummary`, `GetRecentLoans` |
| FR-2 | Kitap listesi / ekle / düzenle / detay | `BookService` CRUD, arama, soft delete; kopya ekleme |
| FR-3 | Üye listesi / ekle / düzenle / detay | `MemberService` CRUD, arama, soft delete; ödünç geçmişi |
| FR-4 | Ödünç ver (`/Loans/Borrow`), İade al (`/Loans/Return`) | `LoanService.BorrowBook`, `ReturnBook` |
| FR-5 | Kitap/üye filtreleri, ödünçtekiler, gecikmişler | `LoanService.GetActiveLoans`, `GetOverdueLoans`; Book/Member arama |
| FR-6 | Raporlar (`/Reports`) | `ReportService` sorguları + CSV export |
| Ayarlar | `/Settings` (Admin) | `SettingsService`; personel Identity yönetimi |

### Ekran envanteri (PDF 8)

1. Giriş  
2. Ana panel  
3. Kitap listesi / arama  
4. Kitap ekle / düzenle  
5. Üye listesi / arama  
6. Üye ekle / düzenle  
7. Üye detay  
8. Ödünç ver  
9. İade al  
10. Gecikmiş kitaplar  
11. Raporlar  
12. Ayarlar (Admin) – ödünç süresi, personel hesapları  
