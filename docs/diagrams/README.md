# Diyagramlar

Kaynak: PDF bölüm 4 (Class), 5 (Object), 6 (Deployment), 7 (ER türevi), 11 (kontrol listesi).

## Dosyalar

| Dosya | Amaç |
|-------|------|
| `class-entities.mmd` | Domain entity Class diyagramı (PDF 4.2) |
| `class-services.mmd` | Servis / repository / controller Class diyagramı (PDF 4.3 kalıbı) |
| `object-scenario.mmd` | 17 Ekim 2026 senaryosu Object diyagramı (PDF 5) |
| `deployment.mmd` | Canlı ortam Deployment diyagramı (PDF 6) |
| `er-diagram.mmd` | Class diyagramından türetilen ER diyagramı (PDF 7) |

Render çıktıları: `out/*.svg`, `out/*.png`

## Nasıl okunur (özet)

### Class (PDF 4.1)
- `+` public, `-` private  
- `*--` kompozisyon; `-->` association; uçlarda çokluk  
- Entity diyagramı ile servis diyagramı ayrıdır  

### Object (PDF 5.1)
- Başlık: `nesneAdı : SınıfAdı`  
- Alanlar gerçek değerlerle; çokluk ve metot yok  
- Her nesne/bağ Class diyagramında karşılık bulmalı  

### Deployment (PDF 6.1)
- Düğüm türü: device / execution environment  
- Artifact’lar doğru düğümde; bağlantılarda protokol ve port  

## Render

```powershell
cd docs/diagrams
.\render.ps1
```

Gerekli: Node.js / npm (`npx @mermaid-js/mermaid-cli`).

Alternatif: [Mermaid Live Editor](https://mermaid.live) ile `.mmd` içeriğini yapıştırıp dışa aktarın.

## Kontrol

- [CHECKLIST.md](CHECKLIST.md) – PDF 11 onay listesi  
- [CONSISTENCY.md](CONSISTENCY.md) – Object↔Class ve ER↔Class tutarlılığı  
