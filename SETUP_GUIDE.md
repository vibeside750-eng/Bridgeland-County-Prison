# Bridgeland County Prison - Setup Guide

## Installation & Setup

### Prerequisites
- Unity 2021.3 LTS or newer
- TextMesh Pro (included with Unity)
- NavMesh system enabled

### Project Structure
```
Bridgeland-County-Prison/
├── Assets/
│   ├── Scripts/
│   │   ├── Core/
│   │   │   ├── GameManager.cs
│   │   │   ├── MapManager.cs
│   │   │   └── PlayerController.cs
│   │   ├── Character/
│   │   │   ├── PlayerData.cs
│   │   │   └── CharacterDisplay.cs
│   │   ├── Security/
│   │   │   ├── DoorController.cs
│   │   │   └── GateController.cs
│   │   ├── NPC/
│   │   │   ├── NPCController.cs
│   │   │   ├── OfficerController.cs
│   │   │   ├── InmateController.cs
│   │   │   ├── NPCData.cs
│   │   │   └── NPCManager.cs
│   │   ├── Systems/
│   │   │   ├── ScheduleManager.cs
│   │   │   └── NPCManager.cs
│   │   ├── UI/
│   │   │   └── UIManager.cs
│   │   ├── Camera/
│   │   │   └── CameraController.cs
│   │   ├── Map/
│   │   │   └── PrisonMapConfig.cs
│   │   └── Audio/
│   │       └── AudioManager.cs
│   ├── Prefabs/
│   ├── Scenes/
│   │   └── PrisonSimulation.unity
│   ├── UI/
│   ├── Audio/
│   └── Materials/
├── README.md
├── DESIGN_DOCUMENT.md
└── SETUP_GUIDE.md
```

### Scene Setup

1. **Create Main Scene** (`PrisonSimulation.unity`)
   - Folder: Assets/Scenes/

2. **Setup Hierarchy**
   ```
   PrisonSimulation
   ├── GameManagers
   │   ├── GameManager
   │   ├── MapManager
   │   ├── ScheduleManager
   │   ├── NPCManager
   │   ├── AudioManager
   │   └── UIManager
   ├── MainCamera
   │   └── CameraController
   ├── Environment
   │   ├── Buildings
   │   ├── Doors
   │   └── Gates
   ├── NPCs
   │   ├── Officers
   │   └── Inmates
   ├── Player
   └── Canvas (UI)
   ```

### GameObject Configuration

#### GameManager
- Add script: `GameManager.cs`
- Assign prefab: Player prefab
- Inspector fields:
  - Player Prefab: (assign player character prefab)
  - Map Manager: (drag MapManager from scene)
  - UI Manager: (drag UIManager from scene)
  - Audio Manager: (drag AudioManager from scene)

#### MapManager
- Add script: `MapManager.cs`
- Add component: `PrisonMapConfig.cs`
- Setup in hierarchy:
  - Create empty GameObject for each location spawn point
  - Add to LocationSpawns list:
    - Name: "StaffParking", Position: (30, 0, -20)
    - Name: "HousingUnitA", Position: (-50, 0, 70)
    - (Continue for all locations in PrisonMapConfig)

#### CameraController
- GameObject: Main Camera
- Add script: `CameraController.cs`
- Inspector fields:
  - Player Target: (assign player)
  - Follow Speed: 5
  - Camera Offset: (0, 15, 0)
  - Is Orthographic: true
  - Orthographic Size: 10
  - Map Boundaries: (adjust to fit prison size)

#### UIManager
- Create Canvas GameObject
- Add script: `UIManager.cs`
- Canvas Render Mode: Screen Space - Overlay
- UI Structure:
  - Panel: MainHUD
    - Text: PlayerName (Top Left)
    - Text: PlayerID (Top Left)
    - Text: PlayerRole (Top Left)
    - Text: LocationDisplay (Top Right)
    - Text: TimeDisplay (Top Right)
    - Text: InteractionPrompt (Center Bottom)
    - Image: JoystickBackground (Bottom Left)
    - Button: InteractButton (Bottom Right)
    - Button: InventoryButton (Bottom Right)
    - Button: MapButton (Bottom Right)
    - Button: CharacterButton (Bottom Right)
    - Button: ScheduleButton (Bottom Right)
  - Panel: CharacterCreationPanel
    - Text: Title "Create Your Character"
    - InputField: FirstNameInput
    - InputField: LastNameInput
    - Button: OfficerRoleButton
    - Button: InmateRoleButton
    - Dropdown: GenderDropdown
    - Dropdown: FaceStyleDropdown
    - Dropdown: PersonalityDropdown
    - Dropdown: EyeColorDropdown
    - Button: CreateCharacterButton

#### NPCManager
- Add script: `NPCManager.cs`
- Create prefabs:
  - OfficerController prefab
    - Add script: `OfficerController.cs`
    - Add child: CharacterDisplay GameObject
    - Add component: NavMeshAgent
  - InmateController prefab
    - Add script: `InmateController.cs`
    - Add child: CharacterDisplay GameObject
    - Add component: NavMeshAgent
- Assign to NPCManager fields:
  - Officer Prefab: (assign officer prefab)
  - Inmate Prefab: (assign inmate prefab)
  - Initial Officer Count: 5
  - Initial Inmate Count: 15

#### DoorController Setup
- Create 3D Cube for each door
- Add script: `DoorController.cs`
- Add component: BoxCollider
- Inspector fields:
  - Required Access Level: 1 (adjust per door)
  - Door Open Duration: 2
  - Door Close Duration: 1.5
  - Open Position: (adjust for sliding direction)
  - Closed Position: (initial position)
  - Audio Source: (create AudioSource component)
  - Access Beep Sound: (assign audio clip)
  - Denied Beep Sound: (assign audio clip)
  - Door Open Close Sound: (assign audio clip)

#### GateController Setup
- Create 3D Cube (larger scale) for each gate
- Add script: `GateController.cs`
- Add component: BoxCollider
- Inspector fields:
  - Gate Name: "Gate A" / "Gate B" / "Gate C"
  - Required Access Level: 3
  - Gate Open Duration: 3
  - Gate Close Duration: 3
  - Open Position: (adjust for sliding)
  - Closed Position: (initial position)
  - Audio Source: (create AudioSource component)
  - Audio clips for access/denied/gate sounds

#### AudioManager
- Create empty GameObject: AudioManager
- Add script: `AudioManager.cs`
- Create two AudioSource children:
  - ExteriorAmbience: Loop = true, Volume = 0.2
  - InteriorAmbience: Loop = true, Volume = 0.3
- Inspector fields:
  - Exterior Ambience: (assign audio source)
  - Interior Ambience: (assign audio source)
  - Outside Ambience Clip: (assign audio)
  - HVAC Clip: (assign audio)
  - Light Hum Clip: (assign audio)

#### ScheduleManager
- Create empty GameObject: ScheduleManager
- Add script: `ScheduleManager.cs`
- No additional setup required (schedule defined in code)

#### PlayerController
- Create Player GameObject
- Add script: `PlayerController.cs`
- Add component: NavMeshAgent
- Add child: Canvas for CharacterDisplay
- Add script to child: `CharacterDisplay.cs`
- Inspector fields:
  - Move Speed: 5
  - Character Display: (assign CharacterDisplay child)
  - NavMesh Agent: (auto-assigned)
  - Interaction Range: 2.5

### NavMesh Setup

1. In scene, select all ground/walkable surfaces
2. Mark as Walkable in Navigation window
3. Bake NavMesh
4. Configure Agent size:
   - Radius: 0.5
   - Height: 2
   - Step Height: 0.3
   - Max Slope: 45

### Input Setup

In Project Settings > Input Manager:
- Horizontal: A/D keys, Left Stick X
- Vertical: W/S keys, Left Stick Y
- Fire1 (Interact): E key, Gamepad Button South (A on Xbox)

### Audio Assets Needed

1. **Ambient Sounds**
   - `OutsideAmbience.wav`: Wind, birds, distant traffic (loop)
   - `HVAC.wav`: Indoor air circulation (loop)
   - `LightHum.wav`: Electrical hum (loop)

2. **Access Sounds**
   - `AccessBeep.wav`: Successful access tone
   - `DeniedBeep.wav`: Access denied tone
   - `DoorOpenClose.wav`: Door sliding sound
   - `GateOpenClose.wav`: Gate sliding sound (lower pitch)

### Testing Checklist

- [ ] Game starts and shows character creation
- [ ] Character creation allows name/role/appearance selection
- [ ] Player spawns at correct location (Officer in Staff Parking, Inmate in Housing Unit A)
- [ ] Player can move with WASD/joystick
- [ ] Camera follows player smoothly
- [ ] HUD displays correct player info
- [ ] Time progresses (30 real minutes = 1 day)
- [ ] NPCs spawn (5 officers, 15 inmates)
- [ ] NPCs move with purposeful behavior
- [ ] Doors respond to interaction (E key)
- [ ] Gates respond to interaction and require Level 3 access
- [ ] Audio plays (ambient and feedback sounds)
- [ ] Access prompts show/hide appropriately
- [ ] Schedule changes at defined times
- [ ] No collisions with buildings/gates
- [ ] Pathfinding works smoothly

## Building for Deployment

1. File > Build Settings
2. Add PrisonSimulation scene
3. Select target platform (Windows/Mac/Linux)
4. Player Settings:
   - Company: "Bridgeland County"
   - Product Name: "Prison Simulation"
   - Version: "0.1.0"
5. Build

## Troubleshooting

### NPCs not moving
- Verify NavMesh is baked correctly
- Check ScheduleManager is in scene
- Ensure location spawns are defined in MapManager

### Doors/Gates not responding
- Verify DoorController/GateController scripts attached
- Check AudioSource components exist
- Ensure access level checks are correct

### Camera not following
- Verify CameraController assigned player target
- Check camera orthographic mode is enabled
- Ensure camera offset is correct (0, 15, 0)

### Character creation not showing
- Verify UIManager is in scene
- Check Canvas is set to Screen Space - Overlay
- Ensure CharacterCreationPanel is active

---

**Setup Guide Version:** 1.0
**Last Updated:** 2026-06-11
