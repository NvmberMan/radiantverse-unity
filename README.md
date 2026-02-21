# Radiantverse Unity

Selamat datang di proyek Unity **Radiantverse**! Proyek ini adalah aplikasi/game berbasis Unity yang memanfaatkan berbagai teknologi modern, mulai dari Machine Learning hingga integrasi layanan Backend.

## 🚀 Fitur & Teknologi

Proyek ini dibangun menggunakan berbagai aset dan *package* unggulan:
- **Unity Engine & URP**: Menggunakan Universal Render Pipeline (URP) untuk rendering grafis yang optimal dan berkualitas tinggi di berbagai platform.
- **Machine Learning (ML-Agents)**: Terintegrasi dengan Unity ML-Agents untuk melatih model kecerdasan buatan (AI) secara *reinforcement learning*.
- **Firebase & Google Sign-In**: Memiliki integrasi Firebase untuk layanan backend (Otentikasi, Database) serta kemudahan proses login menggunakan Google Sign-In.
- **Spine 2D**: Memiliki dukungan animasi *skeletal* 2D (tulang) tingkat lanjut dengan menggunakan *package* Spine-Unity.
- **Cinemachine & Timeline**: Digunakan untuk pengaturan dan pergerakan kamera dinamis serta urutan animasi atau cutscene.
- **ProBuilder & Polybrush**: Peralatan (tools) untuk *whiteboxing*, pembuatan model 3D, dan desain level langsung di dalam editor Unity.
- **Parse**: Terintegrasi dengan Parse Server / SDK sebagai alternatif koneksi backend-as-a-service (BaaS).

## 📁 Struktur Proyek Utama

Struktur penting di dalam folder `Assets/` diatur sebagai berikut:
- **`AI Models/` & `ML-Agents/`**: Berisi konfigurasi dan hasil pelatihan (model) kecerdasan buatan.
- **`Firebase/` & `GoogleSignIn/`**: Modul dan skrip untuk mengamankan otentikasi dan komunikasi data.
- **`Scripts/`**: Kode sumber (*C# scripts*) untuk mekanik utama, logika permainan, dan pengaturan antarmuka.
- **`Scenes/`**: Tempat penyimpanan keseluruhan status level atau tampilan yang bisa dijalankan secara spesifik.
- **`Prefabs/`**: Objek permainan (*GameObject*) yang telah dikonfigurasi dan siap diletakkan berulang-ulang di scene.
- **`Animation & Animator/`**: Sistem animasi untuk karakter, UI, maupun objek lainnya.

## 🛠️ Cara Memulai (Getting Started)

### Persyaratan Sistem
- **Unity Hub** dan **Unity Editor** (sangat direkomendasikan untuk menggunakan versi yang setara ketika proyek dibuat untuk meminimalisasi konflik).
- Akun Firebase (jika Anda bertugas menangani backend atau database sistem).

### Instalasi & Menjalankan Proyek
1. *Clone* atau *Download* arsip proyek ini ke mesin lokal Anda.
2. Buka **Unity Hub**, pilih opsi **Open** -> **Add project from disk**, kemudian arahkan ke folder utama `radiantverse-unity`.
3. Buka proyek tersebut. Unity akan mengambil beberapa waktu di awal untuk memproses aset dan meresolusi *packages* (Universal RP, Firebase, ML-Agents).
4. Setelah terbuka, carilah *scene* yang Anda ingin jalankan di struktur direktori `Assets/Scenes/`. Klik 2 kali, lalu tekan tombol **Play** ▶️ di panel tengah atas Unity.

## 📝 Catatan Penting
- **Konfigurasi Firebase:** Pastikan bahwa *file* kredensial seperti `google-services.json` (Android) atau `GoogleService-Info.plist` (iOS) sudah tersedia di folder yang sesuai dengan environment Anda sebelum melakukan proses *Build*.

## 📄 Lisensi
*(Mohon cantumkan/perbarui tipe lisensi proyek Anda di bagian ini)*
