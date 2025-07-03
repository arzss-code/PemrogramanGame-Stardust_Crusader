using UnityEngine;

/// <summary>
/// Configuration helper for setting up EnemyShip2 waves.
/// This script provides examples and presets for the three wave types:
/// 1. Fast Attack - Fast-moving enemies with aggressive pursuit
/// 2. Formation - Coordinated formation movement
/// 3. High Intensity - High-speed enemies with burst fire and zigzag movement
/// </summary>
[CreateAssetMenu(fileName = "EnemyShip2WaveConfig", menuName = "Game/Enemy Ship 2 Wave Configuration")]
public class EnemyShip2WaveConfig : ScriptableObject
{
    [Header("Configuration Presets")]
    [TextArea(3, 5)]
    public string description = "Presets for EnemyShip2 wave configurations. Use these as templates for setting up your EnemyShip2Spawner in the scene.";

    [Header("Fast Attack Wave Preset")]
    [SerializeField] private EnemyShip2Spawner.WaveConfiguration fastAttackPreset = new EnemyShip2Spawner.WaveConfiguration
    {
        waveType = EnemyShip2.WaveType.FastAttack,
        waveName = "Fast Attack Wave",
        enemyCount = 8,
        spawnInterval = 1.2f,
        initialDelay = 1f
    };

    [Header("Formation Wave Preset")]
    [SerializeField] private EnemyShip2Spawner.WaveConfiguration formationPreset = new EnemyShip2Spawner.WaveConfiguration
    {
        waveType = EnemyShip2.WaveType.Formation,
        waveName = "Formation Wave",
        enemyCount = 9,
        spawnInterval = 3f,
        initialDelay = 2f,
        formationSize = 3,
        formationSpawnDelay = 0.4f
    };

    [Header("High Intensity Wave Preset")]
    [SerializeField] private EnemyShip2Spawner.WaveConfiguration highIntensityPreset = new EnemyShip2Spawner.WaveConfiguration
    {
        waveType = EnemyShip2.WaveType.HighIntensity,
        waveName = "High Intensity Mixed Wave",
        enemyCount = 12,
        spawnInterval = 2f,
        initialDelay = 0.5f,
        burstSpawnInterval = 0.2f,
        burstSize = 3
    };

    public EnemyShip2Spawner.WaveConfiguration GetFastAttackPreset()
    {
        return fastAttackPreset;
    }

    public EnemyShip2Spawner.WaveConfiguration GetFormationPreset()
    {
        return formationPreset;
    }

    public EnemyShip2Spawner.WaveConfiguration GetHighIntensityPreset()
    {
        return highIntensityPreset;
    }

    [ContextMenu("Log Configuration Details")]
    public void LogConfigurationDetails()
    {
        Debug.Log("=== EnemyShip2 Wave Configuration Details ===");
        
        Debug.Log($"🏃 Fast Attack: {fastAttackPreset.enemyCount} enemies, {fastAttackPreset.spawnInterval}s interval");
        Debug.Log("   - High speed pursuit enemies");
        Debug.Log("   - Less frequent but targeted shooting");
        Debug.Log("   - Good for testing player reflexes");
        
        Debug.Log($"🚀 Formation: {formationPreset.enemyCount} enemies in groups of {formationPreset.formationSize}");
        Debug.Log("   - Coordinated group movement");
        Debug.Log("   - Predictable but challenging patterns");
        Debug.Log("   - Strategic positioning gameplay");
        
        Debug.Log($"⚡ High Intensity: {highIntensityPreset.enemyCount} enemies, bursts of {highIntensityPreset.burstSize}");
        Debug.Log("   - Zigzag movement patterns");
        Debug.Log("   - Rapid burst fire attacks");
        Debug.Log("   - Combines speed and firepower");
        Debug.Log("   - Highest difficulty level");
    }

    [Header("Setup Instructions")]
    [TextArea(8, 12)]
    public string setupInstructions = @"SETUP INSTRUCTIONS:

1. PREPARE PREFABS:
   - Create EnemyShip2 prefab with required components
   - Set up projectile prefab (can use existing Enemy3Bullets)
   - Configure audio clips for shoot/hit/destroy sounds

2. CONFIGURE ENEMYSHIP2 PREFAB:
   - Add Rigidbody2D, Collider2D, SpriteRenderer
   - Set health, speed, and score values per wave type
   - Assign projectile prefab and fire point
   - Configure audio clips

3. SETUP SPAWNER:
   - Add EnemyShip2Spawner to scene
   - Create wave configurations for each type
   - Set spawn points (multiple points for variety)
   - Assign enemy prefab to each wave config

4. INTEGRATE WITH LEVEL:
   - Assign EnemyShip2Spawner to LevelController
   - Configure wave order (fast → formation → high intensity)
   - Set timing between waves

5. TESTING:
   - Use context menu methods on spawner for individual wave testing
   - Check debug logs for wave progression
   - Balance enemy health, speed, and spawn rates";
}
