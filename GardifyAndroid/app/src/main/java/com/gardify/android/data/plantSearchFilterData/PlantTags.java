package com.gardify.android.data.plantSearchFilterData;

import com.google.gson.annotations.Expose;
import com.google.gson.annotations.SerializedName;

public class PlantTags {

    @Expose
    @SerializedName("Title")
    private String Title;
    @Expose
    @SerializedName("CategoryId")
    private int CategoryId;
    @Expose
    @SerializedName("Id")
    private int Id;
    @Expose
    @SerializedName("$id")
    private String $id;

    public String getTitle() {
        return Title;
    }

    public void setTitle(String Title) {
        this.Title = Title;
    }

    public int getCategoryId() {
        return CategoryId;
    }

    public void setCategoryId(int CategoryId) {
        this.CategoryId = CategoryId;
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






    /**
     * * * * * * * * * * * * * * * * * * * * * * *
     * following data types are for view model only
     * * * * * * * * * * * * * * * * * * * * * * *
    **/


    private boolean checked = false;

    public boolean isChecked() {
        return checked;
    }

    public void setChecked(boolean checked) {
        this.checked = checked;
    }
}
