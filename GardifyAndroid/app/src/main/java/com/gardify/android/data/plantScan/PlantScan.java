
package com.gardify.android.data.plantScan;

import com.google.gson.annotations.Expose;
import com.google.gson.annotations.SerializedName;

public class PlantScan {

    @SerializedName("GPlants")
    @Expose
    private Object gPlants;
    @SerializedName("GImages")
    @Expose
    private Object gImages;
    @SerializedName("PnResults")
    @Expose
    private PnResults pnResults;

    public Object getGPlants() {
        return gPlants;
    }

    public void setGPlants(Object gPlants) {
        this.gPlants = gPlants;
    }

    public Object getGImages() {
        return gImages;
    }

    public void setGImages(Object gImages) {
        this.gImages = gImages;
    }

    public PnResults getPnResults() {
        return pnResults;
    }

    public void setPnResults(PnResults pnResults) {
        this.pnResults = pnResults;
    }

}
