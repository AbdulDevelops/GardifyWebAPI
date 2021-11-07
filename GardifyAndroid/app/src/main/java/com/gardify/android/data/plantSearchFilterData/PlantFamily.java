package com.gardify.android.data.plantSearchFilterData;

import com.google.gson.annotations.Expose;
import com.google.gson.annotations.SerializedName;

import java.util.List;

public class PlantFamily {


    @Expose
    @SerializedName("JSONArray")
    private List<String> plantFamilyList;

    public List<String> getPlantFamilyList() {
        return plantFamilyList;
    }

    public void setPlantFamilyList(List<String> plantFamilyList) {
        this.plantFamilyList = plantFamilyList;
    }
}
