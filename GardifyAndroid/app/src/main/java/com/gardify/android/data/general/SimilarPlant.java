package com.gardify.android.data.general;

import java.util.List;
import com.google.gson.annotations.Expose;
import com.google.gson.annotations.SerializedName;

public class SimilarPlant {

    @SerializedName("$id")
    @Expose
    private String $id;
    @SerializedName("Id")
    @Expose
    private Integer id;
    @SerializedName("NameLatin")
    @Expose
    private String nameLatin;
    @SerializedName("NameGerman")
    @Expose
    private String nameGerman;
    @SerializedName("Description")
    @Expose
    private Object description;
    @SerializedName("IsInUserGarden")
    @Expose
    private Boolean isInUserGarden;
    @SerializedName("GenusTaxon")
    @Expose
    private Object genusTaxon;
    @SerializedName("PlantTags")
    @Expose
    private Object plantTags;
    @SerializedName("PlantCharacteristics")
    @Expose
    private Object plantCharacteristics;
    @SerializedName("PlantCharacteristicsOld")
    @Expose
    private Object plantCharacteristicsOld;
    @SerializedName("Images")
    @Expose
    private List<Image> images = null;
    @SerializedName("StatusMessage")
    @Expose
    private Object statusMessage;
    @SerializedName("Articles")
    @Expose
    private Object articles;
    @SerializedName("Published")
    @Expose
    private Boolean published;
    @SerializedName("Comments")
    @Expose
    private Object comments;
    @SerializedName("TodoTemplates")
    @Expose
    private Object todoTemplates;
    @SerializedName("Synonym")
    @Expose
    private Object synonym;
    @SerializedName("Family")
    @Expose
    private Object family;
    @SerializedName("Links")
    @Expose
    private Object links;
    @SerializedName("Score")
    @Expose
    private Double score;
    @SerializedName("Herkunft")
    @Expose
    private Object herkunft;
    @SerializedName("GardenCategory")
    @Expose
    private Object gardenCategory;
    @SerializedName("PlantGroups")
    @Expose
    private Object plantGroups;
    @SerializedName("PlantGroupsOld")
    @Expose
    private Object plantGroupsOld;
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

    public String getNameLatin() {
        return nameLatin;
    }

    public void setNameLatin(String nameLatin) {
        this.nameLatin = nameLatin;
    }

    public String getNameGerman() {
        return nameGerman;
    }

    public void setNameGerman(String nameGerman) {
        this.nameGerman = nameGerman;
    }

    public Object getDescription() {
        return description;
    }

    public void setDescription(Object description) {
        this.description = description;
    }

    public Boolean getIsInUserGarden() {
        return isInUserGarden;
    }

    public void setIsInUserGarden(Boolean isInUserGarden) {
        this.isInUserGarden = isInUserGarden;
    }

    public Object getGenusTaxon() {
        return genusTaxon;
    }

    public void setGenusTaxon(Object genusTaxon) {
        this.genusTaxon = genusTaxon;
    }

    public Object getPlantTags() {
        return plantTags;
    }

    public void setPlantTags(Object plantTags) {
        this.plantTags = plantTags;
    }

    public Object getPlantCharacteristics() {
        return plantCharacteristics;
    }

    public void setPlantCharacteristics(Object plantCharacteristics) {
        this.plantCharacteristics = plantCharacteristics;
    }

    public Object getPlantCharacteristicsOld() {
        return plantCharacteristicsOld;
    }

    public void setPlantCharacteristicsOld(Object plantCharacteristicsOld) {
        this.plantCharacteristicsOld = plantCharacteristicsOld;
    }

    public List<Image> getImages() {
        return images;
    }

    public void setImages(List<Image> images) {
        this.images = images;
    }

    public Object getStatusMessage() {
        return statusMessage;
    }

    public void setStatusMessage(Object statusMessage) {
        this.statusMessage = statusMessage;
    }

    public Object getArticles() {
        return articles;
    }

    public void setArticles(Object articles) {
        this.articles = articles;
    }

    public Boolean getPublished() {
        return published;
    }

    public void setPublished(Boolean published) {
        this.published = published;
    }

    public Object getComments() {
        return comments;
    }

    public void setComments(Object comments) {
        this.comments = comments;
    }

    public Object getTodoTemplates() {
        return todoTemplates;
    }

    public void setTodoTemplates(Object todoTemplates) {
        this.todoTemplates = todoTemplates;
    }

    public Object getSynonym() {
        return synonym;
    }

    public void setSynonym(Object synonym) {
        this.synonym = synonym;
    }

    public Object getFamily() {
        return family;
    }

    public void setFamily(Object family) {
        this.family = family;
    }

    public Object getLinks() {
        return links;
    }

    public void setLinks(Object links) {
        this.links = links;
    }

    public Double getScore() {
        return score;
    }

    public void setScore(Double score) {
        this.score = score;
    }

    public Object getHerkunft() {
        return herkunft;
    }

    public void setHerkunft(Object herkunft) {
        this.herkunft = herkunft;
    }

    public Object getGardenCategory() {
        return gardenCategory;
    }

    public void setGardenCategory(Object gardenCategory) {
        this.gardenCategory = gardenCategory;
    }

    public Object getPlantGroups() {
        return plantGroups;
    }

    public void setPlantGroups(Object plantGroups) {
        this.plantGroups = plantGroups;
    }

    public Object getPlantGroupsOld() {
        return plantGroupsOld;
    }

    public void setPlantGroupsOld(Object plantGroupsOld) {
        this.plantGroupsOld = plantGroupsOld;
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

    public class Image {

        @SerializedName("$id")
        @Expose
        private String $id;
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
        @SerializedName("Tags")
        @Expose
        private String tags;

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

        public String getTags() {
            return tags;
        }

        public void setTags(String tags) {
            this.tags = tags;
        }

    }
}