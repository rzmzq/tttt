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
        if (Input.GetKeyDown(KeyCode.Space) && currentBlock == null)
        {
            SpawnBlock();
        }

        if (currentBlock != null)
        {
            if (Input.GetKeyDown(KeyCode.A))
                MoveHorizontal(-1);

            if (Input.GetKeyDown(KeyCode.D))
                MoveHorizontal(+1);

            if (Input.GetKeyDown(KeyCode.Q))
                RotateBlock();

            float interval = fallInterval;
            if (Input.GetKey(KeyCode.W))
                interval = fallInterval / 3f;

            fallTimer += Time.deltaTime;
            if (fallTimer >= interval)
            {
                fallTimer = 0f;
                MoveDown();
            }
        }
    }

    void SpawnBlock()
    {
        int index = Random.Range(0, tetrominoPrefabs.Length);

        Vector3 spawnPos = transform.position;
        spawnPos.x = Mathf.Round(spawnPos.x);
        spawnPos.y = Mathf.Round(spawnPos.y);

        currentBlock = Instantiate(tetrominoPrefabs[index], spawnPos, Quaternion.identity);

        SnapToGrid(currentBlock);
        SnapChildrenToGrid(currentBlock);
    }

    // ▼ 落下判定：床 + 固定ブロック
    bool CanMoveDown(GameObject block)
    {
        foreach (Transform child in block.transform)
        {
            Vector2 pos = child.position;
            Vector2 boxSize = new Vector2(0.9f, 0.9f);

            float castDistance = 0.55f;

            RaycastHit2D hit = Physics2D.BoxCast(
                pos,
                boxSize,
                0f,
                Vector2.down,
                castDistance,
                LayerMask.GetMask("PlacedBlock", "Floor")
            );

            if (hit.collider != null)
                return false;
        }
        return true;
    }

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

    // ▼ 左右移動：SideWall / Floor / PlacedBlock 衝突チェック
    void MoveHorizontal(int dir)
    {
        if (currentBlock == null) return;

        Vector3 pos = currentBlock.transform.position;
        pos.x += dir;
        currentBlock.transform.position = pos;

        if (IsColliding(currentBlock))
        {
            pos.x -= dir;
            currentBlock.transform.position = pos;
        }

        SnapToGrid(currentBlock);
    }

    // ▼ ブロック固定処理
    void StopCurrentBlock()
    {
        Debug.Log("ブロック固定");

        foreach (Transform child in currentBlock.transform)
        {
            child.gameObject.layer = LayerMask.NameToLayer("PlacedBlock");
            Board.AddToGrid(child);
        }

        currentBlock.transform.DetachChildren();
        Destroy(currentBlock);

        Board.CheckLines();

        currentBlock = null;
    }

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

    // ▼ 回転・横移動の衝突判定：SideWall + Floor + PlacedBlock
    bool IsColliding(GameObject block)
    {
        foreach (Transform child in block.transform)
        {
            Collider2D hit = Physics2D.OverlapCircle(
                child.position,
                0.45f,
                LayerMask.GetMask("PlacedBlock", "SideWall", "Floor")
            );

            if (hit != null)
                return true;
        }
        return false;
    }

    void SnapToGrid(GameObject block)
    {
        Vector3 p = block.transform.position;
        p.x = Mathf.Round(p.x);
        p.y = Mathf.Round(p.y);
        block.transform.position = p;
    }

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
