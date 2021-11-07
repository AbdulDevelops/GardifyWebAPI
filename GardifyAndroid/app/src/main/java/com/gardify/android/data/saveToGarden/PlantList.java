package com.gardify.android.data.saveToGarden;

import com.google.gson.annotations.Expose;
import com.google.gson.annotations.SerializedName;

public class PlantList {


    @Expose
    @SerializedName("ListSelected")
    private boolean ListSelected;
    @Expose
    @SerializedName("GardenId")
    private int GardenId;
    @Expose
    @SerializedName("Description")
    private String Description;
    @Expose
    @SerializedName("Name")
    private String Name;
    @Expose
    @SerializedName("Id")
    private int Id;
    @Expose
    @SerializedName("$id")
    private String $id;

    public boolean getListSelected() {
        return ListSelected;
    }

    public void setListSelected(boolean ListSelected) {
        this.ListSelected = ListSelected;
    }

    public int getGardenId() {
        return GardenId;
    }

    public void setGardenId(int GardenId) {
        this.GardenId = GardenId;
    }

    public String getDescription() {
        return Description;
    }

    public void setDescription(String Description) {
        this.Description = Description;
    }

    public String getName() {
        return Name;
    }

    public void setName(String Name) {
        this.Name = Name;
    }

    public int getId() {
        return Id;
    }

    public void setId(int Id) {
        this.Id = Id;
    }

    public String get$id() {
        return $id;
    }

    public void set$id(String $id) {
        this.$id = $id;
    }
}
