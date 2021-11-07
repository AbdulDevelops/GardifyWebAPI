//
//  Links.swift
//  GardifyText
//
//  Created by Netzlab on 27.07.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import Foundation

public struct APP_URL{
    //Base URL
    public static let BASE_ROUTE = "https://gardifybackend.sslbeta.de/"
    
    public static let BASE_ROUTE_INTERN = "https://gardify.de/intern/"
    
    public static let BASE_ROUTE_API = BASE_ROUTE + "api/"
    
    public static let PLANT_DOC_ROUTE = BASE_ROUTE_API + "PlantDocAPI"
    
    public static let PLANT_DOC_ANSWER = PLANT_DOC_ROUTE + "/answer"
    
    public static let PLANT_DOC_ALL_ENTRIES = PLANT_DOC_ROUTE + "/getAllEntry"
    public static let PLANT_DOC_MY_ENTRIES = PLANT_DOC_ROUTE + "/getCurrentUserPosts"
    public static let PLANT_DOC_ASK_NEW_ENTRIES = PLANT_DOC_ROUTE + "/newEntry"
    public static let PLANT_DOC_GET_NOT_READ = PLANT_DOC_ROUTE + "/notread"
    public static let PLANT_DOC_UPLOAD_IMAGE = PLANT_DOC_ROUTE + "/upload/"
    
    public static let DIARY_ROUTE = BASE_ROUTE_API + "/diaryapi"
    
    
    public static let PLANT_SCAN_API = BASE_ROUTE_API + "plantsearchimageapi"
    public static let RATING_TOTAL_ECO = BASE_ROUTE_API + "gardenapi/ratingTotalEcoEl"
    public static let LOCATION_UPDATE = BASE_ROUTE_API + "gardenapi/location"
    
    //User URL
    public static let USER_LOGIN = BASE_ROUTE_API + "AccountAPI/login/true"
    public static let PROFILE_PHOTO_UPLOAD = BASE_ROUTE_API + "AccountAPI/uploadProfilImg"
    public static let ACCOUNT_USER_INFO = BASE_ROUTE_API + "AccountAPI/userinfo/"
    public static let ACCOUNT_USER_SETTING = BASE_ROUTE_API + "AccountAPI/settings/"
    public static let ACCOUNT_USER_IMAGES = BASE_ROUTE_API + "AccountAPI/profilImg/"
    public static let ACCOUNT_SEND_ECOSCAN_MAIL = BASE_ROUTE_API + "AccountAPI/sendScanMail"
    public static let ACCOUNT_USER_SETTING_UPDATE = BASE_ROUTE_API + "AccountAPI/updatesettings/"
    public static let ACCOUNT_USER_PASSWORD_UPDATE = BASE_ROUTE_API + "AccountAPI/update/pass/"
    public static let ACCOUNT_USER_EMAIL_UPDATE = BASE_ROUTE_API + "AccountAPI/update/"
    public static let ACCOUNT_USER_DATA_UPDATE = BASE_ROUTE_API + "AccountAPI/data"
    public static let ACCOUNT_USER_DELETE = BASE_ROUTE_API + "AccountAPI/delete/"
    public static let ACCOUNT_CONTACT = BASE_ROUTE_API + "AccountAPI/contact"
    
    public static let RATING_PLANT = BASE_ROUTE_API + "userplantsapi/ratingPlant"
    public static let DURATION_RATING_PLANT = BASE_ROUTE_API + "userplantsapi/durationRatingPlant"
    public static let MOVE_PLANT = BASE_ROUTE_API + "userplantsapi/movePlant"
    
    //Weather URL
    public static let Forecast_URL = BASE_ROUTE_API + "WeatherAPI/"
    
    public static let PLANT_SEARCH_BY_ID = BASE_ROUTE_API + "plantsearchapi/"
    public static let PLANT_SIBLING_BY_ID = BASE_ROUTE_API + "plantsearchapi/findSiblingById/"
    
    public static let USER_LIST = BASE_ROUTE_API + "UserListAPI"
    
    public static let ADD_USER_GARDEN = USER_LIST + "/create"
    public static let USER_PLANT_BY_ID = USER_LIST + "/plantlists/"
    public static let USER_LIST_UPDATE = USER_LIST + "/updatelist"
    
    
    public static let USER_GARDEN_DETAILS = BASE_ROUTE_API + "gardenapi/details"
    public static let USER_GARDEN_MAIN = BASE_ROUTE_API + "gardenapi/main"
    public static let USER_GARDEN_ECO_ELEMENT_LIST = BASE_ROUTE_API + "gardenapi/ecoelements"
    public static let USER_UPDATE_ECO_ELEMENT = BASE_ROUTE_API + "gardenapi/updateEcoelements"
    
    public static let USER_UPDATE_ECO_ELEMENT_COUNT = BASE_ROUTE_API + "gardenapi/updateEcoelementCounts"
    
    public static let USER_TOTAL_PLANT = BASE_ROUTE_API + "gardenapi/count"
    
    public static let DEVICE_USER_LIST = BASE_ROUTE_API + "deviceapi/"
    public static let DEVICE_COUNT_UPDATE = DEVICE_USER_LIST + "updateCount"
    
    public static let DEVICE_ADMIN_LIST = DEVICE_USER_LIST + "AdminDevices"
    public static let DEVICE_ADMIN_ADD = DEVICE_USER_LIST + "postAdminDevice"
    
    public static let USER_TOTAL_TODOES = BASE_ROUTE_API + "todoesAPI/count"
    
    public static let TODO_IMAGE_UPLOAD = BASE_ROUTE_API + "todoesAPI/upload"
    
    public static let USER_WARNING_COUNT = BASE_ROUTE_API + "WarningAPI/warnings"
    
    public static let USER_PLANT_PROP_ADD = BASE_ROUTE_API + "userplantsapi/prop"
    
    public static let USER_PLANT_POST_TRIGGER = BASE_ROUTE_API + "userplantsapi/userPlantToUserListSingle"
    public static let MY_GARDEN_USER_PLANTS = BASE_ROUTE_API + "userplantsapi/"
    public static let MY_GARDEN_MOVE_ALL_GARDEN = MY_GARDEN_USER_PLANTS + "moveAllPlants"
    
    public static let PLANT_SEARCH_API = BASE_ROUTE_API + "plantsearchapi"
    public static let PLANT_SEARCH_COUNT = PLANT_SEARCH_API + "/count"
    
    public static let PLANT_SEARCH_GET_CATS = BASE_ROUTE_API + "planttagsapi/cats"
    public static let PLANT_SEARCH_GET_TAGS = BASE_ROUTE_API + "planttagsapi"
    public static let PLANT_SEARCH_GET_GROUPS = BASE_ROUTE_API + "groupapi"
    public static let PLANT_SEARCH_GET_FAMILIES = BASE_ROUTE_API + "plantsearchapi/families"
    
    public static let TO_DO_LIST = BASE_ROUTE_API + "todoesAPI/"
    public static let TO_DO_CYCLIC_DETAIL = TO_DO_LIST + "cyclic/"
    
    public static let GARDENING_AZ_LIST = BASE_ROUTE_API + "lexiconapi/"
    
    
    public static let NEWSLETTER_BASE = BASE_ROUTE_API + "newsletterapi"
    
    public static let VIDEO_LIST = BASE_ROUTE_API + "VideosAPI/"
    
    public static let NEWS_LIST = BASE_ROUTE_API + "newsentriesapi/"
    public static let NEWS_INSTA_LIST = NEWS_LIST + "getInstaPost"
    
    public static let UPDATE_PLANT_WARNING_NOTIFICATION = BASE_ROUTE_API + "userplantsapi/updateUserPlantNotification/"
    
    public static let UPDATE_DEVICE_WARNING_NOTIFICATION = BASE_ROUTE_API + "DeviceAPI/flipNotification/"
    
    public static let UPLOAD_PLANT_SUGGEST_IMAGES = BASE_ROUTE_API + "AccountAPI/suggest/"
    
    public static let USER_REGISTER_NEW_USER = BASE_ROUTE_API + "AccountAPI/register/true/"
}
