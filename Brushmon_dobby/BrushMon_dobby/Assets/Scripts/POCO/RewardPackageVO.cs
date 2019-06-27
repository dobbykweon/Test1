using System;
using System.Collections.Generic;

[Serializable]  
public class RewardPackageVO
{
    public string PackageID {get;set;}
    public string PackageName {get;set;}
    public List<RewardVO> RewardList {get;set;}  
}