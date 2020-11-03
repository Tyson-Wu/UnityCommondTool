using System.Collections;
using System.Collections.Generic;
using UnityEditor.Profiling;
using UnityEngine;
/// <summary>
/// 自动行列布局
/// 用法：
/// 1、将脚本脱到一个Panel物体上
/// 2、在Panel物体下建一个名叫Prefab的空物体
/// 3、将需要布局的Item拉到Prefab下，以及本脚本的prefab中
/// 4、将Editor置为Ture，在编辑器中可以修改布局参数，修改完后将Editor置为False
/// 5、运行时可以直接调用 ResetLayout 来动态修改布局
/// </summary>
public class GridLayout : MonoBehaviour
{
    [SerializeField] bool Editor = false;
    [SerializeField] RectTransform prefab;
    [SerializeField] int itemCount = 0;
    [SerializeField] int rowCount=0;
    [SerializeField] int columnCount=0;
    [SerializeField] float spaceXRate=0.1f;
    [SerializeField] float spaceYRate=0.1f;
    [Header("以下只是展示")]
    [SerializeField] List<RectTransform> items = new List<RectTransform>();
    [SerializeField] List<RectTransform> itemsHided = new List<RectTransform>();

    RectTransform GetItem()
    {
        if(itemsHided.Count>0)
        {
            var temp = itemsHided[0];
            itemsHided.RemoveAt(0);
            return temp;
        }
        else
        {
            var temp = RectTransform.Instantiate(prefab);
            return temp;
        }
    }
    void RecycleItem(RectTransform item)
    {
        itemsHided.Add(item);
        item.SetParent(prefab.parent, false);
    }

    /// <summary>
    /// 检验参数有效性
    /// </summary>
    void CheckParam()
    {
        itemCount = itemCount < 0 ? 0 : itemCount;
        rowCount = rowCount < 0 ? 0 : rowCount;
        columnCount = columnCount < 0 ? 0 : columnCount;
        spaceXRate = spaceXRate < 0 ? 0 : spaceXRate;
        spaceYRate = spaceYRate < 0 ? 0 : spaceYRate;
    }
    /// <summary>
    /// 重新计算需的Item
    /// </summary>
    void CalculateItemCount()
    {
        for (int i = items.Count - 1; i >= itemCount; --i)
        {
            var temp = items[i];
            items.RemoveAt(i);
            RecycleItem(temp);
        }
        for (int i = items.Count; i < itemCount; ++i)
        {
            var temp = GetItem();
            temp.SetParent(transform, false);
            items.Add(temp);
        }
    }
    void CalculateItemSize(out float height, out float width, out float lastLineLeftIndent)
    {
        if(itemCount<1)
        {
            height = 0; width = 0; lastLineLeftIndent = 0;
            return;
        }
        RectTransform rectTransform = transform.GetComponent<RectTransform>();
        height = rectTransform.rect.height / (rowCount + (rowCount - 1) * spaceYRate);
        width = rectTransform.rect.width/ (columnCount + (columnCount - 1) * spaceXRate);
        float aspect = prefab.rect.width / prefab.rect.height;
        if (width/ height> aspect)
        {
            width = aspect * height;
            spaceXRate = columnCount - 1 == 0 ? 0 : ((rectTransform.rect.width / width) - columnCount) / (columnCount - 1);
        }
        else
        {
            height = width / aspect;
            spaceYRate = rowCount - 1 == 0 ? 0 : ((rectTransform.rect.height / height) - rowCount) / (rowCount - 1);
        }
        if (itemCount % columnCount != 0)
            lastLineLeftIndent = (rectTransform.rect.width - (itemCount % columnCount + (itemCount % columnCount - 1) * spaceXRate) * width) / 2;
        else
            lastLineLeftIndent = 0;
    }
    void InitItemShape(RectTransform temp, Vector2 size,Vector2 pos)
    {
        temp.anchorMin = Vector2.up;
        temp.anchorMax = Vector2.up;
        temp.sizeDelta = size;
        temp.pivot = Vector2.up;
        temp.anchoredPosition = pos;
    }
    /// <summary>
    /// 重新布局
    /// </summary>
    /// <param name="count">元素个数</param>
    /// <param name="row">行数</param>
    /// <param name="column">列数</param>
    /// <param name="xRate">行间距相对元素宽度比例</param>
    /// <param name="yRate">列间距相对元素高度比例</param>
    public void ResetLayout(int count, int row, int column,float xRate,float yRate)
    {
        count = count > row * column ? row * column : count;
        itemCount = count;
        rowCount = row;
        columnCount = column;
        spaceXRate = xRate;
        spaceYRate = yRate;
        CheckParam();
        CalculateItemCount();
        float height, width, lastLineLeftIndent;
        CalculateItemSize(out height, out width, out lastLineLeftIndent);
        float x = 0, y = 0;
        Vector2 size = new Vector2(width, height);
        for (int r = 0; r < rowCount; ++r)
        {
            if((itemCount - r * columnCount)< columnCount)
                x += lastLineLeftIndent;
            for (int c = 0; c < columnCount; ++c)
            {
                if (r * columnCount + c >= itemCount) return;
                var temp = items[r * columnCount + c];
                InitItemShape(temp, size, new Vector2(x, y));
                x += width * (1 + spaceXRate);
            }
            x = 0;
            y -= height * (1 + spaceYRate);
        }
    }
    private void OnValidate()
    {
        if (Editor) return;
        if (prefab == null) return;
        ResetLayout(itemCount, rowCount, columnCount, spaceXRate, spaceYRate);
    }
}
