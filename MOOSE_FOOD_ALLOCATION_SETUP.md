# Moose Food Allocation System - Setup Guide

## Overview
A centralized **FoodAllocationManager** system has been implemented to ensure moose animals don't compete for the same food objects. Each moose can now claim a unique food target, preventing multiple animals from pathfinding to the same destination.

## What's New

### 1. **FoodAllocationManager.cs** (New Component)
- **Singleton pattern**: Automatically manages food allocations across all moose in the scene
- **Centralized tracking**: Maintains a dictionary of which food is claimed by which moose
- **Automatic creation**: If not found, it creates itself at runtime
- **Fairness**: Returns the nearest unclaimed food to ensure efficient distribution

### 2. **HuntAnimalController.cs** (Updated)
Modified to use the manager for moose:
- Moose now **claim food** before traveling to it
- When eating finishes, moose **release food** so others can use it
- During evasion, moose **automatically releases claimed food**
- On death, all moose food claims are **automatically cleaned up**
- Non-moose animals (deer) continue using the original behavior

## Scene Setup Requirements

### Option 1: Moose-Specific Food (Recommended)
1. Create food objects in your scene for moose
2. Tag them with: `"mooseFood"`
3. The system will automatically use these for moose
4. Deer will continue using `"deerFood"` tagged objects

### Option 2: Shared Food (Fallback)
If you only have one type of food:
1. Tag food objects with: `"deerFood"`
2. The system will automatically fallback to these for moose if `"mooseFood"` tags don't exist
3. All animals (moose and deer) will share the same food pool

## How It Works

### Moose Food Selection Flow:
```
1. Moose needs food
2. Calls SearchForFood()
3. Uses FoodAllocationManager.GetAvailableFood()
4. Gets nearest UNCLAIMED food
5. Attempts to claim it with ClaimFood()
6. If successful → navigate to food
7. If failed (already claimed) → try again next frame
8. When eating done → ReleaseFood()
9. Repeat
```

### Food Lifecycle:
```
[Unclaimed] → Moose claims it → [Claimed by Moose A]
                                    ↓ (eating finishes)
                                [Unclaimed] → Moose B can now claim it
```

### Cleanup on Events:
- **Eating Finished**: Food automatically released
- **Entering Evasion**: Food automatically released
- **Safety Cooldown**: Food automatically released
- **Moose Dies**: ALL claimed food released

## API Reference

### FoodAllocationManager Methods:

```csharp
// Get nearest unclaimed food
GameObject GetAvailableFood(Transform requestingMoose)

// Claim food for a moose (returns true if successful)
bool ClaimFood(GameObject foodObject, Transform claimingMoose)

// Release food claimed by a moose
void ReleaseFood(GameObject foodObject, Transform releasingMoose)

// Release ALL food claimed by a moose (use on death)
void ReleaseAllFoodForMoose(Transform mooseTransform)

// Check if food is claimed
bool IsFoodClaimed(GameObject foodObject)

// Get count of available (unclaimed) food
int GetAvailableFoodCount()

// Debug: Print all food allocation status
void DebugPrintFoodStatus()
```

## Testing & Debugging

### To Monitor Food Allocation:
In the Editor, add this debug call to any script:
```csharp
FoodAllocationManager.Instance?.DebugPrintFoodStatus();
```

This prints:
- Total food count
- Claimed vs available food
- Which moose claims which food

### Expected Console Output:
```
[FoodAllocationManager] Found 5 moose food objects.
[FoodAllocationManager] Food database initialized with 5 total food objects.
[FoodAllocationManager] Validated 5/5 spawn locations.
```

## Performance Characteristics

| Operation | Complexity | Notes |
|-----------|----------|-------|
| Get Available Food | O(n) | Finds unclaimed food once per search |
| Claim Food | O(1) | Dictionary lookup + assignment |
| Release Food | O(1) | Dictionary lookup + null assignment |
| Food Cleanup | O(n) | Only runs when moose dies |

**For 25 moose + 20 food objects**: Negligible performance impact

## Advantages Over Previous System

### Before:
- ❌ Multiple moose could target the same food
- ❌ No centralized tracking
- ❌ Memory leak risk from stale queue references
- ❌ Inefficient random selection with OrderBy()

### After:
- ✅ Each moose claims unique food
- ✅ Centralized, scalable system
- ✅ Automatic memory cleanup on death
- ✅ Efficient nearest-neighbor food selection
- ✅ Realistic grazing behavior

## Realistic Behavior Improvements

1. **No Crowding**: Multiple moose won't navigate to same food source
2. **Smart Selection**: Moose pick nearest available food
3. **Graceful Fallback**: System handles missing food objects
4. **Dynamic Distribution**: As moose eat and release food, others can claim it
5. **Clean Abandonment**: When threatened, moose releases food for others

## Troubleshooting

### Issue: "FoodAllocationManager not found in scene!"
**Solution**: Add an empty GameObject to your scene, name it "FoodAllocationManager", and add the `FoodAllocationManager` component.

### Issue: Moose not finding food
**Solution**: 
1. Verify food objects exist in scene
2. Check they're tagged with `"mooseFood"` or `"deerFood"`
3. Run `DebugPrintFoodStatus()` to check allocation status
4. Ensure moose spawn points are reachable via NavMesh

### Issue: Moose keeps abandoning food during evasion
**This is intentional behavior** - moose release food when threatened to escape faster.

## Future Enhancements

Consider these improvements:
- Add priority system (young moose get closer food)
- Track eating history to encourage roaming
- Support different food types (grass vs. shrubs)
- Add visual debug gizmos in Editor
