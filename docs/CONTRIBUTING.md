# Katkı ve dal stratejisi

## Dalları

- `main` korunur; doğrudan geliştirme yapılmaz.
- Her parça kendi özellik dalında yapılır: `feature/f<faz>-<parça>-<ad>`
  - Örnek: `feature/f2-1-domain`, `feature/f0-3-solution`
- Parçanın PDF / plan “bitti ölçütü” sağlandıktan sonra `main`’e merge edilir.

## Commit mesajı

Biçim: `[F<faz>.<parça>] Kısa açıklama`

Örnekler:

- `[F0] Çözüm iskeleti, Docker SQL Server, dokümanlar`
- `[F2.1] Domain entity'leri ve repository arayüzleri`

## Sıra

Her fazın sonunda “bitti ölçütü” karşılanmadan sonraki faza geçilmez.
