# Hollow Vault — GAME-10020 Assignment 3

**Top-down action-adventure roguelike** | Unity AI FSM Implementation

## AI Agents

- **Crypt Sentinel** — undead armoured guardian with 6-state FSM: Patrol → Investigate → Combat → Staggered → Rallying → Return
- **The Shade** — spectral stalker with 6-state FSM: Drift → Alert → Stalk → Hunt → Retreat → Rallied

## Key Features

- Polymorphic state machine (abstract `State` base class)
- Vision cone with 3-step check (distance, angle, raycast)
- Hearing system via static pub/sub broadcaster
- Light sensor (torch mechanic)
- Agent-agent interaction: Sentinel's Rallying Cry alerts The Shade
- Player stamina influences noise output (Shade indirectly triggers Sentinel)

## Setup in Unity

1. Open the project in Unity 2022 LTS or later
2. Add NavMesh to the scene floor (Window → AI → Navigation → Bake)
3. Attach `CryptSentinel` to the Sentinel GameObject and assign Inspector fields
4. Attach `ShadeBrain` to the Shade GameObject and assign Inspector fields
5. Attach `PlayerController`, `PlayerHealth`, `TorchController` to the Player
6. Add waypoint Transforms and assign them to each agent's `patrolWaypoints` array
7. Set layer masks on `VisionSensor` (obstacleMask = walls, playerMask = Player layer)

## Design Document

See [DESIGN_DOCUMENT.md](DESIGN_DOCUMENT.md) for full FSM diagrams, state descriptions, transition tables, and implementation notes.

## Gameplay Video

> _[YouTube link — to be added]_

---

*GAME-10020 · Yatharth Chopra*
