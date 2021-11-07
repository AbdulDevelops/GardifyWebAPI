
package com.gardify.android.data.myGarden.UserDevice;

import com.google.gson.annotations.Expose;
import com.google.gson.annotations.SerializedName;

public class EntryImage {

    @SerializedName("$id")
    @Expose
    private String $id;
    @SerializedName("Id")
    @Expose
    private Integer id;
    @SerializedName("Author")
    @Expose
    private Object author;
    @SerializedName("License")
    @Expose
    private Object license;
    @SerializedName("FullTitle")
    @Expose
    private String fullTitle;
    @SerializedName("FullDescription")
    @Expose
    private String fullDescription;
    @SerializedName("TitleAttr")
    @Expose
    private String titleAttr;
    @SerializedName("AltAttr")
    @Expose
    private String altAttr;
    @SerializedName("SrcAttr")
    @Expose
    private String srcAttr;
    @SerializedName("Sort")
    @Expose
    private Integer sort;
    @SerializedName("InsertDate")
    @Expose
    private String insertDate;

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

    public Object getAuthor() {
        return author;
    }

    public void setAuthor(Object author) {
        this.author = author;
    }

    public Object getLicense() {
        return license;
    }

    public void setLicense(Object license) {
        this.license = license;
    }

    public String getFullTitle() {
        return fullTitle;
    }

    public void setFullTitle(String fullTitle) {
        this.fullTitle = fullTitle;
    }

    public String getFullDescription() {
        return fullDescription;
    }

    public void setFullDescription(String fullDescription) {
        this.fullDescription = fullDescription;
    }

    public String getTitleAttr() {
        return titleAttr;
    }

    public void setTitleAttr(String titleAttr) {
        this.titleAttr = titleAttr;
    }

    public String getAltAttr() {
        return altAttr;
    }

    public void setAltAttr(String altAttr) {
        this.altAttr = altAttr;
    }

    public String getSrcAttr() {
        return srcAttr;
    }

    public void setSrcAttr(String srcAttr) {
        this.srcAttr = srcAttr;
    }

    public Integer getSort() {
        return sort;
    }

    public void setSort(Integer sort) {
        this.sort = sort;
    }

    public String getInsertDate() {
        return insertDate;
    }

    public void setInsertDate(String insertDate) {
        this.insertDate = insertDate;
    }

}
