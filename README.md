# Monopoly GO - Unity Case Study 🎲

[🇹🇷 Türkçe](#türkçe) | [🇬🇧 English](#english)

---
<img width="1919" height="1079" alt="image" src="https://github.com/user-attachments/assets/7033564e-76a7-4e3f-a75c-5ac23b797952" />

<h2 id="türkçe">🇹🇷 Türkçe</h2>

Joker Games için hazırladığım Unity 3D vaka çalışması. Projede "Monopoly GO" ve "Board Kings" benzeri bir board game altyapısı kurdum. İstenen en kritik kısıtlamalardan biri olan "3. parti eklenti (DoTween vb.) yasak" kuralına sadık kalabilmek için tüm animasyon ve geçişleri kendi lerp ve coroutine tabanlı sistemimle çözdüm.

### 🚀 Neler Yaptım?

* **CustomTween Sistemi:** DoTween vb. eklentiler yasak olduğu için, Lerp ve Coroutine kullanarak kendi tabanlı `CustomTween` animasyon sistemimi yazdım. Projedeki diğer tüm scriptler UI geçişlerini, objelerin scale/jump animasyonlarını ve kameranın pürüzsüz (smooth) takibini bu merkezi sistem üzerinden kullanmaktadır. Dışa bağımlılık sıfırdır.
* **Fiziksel 6 Yüzeyli Zar Mekaniği:** Zar atma işlemini sadece arayüzde değişen bir sayı olarak bırakmadım. Girilen adım sayısına göre zarlar 3D uzayda fiziksel olarak fırlatılıyor ve takla atıyor. 1 ile 6 arasında girilen değerlerde, zar fiziksel bir tutarlılıkla tam olarak o yüzeyde duruyor. (6'dan daha yüksek bir sayı girilirse de karakter girilen sayı kadar adım atar).
* **3D Şehir Arka Planı:** Sadece düz bir tahta yerine, ProBuilder ile kendi modellediğim mesh'ler ve bazı hazır prefab'lar kullanarak arka plana özel bir 3D şehir inşa ettim.
* **Dinamik 3D Harita:** Harita klasik bir kare tahta yerine, istenildiği gibi tek bir çizgi (Line) formatında. `MapConfig.json` dosyasını okuyarak çalışma zamanında (runtime) dinamik olarak oluşturuluyor.
* **Dinamik UI ve Kontroller:** Profesyonel ve akıcı bir arayüz tasarladım. Ses açma/kapama (mute) ve oyundan çıkma gibi temel fonksiyonları barındıran dinamik UI butonları eklendi.
* **Kalıcı Envanter (Persistence):** Toplanan meyveler (elma, armut, çilek) `PlayerPrefs` ile cihaza kaydediliyor. Oyunu kapatıp açtığınızda envanteriniz kaldığı yerden devam ediyor.
* **Efektler ve Geri Bildirim:** Karakter karelerden geçerken veya hedefe ulaştığında materyal üzerinden glow/flash efektleri ve partiküller tetikleniyor. UI üzerindeki sayılar aniden değişmek yerine akıcı bir şekilde sayarak (`CountTo`) artıyor.

### 🏗️ Mimari Yaklaşım ve İş Akışı

* **SOLID ve OOP:** Proje kod tabanını SOLID ve OOP prensiplerine uygun, kolay genişletilebilir bir yapıda tutmaya özen gösterdim. Sistemleri birbirine spagetti gibi bağlamak yerine `DiceManager`, `MapManager`, `InventoryManager` gibi Singleton yöneticiler kullandım.
* **Veri İzolasyonu:** `TileData` ve `MapData` gibi sınıflarla veri tarafını izole ettim, böylece ileride yeni meyveler veya farklı tile özellikleri eklemek çok kolay bir hale getirildi.
* **Git Akışı:** Geliştirme süreci profesyonel standartlara uygun olarak adım adım `Work` branch'inde yürütüldü. Tüm işlemler tamamlandıktan sonra `Main` branch ile merge edilip yayınlandı.

### 📦 Kullanılan Varlıklar ve Kaynaklar
* Oyunda kullanılan ses efektleri **Pixabay**'dan temin edilmiştir.
* Kullanılan Asset Store eşyaları, proje dizinindeki `Downloaded Assets` klasöründe düzenli bir şekilde tutulmaktadır.

### ⚙️ Kurulum ve Oynanış

* **Motor:** Unity 6000.0.68f1
* Projeyi klonladıktan sonra `Scenes` klasöründeki `Game` adlı sahneyi açıp doğrudan Play tuşuna basarak deneyebilirsiniz.
* İsterseniz doğrudan buradaki linkten derlenmiş (build) oyunu indirip oynayabilirsiniz:  
  🔗 [Google Drive İndirme Linki](https://drive.google.com/drive/folders/1r0ZOCp2OQzXQjPG_LEBP77izc74bCOyY?usp=sharing)

### 🎥 Oynanış Videosu
👉 [YouTube Üzerinden İzle](https://youtu.be/abJ3mD5DZUs)

---

<h2 id="english">🇬🇧 English</h2>

A Unity 3D case study prepared for Joker Games. In this project, I built a board game infrastructure similar to "Monopoly GO" and "Board Kings". To adhere to the strict "No 3rd party plugins (like DoTween)" rule, I managed all animations and transitions using a custom-built system.

### 🚀 Features & Implementations

* **CustomTween System:** Since external plugins were forbidden, I developed a custom Lerp and Coroutine-based `CustomTween` animation system. All other scripts in the project use this centralized system for UI transitions, object scale/jump animations, and smooth camera tracking. Zero external dependencies.
* **Physical 6-Sided Dice Mechanics:** Rolling the dice isn't just a UI text update. Depending on the input, the dice are physically thrown into 3D space and tumble. For inputs between 1 and 6, the dice land exactly on the requested face, maintaining physical consistency. (Inputs greater than 6 are also supported for character movement).
* **3D City Background:** Instead of a plain board, I built a custom 3D city environment in the background using custom meshes created with ProBuilder alongside some ready-made prefabs.
* **Dynamic 3D Map:** The map is dynamically generated at runtime as a single line format by reading a `MapConfig.json` file.
* **Dynamic UI & Controls:** Designed a professional and fluid UI, integrating functional buttons such as sound toggle (mute/unmute) and a quit game option.
* **Persistent Inventory:** Collected fruits (apples, pears, strawberries) are saved to the device using `PlayerPrefs`. Your inventory persists even after restarting the game.
* **Effects & Feedback:** Glow/flash effects on materials and particle systems are triggered when the character passes over tiles or reaches a target. UI numbers count up smoothly (`CountTo`) instead of changing instantly.

### 🏗️ Architecture & Workflow

* **SOLID & OOP:** The codebase is structured around SOLID and OOP principles for easy scalability. I used Singleton managers (`DiceManager`, `MapManager`, `InventoryManager`) to avoid spaghetti code.
* **Data Isolation:** Isolated data using classes like `TileData` and `MapData`, making it extremely easy to add new fruits or different tile features in the future.
* **Git Workflow:** Development was carried out step-by-step in a `Work` branch. Once completed, it was merged into the `Main` branch for the final release.

### 📦 Assets & Resources
* Audio and sound effects were sourced from **Pixabay**.
* Unity Asset Store items used in the project are neatly organized under the `Downloaded Assets` folder.

### ⚙️ Installation & Play

* **Engine:** Unity 6000.0.68f1
* After cloning the repository, simply open the `Game` scene in the `Scenes` folder and press Play.
* Alternatively, you can download and play the standalone build directly from this link:  
  🔗 [Google Drive Download Link](https://drive.google.com/drive/folders/1r0ZOCp2OQzXQjPG_LEBP77izc74bCOyY?usp=sharing)

### 🎥 Gameplay Video
👉 [Watch on YouTube](https://youtu.be/abJ3mD5DZUs)

---

### 📬 İletişim / Contact

**Uğur Can**
* **Title:** Game Developer
* **Email:** ugurcanmailtr@gmail.com
* **Portfolio:** [Stechkom Games](https://www.stechkomsoftware.com/stechkom-games)
* **LinkedIn:** [Uğur Can](https://www.linkedin.com/in/u%C4%9Fur-can-a1a72a209/)
