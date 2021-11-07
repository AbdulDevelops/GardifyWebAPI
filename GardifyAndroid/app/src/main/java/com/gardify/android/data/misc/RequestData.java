package com.gardify.android.data.misc;

import android.content.Context;

import com.gardify.android.utils.RequestType;


/**
 * This class is technically not a model class, as it's just supposed to store meta information
 * about requests that were sent via the network manager. When inheriting this class, custom
 * information can be stored as well.
 */
public class RequestData {
    private RequestType requestType;
    private Context context;
    private String requestUrl;

    public RequestData() {
        requestType = RequestType.Unspecified;
    }

    public RequestData(RequestType type) {
        setRequestType(type);
    }

    public Context getContext() {
        return context;
    }

    public void setContext(Context context) {
        this.context = context;
    }

    public RequestType getRequestType() {
        return requestType;
    }

    public void setRequestType(RequestType requestType) {
        this.requestType = requestType;
    }

    public String getRequestUrl() {
        return requestUrl;
    }

    public void setRequestUrl(String requestUrl) {
        this.requestUrl = requestUrl;
    }

}
