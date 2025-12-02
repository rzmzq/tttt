using UnityEngine;

public class TetrisController : MonoBehaviour
{
    [Header("テトリミノのプレハブ（7種類）")]
    public GameObject[] tetrominoPrefabs;

    private GameObject currentBlock;
    private float fallTimer = 0f;
    private float fallInterval = 0.5f;

    void Update()
    {
        // 新しいブロック生成
        if (Input.GetKeyDown(KeyCode.Space) && currentBlock == null)
        {
            SpawnBlock();
        }

        if (currentBlock != null)
        {
            // 左右移動
            if (Input.GetKeyDown(KeyCode.A))
                MoveHorizontal(-1);

            if (Input.GetKeyDown(KeyCode.D))
                MoveHorizontal(+1);

            // ▼ 回転（Qキー）
            if (Input.GetKeyDown(KeyCode.Q))
                RotateBlock();

            // ▼ 落下速度（Wを押している間は3倍）
            float interval = fallInterval;
            if (Input.GetKey(KeyCode.W))
                interval = fallInterval / 3f;

            // ▼ 自動落下
            fallTimer += Time.deltaTime;
            if (fallTimer >= interval)
            {
                fallTimer = 0f;
                MoveDown();
            }
        }
    }

    // ▼ ブロック生成
    void SpawnBlock()
    {
        if (tetrominoPrefabs.Length == 0)
        {
            Debug.LogError("テトリミノのプレハブが登録されていません！");
            return;
        }

        int index = Random.Range(0, tetrominoPrefabs.Length);

        Vector3 spawnPos = transform.position;
        spawnPos.x = Mathf.Round(spawnPos.x);
        spawnPos.y = Mathf.Round(spawnPos.y);

        currentBlock = Instantiate(tetrominoPrefabs[index], spawnPos, Quaternion.identity);

        SnapToGrid(currentBlock);
        SnapChildrenToGrid(currentBlock);

        currentBlock.GetComponent<BlockController>().parentController = this;

        Debug.Log("Spawned Block: " + tetrominoPrefabs[index].name);
    }

    // ▼ 真下だけ判定して落下可能かどうか調べる
    bool CanMoveDown(GameObject block)
    {
        foreach (Transform child in block.transform)
        {
            Vector2 pos = child.position;
            Vector2 boxSize = new Vector2(0.9f, 0.9f);

            RaycastHit2D hit = Physics2D.BoxCast(
                pos,
                boxSize,
                0f,
                Vector2.down,
                1f,
                LayerMask.GetMask("Block", "Wall")
            );

            if (hit.collider != null)
            {
                return false;
            }
        }
        return true;
    }

    // ▼ 1マス落下
    void MoveDown()
    {
        if (currentBlock == null) return;

        if (CanMoveDown(currentBlock))
        {
            Vector3 pos = currentBlock.transform.position;
            pos.y -= 1;
            currentBlock.transform.position = pos;
            SnapToGrid(currentBlock);
        }
        else
        {
            StopCurrentBlock();
        }
    }

    // ▼ 左右移動
    void MoveHorizontal(int dir)
    {
        if (currentBlock == null) return;

        Vector3 pos = currentBlock.transform.position;
        pos.x += dir;

        currentBlock.transform.position = pos;
        SnapToGrid(currentBlock);
    }

    // ▼ ブロック固定
    public void StopCurrentBlock()
    {
        Debug.Log("ブロック固定");
        currentBlock = null;
    }

    // ▼ 回転（安全版）
    void RotateBlock()
    {
        if (currentBlock == null) return;

        Quaternion oldRot = currentBlock.transform.rotation;

        currentBlock.transform.Rotate(0, 0, 90);

        if (IsColliding(currentBlock))
        {
            currentBlock.transform.rotation = oldRot;
        }
        else
        {
            SnapChildrenToGrid(currentBlock);
        }
    }

    // ▼ 回転時の衝突判定
    bool IsColliding(GameObject block)
    {
        foreach (Transform child in block.transform)
        {
            Collider2D hit = Physics2D.OverlapCircle(
                child.position,
                0.45f,
                LayerMask.GetMask("Block", "Wall")
            );

            if (hit != null)
                return true;
        }
        return false;
    }

    // ▼ 親オブジェクトをグリッドにスナップ
    void SnapToGrid(GameObject block)
    {
        Vector3 p = block.transform.position;
        p.x = Mathf.Round(p.x);
        p.y = Mathf.Round(p.y);
        block.transform.position = p;
    }

    // ▼ 子ブロックをスナップ
    void SnapChildrenToGrid(GameObject block)
    {
        foreach (Transform child in block.transform)
        {
            Vector3 p = child.localPosition;
            p.x = Mathf.Round(p.x);
            p.y = Mathf.Round(p.y);
            child.localPosition = p;
        }
    }
}
