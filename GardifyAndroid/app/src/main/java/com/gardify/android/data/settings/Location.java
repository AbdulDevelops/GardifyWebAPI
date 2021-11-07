package com.gardify.android.data.settings;

import com.google.gson.annotations.Expose;
import com.google.gson.annotations.SerializedName;

public class Location {

    @Expose
    @SerializedName("NewMessages")
    private int NewMessages;
    @Expose
    @SerializedName("CurrentTodoCount")
    private int CurrentTodoCount;
    @Expose
    @SerializedName("Points")
    private int Points;
    @Expose
    @SerializedName("PlantCount")
    private int PlantCount;
    @Expose
    @SerializedName("ShopcartCounter")
    private int ShopcartCounter;
    @Expose
    @SerializedName("Country")
    private String Country;
    @Expose
    @SerializedName("City")
    private String City;
    @Expose
    @SerializedName("Zip")
    private String Zip;
    @Expose
    @SerializedName("Street")
    private String Street;
    @Expose
    @SerializedName("$id")
    private String $id;

    public int getNewMessages() {
        return NewMessages;
    }

    public void setNewMessages(int NewMessages) {
        this.NewMessages = NewMessages;
    }

    public int getCurrentTodoCount() {
        return CurrentTodoCount;
    }

    public void setCurrentTodoCount(int CurrentTodoCount) {
        this.CurrentTodoCount = CurrentTodoCount;
    }

    public int getPoints() {
        return Points;
    }

    public void setPoints(int Points) {
        this.Points = Points;
    }

    public int getPlantCount() {
        return PlantCount;
    }

    public void setPlantCount(int PlantCount) {
        this.PlantCount = PlantCount;
    }

    public int getShopcartCounter() {
        return ShopcartCounter;
    }

    public void setShopcartCounter(int ShopcartCounter) {
        this.ShopcartCounter = ShopcartCounter;
    }

    public String getCountry() {
        return Country;
    }

    public void setCountry(String Country) {
        this.Country = Country;
    }

    public String getCity() {
        return City;
    }

    public void setCity(String City) {
        this.City = City;
    }

    public String getZip() {
        return Zip;
    }

    public void setZip(String Zip) {
        this.Zip = Zip;
    }

    public String getStreet() {
        return Street;
    }

    public void setStreet(String Street) {
        this.Street = Street;
    }

    public String get$id() {
        return $id;
    }

    public void set$id(String $id) {
        this.$id = $id;
    }
}
