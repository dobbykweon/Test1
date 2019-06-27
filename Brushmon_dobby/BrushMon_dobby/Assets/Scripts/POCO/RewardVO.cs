using System;
using System.Collections.Generic;

[Serializable]  
public class RewardVO
{
    public string RewardID {get;set;}
    public string StickerID {get;set;}
    public string StickerName {get;set;}
    public DateTime CreateDate {get;set;}
    public string AccessoryName {get;set;}
    public string AccessoryType {get;set;}   
    public List<RewardResultVO> BrushingList {get;set;}   
}