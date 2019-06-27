using System.Collections.Generic;
using UnityEngine;

using ULSTrackerForUnity;

public enum AccessoryIndex
{
    Glasses = 0,
    Hat = 1,
    Max = 100
}

public class AccessoryController : MonoBehaviour
{
    [SerializeField] private List<SpriteRenderer> listRen;

    Transform thisTransform;

    List<TextMesh> listTarget;

    bool isEnable = false;
    bool isTracking = true;

    List<AccessoryIndex> listIdx;

    public void Init(List<TextMesh> _listTarget)
    {
        thisTransform = transform;
        listTarget = _listTarget;

        for(int i = 0; i < listRen.Count; i++)
        {
            listRen[i].flipX = true;
            listRen[i].flipY = true;
        }

        SetEnable(true);
    }

    public void SetEnable(bool _isEnable)
    {
        isEnable = _isEnable;
        gameObject.SetActive(isEnable);
    }

    public void SetEnableAccessory(AccessoryIndex _idx, bool _isEnable)
    {
        listRen[(int)_idx].enabled = _isEnable;
    }

    private void Update()
    {
        if(isEnable == true)
        {
            if (isTracking == true)
            {
                //사이즈 조절
                float _scale = (20f * Plugins.ULS_UnityGetScaleInImage()) / 2.3f;

                for (int i = 0; i < listRen.Count; i++)
                {
                    listRen[i].transform.localScale = Vector3.one * _scale;
                    listRen[i].transform.localEulerAngles = Vector3.zero;
                }

                // 위치조절
                // 모자
                thisTransform.position = Vector2.Lerp(listTarget[5].transform.position, listTarget[8].transform.position, 0.5f);

                float x = Vector2.Distance(listTarget[9].transform.localPosition, listTarget[10].transform.localPosition);

                listRen[(int)AccessoryIndex.Hat].transform.localPosition = Vector3.down * x * 2.3f;

                // 안경
                listRen[(int)AccessoryIndex.Glasses].transform.position = listTarget[9].transform.position;

                // 각도조절
                Vector2 relative = listTarget[14].transform.InverseTransformPoint(listTarget[20].transform.position);
                float angle = Mathf.Atan2(relative.y, relative.x) * Mathf.Rad2Deg;
                thisTransform.eulerAngles = new Vector3(0, 0, angle);
            }
        }
    }

}
