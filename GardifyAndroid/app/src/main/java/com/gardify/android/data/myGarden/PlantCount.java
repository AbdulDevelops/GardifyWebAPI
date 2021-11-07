package com.gardify.android.data.myGarden;

import com.google.gson.annotations.Expose;
import com.google.gson.annotations.SerializedName;

public class PlantCount {

    @SerializedName("$id")
    @Expose
    private String $id;
    @SerializedName("Sorts")
    @Expose
    private Integer sorts;
    @SerializedName("Total")
    @Expose
    private Integer total;

    public String get$id() {
        return $id;
    }

    public void set$id(String $id) {
        this.$id = $id;
    }

    public Integer getSorts() {
        return sorts;
    }

    public void setSorts(Integer sorts) {
        this.sorts = sorts;
    }

    public Integer getTotal() {
        return total;
    }

    public void setTotal(Integer total) {
        this.total = total;
    }

}