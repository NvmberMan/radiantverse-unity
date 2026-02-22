# 🌌 Radiantverse Unity: Next-Gen 2.5D Competitive Runner

Welcome to the official repository of **Radiantverse**, an ambitious 2.5D competitive racing game built with Unity. Radiantverse redefines the runner genre by merging high-speed platforming with a sophisticated combat system and adaptive AI opponents.

---

## 🎮 Core Gameplay Experience

Radiantverse isn't just about running; it's about mastery. The game features:
* **High-Octane Competition**: Multi-lane 2.5D racing where every millisecond counts.
* **Tactical Combat System**: Integrated offensive and defensive abilities, allowing players to disrupt opponents while maintaining momentum.
* **Dynamic Obstacles**: Procedurally influenced level design that keeps every run fresh and challenging.

---

## 🚀 Technical Stack & Architecture

### 1. Visuals & Animation
* **URP (Universal Render Pipeline)**: Custom shaders and lighting optimized for high-performance mobile and PC rendering.
* **Spine 2D Integration**: Advanced skeletal animation for fluid character transitions between running, attacking, and dodging.
* **Cinemachine & Timeline**: Adaptive camera framing that zooms and pans dynamically based on player speed and combat intensity.
* **ProBuilder & Polybrush**: Rapid level prototyping and "whiteboxing" directly within the Unity Editor for iterative design.

### 2. Artificial Intelligence (ML-Agents)
* **Reinforcement Learning**: Opponents are trained using **Unity ML-Agents** to simulate human-like competitive behavior.
* **Curriculum Learning**: AI "Bot" agents undergo staged training to master complex maneuvers, from basic jumping to advanced combat tactics.
* **Inference**: Pre-trained models (.onnx) are deployed for real-time, low-overhead AI competition.

### 3. Backend & Services
* **Firebase Ecosystem**: 
    * **Authentication**: Secure login via Google Sign-In.
    * **Realtime Database/Firestore**: Cloud-based player profiles, progression, and leaderboards.
* **Parse Server SDK**: Utilized as a flexible Logic Layer for complex data relations and backend-as-a-service (BaaS) functionality.

---

## 📁 Project Structure

```text
Assets/
├── 🤖 ML-Agents/       # Neural network configurations, Brains, and .onnx models.
├── ⚔️ Scripts/          # C# Source Code
│   ├── Core/           # Game Managers and Singleton patterns.
│   ├── FSM/            # Finite State Machine for Player/AI logic.
│   ├── Combat/         # Damage dealers, hitboxes, and ability logic.
│   └── UI/             # Responsive HUD and Menu systems.
├── 🦴 Spine/            # Character skeletons, Atlases, and Animation Controllers.
├── 🏗️ LevelDesign/     # Environment prefabs, materials, and ProBuilder data.
├── 🔌 Integrations/    # Third-party SDKs (Firebase, GoogleSignIn, Parse).
└── 🎬 Scenes/           # Main Menu, Training Grounds, and Competitive Levels.
