package com.gardify.android.viewModelData;

import org.json.JSONException;
import org.json.JSONObject;

public class UserDetail {

    public static JSONObject deviceDetail() {
        JSONObject jsonObject=new JSONObject();
        try {
            jsonObject.put("IsAndroid", true);
            jsonObject.put("IsIos", false);
            jsonObject.put("IsWebPage", false);
        } catch (JSONException e) {
            e.printStackTrace();
        }
        //  convert JSONObject to JSON to String
        return jsonObject;
    }
}
