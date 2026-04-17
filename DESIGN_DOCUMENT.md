# Hollow Vault — AI Design Document
**GAME-10020 · Assignment 3: AI System Implementation**  
Student: Yatharth Chopra

---

## Game Concept

**Hollow Vault** is a top-down action-adventure roguelike set inside an underground cursed treasury. The player — a relic thief — navigates procedurally assembled vault rooms, loots relics, and escapes alive. Two AI agents guard the vault, each with distinct sensory systems and emergent co-dependence.

---

## Gameplay Video

> _[Link to be added once recorded and uploaded to YouTube]_

---

## AI Agents Overview

| Agent | Type | Sensor | Role |
|---|---|---|---|
| **Crypt Sentinel** | Undead armoured guardian | Vision cone 60° / 9 m, hearing 8 m | Slow, high-HP pursuer — punishes visibility mistakes |
| **The Shade** | Spectral stalker | Sound radius 8 m, light sensor 5 m | Fragile attrition threat — punishes noise and rewards torch use |

---

## FSM Diagram — Crypt Sentinel

```
               ┌──────────────────────────────────────┐
               │         waypoint loop                │
               ▼                                      │
         ┌──────────┐  hears noise /   ┌─────────────┐│  confirms sighting ┌────────┐
  ●─────►│  PATROL  │──────────────────►  INVESTIGATE ├────────────────────►  COMBAT│
         └────┬─────┘  sees player     └──────┬──────┘                     └───┬────┘
              │                    no target found│                             │
              │◄───────────────────────────────────┘           player out of   │
              │                                                 range (4s lost) │
         ┌────▼─────┐  target lost >4s ┌─────────────┐         ┌──────────────┘
         │  RETURN  │◄─────────────────│   RALLYING  │◄──────── hit by player ── ► STAGGERED
         └──────────┘                  └─────────────┘          recover (1.5s) ──────────►(COMBAT)
              │ reached home waypoint         ▲
              └──────────────────────────────►│  HP < 50%, Shade in range
                        → PATROL              │
```

### Crypt Sentinel — States

| State | Behaviour |
|---|---|
| **PATROL** | Walks between waypoints in a loop at 1.8 m/s. Vision cone active. Transitions to INVESTIGATE on sound or sight. |
| **INVESTIGATE** | Moves toward last known noise/sight position at patrol speed. Shows **?** icon. Escalates to COMBAT on confirmed sighting; returns after 6 s timeout. |
| **COMBAT** | Charges player at 3.2 m/s and attacks at melee range. Shows **!** icon. Maintains 4 s memory of last seen position after losing sight. |
| **STAGGERED** | Brief 1.5 s stun after being hit. Agent stops moving. Gives player a tactical window. Transitions to RALLYING if HP ≤ 50%, otherwise resumes COMBAT. |
| **RALLYING** | Stops and broadcasts a rally cry. If The Shade is within 10 m, it enters RALLIED → STALK. Resumes COMBAT after 1.2 s. One-shot per life. |
| **RETURN** | Walks back to patrol origin. Vision and hearing still active — can re-enter INVESTIGATE en route. |

### Crypt Sentinel — Transitions

| From | To | Condition |
|---|---|---|
| PATROL | INVESTIGATE | Player enters view cone OR hearing detects sound |
| INVESTIGATE | COMBAT | Player confirmed in view cone |
| INVESTIGATE | RETURN | Investigation timeout (6 s, no sighting) |
| COMBAT | STAGGERED | `TakeDamage()` called (player hits Sentinel) |
| COMBAT | RALLYING | HP ≤ 50% threshold, Shade within 10 m, not yet rallied |
| COMBAT | RETURN | Player out of sight for > 4 s (memory expired) |
| STAGGERED | RALLYING | Recovery complete, HP ≤ 50%, not yet rallied |
| STAGGERED | COMBAT | Recovery complete, HP > 50% |
| RALLYING | COMBAT | Rally animation finished (1.2 s) |
| RETURN | PATROL | Home waypoint reached |
| RETURN | INVESTIGATE | Sound or sight detected while returning |

---

## FSM Diagram — The Shade

```
         perimeter loop
              ┌──────────────────────────────────────────────────────────────────┐
              ▼                                                                  │
  ●──────► DRIFT ────sound detected────► ALERT ────location confirmed────► STALK ─┘
              ▲         ▲         sound fades (3s) ◄──location lost──┘     │
              │         │                                                   │ player within 3m
              │         │                                                   ▼
              │         │                             torch lit nearby ──► HUNT
              │         │                                                   │ player escapes >5m
              │    safe shadow reached                                      │ ── ► STALK
              │         │
              │     RETREAT ◄──────────────────── torch lit (from STALK/HUNT/ALERT)
              │
          RALLIED ─── summoned by Sentinel RALLYING CRY → immediately enters STALK
```

### The Shade — States

| State | Behaviour |
|---|---|
| **DRIFT** | Floats slowly (2.5 m/s) along room perimeter waypoints. Passive. Sound sensor active. |
| **ALERT** | Stops and orients toward sound source. Waits for confirmation. Transitions to STALK if sound heard again; returns to DRIFT after 3 s of silence. |
| **STALK** | Moves silently toward last known sound origin. Does not increase speed. Light sensor active — retreats immediately on torch. |
| **HUNT** | Locks onto player directly. Closes at 4.5 m/s. Drains player stamina on contact. |
| **RETREAT** | Flees to furthest perimeter waypoint (shadow). Triggered by torch detection. Waits until torch is extinguished AND shadow reached before resuming DRIFT. |
| **RALLIED** | Entered via Sentinel's RALLYING CRY. One-frame state that immediately transitions to STALK with Sentinel's last-known player position. |

### The Shade — Transitions

| From | To | Condition |
|---|---|---|
| DRIFT | ALERT | `heardSoundThisFrame` within 8 m hearing radius |
| ALERT | STALK | Sound heard again within alert timeout (location confirmed) |
| ALERT | DRIFT | Alert timeout (3 s) without new sound |
| ALERT | RETREAT | Torch detected within 5 m |
| STALK | HUNT | Player within 3 m (`huntRange`) |
| STALK | ALERT | Sound fades for 3 s (no new sounds) |
| STALK | RETREAT | Torch detected within 5 m |
| HUNT | RETREAT | Torch detected within 5 m |
| HUNT | STALK | Player escapes beyond 5 m (`escapeRange`) |
| RETREAT | DRIFT | Torch extinguished AND safe shadow waypoint reached |
| RALLIED | STALK | Immediately on Enter (one-frame passthrough) |

---

## Sensory Systems

### Sentinel — Vision Cone
Implemented in `VisionSensor.cs`. Three-step check per frame:
1. Distance ≤ `viewRadius` (9 m default)
2. Angle ≤ half `viewAngle` (30° either side of forward vector)
3. Physics raycast — checks `obstacleMask` for line-of-sight occlusion

When the player's torch is lit, the Sentinel's `viewRadius` is boosted by +4 m via `TorchController`, making the Sentinel more dangerous when the player tries to counter the Shade.

### Sentinel — Hearing
Implemented in `HearingSensor.cs` + `SoundEventBroadcaster.cs`. Uses a static pub/sub pattern — any game object emits sound via `SoundEventBroadcaster.Broadcast(position, radius)`. The `HearingSensor` registers itself and receives events each frame.

### Shade — Sound Radius
Same `HearingSensor` component (radius 8 m). The Shade responds to footsteps, chest openings, and any player action that calls `Broadcast()`.

### Shade — Light Sensor
Implemented in `LightSensor.cs`. Reads `TorchController.IsLit` (static bool) and checks distance ≤ 5 m. Triggers RETREAT instantly from any state.

### Player Noise Scaling
`PlayerController.cs` emits footstep sounds with a radius that scales inversely with player stamina (`StaminaRatio`). When the Shade drains stamina in HUNT, the player moves slower and makes louder sounds — which can alert the Sentinel and create a cascade of both threats.

---

## Player-AI Interaction Scenario

### The Torch Trade-off
The most critical player decision is whether to equip the torch:
- **Torch ON**: Shade immediately retreats (removes one threat). But Sentinel vision radius increases by +4 m — makes the Sentinel significantly more dangerous.
- **Torch OFF**: Shade stalks freely but Sentinel operates at normal range.

This creates a moment-to-moment risk/reward choice that neither option trivially resolves.

### Agent-Agent Interaction
The **RALLYING CRY** mechanic creates emergent flanking without a shared state machine:
1. Player damages Sentinel below 50% HP
2. Sentinel enters STAGGERED → RALLYING, stops and broadcasts the cry
3. If Shade is within 10 m, it enters RALLIED → immediately transitions to STALK
4. Player now faces a recovering Sentinel from one direction and a Shade closing from another

---

## Challenges and Solutions

### Challenge 1: Agent-agent communication without tight coupling
**Problem**: The Sentinel needed to "call" the Shade without both scripts holding direct component references in a way that breaks modularity.  
**Solution**: `CryptSentinel` holds a serialised `ShadeBrain` reference (assigned in Inspector). The actual call is a single public method `shade.ReceiveRally(position)`. The Shade has no reference back to the Sentinel. This keeps coupling one-directional and Inspector-driven.

### Challenge 2: Sound system reaching multiple AI agents
**Problem**: Individual `Physics.OverlapSphere` calls per agent each frame would become expensive as agent count grows.  
**Solution**: Implemented a static pub/sub broadcaster (`SoundEventBroadcaster`). Sound emitters call one static method; all `HearingSensor` components listen. This scales well and decouples emitters from receivers entirely.

### Challenge 3: Shade RALLIED as a discrete FSM state
**Problem**: The assignment FSM diagram required RALLIED to be a visible state, but its behaviour is simply "go to STALK immediately."  
**Solution**: `ShadeRalliedState` calls `ChangeState(new ShadeStalkState())` in its `Enter()` method — it is a real one-frame state that the FSM passes through, keeping the diagram faithful without redundant logic.

### Challenge 4: Torch affecting both AI systems differently
**Problem**: The torch needed to simultaneously help the player (repel Shade) and hurt the player (boost Sentinel vision) — this cross-system interaction had to be clean.  
**Solution**: `TorchController` owns both effects. `LightSensor` reads the static `TorchController.IsLit` flag (no direct reference needed). Sentinel vision boost/restore is handled in the toggle callback with a `boostedSentinel` flag to prevent stacking.

---

## Code Architecture Summary

```
Assets/Scripts/
├── AI/
│   ├── State.cs                    ← Abstract base (polymorphic FSM)
│   ├── AIBrain.cs                  ← Shared MonoBehaviour host
│   ├── CryptSentinel.cs            ← Sentinel brain + rally logic
│   ├── ShadeBrain.cs               ← Shade brain
│   ├── Sensors/
│   │   ├── VisionSensor.cs         ← 3-step view cone + raycast
│   │   ├── HearingSensor.cs        ← Radius + pub/sub registration
│   │   └── LightSensor.cs          ← Torch detection
│   └── States/
│       ├── Sentinel/               ← 6 Sentinel states
│       └── Shade/                  ← 6 Shade states
├── Player/
│   ├── PlayerController.cs         ← Movement + footstep emission
│   ├── PlayerHealth.cs             ← HP and stamina
│   └── TorchController.cs          ← Torch toggle + cross-AI effects
└── Systems/
    └── SoundEventBroadcaster.cs    ← Static pub/sub sound system
```

---

*GAME-10020 · Assignment 3 · Yatharth Chopra*
