# Wheel Rewards Progression Game

A Unity game based on spinning mechanics and zone progression.  
The player spins the wheel to collect rewards, encounters higher-value rewards as zones advance, and avoids bombs while expanding their inventory.  
Core systems include Wheel mechanics, Zone progression, Slot/Reward generation, Card inventory, Object Pooling, and animated UI structures.

---

## 🎮 Playable Demo

👉 **https://omerozerf.itch.io/wheel-of-fortune**

---

## 🎥 Gameplay Videos & Screenshots:  
👉 **https://drive.google.com/drive/folders/1vAjKEF8tkwy2UwZllpBJsYHQo_hDQJl1?usp=sharing**

---

## 🎮 Gameplay Preview

| Image | Description |
|-------|-------------|
| ![](./MyRecordings/GameplayGIF.gif) | Gameplay GIF — Aspect Ratio: 16:9 |

---

# 📸 Screenshots

## In-Game Screens

| 16:9 | 20:9 | 4:3 |
|------|------|-----|
| ![](./MyRecordings/InGameScreen16-9.png) | ![](./MyRecordings/InGameScreen20-9.png) | ![](./MyRecordings/InGameScreen4-3.png) |

---

## Reward Screens

| 16:9 | 20:9 | 4:3 |
|------|------|-----|
| ![](./MyRecordings/RewardsScreen16-9.png) | ![](./MyRecordings/RewardsScreen20-9.png) | ![](./MyRecordings/RewardsScreen4-3.png) |

---

## Bomb / Lose Screens

| 16:9 | 20:9 | 4:3 |
|------|------|-----|
| ![](./MyRecordings/BombScreen16-9.png) | ![](./MyRecordings/BombScreen20-9.png) | ![](./MyRecordings/BombScreen4-3.png) |

---

## Exit Screens

| 16:9 | 20:9 | 4:3 |
|------|------|-----|
| ![](./MyRecordings/ExitScreen16-9.png) | ![](./MyRecordings/ExitScreen20-9.png) | ![](./MyRecordings/ExitScreen4-3.png) |

---

# 🚀 Core Features

## Wheel System
- All wheel parameters adjustable via Inspector.
- WheelSlotController selects SlotSO for each slice.
- Empty `allowedSlots` → selection from global pool.
- Defined `allowedSlots` → restricted selection.
- Normal Zones → **1 bomb** assigned randomly.
- Safe/Super Zones → **0 bombs**.
- Rarity weights increase with zone progression.

## Reward System
- Automatic rarity weighting (Common–Legendary).
- SlotSO stores icon, rarity, metadata, and values.
- Reward scaling tied to zone index.
- Zone power affects min-max reward outputs.

## Zone System
- Infinite horizontal scrolling structure.
- Zones recycled when leaving the screen.
- Safe Zone every 5 levels, Super Zone every 30 levels.
- UI colors adapt dynamically per zone.

## Card System
- New rewards create a new Card; duplicates increase count.
- VFX sequence: scatter → move → fade.
- All VFX controlled by Object Pool.

## UI & Screen System
- All screens use CanvasGroup fade transitions.
- Bomb triggers red glow effect.
- Buttons connected only via scripts (no Inspector OnClick).
- UI animators stored on separate child objects.

## Object Pooling
- All VFX pooled.
- No Instantiate/Destroy during gameplay.
- DOTween sequences auto-despawn.

---

# 🛠 Technical Compliance

- Canvas Scaler: **Expand**
- All UI: **TextMeshPro**
- Anchors/pivots verified for 20:9, 16:9, 4:3
- Sliced Sprites used where needed
- Non-interactive images → RaycastTarget disabled
- No OnClick events in Inspector
- Animator components not placed on root transforms
- Dynamic UI names end with `_value`
- Required resolution screenshots provided

---

# 📱 Minimum System Requirements for APK (Android)

- Android 7.0+
- ARMv7 & ARM64 supported
- Landscape-only
- Tested on: 20:9, 16:9, 4:3

---

# 🎡 Wheel Spin Behaviour (Technical)

- Spin uses DOTween → `DORotate`
- Ease: **OutCubic**
- Spin duration increases per zone
- Final rotation snaps to nearest slice angle
- Slice index determined from normalized Z rotation
- Consistent behavior across all aspect ratios

---

# 📈 Zone Progression Formula

- `zonePower = zoneIndex * powerMultiplier`
- Reward ranges scale from zonePower
- Rarity weights ramp with zoneIndex
- Safe/Super Zones override bomb logic and modify rarity probabilities

---

# 💣 Bomb Logic

- Normal Zones → **exactly 1 bomb**
- Safe Zones → **0 bombs**
- Super Zones → **0 bombs**
- Bomb always assigned to a valid non-reward slice
- If slice restrictions exist, bomb respects allowedSlots rules

---

# 🧩 ScriptableObject Structure

## SlotSO
- Icon  
- Reward type  
- Rarity  
- Metadata  
- Allowed slice configuration  

## ZoneInfoSO
- Zone type (Normal, Safe, Super)  
- Theme colors  
- Reward modifiers  
- Rarity weighting  

## CommonVariablesSO
- Safe Zone interval  
- Super Zone interval  
- Base multipliers  
- Global gameplay values  

---

# 🧩 Architectural Flow

1. Game loads → Zones created  
2. Player taps → Wheel spins  
3. Wheel stops → Slice identified  
4. Reward processed → Card updated  
5. Zone progresses → New slots generated  
6. Exit only possible in Safe & Super Zones  
