# Diyagram tutarlılık kontrolü (Faz 1.4)

Senaryo tarihi: **17 Ekim 2026** (PDF 5.2).

## 1. Object → Class eşlemesi

| Object nesnesi | Class | Alan tutarlılığı |
|---------------|-------|------------------|
| zeynep | Staff | FullName, Role=Librarian |
| ali | Member | MemberNumber, FullName, IsActive=true |
| kitap1 | Book | Title, PublishYear=1943 |
| kitap2 | Book | Title, PublishYear=1866 |
| yazar1 | Author | FullName=Sabahattin Ali |
| yazar2 | Author | FullName=Fyodor Dostoyevski |
| kopya1 | BookCopy | Barcode=B-0001, Status=Loaned |
| kopya2 | BookCopy | Barcode=B-0002, Status=Available |
| kopya3 | BookCopy | Barcode=B-0101, Status=Loaned |
| odunc1 | Loan | LoanDate=2026-10-01, DueDate=2026-10-15, ReturnDate=null |
| odunc2 | Loan | LoanDate=2026-10-10, DueDate=2026-10-24, ReturnDate=null |

## 2. Object → Class bağları

| Object bağı | Class ilişkisi |
|-------------|----------------|
| kitap1 — kopya1, kopya2 | Book *-- BookCopy (kompozisyon) |
| kitap2 — kopya3 | Book *-- BookCopy |
| kitap1 — yazar1 | Book *--* Author |
| kitap2 — yazar2 | Book *--* Author |
| odunc1 — ali | Member → Loan |
| odunc1 — kopya1 | BookCopy → Loan |
| odunc1 — zeynep | Staff → Loan |
| odunc2 — ali, kopya3, zeynep | aynı üç ilişki |

Sonuç: Object diyagramındaki her nesne ve bağ Class diyagramında karşılık buluyor.

## 3. PDF 5.5 kontrol hesapları

| Kontrol | Beklenen | Doğrulama |
|---------|----------|-----------|
| odunc1 gecikmiş mi? | `ReturnDate == null` ve `DueDate (15.10) < 17.10` → `IsOverdue() = true` | Evet |
| Ana panel gecikmiş sayısı | 1 (yalnızca odunc1) | odunc2 DueDate=24.10 → süresinde |
| kitap1 toplam kopya | 2 (kopya1, kopya2) | Evet |
| kitap1 müsait kopya | 1 (kopya2 Available) | Evet |

## 4. ER → Class eşlemesi

| ER tablo | Class / not |
|----------|-------------|
| BOOKS | Book |
| BOOK_COPIES | BookCopy |
| AUTHORS | Author |
| BOOK_AUTHORS | BookAuthor (bileşik PK; Book *--* Author ara tablosu) |
| PUBLISHERS | Publisher |
| CATEGORIES | Category |
| MEMBERS | Member |
| LOANS | Loan |
| STAFF | Staff |
| SETTINGS | Setting |
| ASP_NET_USERS / ASP_NET_ROLES | Identity (Data katmanı; karar K1) |

## 5. Kararlar ↔ diyagramlar

| Karar | Diyagram yansıması |
|-------|-------------------|
| K1 Staff ↔ Identity | ER’de STAFF ‖—o\| ASP_NET_USERS; Class entity’de yalnızca Staff |
| K2 Soft delete alanları | Book.IsDeleted, BookCopy.IsDeleted, Member.IsActive Class’ta |
| K3 Gün hassasiyeti | Object LoanDate/DueDate tarih-only; gecikme formülü CONSISTENCY §3 |
| Setting | Class `Setting`, ER `SETTINGS` |

## 6. Sonuç

- CHECKLIST.md maddeleri tamamlandı.  
- `out/` altında 5 diyagramın SVG ve PNG çıktıları mevcut.  
- Faz 1 bitti ölçütü sağlandı → Faz 2’ye geçilebilir.  
