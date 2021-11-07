package com.gardify.android.data.todos;

import com.google.gson.annotations.Expose;
import com.google.gson.annotations.SerializedName;

import java.util.List;

public class TodoList {

    @SerializedName("$id")
    @Expose
    private String $id;
    @SerializedName("Id")
    @Expose
    private Integer id;
    @SerializedName("Description")
    @Expose
    private String description;
    @SerializedName("Title")
    @Expose
    private String title;
    @SerializedName("Notification")
    @Expose
    private Integer notification;
    @SerializedName("Notes")
    @Expose
    private String notes;
    @SerializedName("DateStart")
    @Expose
    private String dateStart;
    @SerializedName("DateEnd")
    @Expose
    private String dateEnd;
    @SerializedName("ReferenceId")
    @Expose
    private Integer referenceId;
    @SerializedName("ReferenceType")
    @Expose
    private Integer referenceType;
    @SerializedName("ReferenceText")
    @Expose
    private String referenceText;
    @SerializedName("Finished")
    @Expose
    private Boolean finished;
    @SerializedName("Ignored")
    @Expose
    private Boolean ignored;
    @SerializedName("Deleted")
    @Expose
    private Boolean deleted;
    @SerializedName("CyclicId")
    @Expose
    private Integer cyclicId;
    @SerializedName("UserId")
    @Expose
    private String userId;
    @SerializedName("VideoReferenceList")
    @Expose
    private List<Object> videoReferenceList = null;
    @SerializedName("Articles")
    @Expose
    private Object articles;
    @SerializedName("EntryImages")
    @Expose
    private List<EntryImage> entryImages = null;
    @SerializedName("RelatedPlantId")
    @Expose
    private Integer relatedPlantId;
    @SerializedName("RelatedPlantName")
    @Expose
    private String relatedPlantName;
    @SerializedName("ShopcartCounter")
    @Expose
    private Integer shopcartCounter;
    @SerializedName("PlantCount")
    @Expose
    private Integer plantCount;
    @SerializedName("Points")
    @Expose
    private Integer points;
    @SerializedName("CurrentTodoCount")
    @Expose
    private Integer currentTodoCount;
    @SerializedName("NewMessages")
    @Expose
    private Integer newMessages;

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

    public String getDescription() {
        return description;
    }

    public void setDescription(String description) {
        this.description = description;
    }

    public String getTitle() {
        return title;
    }

    public void setTitle(String title) {
        this.title = title;
    }

    public Integer getNotification() {
        return notification;
    }

    public void setNotification(Integer notification) {
        this.notification = notification;
    }

    public String getNotes() {
        return notes;
    }

    public void setNotes(String notes) {
        this.notes = notes;
    }

    public String getDateStart() {
        return dateStart;
    }

    public void setDateStart(String dateStart) {
        this.dateStart = dateStart;
    }

    public String getDateEnd() {
        return dateEnd;
    }

    public void setDateEnd(String dateEnd) {
        this.dateEnd = dateEnd;
    }

    public Integer getReferenceId() {
        return referenceId;
    }

    public void setReferenceId(Integer referenceId) {
        this.referenceId = referenceId;
    }

    public Integer getReferenceType() {
        return referenceType;
    }

    public void setReferenceType(Integer referenceType) {
        this.referenceType = referenceType;
    }

    public String getReferenceText() {
        return referenceText;
    }

    public void setReferenceText(String referenceText) {
        this.referenceText = referenceText;
    }

    public Boolean getFinished() {
        return finished;
    }

    public void setFinished(Boolean finished) {
        this.finished = finished;
    }

    public Boolean getIgnored() {
        return ignored;
    }

    public void setIgnored(Boolean ignored) {
        this.ignored = ignored;
    }

    public Boolean getDeleted() {
        return deleted;
    }

    public void setDeleted(Boolean deleted) {
        this.deleted = deleted;
    }

    public Integer getCyclicId() {
        return cyclicId;
    }

    public void setCyclicId(Integer cyclicId) {
        this.cyclicId = cyclicId;
    }

    public String getUserId() {
        return userId;
    }

    public void setUserId(String userId) {
        this.userId = userId;
    }

    public List<Object> getVideoReferenceList() {
        return videoReferenceList;
    }

    public void setVideoReferenceList(List<Object> videoReferenceList) {
        this.videoReferenceList = videoReferenceList;
    }

    public Object getArticles() {
        return articles;
    }

    public void setArticles(Object articles) {
        this.articles = articles;
    }

    public List<EntryImage> getEntryImages() {
        return entryImages;
    }

    public void setEntryImages(List<EntryImage> entryImages) {
        this.entryImages = entryImages;
    }

    public Integer getRelatedPlantId() {
        return relatedPlantId;
    }

    public void setRelatedPlantId(Integer relatedPlantId) {
        this.relatedPlantId = relatedPlantId;
    }

    public String getRelatedPlantName() {
        return relatedPlantName;
    }

    public void setRelatedPlantName(String relatedPlantName) {
        this.relatedPlantName = relatedPlantName;
    }

    public Integer getShopcartCounter() {
        return shopcartCounter;
    }

    public void setShopcartCounter(Integer shopcartCounter) {
        this.shopcartCounter = shopcartCounter;
    }

    public Integer getPlantCount() {
        return plantCount;
    }

    public void setPlantCount(Integer plantCount) {
        this.plantCount = plantCount;
    }

    public Integer getPoints() {
        return points;
    }

    public void setPoints(Integer points) {
        this.points = points;
    }

    public Integer getCurrentTodoCount() {
        return currentTodoCount;
    }

    public void setCurrentTodoCount(Integer currentTodoCount) {
        this.currentTodoCount = currentTodoCount;
    }

    public Integer getNewMessages() {
        return newMessages;
    }

    public void setNewMessages(Integer newMessages) {
        this.newMessages = newMessages;
    }

}
