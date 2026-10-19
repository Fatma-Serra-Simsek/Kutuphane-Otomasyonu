# Ekran taslakları (wireframe)

Kaynak: PDF bölüm 8. Metin tabanlı; her ekran aynı şablonu kullanır.  
Roller: **Admin**, **Librarian** (aksi belirtilmedikçe her ikisi).

Ortak kurallar:
- Sayfalama varsayılanı: 20 kayıt
- Boş listelerde boş durum mesajı
- Sunucu tarafı doğrulama + toast bildirim
- Barkod alanları: autofocus, Enter ile gönderim, işlem sonrası odak geri alanda

---

## 1. Giriş

| | |
|--|--|
| **Rota** | `/Account/Login` |
| **Roller** | Anonim (giriş sonrası personel) |
| **FR** | NFR güvenlik / Identity |
| **Servis** | ASP.NET Core Identity `SignInManager` |

**Alanlar:** E-posta / kullanıcı adı, Parola, Beni hatırla (isteğe bağlı)

**Aksiyonlar:** Giriş · Çıkış (layout’ta)

**Doğrulama / hatalar:** Geçersiz kimlik bilgisi; kilitli hesap

**Boş durum:** —

```
+----------------------------------+
|  Kütüphane Yönetim Sistemi       |
|  [e-posta          ]             |
|  [parola           ]             |
|  [ Giriş ]                       |
+----------------------------------+
```

---

## 2. Ana panel

| | |
|--|--|
| **Rota** | `/` veya `/Home/Index` |
| **Roller** | Admin, Librarian |
| **FR** | FR-1 |
| **Servis** | `DashboardService.GetSummary`, `GetRecentLoans` |

**Alanlar / içerik:**
- Kartlar: Toplam kitap | Toplam üye | Ödünçteki | Gecikmiş
- Tablo: Son ödünç işlemleri (Tarih, Üye, Barkod/Kitap, Teslim tarihi, Durum)
- Kısayol: Gecikmiş kitaplar

**Aksiyonlar:** Kart/kısayol tıklanınca ilgili listeye git

**Boş durum:** “Henüz ödünç işlemi yok.”

```
+------+ +------+ +------+ +------+
|Kitap | |Üye   | |Ödünç | |Gecik.|
+------+ +------+ +------+ +------+
Son ödünçler: [tablo...]
[ Gecikmişlere git ]
```

---

## 3. Kitap listesi / arama

| | |
|--|--|
| **Rota** | `/Books` |
| **Roller** | Admin, Librarian |
| **FR** | FR-2.7, FR-2.8, FR-5.1–5.3 |
| **Servis** | `BookService.Search` |

**Alanlar:** Ad, Yazar, Kategori filtresi

**Tablo sütunları:** Ad, Yazar(lar), Kategori, Toplam kopya, Müsait, Durum rozeti, İşlemler

**Aksiyonlar:** Yeni kitap · Düzenle · Pasife al · Detay · Sayfalama / sıralama

**Boş durum:** “Arama kriterine uygun kitap bulunamadı.”

---

## 4. Kitap ekle / düzenle

| | |
|--|--|
| **Rota** | `/Books/Create`, `/Books/Edit/{id}` |
| **Roller** | Admin, Librarian |
| **FR** | FR-2.1–2.4 |
| **Servis** | `BookService.Create`, `Update` |

**Alanlar:** Title*, ISBN, PublishYear, Description, Category*, Publisher*, Authors* (çoklu)

**Aksiyonlar:** Kaydet · İptal

**Doğrulama:** Zorunlu alanlar; PublishYear aralığı; ISBN biçimi

**Not (detay ekranı / aynı form altı):** Kopya ekleme (Barkod*), kopya listesi, durum değiştirme (Lost/Damaged matrisine göre) — FR-2.6, K5

---

## 5. Üye listesi / arama

| | |
|--|--|
| **Rota** | `/Members` |
| **Roller** | Admin, Librarian |
| **FR** | FR-3.5–3.6, FR-5.4 |
| **Servis** | `MemberService.Search` |

**Alanlar:** Ad arama, Aktif/Pasif filtresi

**Tablo:** Üye no, Ad soyad, E-posta, Telefon, Durum, İşlemler

**Aksiyonlar:** Yeni üye · Düzenle · Pasife al · Detay

**Boş durum:** “Üye bulunamadı.”

---

## 6. Üye ekle / düzenle

| | |
|--|--|
| **Rota** | `/Members/Create`, `/Members/Edit/{id}` |
| **Roller** | Admin, Librarian |
| **FR** | FR-3.1–3.4 |
| **Servis** | `MemberService.Create`, `Update` |

**Alanlar:** MemberNumber* (veya otomatik), FullName*, Email, Phone, IsActive

**Doğrulama:** MemberNumber benzersiz; e-posta biçimi

**Pasife alma uyarısı:** Aktif ödünç varsa engel/uyarı (PDF 3.6)

---

## 7. Üye detay

| | |
|--|--|
| **Rota** | `/Members/Details/{id}` |
| **Roller** | Admin, Librarian |
| **FR** | FR-3.7 |
| **Servis** | `MemberService.GetLoans` |

**İçerik:** Üye bilgileri; Mevcut ödünçler tablosu; Geçmiş ödünçler tablosu

**Boş durum:** “Bu üyenin ödünç kaydı yok.”

---

## 8. Ödünç ver

| | |
|--|--|
| **Rota** | `/Loans/Borrow` |
| **Roller** | Admin, Librarian |
| **FR** | FR-4.1–4.4, FR-4.8 |
| **Servis** | `LoanService.BorrowBook` |

**Barkod odaklı akış:**
1. Üye seç (arama)  
2. Barkod alanına odak (autofocus)  
3. Okut / Enter → teslim tarihi önizleme (`bugün + LoanPeriodDays`)  
4. Onay → sonuç koduna göre mesaj  
5. Odak tekrar barkod alanına  

**Alanlar:** Üye*, Barkod*, LoanDate (otomatik), DueDate (önizleme)

**Hatalar:** `MemberInactive`, `CopyNotAvailable`, `CopyNotFound`, `MemberNotFound`

```
Üye: [ ara / seç ............ ]
Barkod: [____________]  <- autofocus
Teslim: 31.10.2026 (önizleme)
[ Ödünç ver ]
```

---

## 9. İade al

| | |
|--|--|
| **Rota** | `/Loans/Return` |
| **Roller** | Admin, Librarian |
| **FR** | FR-4.5–4.7 |
| **Servis** | `LoanService.ReturnBook` |

**Akış:** Barkod okut → aktif ödünç bilgisi + gecikme günü → Onay → toast → odak barkoda

**Hatalar:** `NoActiveLoan`, `CopyNotFound`

---

## 10. Gecikmiş kitaplar

| | |
|--|--|
| **Rota** | `/Loans/Overdue` |
| **Roller** | Admin, Librarian |
| **FR** | FR-5.6, FR-1.6 |
| **Servis** | `LoanService.GetOverdueLoans` |

**Tablo:** Üye, Kitap, Barkod, DueDate, Gecikme günü, Personel

**Filtre:** Yalnızca `ReturnDate == null && DueDate < bugün`

**Boş durum:** “Gecikmiş ödünç yok.”

---

## 11. Raporlar

| | |
|--|--|
| **Rota** | `/Reports` (+ alt rotalar) |
| **Roller** | Admin, Librarian |
| **FR** | FR-6 |
| **Servis** | `ReportService.*` + CSV export |

**Raporlar:**
1. En çok ödünç alınan kitaplar  
2. En çok kitap alan üyeler  
3. Ödünçteki kitaplar  
4. Gecikmiş kitaplar  
5. Tarih aralığına göre ödünçler  

**Alanlar:** Başlangıç / Bitiş tarihi (rapor 5 ve isteğe bağlı diğerleri)

**Aksiyonlar:** Görüntüle · CSV indir

**Boş durum:** “Seçilen aralıkta kayıt yok.”

---

## 12. Ayarlar (Admin)

| | |
|--|--|
| **Rota** | `/Settings` |
| **Roller** | **Admin** |
| **FR** | PDF 8 Ayarlar |
| **Servis** | `SettingsService`; Identity personel yönetimi |

**Alanlar / bölümler:**
- `LoanPeriodDays` (örn. 14)
- Personel listesi: Ad, E-posta, Rol (Admin/Librarian), Durum
- Personel ekleme / rol atama / pasife alma (Lockout)

**Doğrulama:** LoanPeriodDays > 0; e-posta benzersiz

```
Ödünç süresi (gün): [ 14 ] [ Kaydet ]

Personel:
| Ad | E-posta | Rol | [Ekle] |
```

---

## Özet rota listesi

| Ekran | Rota |
|-------|------|
| Giriş | `/Account/Login` |
| Ana panel | `/` |
| Kitaplar | `/Books`, `/Books/Create`, `/Books/Edit/{id}`, `/Books/Details/{id}` |
| Üyeler | `/Members`, `/Members/Create`, `/Members/Edit/{id}`, `/Members/Details/{id}` |
| Ödünç | `/Loans/Borrow` |
| İade | `/Loans/Return` |
| Gecikmiş | `/Loans/Overdue` |
| Ödünçtekiler | `/Loans/Active` |
| Raporlar | `/Reports` |
| Ayarlar | `/Settings` |
