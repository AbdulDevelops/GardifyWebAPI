package com.gardify.android.data.plantSearchModel;

import com.google.gson.annotations.Expose;
import com.google.gson.annotations.SerializedName;

import java.util.List;

public class Plant {
    @Expose
    @SerializedName("Badges")
    private List<Badge> Badge;
    @Expose
    @SerializedName("Synonym")
    private String Synonym;
    @Expose
    @SerializedName("Colors")
    private List<String> Colors;
    @Expose
    @SerializedName("Images")
    private List<Image> Image;
    @Expose
    @SerializedName("IsInUserGarden")
    private boolean IsInUserGarden;
    @Expose
    @SerializedName("Description")
    private String Description;
    @Expose
    @SerializedName("NameGerman")
    private String NameGerman;
    @Expose
    @SerializedName("NameLatin")
    private String NameLatin;
    @Expose
    @SerializedName("Id")
    private int Id;
    @Expose
    @SerializedName("$id")
    private String $id;

    public List<Badge> getBadge() {
        return Badge;
    }

    public void setBadge(List<Badge> Badge) {
        this.Badge = Badge;
    }

    public String getSynonym() {
        return Synonym;
    }

    public void setSynonym(String Synonym) {
        this.Synonym = Synonym;
    }

    public List<String> getColors() {
        return Colors;
    }

    public void setColors(List<String> Colors) {
        this.Colors = Colors;
    }

    public List<Image> getImage() {
        return Image;
    }

    public void setImage(List<Image> Image) {
        this.Image = Image;
    }

    public boolean getIsInUserGarden() {
        return IsInUserGarden;
    }

    public void setIsInUserGarden(boolean IsInUserGarden) {
        this.IsInUserGarden = IsInUserGarden;
    }

    public String getDescription() {
        return Description;
    }

    public void setDescription(String Description) {
        this.Description = Description;
    }

    public String getNameGerman() {
        return NameGerman;
    }

    public void setNameGerman(String NameGerman) {
        this.NameGerman = NameGerman;
    }

    public String getNameLatin() {
        return NameLatin;
    }

    public void setNameLatin(String NameLatin) {
        this.NameLatin = NameLatin;
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

    /**
     * Spinner string value
     */
    @Override  // Mandatory
    public String toString() {
        return NameGerman; // Return anything, what you want to show in spinner
    }

}
