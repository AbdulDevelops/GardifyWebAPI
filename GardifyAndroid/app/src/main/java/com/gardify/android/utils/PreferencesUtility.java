package com.gardify.android.utils;

import android.content.Context;
import android.content.SharedPreferences;
import android.preference.PreferenceManager;

import com.gardify.android.data.account.ApplicationUser;
import com.gardify.android.data.account.UserMainGarden;
import com.gardify.android.data.myGarden.PlantCount;
import com.gardify.android.data.todos.TodoCount;
import com.google.gson.Gson;
import com.google.gson.reflect.TypeToken;

import java.util.HashMap;

/**
 * This class is used to interact with the SharedPreferences more easily. The existing functions all
 * call either setString, getString or stringExists with the class constants as keys.
 */
public class PreferencesUtility {
    public static final String LOGGED_IN_USER_PREF = "logged_in_user";
    public static final String USER_MAIN_GARDEN_PREF = "user_main_garden";
    public static final String USER_TODO_COUNT = "user_todo_count";
    private static final String USER_PLANT_COUNT = "user_plant_count";
    private static final String USER_WARNING_COUNT = "user_warning_count";
    private static final String LATEST_WATCHED_VIDEO_DATE = "latest_watched_video_date";
    private static final String LATEST_SEEN_NEWS_DATE = "latest_seen_news_date";
    private static final String PLANT_DOC_SEEN_ANSWERS = "plant_doc_seen_answers";
    public static final String THEME = "theme";
    public static final String THEME_DARK = "dark";
    public static final String THEME_BLUE = "blue";
    private static final String ECO_SCAN_SURFACE_AREA = "surface_area";
    private static final String ECO_SCAN_GREEN_AREA = "green_area";
    private static final String FLAG_FIRST_TIME_USER = "flag_first_time_user";

    static SharedPreferences getPreferences(Context context) {
        return PreferenceManager.getDefaultSharedPreferences(context);
    }

    /**
     * Get the Login Status
     *
     * @param context
     * @return boolean: login status
     */
    public static boolean getLoggedIn(Context context) {
        String userString = getUserString(context);
        return !userString.equals("");
    }

    public static void setLoggedOut(Context context) {
        setUser(context, "");
    }

    /**
     * Writes the user into SharedPreferences as a JSON blob. Also takes the timezone id, country
     * id, language id and newsletter subscription status and also writes them into
     * SharedPreferences.
     *
     * @param context Context
     * @param user    ApplicationUser object to store
     */
    public static void setUser(Context context, ApplicationUser user) {
        String serializedUser = new Gson().toJson(user);
        setUser(context, serializedUser);
    }

    /**
     * Writes the user into SharedPreferences as a JSON blob. Also takes the timezone id, country
     * id, language id and newsletter subscription status and also writes them into
     * SharedPreferences.
     *
     * @param context        Context
     * @param serializedUser JSON string of the ApplicationUser object
     */
    public static void setUser(Context context, String serializedUser) {
        ApplicationUser user = new Gson().fromJson(serializedUser, ApplicationUser.class);
        setString(context, LOGGED_IN_USER_PREF, serializedUser);
        if (user == null) {
            return;
        }
    }

    public static ApplicationUser getUser(Context context) {
        try {
            return new Gson().fromJson(getUserString(context), ApplicationUser.class);
        } catch (Exception e) {
            return null;
        }
    }

    /**
     * Writes the user into SharedPreferences as a JSON blob.
     *
     * @param context       Context
     * @param jsonString    JSON string of the MainUserGarden object
     */
    public static void setUserMainGarden(Context context, String jsonString) {
        UserMainGarden mainGarden = new Gson().fromJson(jsonString, UserMainGarden.class);
        setString(context, USER_MAIN_GARDEN_PREF, jsonString);
        if (mainGarden == null) {
            return;
        }
    }

    public static UserMainGarden getUserMainGarden(Context context) {
        try {
            return new Gson().fromJson(getString(context, USER_MAIN_GARDEN_PREF), UserMainGarden.class);
        } catch (Exception e) {
            return null;
        }
    }

    /**
     * Writes the todoCount into SharedPreferences as a JSON blob.
     *
     * @param context       Context
     * @param todoCount    TodoCount object
     */
    public static void setTodoCount(Context context, TodoCount todoCount) {
        String jsonString = new Gson().toJson(todoCount);

        setString(context, USER_TODO_COUNT, jsonString);
        if (todoCount == null) {
            return;
        }
    }

    public static TodoCount getTodoCount(Context context) {
        try {
            return new Gson().fromJson(getString(context,USER_TODO_COUNT), TodoCount.class);
        } catch (Exception e) {
            return null;
        }
    }

    /**
     * Writes the plantCount into SharedPreferences as a JSON blob.
     *
     * @param context       Context
     * @param plantCount    PlantCount object
     */
    public static void setPlantCount(Context context, PlantCount plantCount) {
        String jsonString = new Gson().toJson(plantCount);
        setString(context, USER_PLANT_COUNT, jsonString);
        if (plantCount == null) {
            return;
        }
    }

    public static PlantCount getPlantCount(Context context) {
        try {
            return new Gson().fromJson(getString(context,USER_PLANT_COUNT), PlantCount.class);
        } catch (Exception e) {
            return null;
        }
    }
    /**
     * Writes the latest watched video date in SharedPreferences.
     *
     * @param context       Context
     * @param latestVideoDate    sets Latest watched video date
     */
    public static void setLatestWatchedVideoDate(Context context, String latestVideoDate) {
        setString(context, LATEST_WATCHED_VIDEO_DATE, latestVideoDate);
    }
    public static String getLatestWatchedVideoDate(Context context) {
        return getString(context, LATEST_WATCHED_VIDEO_DATE);
    }
    /**
     * Writes the latest seen news date in SharedPreferences.
     *
     * @param context       Context
     * @param latestVideoDate    sets Latest seen video date
     */
    public static void setLatestSeenNewsDate(Context context, String latestVideoDate) {
        setString(context, LATEST_SEEN_NEWS_DATE, latestVideoDate);
    }
    public static String getLatestSeenNewsDate(Context context) {
        return getString(context, LATEST_SEEN_NEWS_DATE);
    }

    /**
     * Writes the plantCount into SharedPreferences as a JSON blob.
     *
     * @param context       Context
     * @param warningCount    set total count of warnings
     */
    public static void setWarningCount(Context context, String warningCount) {
        setString(context, USER_WARNING_COUNT, warningCount);
    }
    public static String getWarningCount(Context context) {
        return getString(context, USER_WARNING_COUNT);
    }
    /**
     * Writes the Eco Scan Surface Area into SharedPreferences as a JSON blob.
     *
     * @param context       Context
     * @param surfaceArea   Eco scan surfaceArea (Außenfläche)
     */
    public static void setSurfaceArea(Context context, String surfaceArea) {
        setString(context, ECO_SCAN_SURFACE_AREA, surfaceArea);
    }
    public static String getSurfaceArea(Context context) {
        return getString(context, ECO_SCAN_SURFACE_AREA);
    }

    /**
     * Writes the Eco Scan Green Area into SharedPreferences as a JSON blob.
     *
     * @param context       Context
     * @param greenArea   Eco scan GreenArea (Begrünte Fläche)
     */
    public static void setGreenArea(Context context, String greenArea) {
        setString(context, ECO_SCAN_GREEN_AREA, greenArea);
    }
    public static String getGreenArea(Context context) {
        return getString(context, ECO_SCAN_GREEN_AREA);
    }
    /**
     * @param context
     * @param jsonMap  hashMap of plantDoc question ids and answer lastSeen Date
     */

    public static void setSeenAnswerDate(Context context, HashMap<Integer, Integer> jsonMap) {
        String plantDocSeenAnswerHashMap = new Gson().toJson(jsonMap);
        setString(context, PLANT_DOC_SEEN_ANSWERS, plantDocSeenAnswerHashMap);
    }

    public static HashMap<Integer, Integer> getSeenAnswerDate(Context context){
        String json= getString(context, PLANT_DOC_SEEN_ANSWERS);
        TypeToken<HashMap<Integer, Integer>> token = new TypeToken<HashMap<Integer, Integer>>() {};
        HashMap<Integer, Integer> retrievedMap=new Gson().fromJson(json,token.getType());
        return retrievedMap;
    }


    public static String getUserString(Context context) {
        return getString(context, LOGGED_IN_USER_PREF);
    }

    public static void setTheme(Context context, String theme) {
        setString(context, THEME, theme);
    }

    public static String getTheme(Context context) {
        return getString(context, THEME);
    }

    public static void setIsFirstTimeUser(Context context, boolean flag) {
        setBool(context, FLAG_FIRST_TIME_USER, flag);
    }

    public static boolean getIsFirstTimeUser(Context context) {
        return getBool(context, FLAG_FIRST_TIME_USER);
    }

    /**
     * Returns whether or not a string for a given key exists
     *
     * @param context Context
     * @param key     Key
     * @return true if value for key exists; false if it's empty
     */
    public static boolean stringExists(Context context, String key) {
        String value = getString(context, key);
        return !value.equals("");
    }

    /**
     * Writes the value for a given key into SharedPreferences. If the key already exists, it will
     * be overwritten.
     *
     * @param context Context
     * @param key     Key
     * @param value   Value
     */
    private static void setString(Context context, String key, String value) {
        SharedPreferences.Editor editor = getPreferences(context).edit();
        editor.putString(key, value);
        editor.apply();
    }


    /**
     * Retrieves the value for the given key from SharedPreferences
     *
     * @param context Context
     * @param key     Key
     * @return Value for given key or an empty string, if key doesn't exist
     */
    private static String getString(Context context, String key) {
        return getPreferences(context).getString(key, "");
    }

    /**
     * Writes the value for a given key into SharedPreferences. If the key already exists, it will
     * be overwritten.
     *
     * @param context Context
     * @param key     Key
     * @param flag   flag
     */
    private static void setBool(Context context, String key, boolean flag) {
        SharedPreferences.Editor editor = getPreferences(context).edit();
        editor.putBoolean(key, flag);
        editor.apply();
    }


    /**
     * Retrieves the value for the given key from SharedPreferences
     *
     * @param context Context
     * @param key     Key
     * @return Value for given key or false, if key doesn't exist
     */
    private static boolean getBool(Context context, String key) {
        return getPreferences(context).getBoolean(key,true);
    }
}