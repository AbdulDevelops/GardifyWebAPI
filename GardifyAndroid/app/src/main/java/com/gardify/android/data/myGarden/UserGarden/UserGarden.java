package com.gardify.android.data.myGarden.UserGarden;

import com.google.gson.annotations.Expose;
import com.google.gson.annotations.SerializedName;

public class UserGarden {

    @SerializedName("$id")
    @Expose
    private String $id;
    @SerializedName("Id")
    @Expose
    private Integer id;
    @SerializedName("Name")
    @Expose
    private String name;
    @SerializedName("Description")
    @Expose
    private String description;
    @SerializedName("GardenId")
    @Expose
    private Integer gardenId;
    @SerializedName("ListSelected")
    @Expose
    private Boolean listSelected;


    public UserGarden() {
    }

    public UserGarden(Integer id, String name) {
        this.id = id;
        this.name = name;
    }

    public String get$id() {
        return $id;
    }

    public void set$id(String $id) {
        this.$id = $id;
    }

    public Integer getId() {
        return id;
    }

    public void setId(Integer id) {
        this.id = id;
    }

    public String getName() {
        return name;
    }

    public void setName(String name) {
        this.name = name;
    }

    public String getDescription() {
        return description;
    }

    public void setDescription(String description) {
        this.description = description;
    }

    public Integer getGardenId() {
        return gardenId;
    }

    public void setGardenId(Integer gardenId) {
        this.gardenId = gardenId;
    }

    public Boolean getListSelected() {
        return listSelected;
    }

    public void setListSelected(Boolean listSelected) {
        this.listSelected = listSelected;
    }

    /**
     * Spinner string value
     */
    @Override  // Mandatory
    public String toString() {
        return name; // Return anything, what you want to show in spinner
    }

}