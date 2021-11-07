package com.gardify.android.utils;

import android.content.Context;
import android.util.Log;

import androidx.core.util.Consumer;

import com.android.volley.NetworkResponse;
import com.android.volley.Request;
import com.android.volley.VolleyError;
import com.android.volley.VolleyLog;
import com.gardify.android.data.account.ApplicationUser;

import org.apache.http.HttpEntity;

import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.util.HashMap;
import java.util.Map;

public class RequestImageUpload {
    private Context context;

    public RequestImageUpload(Context context) {
        this.context = context;
    }

    public VolleyMultipartRequest imageRequest(String url, Consumer<NetworkResponse> successMethod, Consumer<VolleyError> errorMethod, HttpEntity httpEntity) {
        VolleyMultipartRequest myRequest = new VolleyMultipartRequest(Request.Method.POST, url, response -> {
            LogNetwork("Response Received Image Request", response.toString());
            if (successMethod != null) {
                successMethod.accept(response);
            }
        },
                error -> {
                    LogNetwork("Error received", error.toString());
                    if (errorMethod != null) {
                        errorMethod.accept(error);
                    }
                }
        ) {
            @Override
            public String getBodyContentType() {
                return httpEntity.getContentType().getValue();
            }

            @Override
            public Map<String, String> getHeaders() {
                ApplicationUser user = PreferencesUtility.getUser(context);
                Map<String, String> headers = new HashMap<>();
                headers.put("Authorization", "Bearer " + user.getToken());
                return headers;
            }

            @Override
            public byte[] getBody() {
                ByteArrayOutputStream bos = new ByteArrayOutputStream();
                try {
                    httpEntity.writeTo(bos);
                } catch (IOException e) {
                    VolleyLog.e("IOException writing to ByteArrayOutputStream");
                }
                return bos.toByteArray();
            }
        };
        return myRequest;
    }

    private void LogNetwork(String title, String content) {
        Log.i("Network", title + ": " + content);
    }

}
