namespace Library.Data.Seed;

public static class SeedCatalogData
{
    public static string CoverUrl(string isbn) =>
        $"https://covers.openlibrary.org/b/isbn/{isbn}-L.jpg";

    public static readonly string[] Categories =
    [
        "Roman",
        "Bilim",
        "Tarih",
        "Klasik",
        "Fantastik",
        "Felsefe",
        "Çocuk",
        "Polisiye"
    ];

    public static readonly string[] Publishers =
    [
        "Yapı Kredi Yayınları",
        "Tübitak Yayınları",
        "Penguin Classics",
        "HarperCollins",
        "Vintage",
        "Oxford University Press",
        "Can Yayınları",
        "İletişim Yayınları"
    ];

    public static IReadOnlyList<SeedBookDefinition> Books { get; } =
    [
        new("1984", "9780451524935", 1949, "Distopik bir gelecek romanı.", "Klasik", "Penguin Classics", "George Orwell"),
        new("Hayvan Çiftliği", "9780451526342", 1945, "Alegorik siyasi hiciv.", "Klasik", "Penguin Classics", "George Orwell"),
        new("Gurur ve Önyargı", "9780141439518", 1813, "İngiliz toplumunda aşk ve sınıf.", "Klasik", "Penguin Classics", "Jane Austen"),
        new("Bülbülü Öldürmek", "9780061120084", 1960, "Adalet ve ırkçılık üzerine bir klasik.", "Roman", "HarperCollins", "Harper Lee"),
        new("Muhteşem Gatsby", "9780743273565", 1925, "Amerikan rüyasının eleştirisi.", "Klasik", "Penguin Classics", "F. Scott Fitzgerald"),
        new("Harry Potter ve Felsefe Taşı", "9780747532699", 1997, "Büyücülük dünyasına giriş.", "Fantastik", "Bloomsbury", "J. K. Rowling"),
        new("Hobbit", "9780547928227", 1937, "Orta Dünya macerası.", "Fantastik", "HarperCollins", "J. R. R. Tolkien"),
        new("Yüzüklerin Efendisi", "9780544003415", 1954, "Epik fantastik üçleme.", "Fantastik", "HarperCollins", "J. R. R. Tolkien"),
        new("Cesur Yeni Dünya", "9780060850524", 1932, "Teknolojik distopya.", "Klasik", "HarperCollins", "Aldous Huxley"),
        new("Fahrenheit 451", "9781451673319", 1953, "Kitapların yasaklandığı bir dünya.", "Roman", "Simon & Schuster", "Ray Bradbury"),
        new("Suç ve Ceza", "9780140449136", 1866, "Vicdan ve ahlak üzerine Rus klasiği.", "Klasik", "Penguin Classics", "Fyodor Dostoyevski"),
        new("Anna Karenina", "9780143035008", 1877, "Aşk, aile ve toplum.", "Klasik", "Penguin Classics", "Lev Tolstoy"),
        new("Savaş ve Barış", "9781400079988", 1869, "Napoleon savaşları döneminde Rusya.", "Klasik", "Vintage", "Lev Tolstoy"),
        new("Odysseia", "9780140268867", 1996, "Homeros'un epik yolculuğu (Fagles çevirisi).", "Klasik", "Penguin Classics", "Homeros"),
        new("Hamlet", "9780743477123", 1603, "Shakespeare trajedisi.", "Klasik", "Simon & Schuster", "William Shakespeare"),
        new("Romeo ve Juliet", "9780743477116", 1597, "Yasak aşk trajedisi.", "Klasik", "Simon & Schuster", "William Shakespeare"),
        new("Yüzyıllık Yalnızlık", "9780060883287", 1967, "Macondo ailesinin büyülü gerçekçi öyküsü.", "Roman", "HarperCollins", "Gabriel García Márquez"),
        new("Simyacı", "9780062315007", 1988, "Kişisel efsaneyi arayan bir çoban.", "Roman", "HarperCollins", "Paulo Coelho"),
        new("Mezbaha Beş", "9780385333849", 1969, "Savaş ve zaman yolculuğu.", "Roman", "Vintage", "Kurt Vonnegut"),
        new("Küçük Prens", "9780156013987", 1943, "Çocuklar ve yetişkinler için masal.", "Çocuk", "Houghton Mifflin", "Antoine de Saint-Exupéry"),
        new("Don Kişot", "9780060934347", 1605, "Şövalyelik üzerine hiciv.", "Klasik", "HarperCollins", "Miguel de Cervantes"),
        new("Moby Dick", "9780142437247", 1851, "Balina avı ve takıntı.", "Klasik", "Penguin Classics", "Herman Melville"),
        new("Frankenstein", "9780486282114", 1818, "Yaratılış ve sorumluluk.", "Klasik", "Dover", "Mary Shelley"),
        new("Dracula", "9780486411095", 1897, "Gotik vampir romanı.", "Klasik", "Dover", "Bram Stoker"),
        new("Jane Eyre", "9780141441146", 1847, "Bağımsız bir kadının hikâyesi.", "Klasik", "Penguin Classics", "Charlotte Brontë"),
        new("Uğultulu Tepeler", "9780141439556", 1847, "Tutku ve intikam.", "Klasik", "Penguin Classics", "Emily Brontë"),
        new("Akıl ve Duygu", "9780141439662", 1811, "İki kız kardeşin evlilik hikâyesi.", "Klasik", "Penguin Classics", "Jane Austen"),
        new("Emma", "9780141439587", 1815, "Eşleştirme ve kendini bulma.", "Klasik", "Penguin Classics", "Jane Austen"),
        new("Dorian Gray'in Portresi", "9780141439570", 1890, "Güzellik ve ahlak.", "Klasik", "Penguin Classics", "Oscar Wilde"),
        new("Sherlock Holmes'un Maceraları", "9780199536955", 1892, "Klasik dedektif öyküleri.", "Polisiye", "Oxford University Press", "Arthur Conan Doyle"),
        new("Alice Harikalar Diyarında", "9780141439761", 1865, "Hayal gücünün klasiği.", "Çocuk", "Penguin Classics", "Lewis Carroll"),
        new("Huckleberry Finn'in Maceraları", "9780486280615", 1884, "Mississippi boyunca kaçış.", "Klasik", "Dover", "Mark Twain"),
        new("Fareler ve İnsanlar", "9780140177398", 1937, "Dostluk ve Amerikan rüyası.", "Roman", "Penguin Classics", "John Steinbeck"),
        new("Gazap Üzümleri", "9780143039433", 1939, "Büyük Buhran döneminde göç.", "Roman", "Penguin Classics", "John Steinbeck"),
        new("Görünmez Adam", "9780679732761", 1952, "Kimlik ve ırk üzerine roman.", "Roman", "Vintage", "Ralph Ellison"),
        new("Sevgili", "9781400033416", 1987, "Kölelik mirası üzerine.", "Roman", "Vintage", "Toni Morrison"),
        new("Damızlık Kızın Öyküsü", "9780385490818", 1985, "Feminist distopya.", "Roman", "Vintage", "Margaret Atwood"),
        new("Neuromancer", "9780441569595", 1984, "Siberpunk klasiği.", "Fantastik", "Ace", "William Gibson"),
        new("Dune", "9780441172719", 1965, "Çöl gezegeni Arrakis'in destanı.", "Fantastik", "Ace", "Frank Herbert"),
        new("Vakıf", "9780553293357", 1951, "Galaktik imparatorluk ve psikotarih.", "Fantastik", "Bantam", "Isaac Asimov"),
        new("Otomatik Portakal", "9780393312836", 1962, "Şiddet ve özgür irade.", "Roman", "W. W. Norton", "Anthony Burgess"),
        new("Otostopçunun Galaksi Rehberi", "9780345391803", 1979, "Komik bilimkurgu klasiği.", "Fantastik", "Del Rey", "Douglas Adams"),
        new("Ender'in Oyunu", "9780812550702", 1985, "Askeri strateji ve çocuk askerler.", "Fantastik", "Tor", "Orson Scott Card"),
        new("Zamanın Kısa Tarihi", "9780553380163", 1988, "Evren bilimine giriş.", "Bilim", "Bantam", "Stephen Hawking"),
        new("Sapiens", "9780062316097", 2011, "İnsanlığın kısa tarihi.", "Tarih", "HarperCollins", "Yuval Noah Harari"),
        new("Homo Deus", "9780062464316", 2015, "İnsanlığın geleceği üzerine.", "Tarih", "HarperCollins", "Yuval Noah Harari"),
        new("Hızlı ve Yavaş Düşünme", "9780374533557", 2011, "Davranışsal ekonomi.", "Bilim", "Farrar Straus", "Daniel Kahneman"),
        new("Atomik Alışkanlıklar", "9780735211292", 2018, "Küçük değişikliklerle büyük sonuçlar.", "Bilim", "Avery", "James Clear"),
        new("Bencil Gen", "9780199291151", 1976, "Evrimsel biyoloji perspektifi.", "Bilim", "Oxford University Press", "Richard Dawkins"),
        new("Kozmos", "9780345539434", 1980, "Evrenin hikâyesi.", "Bilim", "Ballantine", "Carl Sagan"),
        new("Tüfek, Mikrop ve Çelik", "9780393317558", 1997, "Uygarlıkların yükselişi.", "Tarih", "W. W. Norton", "Jared Diamond"),
        new("Benim Adım Kırmızı", "9780375701672", 1998, "Osmanlı minyatürcüleri arasında cinayet.", "Roman", "Vintage", "Orhan Pamuk"),
        new("Kar", "9780375706868", 2002, "Kars'ta geçen siyasi roman.", "Roman", "Vintage", "Orhan Pamuk"),
        new("Masumiyet Müzesi", "9780307385833", 2008, "İstanbul'da bir aşk romanı.", "Roman", "Vintage", "Orhan Pamuk"),
        new("İstanbul: Hatıralar ve Şehir", "9781400033881", 2003, "Şehir anıları.", "Tarih", "Vintage", "Orhan Pamuk"),
        new("Beyaz Kale", "9780375701610", 1985, "Kimlik ve ikilik.", "Roman", "Vintage", "Orhan Pamuk"),
        new("Çalıkuşu", "9789750738609", 1922, "Türk edebiyatının sevilen romanı.", "Roman", "Can Yayınları", "Reşat Nuri Güntekin"),
        new("Kürk Mantolu Madonna", "9789754700114", 1943, "Aşk ve yalnızlık.", "Roman", "Yapı Kredi Yayınları", "Sabahattin Ali"),
        new("İnce Memed", "9789750802928", 1955, "Toroslarda eşkıya destanı.", "Roman", "Yapı Kredi Yayınları", "Yaşar Kemal"),
        new("Tutunamayanlar", "9789754704518", 1971, "Modern Türk edebiyatının dönüm noktası.", "Roman", "İletişim Yayınları", "Oğuz Atay")
    ];
}

public sealed record SeedBookDefinition(
    string Title,
    string Isbn,
    int PublishYear,
    string Description,
    string Category,
    string Publisher,
    string Author);
