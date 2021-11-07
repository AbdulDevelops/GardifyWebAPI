package com.gardify.android.data.news;
import java.util.List;
import com.google.gson.annotations.Expose;
import com.google.gson.annotations.SerializedName;

public class News {

    @SerializedName("$id")
    @Expose
    private String $id;
    @SerializedName("ListEntries")
    @Expose
    private List<ListEntry> listEntries = null;
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

    public List<ListEntry> getListEntries() {
        return listEntries;
    }

    public void setListEntries(List<ListEntry> listEntries) {
        this.listEntries = listEntries;
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



    public class ListEntry {

        @SerializedName("$id")
        @Expose
        private String $id;
        @SerializedName("SubTitle")
        @Expose
        private Object subTitle;
        @SerializedName("Theme")
        @Expose
        private String theme;
        @SerializedName("Author")
        @Expose
        private String author;
        @SerializedName("Timing")
        @Expose
        private String timing;
        @SerializedName("Tipp")
        @Expose
        private Object tipp;
        @SerializedName("Id")
        @Expose
        private Integer id;
        @SerializedName("Title")
        @Expose
        private String title;
        @SerializedName("Text")
        @Expose
        private String text;
        @SerializedName("Date")
        @Expose
        private String date;
        @SerializedName("ValidFrom")
        @Expose
        private String validFrom;
        @SerializedName("ValidTo")
        @Expose
        private String validTo;
        @SerializedName("IsVisibleOnPage")
        @Expose
        private Boolean isVisibleOnPage;
        @SerializedName("EntryImages")
        @Expose
        private List<EntryImage> entryImages = null;
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

        public Object getSubTitle() {
            return subTitle;
        }

        public void setSubTitle(Object subTitle) {
            this.subTitle = subTitle;
        }

        public String getTheme() {
            return theme;
        }

        public void setTheme(String theme) {
            this.theme = theme;
        }

        public String getAuthor() {
            return author;
        }

        public void setAuthor(String author) {
            this.author = author;
        }

        public String getTiming() {
            return timing;
        }

        public void setTiming(String timing) {
            this.timing = timing;
        }

        public Object getTipp() {
            return tipp;
        }

        public void setTipp(Object tipp) {
            this.tipp = tipp;
        }

        public Integer getId() {
            return id;
        }

        public void setId(Integer id) {
            this.id = id;
        }

        public String getTitle() {
            return title;
        }

        public void setTitle(String title) {
            this.title = title;
        }

        public String getText() {
            return text;
        }

        public void setText(String text) {
            this.text = text;
        }

        public String getDate() {
            return date;
        }

        public void setDate(String date) {
            this.date = date;
        }

        public String getValidFrom() {
            return validFrom;
        }

        public void setValidFrom(String validFrom) {
            this.validFrom = validFrom;
        }

        public String getValidTo() {
            return validTo;
        }

        public void setValidTo(String validTo) {
            this.validTo = validTo;
        }

        public Boolean getIsVisibleOnPage() {
            return isVisibleOnPage;
        }

        public void setIsVisibleOnPage(Boolean isVisibleOnPage) {
            this.isVisibleOnPage = isVisibleOnPage;
        }

        public List<EntryImage> getEntryImages() {
            return entryImages;
        }

        public void setEntryImages(List<EntryImage> entryImages) {
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

    public class EntryImage {

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

    }
}
