package com.gardify.android.utils;

import android.content.Context;
import android.util.Log;

import androidx.core.util.Consumer;

import com.android.volley.AuthFailureError;
import com.android.volley.DefaultRetryPolicy;
import com.android.volley.NetworkResponse;
import com.android.volley.ParseError;
import com.android.volley.Request;
import com.android.volley.RequestQueue;
import com.android.volley.Response;
import com.android.volley.RetryPolicy;
import com.android.volley.VolleyError;
import com.android.volley.VolleyLog;
import com.android.volley.toolbox.HttpHeaderParser;
import com.android.volley.toolbox.JsonArrayRequest;
import com.android.volley.toolbox.JsonObjectRequest;
import com.android.volley.toolbox.StringRequest;
import com.android.volley.toolbox.Volley;
import com.gardify.android.data.account.ApplicationUser;
import com.gardify.android.data.misc.RequestData;
import com.google.gson.Gson;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

import java.io.UnsupportedEncodingException;
import java.util.HashMap;
import java.util.Map;
import java.util.function.BiConsumer;

/**
 * Network manager class to handle all network requests within the application. Uses the volley
 * library as a basis. Uses the singleton pattern so that only one network queue exists for the
 * entire application.
 * <p>
 * Default headers will be added to every request; see getHeaders()
 */
public class RequestQueueSingleton {
    private static RequestQueueSingleton instance;
    private static Context ctx;
    private RequestQueue requestQueue;

    private RequestQueueSingleton(Context context) {
        ctx = context;
        requestQueue = getRequestQueue();
    }

    public static synchronized RequestQueueSingleton getInstance(Context context) {
        if (instance == null) {
            instance = new RequestQueueSingleton(context);
        }
        return instance;
    }

    /**
     * Returns the active request queue
     *
     * @return Volley request queue
     */
    public RequestQueue getRequestQueue() {
        if (requestQueue == null) {
            // getApplicationContext() is key, it keeps you from leaking the
            // Activity or BroadcastReceiver if someone passes one in.
            requestQueue = Volley.newRequestQueue(ctx.getApplicationContext());
        }
        return requestQueue;
    }

    public void cancelAllRequests() {
        if (requestQueue == null) {
            requestQueue.cancelAll(request -> true);
        }
    }

    /**
     * Low-level function, mostly unused throughout the application.
     * Adds a new request to the queue.
     *
     * @param req
     * @param <T>
     */
    public <T> void addToRequestQueue(Request<T> req) {
        LogNetwork("Added to queue", "HTTP " + (req.getMethod() == Request.Method.GET ? "GET" : "POST") + ", " + req.getUrl());
        req.setRetryPolicy(getRetryPolicy());
        getRequestQueue().add(req);
    }

    /**
     * Low-level function, mostly unused throughout the application.
     * Constructs a new GET request and adds it to the request queue. Calls given functions upon
     * success or error.
     * Can be used to request raw JSON objects
     *
     * @param url           Url to send the request to
     * @param method        type of method to use Get, Post, Put or Delete
     * @param bodyParams    body params for Post or Put method
     * @param successMethod Function to call when the request was successful
     * @param errorMethod   Function to call when the request failed
     */
    public void objectRequest(String url, int method, Consumer<JSONObject> successMethod, Consumer<VolleyError> errorMethod, JSONObject bodyParams) {
        JsonObjectRequest getRequest = new JsonObjectRequest(method, url, bodyParams,
                new Response.Listener<JSONObject>() {
                    @Override
                    public void onResponse(JSONObject response) {
                        LogNetwork("Response Received", "Object Request", (response == null ? "" : response.toString()));
                        if (successMethod != null) {
                            successMethod.accept(response);
                        }
                    }
                },
                new Response.ErrorListener() {
                    @Override
                    public void onErrorResponse(VolleyError error) {
                        LogNetworkError("Error received", error.toString());
                        if (errorMethod != null) {
                            errorMethod.accept(error);
                        }
                    }
                }
        ) {
            @Override
            public Map<String, String> getHeaders() throws AuthFailureError {
                return RequestQueueSingleton.this.getHeaders(url);
            }

            // if response of the JsonObjectRequest is successful but  empty.
            @Override
            protected Response<JSONObject> parseNetworkResponse(NetworkResponse response) {
                try {
                    String jsonString = new String(response.data,
                            HttpHeaderParser.parseCharset(response.headers, PROTOCOL_CHARSET));

                    JSONObject result = null;

                    if (jsonString != null && jsonString.length() > 0)
                        result = new JSONObject(jsonString);

                    return Response.success(result,
                            HttpHeaderParser.parseCacheHeaders(response));
                } catch (UnsupportedEncodingException e) {
                    return Response.error(new ParseError(e));
                } catch (JSONException je) {
                    return Response.error(new ParseError(je));
                }
            }
        };
        addToRequestQueue(getRequest);
    }


    /**
     * Low-level function, mostly unused throughout the application.
     * Constructs a new GET request and adds it to the request queue. Calls given functions upon
     * success or error.
     * Can be used to request raw JSON arrays
     *
     * @param url           Url to send the request to
     * @param method        type of method to use Get, Post or Put
     * @param bodyParams    body params for Post or Put method
     * @param successMethod Function to call when the request was successful
     * @param errorMethod   Function to call when the request failed
     */
    public void arrayRequest(String url, int method, Consumer<JSONArray> successMethod, Consumer<VolleyError> errorMethod, JSONArray bodyParams) {
        JsonArrayRequest getRequest = new JsonArrayRequest(method, url, bodyParams,
                new Response.Listener<JSONArray>() {
                    @Override
                    public void onResponse(JSONArray response) {
                        LogNetwork("Response Received", "Array Request", response.toString());
                        if (successMethod != null) {
                            successMethod.accept(response);
                        }
                    }
                },
                new Response.ErrorListener() {
                    @Override
                    public void onErrorResponse(VolleyError error) {
                        LogNetworkError("Error received", error.toString());
                        if (errorMethod != null) {
                            errorMethod.accept(error);
                        }
                    }
                }
        ) {
            @Override
            public Map<String, String> getHeaders() throws AuthFailureError {
                return RequestQueueSingleton.this.getHeaders(url);
            }
        };
        addToRequestQueue(getRequest);
    }

    /**
     * Low-level function, mostly unused throughout the application.
     * Constructs a new GET request and adds it to the request queue. Calls given functions upon
     * success or error.
     * Can be used to request raw strings.
     *
     * @param url           Url to send the request to
     * @param successMethod Function to call when the request was successful
     * @param errorMethod   Function to call when the request failed
     */
    public void stringRequest(String url, int method, Consumer<String> successMethod, Consumer<VolleyError> errorMethod,
                              JSONObject jsonObject) {
        StringRequest getRequest = new StringRequest(method, url,
                new Response.Listener<String>() {
                    @Override
                    public void onResponse(String response) {
                        LogNetwork("Response Received", "String Request", response.toString());
                        if (successMethod != null) {
                            successMethod.accept(response);
                        }
                    }
                },
                new Response.ErrorListener() {
                    @Override
                    public void onErrorResponse(VolleyError error) {
                        LogNetworkError("Error received", error.toString());
                        if (errorMethod != null) {
                            errorMethod.accept(error);
                        }
                    }
                }
        ) {

            @Override
            public byte[] getBody() throws AuthFailureError {
                try {
                    return jsonObject == null ? null : jsonObject.toString().getBytes("utf-8");
                } catch (UnsupportedEncodingException uee) {
                    VolleyLog.wtf("Unsupported Encoding while trying to get the bytes of %s using %s", jsonObject, "utf-8");
                    return null;
                }
            }

            @Override
            public String getBodyContentType() {
                return "application/json";
            }

            @Override
            public Map<String, String> getHeaders() throws AuthFailureError {
                return RequestQueueSingleton.this.getHeaders(url);
            }
        };
        addToRequestQueue(getRequest);
    }

    /**
     * Low-level function, mostly unused throughout the application.
     * Constructs a new GET request and adds it to the request queue. Calls given functions upon
     * success or error.
     * Can be used to request raw strings.
     *
     * @param url           Url to send the request to
     * @param successMethod Function to call when the request was successful
     * @param errorMethod   Function to call when the request failed
     */
    public void stringRequestFormData(String url, int method, Consumer<String> successMethod, Consumer<VolleyError> errorMethod,
                                      Map<String,String> params) {
        StringRequest getRequest = new StringRequest(method, url,
                response -> {
                    LogNetwork("Response Received", "String Request", response);
                    if (successMethod != null) {
                        successMethod.accept(response);
                    }
                },
                error -> {
                    LogNetworkError("Error received", error.toString());
                    if (errorMethod != null) {
                        errorMethod.accept(error);
                    }
                }
        ) {

            @Override
            protected Map<String,String> getParams(){
                return params;
            }

            @Override
            public Map<String, String> getHeaders() {
                return RequestQueueSingleton.this.getHeaders(url);
            }
        };
        addToRequestQueue(getRequest);
    }

    /**
     * Makes a network GET request with the given parameters and calls the given callback functions.
     * Also converts the response object to the supplied type of T.
     *
     * @param url           The url to send the request to
     * @param successMethod Function to call if the request and parsing was successful
     * @param errorMethod   Function to call if either the request or the parsing failed
     * @param classOfT      Type to convert the response object to
     */
    public <T> void typedRequest(String url, BiConsumer<T, RequestData> successMethod, BiConsumer<Exception, RequestData> errorMethod, Class<T> classOfT) {
        typedRequest(url, successMethod, errorMethod, classOfT, new RequestData());
    }

    /**
     * Makes a network GET request with the given parameters and calls the given callback functions.
     * Also converts the response object to the supplied type of T.
     *
     * @param url           The url to send the request to
     * @param successMethod Function to call if the request and parsing was successful
     * @param errorMethod   Function to call if either the request or the parsing failed
     * @param classOfT      Type to convert the response object to
     * @param data          Additional information about the request such as the calling context and metadata
     */
    public <T> void typedRequest(String url, BiConsumer<T, RequestData> successMethod, BiConsumer<Exception, RequestData> errorMethod, Class<T> classOfT, RequestData data) {
        data.setContext(ctx);
        data.setRequestUrl(url);
        if (classOfT.isArray()) {
            JsonArrayRequest getRequest = new JsonArrayRequest(Request.Method.GET, url, null,
                    new Response.Listener<JSONArray>() {
                        @Override
                        public void onResponse(JSONArray response) {
                            try {
                                LogNetwork("Response Received", "Array Request", response.toString());
                                T parsedObject = new Gson().fromJson(response.toString(), classOfT);
                                if (successMethod != null) {
                                    successMethod.accept(parsedObject, data);
                                }
                            } catch (Exception e) {
                                LogNetworkError("Error while parsing", e.toString());
                                if (errorMethod != null) {
                                    errorMethod.accept(e, data);
                                }
                            }
                        }
                    },
                    new Response.ErrorListener() {
                        @Override
                        public void onErrorResponse(VolleyError error) {
                            LogNetworkError("Error received", error.toString());
                            if (errorMethod != null) {
                                errorMethod.accept(error, data);
                            }
                        }
                    }
            ) {
                @Override
                public Map<String, String> getHeaders() throws AuthFailureError {
                    return RequestQueueSingleton.this.getHeaders(url);
                }
            };
            addToRequestQueue(getRequest);
        } else {
            JsonObjectRequest getRequest = new JsonObjectRequest(Request.Method.GET, url, null,
                    new Response.Listener<JSONObject>() {
                        @Override
                        public void onResponse(JSONObject response) {
                            try {
                                LogNetwork("Response Received", "Object Request", response.toString());
                                T parsedObject = new Gson().fromJson(response.toString(), classOfT);
                                if (successMethod != null) {
                                    successMethod.accept(parsedObject, data);
                                }
                            } catch (Exception e) {
                                LogNetworkError("Error while parsing", e.toString());
                                if (errorMethod != null) {
                                    errorMethod.accept(e, data);
                                }
                            }
                        }
                    },
                    new Response.ErrorListener() {
                        @Override
                        public void onErrorResponse(VolleyError error) {
                            LogNetworkError("Error received", error.toString());
                            if (errorMethod != null) {
                                errorMethod.accept(error, data);
                            }
                        }
                    }
            ) {
                @Override
                public Map<String, String> getHeaders() throws AuthFailureError {
                    return RequestQueueSingleton.this.getHeaders(url);
                }
            };
            addToRequestQueue(getRequest);
        }
    }

    private void LogNetwork(String title, String content) {
        Log.i("Network", title + ": " + content);
    }

    private void LogNetwork(String title, String content, String object) {
        LogNetwork(title, content);
        LogObject(object);
    }

    private void LogObject(String object) {
        if (object.length() > 100) {
            object = object.substring(0, 100) + "...";
        }
        Log.i("Network", "Related object: " + object);
    }

    private void LogNetworkError(String title, String content) {
        Log.e("Network", title + ": " + content);
    }

    /**
     * Returns standard headers for authentication, if the user is logged in.
     *
     * @param url URL to check whether the bearer token should be added to the headers
     * @return List of headers to be used in a volley network request
     */
    private Map<String, String> getHeaders(String url) {
        Map<String, String> headers = new HashMap<>();

        //Make sure the request is not going to a different server - we don't want to give the user's login token to someone else
        if (!url.contains("gardify")) {
            return headers;
        }
        ApplicationUser user = PreferencesUtility.getUser(ctx);
        if (user == null) {
            return headers;
        }

        headers.put("Authorization", "Bearer " + user.getToken());
        return headers;
    }

    /**
     * Returns an updated retry policy.
     *
     * @return Retry policy with 50 seconds timeout and 5 retries
     */
    private RetryPolicy getRetryPolicy() {
        //TODO reduce timeout after api fix .. current timeout time is 3 min
        return new DefaultRetryPolicy(180000, 5, DefaultRetryPolicy.DEFAULT_BACKOFF_MULT);
    }
}
