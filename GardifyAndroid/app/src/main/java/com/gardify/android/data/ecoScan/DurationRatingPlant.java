package com.gardify.android.data.ecoScan;

import com.google.gson.annotations.Expose;
import com.google.gson.annotations.SerializedName;

public class DurationRatingPlant {

    @SerializedName("$id")
    @Expose
    private String $id;
    @SerializedName("Month")
    @Expose
    private Integer month;
    @SerializedName("PlantCount")
    @Expose
    private Integer plantCount;

    public String get$id() {
        return $id;
    }

    public void set$id(String $id) {
        this.$id = $id;
    }

    public Integer getMonth() {
        return month;
    }

    public void setMonth(Integer month) {
        this.month = month;
    }

    public Integer getPlantCount() {
        return plantCount;
    }

    public void setPlantCount(Integer plantCount) {
        this.plantCount = plantCount;
    }
}
