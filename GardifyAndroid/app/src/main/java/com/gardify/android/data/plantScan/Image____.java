
package com.gardify.android.data.plantScan;

import java.util.List;
import com.google.gson.annotations.Expose;
import com.google.gson.annotations.SerializedName;

public class Image____ {

    @SerializedName("Id")
    @Expose
    private Integer id;
    @SerializedName("Author")
    @Expose
    private String author;
    @SerializedName("License")
    @Expose
    private String license;
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
    @SerializedName("TakenDate")
    @Expose
    private Object takenDate;
    @SerializedName("Comments")
    @Expose
    private String comments;
    @SerializedName("Tags")
    @Expose
    private String tags;
    @SerializedName("Note")
    @Expose
    private Object note;
    @SerializedName("Rating")
    @Expose
    private Double rating;
    @SerializedName("Albums")
    @Expose
    private List<Object> albums = null;

    public Integer getId() {
        return id;
    }

    public void setId(Integer id) {
        this.id = id;
    }

    public String getAuthor() {
        return author;
    }

    public void setAuthor(String author) {
        this.author = author;
    }

    public String getLicense() {
        return license;
    }

    public void setLicense(String license) {
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

    public Object getTakenDate() {
        return takenDate;
    }

    public void setTakenDate(Object takenDate) {
        this.takenDate = takenDate;
    }

    public String getComments() {
        return comments;
    }

    public void setComments(String comments) {
        this.comments = comments;
    }

    public String getTags() {
        return tags;
    }

    public void setTags(String tags) {
        this.tags = tags;
    }

    public Object getNote() {
        return note;
    }

    public void setNote(Object note) {
        this.note = note;
    }

    public Double getRating() {
        return rating;
    }

    public void setRating(Double rating) {
        this.rating = rating;
    }

    public List<Object> getAlbums() {
        return albums;
    }

    public void setAlbums(List<Object> albums) {
        this.albums = albums;
    }

}
