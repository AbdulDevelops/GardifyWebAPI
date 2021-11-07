
package com.gardify.android.data.plantScan;

import java.util.List;
import com.google.gson.annotations.Expose;
import com.google.gson.annotations.SerializedName;

public class DoubleResult {

    @SerializedName("score")
    @Expose
    private Double score;
    @SerializedName("species")
    @Expose
    private Species_ species;
    @SerializedName("images")
    @Expose
    private List<Image__> images = null;

    public Double getScore() {
        return score;
    }

    public void setScore(Double score) {
        this.score = score;
    }

    public Species_ getSpecies() {
        return species;
    }

    public void setSpecies(Species_ species) {
        this.species = species;
    }

    public List<Image__> getImages() {
        return images;
    }

    public void setImages(List<Image__> images) {
        this.images = images;
    }

}
