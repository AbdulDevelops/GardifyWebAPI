
package com.gardify.android.data.myGarden.Eco;

import java.util.List;
import com.google.gson.annotations.Expose;
import com.google.gson.annotations.SerializedName;

public class EcoElement {

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
    @SerializedName("Checked")
    @Expose
    private Boolean checked;
    @SerializedName("EcoElementsImages")
    @Expose
    private List<EcoElementsImage> ecoElementsImages = null;

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

    public Boolean getChecked() {
        return checked;
    }

    public void setChecked(Boolean checked) {
        this.checked = checked;
    }

    public List<EcoElementsImage> getEcoElementsImages() {
        return ecoElementsImages;
    }

    public void setEcoElementsImages(List<EcoElementsImage> ecoElementsImages) {
        this.ecoElementsImages = ecoElementsImages;
    }

}
