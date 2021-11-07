package com.gardify.android.utils;

public class APP_URL {

    //Base URL
    public static String BASE_ROUTE = "https://gardifybackend.sslbeta.de/";

    public static String BASE_ROUTE_INTERN = "https://gardify.de/intern/";

    public static String BASE_ROUTE_API = BASE_ROUTE + "api/";

    public static String PLANT_TAGS_API = BASE_ROUTE_API + "planttagsapi/";

    public static String PLANT_CATS_API = PLANT_TAGS_API + "cats";

    public static String PLANT_GROUP_API = BASE_ROUTE_API + "groupapi/";


    public static String PLANT_DOC_ROUTE = BASE_ROUTE_API + "PlantDocAPI/";
    public static String PLANT_DOC_ALL_ENTRIES = PLANT_DOC_ROUTE + "getAllEntry";


    public static String PLANT_SCAN_API = BASE_ROUTE_API + "plantsearchimageapi";

    public static String PLANT_SUGGESTION_API = BASE_ROUTE_API + "AccountAPI/AddPlant";

    public static String ACCOUNT_API = BASE_ROUTE_API + "AccountAPI/";

    public static String USER_LOGIN = BASE_ROUTE_API + "AccountAPI/login/true";

    public static String DELETE_ACCOUNT = ACCOUNT_API + "delete/";

    //Weather URL
    public static String Forecast_URL = BASE_ROUTE_API + "WeatherAPI/";

    public static String PLANT_SEARCH = BASE_ROUTE_API + "plantsearchapi/";

    public static String PLANT_SEARCH_TOTAL_COUNT = BASE_ROUTE_API + "plantsearchapi/count";


    public static String PLANT_FAMILY = PLANT_SEARCH + "families/";
    public static String USER_LIST_API = BASE_ROUTE_API + "UserListAPI/";

    public static String USER_PLANT_BY_ID = BASE_ROUTE_API + "UserListAPI/plantlists/";

    public static String USER_GARDEN_API = BASE_ROUTE_API + "gardenapi/";

    public static String USER_GARDEN_DETAILS = BASE_ROUTE_API + "gardenapi/details";

    public static String USER_PLANT_API = BASE_ROUTE_API + "userplantsapi/";

    public static String USER_PLANT_PROP_ADD = BASE_ROUTE_API + "userplantsapi/prop";

    public static String USER_PLANT_POST_TRIGGER = BASE_ROUTE_API + "userplantsapi/userPlantToUserListSingle";

    public static String NEWSLETTER_API = BASE_ROUTE_API + "newsletterapi/";

    public static String PLANT_SEARCH_API = BASE_ROUTE_API + "plantsearchapi/";

    public static String TODO_API = BASE_ROUTE_API + "todoesAPI/";

    public static String DIARY_API = BASE_ROUTE_API + "diaryapi/";

    public static String TODO_COUNT_API = TODO_API + "count";

    public static String DEVICE_API = BASE_ROUTE_API + "deviceapi/";
    public static String UPDATE_DEVICE_COUNT_API = DEVICE_API + "updateCount";

    public static String WARNING_API = BASE_ROUTE_API + "WarningAPI/";
    public static String UPDATE_WARNING_API = USER_PLANT_API + "updateUserPlantNotification/";

    public static String VIDEO_API = BASE_ROUTE_API + "VideosAPI";

    public static String NEWS_API = BASE_ROUTE_API + "newsentriesapi/";

    public static String INSTA_NEWS_API = NEWS_API + "getInstaPost/";

    public static String isAndroid() {
        return String.format("?isAndroid=%s", true);
    }

}
