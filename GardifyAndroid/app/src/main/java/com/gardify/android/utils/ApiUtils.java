package com.gardify.android.utils;

import com.google.gson.Gson;
import com.google.gson.GsonBuilder;

public class ApiUtils {

    /**
     * Generates a set of query string parameters for a given url in way such that the .NET Core
     * model binder can deal with it.
     * Expects url ending on ? or &, returns url ending on &
     *
     * @param baseUrl         The url to append the query string to
     * @param parameterName   Name of the array parameter
     * @param parameterValues Values of the array parameter
     * @return Fully generated url with parameters at the end
     */
    public static String generateArrayUrl(String baseUrl, String parameterName, int[] parameterValues) {
        String[] convertedValues = new String[parameterValues.length];
        for (int i = 0; i < parameterValues.length; i++) {
            convertedValues[i] = "" + parameterValues[i];
        }
        return generateArrayUrl(baseUrl, parameterName, convertedValues);
    }

    /**
     * Generates a set of query string parameters for a given url in way such that the .NET Core
     * model binder can deal with it.
     * Expects url ending on ? or &, returns url ending on &
     *
     * @param baseUrl         The url to append the query string to
     * @param parameterName   Name of the array parameter
     * @param parameterValues Values of the array parameter
     * @return Fully generated url with parameters at the end
     */
    public static String generateArrayUrl(String baseUrl, String parameterName, String[] parameterValues) {
        for (String parameterValue : parameterValues) {
            baseUrl += parameterName + "=" + parameterValue + "&";
        }

        return baseUrl;
    }

    /**
     * Parses JSON on using GSON
     */
    private static Gson gson;

    public static Gson getGsonParser() {
        if (null == gson) {
            GsonBuilder builder = new GsonBuilder();
            gson = builder.create();
        }
        return gson;
    }

    /**
     * @param currentPage
     * @return decrements count by 1 (Api count starts from 0)
     */
    public static int startCountFromZero(int currentPage) {
        return currentPage - 1;
    }
}
