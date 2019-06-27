using System;
using System.Collections.Generic;

[Serializable]
public class UserAccountVo
{
    public List<SignVo> list;

    public UserAccountVo()
    {
        list = new List<SignVo>();
    }
}
