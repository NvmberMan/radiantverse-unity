# 🌌 Radiantverse Unity Project

Selamat datang di repositori utama **Radiantverse**, sebuah game **2.5D Competitive Runner** yang menggabungkan kecepatan, strategi combat, dan teknologi AI mutakhir. Proyek ini dibangun di atas Unity dengan fokus pada performa tinggi dan pengalaman kompetitif yang dinamis.

---

## 🎮 Core Gameplay & Mekanik
Berbeda dengan runner biasa, Radiantverse mengintegrasikan:
* **High-Speed Competition**: Balapan lari kompetitif dalam perspektif 2.5D.
* **Integrated Combat System**: Karakter dilengkapi dengan kemampuan menyerang dan bertahan yang diatur melalui *State Machine* yang presisi.
* **Advanced AI Opponents**: Lawan yang dilatih menggunakan **Reinforcement Learning (ML-Agents)** untuk memberikan tantangan yang adaptif bagi pemain.

---

## 🚀 Fitur & Teknologi Utama

### 1. Visual & Rendering
* **URP (Universal Render Pipeline)**: Dioptimalkan untuk visual estetik namun tetap ringan untuk performa kompetitif.
* **Spine 2D**: Animasi *skeletal* karakter yang halus, memungkinkan transisi antar *state* (lari, lompat, serang) terasa organik.
* **Cinemachine & Timeline**: Sistem kamera dinamis yang mengikuti aksi intens tanpa kehilangan fokus pada jalur balapan.

### 2. Kecerdasan Buatan (AI)
* **Unity ML-Agents**: Implementasi model AI untuk bot kompetitor.
* **Curriculum Learning**: Bot dilatih secara bertahap untuk menguasai rintangan kompleks di dalam level.

### 3. Backend & Infrastruktur
* **Firebase & Google Sign-In**: Menangani otentikasi pemain, penyimpanan profil, dan sinkronisasi data cloud.
* **Parse SDK**: Digunakan sebagai *logic layer* tambahan untuk manajemen data backend yang fleksibel.
* **ProBuilder & Polybrush**: Digunakan untuk *whiteboxing* dan desain level cepat langsung di dalam editor.

---

## 📁 Struktur Proyek Utama
```text
Assets/
├── 🤖 ML-Agents/       # Konfigurasi Brain, Academy, dan hasil Training model AI.
├── ⚔️ Scripts/          # Core Logic: State Machines, Combat System, & Controller.
├── 🦴 Spine/            # Aset karakter Spine 2D (Atlas, Skeleton data, Animations).
├── 🏗️ LevelDesign/     # Prefabs ProBuilder & Polybrush untuk prototyping level.
└── 🔌 Integrations/    # Folder khusus Firebase, GoogleSignIn, dan Parse SDK.
