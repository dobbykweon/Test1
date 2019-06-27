using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BrushingResultDTO {
    public long profile_idx;
    public string sticker_name;
    public List<string> result_list;
    public long create_date;
    public bool use_brush;
    public string os;
}