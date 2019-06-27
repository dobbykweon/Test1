using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TrackingManager : GlobalMonoSingleton<TrackingManager> {

    public void Tracking (string action, string description)
    {
        TokenVo tokenVo = null;
        try {
            tokenVo = ConfigurationData.Instance.GetValueFromJson<TokenVo> ("Token");
        } catch (Exception exception) {
            LogManager.Log (exception.Message);
        }

        if (tokenVo != null) {
            ProfileVo currentProfileVo = null;
            try {
                currentProfileVo = ConfigurationData.Instance.GetValueFromJson<ProfileVo> ("CurrentProfile");
            } catch (Exception exception) {
                LogManager.Log (exception.Message);
            }
            if (currentProfileVo != null) {
                TrackingVo trackingVo = new TrackingVo ();
                trackingVo.action = action;
                trackingVo.description = description;
                trackingVo.profile_idx = currentProfileVo.profile_idx;
                RestService.Instance.Tracking (tokenVo, trackingVo);
            }
        }
    }

}