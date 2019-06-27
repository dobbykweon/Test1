using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gradation : MonoBehaviour
{

    private Image gradationImage;
    
    void Awake()
    {
        gradationImage = GetComponent<Image>();
    }

    private BrushStatus brushStatus;

    public void shwoGradation(BrushStatus brushStatus)
    {
        SetGradation(brushStatus);

      //  if (brushStatus.ToString().StartsWith("BRUSH_AR_GUIDE"))
      //  {
      //gradationImage.enabled = false;
      //  }
      //  else
      //  {
      //      //string ResourceName;
      //      //if (brushStatus < BrushStatus.GREEN_MOLD_01)
      //      //{
      //      //    ResourceName = String.Concat("Textures/brushing_bg_", "00");
      //      //}
      //      //else if (brushStatus >= BrushStatus.GREEN_MOLD_01 && brushStatus < BrushStatus.GREEN_MOLD_02)
      //      //{
      //      //    ResourceName = String.Concat("Textures/brushing_bg_", "01");
      //      //}
      //      //else if (brushStatus >= BrushStatus.GREEN_MOLD_02 && brushStatus < BrushStatus.GREEN_MOLD_03)
      //      //{
      //      //    ResourceName = String.Concat("Textures/brushing_bg_", "02");
      //      //}
      //      //else if (brushStatus >= BrushStatus.GREEN_MOLD_03 && brushStatus < BrushStatus.GREEN_MOLD_04)
      //      //{
      //      //    ResourceName = String.Concat("Textures/brushing_bg_", "03");
      //      //}
      //      //else if (brushStatus >= BrushStatus.GREEN_MOLD_04 && brushStatus < BrushStatus.FINISH_TONGUE)
      //      //{
      //      //    ResourceName = String.Concat("Textures/brushing_bg_", "04");
      //      //}
      //      //else
      //      //{
      //      //    ResourceName = String.Concat("Textures/brushing_bg_", "00");
      //      //}


        //      //gradationImage.sprite = Resources.Load<Sprite>(ResourceName);
        //      gradationImage.enabled = true;
        //  }
    }

    public void SetGradation(BrushStatus brushStatus)
    {
        if (brushStatus.ToString().StartsWith("BRUSH_AR_GUIDE") || brushStatus == BrushStatus.INTRO_MOVIE)
        {
            gradationImage.enabled = false;
        }
        else
        {
            Color startColor = Color.black;
            Color endColor = Color.black;

            if (brushStatus < BrushStatus.GREEN_MOLD_01)
            {
                startColor = endColor = GetColor(0x00, 0x00, 0x00, 179);
            }
            else if (brushStatus >= BrushStatus.GREEN_MOLD_01 && brushStatus < BrushStatus.GREEN_MOLD_02)
            {
                startColor = GetColor(0xff, 0x00, 0x6f, 179);
                endColor = GetColor(0x86, 0x23, 0x83, 179);
            }
            else if (brushStatus >= BrushStatus.GREEN_MOLD_02 && brushStatus < BrushStatus.GREEN_MOLD_03)
            {
                startColor = GetColor(0xff, 0x00, 0xf0, 179);
                endColor = GetColor(0x62, 0x23, 0x86, 179);
            }
            else if (brushStatus >= BrushStatus.GREEN_MOLD_03 && brushStatus < BrushStatus.GREEN_MOLD_04)
            {
                startColor = GetColor(0x9c, 0x00, 0xff, 179);
                endColor = GetColor(0x3a, 0x23, 0x86, 179);
            }
            else if (brushStatus >= BrushStatus.GREEN_MOLD_04 && brushStatus < BrushStatus.FINISH_TONGUE)
            {
                startColor = GetColor(0x24, 0x00, 0xff, 179);
                endColor = GetColor(0x23, 0x46, 0x86, 179);
            }
            else
            {
                startColor = endColor = GetColor(0x00, 0x00, 0x00, 179);
            }

            gradationImage.material.SetColor("_StartColor", startColor);
            gradationImage.material.SetColor("_EndColor", endColor);

            gradationImage.enabled = true;
        }
    }

    Color GetColor(float r, float g, float b, float a)
    {
        return new Color(r / 255f, g / 255f, b / 255f, a / 255f);
    }

    public void hideGradation()
    {
        gradationImage.enabled = false;
    }

    public void InitAnimation()
    {
        LogManager.LogError("Gradation.InitAnimation");
        gradationImage.sprite = Resources.Load<Sprite>(String.Concat("Textures/brushing_bg_", "00"));
    }

}
