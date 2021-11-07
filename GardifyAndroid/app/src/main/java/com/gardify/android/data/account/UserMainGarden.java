package com.gardify.android.data.account;

import com.google.gson.annotations.Expose;
import com.google.gson.annotations.SerializedName;

import java.util.List;

public class UserMainGarden {

    @SerializedName("$id")
    @Expose
    private String $id;
    @SerializedName("Id")
    @Expose
    private Integer id;
    @SerializedName("Name")
    @Expose
    private String name;
    @SerializedName("GroundType")
    @Expose
    private Integer groundType;
    @SerializedName("PhType")
    @Expose
    private Integer phType;
    @SerializedName("Wetness")
    @Expose
    private Integer wetness;
    @SerializedName("Description")
    @Expose
    private String description;
    @SerializedName("CardinalDirection")
    @Expose
    private Integer cardinalDirection;
    @SerializedName("ShadowStrength")
    @Expose
    private Integer shadowStrength;
    @SerializedName("Inside")
    @Expose
    private Boolean inside;
    @SerializedName("MainImageId")
    @Expose
    private Integer mainImageId;
    @SerializedName("Temperature")
    @Expose
    private Integer temperature;
    @SerializedName("Light")
    @Expose
    private Integer light;
    @SerializedName("IsPrivate")
    @Expose
    private Boolean isPrivate;
    @SerializedName("Images")
    @Expose
    private List<Image> images = null;
    @SerializedName("PlantsLight")
    @Expose
    private Object plantsLight;
    @SerializedName("Plants")
    @Expose
    private Object plants;
    @SerializedName("TodoList")
    @Expose
    private Object todoList;
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

    public String getName() {
        return name;
    }

    public void setName(String name) {
        this.name = name;
    }

    public Integer getGroundType() {
        return groundType;
    }

    public void setGroundType(Integer groundType) {
        this.groundType = groundType;
    }

    public Integer getPhType() {
        return phType;
    }

    public void setPhType(Integer phType) {
        this.phType = phType;
    }

    public Integer getWetness() {
        return wetness;
    }

    public void setWetness(Integer wetness) {
        this.wetness = wetness;
    }

    public String getDescription() {
        return description;
    }

    public void setDescription(String description) {
        this.description = description;
    }

    public Integer getCardinalDirection() {
        return cardinalDirection;
    }

    public void setCardinalDirection(Integer cardinalDirection) {
        this.cardinalDirection = cardinalDirection;
    }

    public Integer getShadowStrength() {
        return shadowStrength;
    }

    public void setShadowStrength(Integer shadowStrength) {
        this.shadowStrength = shadowStrength;
    }

    public Boolean getInside() {
        return inside;
    }

    public void setInside(Boolean inside) {
        this.inside = inside;
    }

    public Integer getMainImageId() {
        return mainImageId;
    }

    public void setMainImageId(Integer mainImageId) {
        this.mainImageId = mainImageId;
    }

    public Integer getTemperature() {
        return temperature;
    }

    public void setTemperature(Integer temperature) {
        this.temperature = temperature;
    }

    public Integer getLight() {
        return light;
    }

    public void setLight(Integer light) {
        this.light = light;
    }

    public Boolean getIsPrivate() {
        return isPrivate;
    }

    public void setIsPrivate(Boolean isPrivate) {
        this.isPrivate = isPrivate;
    }

    public List<Image> getImages() {
        return images;
    }

    public void setImages(List<Image> images) {
        this.images = images;
    }

    public Object getPlantsLight() {
        return plantsLight;
    }

    public void setPlantsLight(Object plantsLight) {
        this.plantsLight = plantsLight;
    }

    public Object getPlants() {
        return plants;
    }

    public void setPlants(Object plants) {
        this.plants = plants;
    }

    public Object getTodoList() {
        return todoList;
    }

    public void setTodoList(Object todoList) {
        this.todoList = todoList;
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


    // Image class

    public class Image {

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

}
