
package com.gardify.android.data.plantScan;

import java.util.List;
import com.google.gson.annotations.Expose;
import com.google.gson.annotations.SerializedName;

public class Species_ {

    @SerializedName("commonNames")
    @Expose
    private List<String> commonNames = null;
    @SerializedName("scientificNameWithoutAuthor")
    @Expose
    private String scientificNameWithoutAuthor;
    @SerializedName("family")
    @Expose
    private Family_ family;
    @SerializedName("genus")
    @Expose
    private Genus_ genus;

    public List<String> getCommonNames() {
        return commonNames;
    }

    public void setCommonNames(List<String> commonNames) {
        this.commonNames = commonNames;
    }

    public String getScientificNameWithoutAuthor() {
        return scientificNameWithoutAuthor;
    }

    public void setScientificNameWithoutAuthor(String scientificNameWithoutAuthor) {
        this.scientificNameWithoutAuthor = scientificNameWithoutAuthor;
    }

    public Family_ getFamily() {
        return family;
    }

    public void setFamily(Family_ family) {
        this.family = family;
    }

    public Genus_ getGenus() {
        return genus;
    }

    public void setGenus(Genus_ genus) {
        this.genus = genus;
    }

}
