using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyZoneView : MonoBehaviour
{

    public float spacing = 2f;

    public void RepositionChildrenEnemySlotView()
    {
        int count = transform.childCount;
        float offset = (count - 1) * spacing * 0.5f;

        for (int i = 0; i < count; i++)
        {
            Transform child = transform.GetChild(i);
            child.localPosition = new Vector3(i * spacing - offset, 0, 0);
        }
    }
    
    public void RepositionChildrenEnemySlotViewCenterOut()
    {
        int count = transform.childCount;
        int half = count / 2;

        for (int i = 0; i < count; i++)
        {
            Transform child = transform.GetChild(i);

            int indexOffset;
            if (i < half)
            {
                // Partie gauche
                indexOffset = i - half;
            }
            else
            {
                // Partie droite : saute le centre
                indexOffset = i - half + 1;
            }

            float posX = indexOffset * spacing;
            child.localPosition = new Vector3(posX, 0, 0);
        }
    }
}
