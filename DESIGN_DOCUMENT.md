# DESIGN DOCUMENT: Bridgeland County Prison Simulation

## PROJECT OVERVIEW

Bridgeland County Prison is a top-down prison simulation prototype that emphasizes realistic prison operations, NPC behavior, and modern minimalist UI design. The simulation focuses on creating a believable, functioning correctional facility with dynamic daily routines and interactive security systems.

## CORE SYSTEMS IMPLEMENTED

### 1. Game Management
- **GameManager.cs**: Central game state, time progression, initialization
  - 30 real minutes = 1 prison day cycle
  - Time scale from 6:00 AM - 10:00 PM
  - Player spawning and character initialization

### 2. Map & Navigation
- **MapManager.cs**: Location management, pathfinding, spatial layout
- **PrisonMapConfig.cs**: Complete prison layout configuration
  - Progressive security zones (exterior → interior)
  - 17+ building locations
  - Access level requirements per area
  
**Map Layout (Top-Down Security Flow):**
```
Public Road
  ↓ Visitor Entrance
  ↓ Visitor Parking | Staff Parking
  ↓ Staff Security Checkpoint
  ↓ Gate A (Level 3)
  ↓ Vehicle Inspection Area
  ↓ Gate B (Level 3)
  ↓ Security Processing Corridor
  ↓ Gate C (Level 3)
  ↓ Administration | Control Center | Server Room
  ↓ Medical Building
  ↓ Kitchen
  ↓ Programs Building
  ↓ Housing Unit A | Housing Unit B
  ↓ Recreation Yard
```

### 3. Character System

#### Character Creation
- **PlayerData.cs**: Complete player profile
  - Name (First/Last)
  - Role selection (Officer/Inmate)
  - Gender (Male/Female)
  - Face style (Professional, Friendly, Cute, Stoic, Aggressive, Nervous)
  - Personality (Professional, Friendly, Quiet, Confident, Nervous, Aggressive)
  - Eye color (Brown, Blue, Green, Hazel, Gray)
  - Auto-generated ID (OFF-XXXXXX / INM-XXXXXX)
  - Access level assignment

#### Character Display
- **CharacterDisplay.cs**: Visual representation
  - Minimalist circle-based character design
  - Dynamic eye tracking and blinking
  - Expression animations
  - Name/ID/Role display above character
  
**Face Expressions:**
```
Professional: :)  :|  ¬_¬
Friendly:    :D  :)  ^-^
Cute:        :3  ^w^  >w<
Stoic:       :|  ._.  —_—
Aggressive:  >:[ >:(
Nervous:     :s  :o
```

### 4. Player Controller
- **PlayerController.cs**: Player movement and interaction
  - WASD/Joystick movement
  - NavMesh-based pathfinding
  - Interaction range detection
  - Door/Gate interaction (E key)
  - Real-time location tracking

### 5. Security Systems

#### Door Controller
- **DoorController.cs**
  - Keycard access verification
  - Smooth sliding animation
  - Automatic locking/unlocking
  - Access feedback sounds (beep/denied)
  - Configurable access levels per door

#### Gate Controller
- **GateController.cs**
  - Vehicle-scale sliding mechanics
  - Three gates (A, B, C) with Level 3+ access
  - Auto-close after 5 seconds
  - Smooth eased animation
  - Audio feedback (access/denied/gate sounds)

### 6. NPC System

#### Base NPC Controller
- **NPCController.cs**: Foundation for all NPC behavior
  - Schedule management integration
  - Task-based behavior system
  - Patrol route management
  - Face and personality display

#### Officer Controller
- **OfficerController.cs**: Security personnel
  - Defined patrol routes through facility
  - Nearby character detection (15m range)
  - Shift-based activities
  - Security duties

#### Inmate Controller
- **InmateController.cs**: Prisoner behavior
  - Strict schedule adherence
  - Activity location navigation
  - Idle behaviors and interactions
  - Conversation system foundation

#### NPC Data
- **NPCData.cs**: NPC profile system
  - Randomly generated names (historically accurate)
  - Role assignment
  - Face/personality generation
  - Access level assignment

#### NPC Manager
- **NPCManager.cs**: NPC spawning and lifecycle
  - Initial spawn: 5 officers, 15 inmates
  - Procedural data generation
  - Spawn location management
  - NPC list tracking

### 7. Schedule System
- **ScheduleManager.cs**: Prison schedule management
  - Complete daily timeline (6 AM - 10 PM)
  - Role-specific schedules (officers/inmates)
  - Phase transitions with descriptions
  - Time-until-next-phase calculations

**Inmate Daily Schedule:**
```
0600 - Wake Up
0700 - Breakfast
0800 - Work Assignment (until 12:00)
1200 - Lunch
1300 - Programs (until 17:00)
1700 - Dinner
1800 - Recreation (until 20:00)
2000 - Return To Housing
2100 - Count Time
2200 - Lights Out
```

**Officer Shift Schedule:**
```
0600 - Patrol (until 14:00)
1400 - Lunch Break
1430 - Patrol (until 22:00)
```

### 8. User Interface
- **UIManager.cs**: Complete UI management
  - Character creation menu
  - Real-time HUD display
  - Menu system (Inventory, Map, Character, Schedule)
  - Interaction prompts
  - Location/Time display

**HUD Layout:**
```
┌─────────────────────────────────────────┐
│ Player Name | ID | Role    Location │ Time │
│                                         │
│                                         │
│                                         │
│                                         │
│                    [Game View]          │
│                                         │
│                                         │
│  [Movement]                  [E] Interact│
│                                         │
│ [Inv] [Map] [Char] [Sched]             │
└─────────────────────────────────────────┘
```

### 9. Camera System
- **CameraController.cs**: Top-down camera control
  - Strict orthographic perspective
  - Smooth follow with configurable speed
  - Map boundary clamping
  - No rotation/zoom/first-person
  - Consistent 90° downward angle

### 10. Audio System
- **AudioManager.cs**: Environmental audio management
  - Exterior ambience (wind, birds, distant traffic)
  - Interior HVAC sounds
  - Ambient background audio
  - Volume controls
  - Sound effect integration

## ACCESS LEVEL SYSTEM

```
Level 1 - Visitor (Inmates, general visitors)
Level 2 - Staff (Administrative, support staff)
Level 3 - Correctional Officer (Primary security)
Level 4 - Supervisor (Administrative oversight)
Level 5 - Executive Staff (Management)
Level 6 - Director (Facility command)
```

**Gate Access Requirements:**
- Gate A: Level 3+ (Officer)
- Gate B: Level 3+ (Officer)
- Gate C: Level 3+ (Officer)

## VISUAL DESIGN

### Color Palette
- **Concrete Gray**: #A9A9A9 (walls, buildings)
- **Asphalt Black**: #1C1C1C (roads, pavement)
- **Chain-Link Silver**: #C0C0C0 (fences, security)
- **Grass Green**: #4CAF50 (outdoor areas)
- **White Markings**: #FFFFFF (lane markers, safety lines)
- **Security Yellow**: #FFD700 (hazard zones, warnings)

### UI Design
- Modern minimalist approach
- Apple-inspired aesthetic
- Clean sans-serif typography
- Subtle shadows and depth
- High contrast for accessibility
- Smooth animations and transitions

## GAMEPLAY FLOW

### Session Start
1. Game initializes (MapManager, ScheduleManager, AudioManager)
2. Player sees character creation menu
3. Player selects role (Officer/Inmate)
4. Player customizes appearance (name, face, personality)
5. Player spawns at appropriate location:
   - Officers: Staff Parking
   - Inmates: Housing Unit A
6. NPCs spawn automatically (5 officers, 15 inmates)
7. Ambient audio starts
8. Game HUD appears

### During Gameplay
1. Player navigates using WASD/joystick
2. Camera smoothly follows player
3. Real-time HUD shows location, time, player info
4. Player can interact with doors/gates (E key)
5. NPCs follow daily schedule automatically
6. Officers patrol assigned routes
7. Inmates move between activity locations
8. Schedule changes trigger NPC behavior shifts
9. Time progresses (30 real minutes = 1 full day)

## DESIGN REQUIREMENTS SATISFIED

✓ **Map Structure**
- Plain text layout documentation
- Logical progression from exterior to interior
- Progressive security increases
- No overlapping buildings
- No inaccessible areas

✓ **Gates & Doors**
- Smooth sliding animations
- Automatic opening/closing
- Access level verification
- Audio feedback (beep/denied)
- Proper spacing and collision avoidance

✓ **Character System**
- Complete face customization
- Eye tracking and blinking
- Expression animations
- Personality system
- Dynamic display above character

✓ **NPC Behavior**
- Daily schedule adherence
- Intelligent pathfinding
- Role-specific duties
- Continuous purposeful movement
- Face/personality integration

✓ **Environmental Design**
- Realistic prison layout
- Proper security layering
- Believable building shapes
- Logical room placement
- Realistic scale

✓ **UI/UX**
- Modern minimalist design
- Apple-inspired aesthetics
- Clean information hierarchy
- Real-time HUD
- Non-intrusive menus

✓ **Audio Design**
- Ambient exterior sounds
- Interior HVAC ambience
- No silent areas
- Feedback sounds
- Immersive environment

## NOT IMPLEMENTED (For Future Phases)

- Lockdowns
- Escape attempts
- Riots and mass disturbances
- Combat systems
- Weapons
- Vehicles
- Emergency response systems
- Advanced incident management

## TECHNICAL ARCHITECTURE

### Managers (Singletons)
- GameManager: Central game state
- MapManager: Navigation and spatial data
- ScheduleManager: Time and activity management
- UIManager: All UI elements
- AudioManager: Sound management
- NPCManager: NPC lifecycle
- CameraController: Camera behavior

### Controllers
- PlayerController: Player movement/interaction
- DoorController: Individual door behavior
- GateController: Individual gate behavior
- NPCController: Base NPC behavior
  - OfficerController: Officer-specific AI
  - InmateController: Inmate-specific AI

### Data Classes
- PlayerData: Player profile
- NPCData: NPC profile
- ScheduleManager.SchedulePhase: Schedule definitions

## PERFORMANCE CONSIDERATIONS

- NavMesh-based pathfinding for efficiency
- Object pooling recommended for doors/gates
- NPC update optimization via task-based system
- Camera clamping prevents off-map rendering
- Audio loop management for ambient sounds

## FUTURE ENHANCEMENTS

1. **Dialogue System**
   - NPC conversation trees
   - Player dialogue options
   - Relationship tracking

2. **Advanced NPC AI**
   - Emotional states
   - Conflict resolution
   - Social interactions

3. **Expanded Locations**
   - Additional housing units
   - Specialized facilities
   - Maintenance areas

4. **Player Objectives**
   - Daily tasks
   - Role-specific duties
   - Progression system

5. **Visual Polish**
   - Weather effects
   - Dynamic lighting
   - Particle effects

6. **Security Features** (Phase 2+)
   - Lockdown protocols
   - Incident response
   - Emergency systems

---

**Project Status:** Prototype - Core Systems Complete
**Last Updated:** 2026-06-11
**Version:** 0.1.0
