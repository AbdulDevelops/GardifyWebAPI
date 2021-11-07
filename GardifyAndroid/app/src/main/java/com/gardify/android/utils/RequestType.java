package com.gardify.android.utils;

/**
 * Enums for all the different request types. Used in RequestData as a means to identify specific
 * requests within network response functions.
 */
public enum RequestType {
    //Base type
    Unspecified,

    //Gardify Api PflanzenSuche
    PflanzenSucheModel,
    PflanzenDocModel,
    PflanzenDocViewModelAnswer,
    PlantDetail,
    SimilarPlant,
    Cats,
    PlantTags,
    UserPlants,
    PlantGroup,
    PlantFamily,
    MyGarden,
    UserInfo,
    Settings,
    Location,
    PlantList,
    TodayWeather,
    DailyWeather,
    UserDevice,
    AdminDevice,
    EcoElement,
    UserGarden,
    Todo,
    TodoList,
    Diary,
    EcoScan,
    DurationRatingPlant,
    TodoCount,
    PlantCount,
    Warning,
    Video,
    News,
    GardenKnowledge,
    InstaNews,
}
