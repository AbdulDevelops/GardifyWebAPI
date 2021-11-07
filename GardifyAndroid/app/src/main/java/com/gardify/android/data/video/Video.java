package com.gardify.android.data.video;


import java.util.List;
import com.google.gson.annotations.Expose;
import com.google.gson.annotations.SerializedName;

public class Video {

    @SerializedName("$id")
    @Expose
    private String $id;
    @SerializedName("Id")
    @Expose
    private Integer id;
    @SerializedName("YTLink")
    @Expose
    private String yTLink;
    @SerializedName("Title")
    @Expose
    private String title;
    @SerializedName("SubTitle")
    @Expose
    private String subTitle;
    @SerializedName("Text")
    @Expose
    private String text;
    @SerializedName("ViewCount")
    @Expose
    private Integer viewCount;
    @SerializedName("Duration")
    @Expose
    private String duration;
    @SerializedName("Tags")
    @Expose
    private List<String> tags = null;
    @SerializedName("Date")
    @Expose
    private String date;

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

    public String getYTLink() {
        return yTLink;
    }

    public void setYTLink(String yTLink) {
        this.yTLink = yTLink;
    }

    public String getTitle() {
        return title;
    }

    public void setTitle(String title) {
        this.title = title;
    }

    public String getSubTitle() {
        return subTitle;
    }

    public void setSubTitle(String subTitle) {
        this.subTitle = subTitle;
    }

    public String getText() {
        return text;
    }

    public void setText(String text) {
        this.text = text;
    }

    public Integer getViewCount() {
        return viewCount;
    }

    public void setViewCount(Integer viewCount) {
        this.viewCount = viewCount;
    }

    public String getDuration() {
        return duration;
    }

    public void setDuration(String duration) {
        this.duration = duration;
    }

    public List<String> getTags() {
        return tags;
    }

    public void setTags(List<String> tags) {
        this.tags = tags;
    }

    public String getDate() {
        return date;
    }

    public void setDate(String date) {
        this.date = date;
    }

}