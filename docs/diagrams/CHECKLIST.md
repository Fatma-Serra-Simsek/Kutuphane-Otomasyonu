# Diyagram çizim kontrol listesi (PDF 11)

Faz 1 kapanışında tüm maddeler işaretlenir.

## Class diyagramı

- [x] Her sınıfın adı isim (tekil, PascalCase), özellikleri ve gerekli metotları yazılı
- [x] İlişki türleri doğru: kompozisyon, ilişki, kalıtım, bağımlılık, gerçekleme
- [x] Her ilişkinin iki ucunda çokluk var (entity diyagramı)
- [x] Getter / setter ve yardımcı metotlar gösterilmemiş
- [x] Entity diyagramı ile servis diyagramı ayrılmış (`class-entities.mmd` / `class-services.mmd`)

## Object diyagramı

- [x] Başlıklar `nesne : Sınıf` biçiminde
- [x] Alan değerleri gerçekçi ve tutarlı (gecikmiş ödünç gerçekten gecikmiş)
- [x] Çokluk ve metot yok
- [x] Her bağlantının Class diyagramında karşılığı var

## Deployment diyagramı

- [x] Düğümlerin türü (device / execution environment) belirtilmiş
- [x] Artifact’lar doğru düğümün içinde
- [x] Her bağlantıda protokol ve port var
- [x] Canlı ortam gösteriliyor; yedekleme gibi isteğe bağlı parçalar açıkça işaretli

## Araçlar

- Mermaid (kod olarak sürümlenebilir) – tercih edilen  
- draw.io / PlantUML / Visual Studio Class Designer – alternatif  
