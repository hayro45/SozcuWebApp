# SozcuCom Web Uygulaması

Bu proje, Elasticsearch kullanarak haberleri arama, listeleme ve görüntüleme yeteneği sunan bir ASP.NET Core Razor Pages web uygulamasıdır.

## Proje Amacı

Projenin amacı, bir haber web sitesinden toplanan verileri (örneğin, `news.json` dosyası) Elasticsearch veritabanına kaydetmek ve bu verileri Razor Pages kullanarak kullanıcıya sunmaktır. Kullanıcılar, arama ve filtreleme özellikleri sayesinde haberleri kolayca bulabilir ve detaylarını okuyabilir.

## Kullanılan Teknolojiler

*   **ASP.NET Core Razor Pages:** Web uygulamasının arayüzünü oluşturmak için kullanılır.
*   **Elasticsearch:** Haber verilerini depolamak, aramak ve filtrelemek için kullanılan dağıtık arama motorudur.
*   **NEST:** .NET uygulamalarından Elasticsearch ile iletişim kurmak için kullanılan resmi kütüphanedir.
*   **Bootstrap:** Web arayüzü için duyarlı ve modern bir görünüm sağlamak için kullanılan CSS framework'üdür.
*   **Python:** JSON verisini okuyup Elasticsearch formatına dönüştürmek için kullanılmıştır.

## Kurulum

1.  **Elasticsearch Kurulumu:**
    *   [Elasticsearch resmi web sitesinden](https://www.elastic.co/downloads/elasticsearch) Elasticsearch'ü indirin ve kurun.
    *   Elasticsearch'ün `localhost:9200` adresinde çalıştığından emin olun (veya `Program.cs` dosyasındaki bağlantı ayarlarını buna göre düzenleyin).
    *   Kibana da kullanmak isterseniz kurulumunu yapın `http://localhost:5601` adresinden ulaşabilirsiniz.

2.  **Elasticsearch Turkish Analyzer Plugin Kurulumu:**
    *   Aşağıdaki komutlar ile Turkish Analyzer pluginini kurun.

    ```bash
        ./bin/elasticsearch-plugin install analysis-icu
        ./bin/elasticsearch-plugin install analysis-kuromoji
    ```

3.  **Python Ortamı Kurulumu:**
    *   Python'u kurun.
    *   Sanal ortam oluşturun ve aktif edin:

    ```bash
    python -m venv .venv
    .venv/Scripts/activate # Windows
    source .venv/bin/activate # macOS veya Linux
    ```
    *   Elasticsearch kütüphanesini yükleyin:

    ```bash
    pip install elasticsearch
    ```

4.  **Verileri Hazırlama:**
    *   `news.json` dosyasını ekteki gibi indiriniz ve  `src` klasörünün içerisine koyunuz.
    *   Python kodunu kullanarak Elasticsearch'e uygun veriyi `elasticsearch_data.json` adında aynı dizinde oluşturun:
    ```bash
        python olusturucu.py
    ```
     *   Oluşturulan `elasticsearch_data.json` dosyasını kontrol edin.
    
5.  **Elasticsearch İndeksini Oluşturun:**
    *   Kibana Dev Tools (veya Python) kullanarak `haberler` indeksini ve mapping'ini aşağıdaki gibi oluşturun (bkz: **`JSON Dosyasının Yapısını İnceleme ve Elasticsearch'e Uygun Hale Getirme`** bölümü):

        ```json
        PUT /haberler
        {
          "mappings": {
            "properties": {
              "title": { "type": "text", "analyzer": "turkish" },
              "content": { "type": "text", "analyzer": "turkish" },
              "describe": { "type": "text", "analyzer": "turkish" },
                "author": { "type": "keyword" },
               "created_date": { "type": "date" },
               "url": { "type": "keyword" },
                "kategori": { "type": "keyword" }
            }
          }
        }
        ```
        *  Python kodu kullanarak veriyi yükleyin.

       ```bash
        python olusturucu.py
       ```

6.  **ASP.NET Core Projesini Oluşturma (Eğer Yoksa):**
    *   Visual Studio veya benzer bir .NET geliştirme ortamını kullanarak yeni bir ASP.NET Core Web App projesi oluşturun.
    *   Proje oluştururken **Razor Pages** seçeneğini işaretleyin.
7.  **NuGet Paketlerini Yükleme:**
    *   Projenize `NEST` NuGet paketini ekleyin.

8.  **`Program.cs` Dosyasını Düzenleme:**
    *   Elasticsearch bağlantı bilgilerini `Program.cs` dosyasında ayarlayın. (Bkz: **Razor Pages ile Verileri Listeleme (Adım Adım)** bölümü)
9.  **Razor Pages Dosyalarını Oluşturma:**
    *   `Index.cshtml`, `Index.cshtml.cs` ve `_HaberPartial.cshtml` dosyalarını projenize ekleyin.(Bkz: **Razor Pages ile Verileri Listeleme (Adım Adım)** bölümü)
10. **Gerekli CSS ve Scriptleri ekleyin.**
     *   `Pages/_Layout.cshtml` dosyasını düzenleyerek, gerekli link ve scriptleri ekleyin (Bkz: **Razor Pages ile Verileri Listeleme (Adım Adım)** bölümü) ve `wwwroot` klasörü altında CSS dosyası oluşturun ve içeriği tanımlayın.
11. **Projeyi Çalıştırın:** Projenizi build edip çalıştırın.

**Temel Özellikler**

*   **Arama:** Haber başlıkları, içerikleri ve açıklamalarında arama yapılabilir.
*   **Filtreleme:** Haberler kategoriye göre filtrelenebilir (`gundem`, `dunya`, `ekonomi`).
*    **Dinamik Sayfalama:** Sayfa numaraları dinamik olarak oluşturulur, sayfa başına haber sayısı ayarlanabilir. Ayrıca "Daha Fazla Yükle" butonu ile sonraki sayfalara gidebilirsiniz.
*   **Modal ile Haber Detayı:** Haber kartlarına tıklayınca haberin tamamı modalda görünür.
*   **Kullanıcı Dostu Arayüz:**  Arayüzde Bootstrap kullanılarak duyarlı ve modern bir tasarım sağlanmıştır.

## Model Özellikleri

Aşağıdaki tablo, `Models/Haber.cs` dosyasında tanımlanan `Haber` modelinin özelliklerini ve açıklamalarını içermektedir.

| Özellik Adı      | Veri Tipi   | Açıklama                                                                      |
| ---------------- | ----------- | ------------------------------------------------------------------------------ |
| `title`          | `string`    | Haberin başlığını içerir.                                                       |
| `content`        | `string`    | Haberin tam içeriğini içerir.                                                   |
| `describe`       | `string`    | Haberin kısa özetini veya açıklamasını içerir.                                 |
| `author`         | `string`    | Haberi yazan veya yayınlayan kişinin adını içerir.                             |
| `created_date`    | `DateTime`  | Haberin oluşturulduğu veya yayınlandığı tarih ve saati içerir.  |
| `url`            | `string`    | Haberin orijinal kaynağındaki URL'sini içerir.                                 |
| `kategori`       | `string`   | Haberin ait olduğu kategori bilgisini içerir (örneğin, `gundem`, `dunya`, `ekonomi`). |

## Ekran Görüntüleri

![Arama](https://github.com/user-attachments/assets/851e5b22-fa11-4ca6-8553-52810161cca6)
![Sayfalama](https://github.com/user-attachments/assets/3524d2de-aac4-48eb-9691-ae41610e95f2)
![Modal](https://github.com/user-attachments/assets/4221a68e-7b30-405d-9fed-ef6e47a85666)
![Haber Detay](https://github.com/user-attachments/assets/ff5c82a0-c24d-4e05-bdcc-c740bf115620)
![Bilgi Mesajı](https://github.com/user-attachments/assets/0cc62ed5-d548-4a28-b412-cd1bff795179)
![Hover Effect](https://github.com/user-attachments/assets/6fe0d412-60ef-429c-9199-9883fad62ab0)
![Arayüz](https://github.com/user-attachments/assets/8d75c486-45a9-463f-8225-51641f869d88)

**Geliştirme Notları**

*   Bu proje, temel bir haber listeleme ve arama uygulamasıdır. İhtiyaçlarınıza göre daha fazla özellik (örneğin, kullanıcı kayıt, haber ekleme, filtreleme seçenekleri) ekleyebilirsiniz.
*   Elasticsearch sorgularını, proje gereksinimlerinize uygun şekilde optimize etmeyi unutmayın.
*   **Sayfa Düzeni:** Uygulamanın `Index` sayfasında arama yapabilir ve sonuçları görüntüleyebilirsiniz. Sayfalar arasında geçiş yapabilir ve haber detaylarını görebilirsiniz.
*   **Kullanıcı Deneyimi:** Projeyi daha kullanıcı dostu hale getirmek için CSS ve JavaScript kullanarak ek özellikler uygulayabilirsiniz.
