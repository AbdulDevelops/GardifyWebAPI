package com.gardify.android.data.todos;


import com.google.gson.annotations.Expose;
import com.google.gson.annotations.SerializedName;

import java.util.List;

public class EcoScan {

    @SerializedName("$id")
    @Expose
    private String $id;
    @SerializedName("ObjectId")
    @Expose
    private Integer objectId;
    @SerializedName("DiaryType")
    @Expose
    private Integer diaryType;
    @SerializedName("ListEntries")
    @Expose
    private List<ListEntry> listEntries = null;
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

    public Integer getObjectId() {
        return objectId;
    }

    public void setObjectId(Integer objectId) {
        this.objectId = objectId;
    }

    public Integer getDiaryType() {
        return diaryType;
    }

    public void setDiaryType(Integer diaryType) {
        this.diaryType = diaryType;
    }

    public List<ListEntry> getListEntries() {
        return listEntries;
    }

    public void setListEntries(List<ListEntry> listEntries) {
        this.listEntries = listEntries;
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

    public class ListEntry {

        @SerializedName("$id")
        @Expose
        private String $id;
        @SerializedName("Id")
        @Expose
        private Integer id;
        @SerializedName("ObjectId")
        @Expose
        private Integer objectId;
        @SerializedName("DiaryType")
        @Expose
        private Integer diaryType;
        @SerializedName("Title")
        @Expose
        private String title;
        @SerializedName("Description")
        @Expose
        private String description;
        @SerializedName("Date")
        @Expose
        private String date;
        @SerializedName("EntryImages")
        @Expose
        private List<Object> entryImages = null;
        @SerializedName("StatusMessage")
        @Expose
        private Object statusMessage;
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

        public Integer getObjectId() {
            return objectId;
        }

        public void setObjectId(Integer objectId) {
            this.objectId = objectId;
        }

        public Integer getDiaryType() {
            return diaryType;
        }

        public void setDiaryType(Integer diaryType) {
            this.diaryType = diaryType;
        }

        public String getTitle() {
            return title;
        }

        public void setTitle(String title) {
            this.title = title;
        }

        public String getDescription() {
            return description;
        }

        public void setDescription(String description) {
            this.description = description;
        }

        public String getDate() {
            return date;
        }

        public void setDate(String date) {
            this.date = date;
        }

        public List<Object> getEntryImages() {
            return entryImages;
        }

        public void setEntryImages(List<Object> entryImages) {
            this.entryImages = entryImages;
        }

        public Object getStatusMessage() {
            return statusMessage;
        }

        public void setStatusMessage(Object statusMessage) {
            this.statusMessage = statusMessage;
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
}