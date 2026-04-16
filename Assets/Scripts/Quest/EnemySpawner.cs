using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject enemyPrefab; // 哥布林预制体
    public int spawnCount = 7; // 生成数量
    public string enemyTag = "Enemy"; // 敌人标签

    [Header("Spawn Area")]
    public Vector2 spawnAreaMin; // 生成区域最小坐标
    public Vector2 spawnAreaMax; // 生成区域最大坐标
    public float minSpawnDistance = 2f; // 最小生成间距（避免重叠）

    [Header("Spawn Validation")]
    public LayerMask obstacleLayer; // 障碍物图层（避免生成在墙里）
    public float checkRadius = 0.5f; // 检测半径

    private List<GameObject> spawnedEnemies = new List<GameObject>();

    // 生成敌人
    public void SpawnEnemies()
    {
        Debug.Log("=== 开始生成敌人 ===");
        Debug.Log($"Enemy Prefab: {(enemyPrefab != null ? enemyPrefab.name : "NULL")}");
        Debug.Log($"Spawn Area: Min({spawnAreaMin.x}, {spawnAreaMin.y}) Max({spawnAreaMax.x}, {spawnAreaMax.y})");

        ClearSpawnedEnemies(); // 清除之前生成的敌人

        for (int i = 0; i < spawnCount; i++)
        {
            Vector2 spawnPos = GetValidSpawnPosition();
            if (spawnPos != Vector2.zero)
            {
                // 使用 Vector3，确保 Z 轴为 0
                Vector3 spawnPos3D = new Vector3(spawnPos.x, spawnPos.y, 0f);
                GameObject enemy = Instantiate(enemyPrefab, spawnPos3D, Quaternion.identity);
                enemy.tag = enemyTag; // 设置标签
                spawnedEnemies.Add(enemy);
                Debug.Log($"✓ 生成敌人 {i + 1}/{spawnCount} 在位置: {spawnPos3D}");
            }
            else
            {
                Debug.LogWarning($"✗ 无法找到有效的生成位置，跳过第 {i + 1} 个敌人");
            }
        }

        Debug.Log($"=== 生成完成，共生成 {spawnedEnemies.Count}/{spawnCount} 个敌人 ===");
    }

    // 获取有效的生成位置
    private Vector2 GetValidSpawnPosition()
    {
        int maxAttempts = 30; // 最大尝试次数
        for (int i = 0; i < maxAttempts; i++)
        {
            // 随机生成位置
            Vector2 randomPos = new Vector2(
                Random.Range(spawnAreaMin.x, spawnAreaMax.x),
                Random.Range(spawnAreaMin.y, spawnAreaMax.y)
            );

            // 检查是否与障碍物重叠
            if (Physics2D.OverlapCircle(randomPos, checkRadius, obstacleLayer))
            {
                continue; // 位置无效，重新尝试
            }

            // 检查是否与已生成的敌人距离太近
            bool tooClose = false;
            foreach (var enemy in spawnedEnemies)
            {
                if (enemy != null && Vector2.Distance(randomPos, enemy.transform.position) < minSpawnDistance)
                {
                    tooClose = true;
                    break;
                }
            }

            if (!tooClose)
            {
                return randomPos; // 找到有效位置
            }
        }

        Debug.LogWarning("无法找到有效的生成位置！");
        return Vector2.zero;
    }

    // 清除已生成的敌人
    public void ClearSpawnedEnemies()
    {
        foreach (var enemy in spawnedEnemies)
        {
            if (enemy != null)
            {
                Destroy(enemy);
            }
        }
        spawnedEnemies.Clear();
    }

    // 在编辑器中可视化生成区域
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector2 center = (spawnAreaMin + spawnAreaMax) / 2;
        Vector2 size = spawnAreaMax - spawnAreaMin;
        Gizmos.DrawWireCube(center, size);
    }
}
