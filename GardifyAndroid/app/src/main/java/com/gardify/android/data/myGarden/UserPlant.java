
package com.gardify.android.data.myGarden;

import java.util.List;

import com.google.gson.annotations.Expose;
import com.google.gson.annotations.SerializedName;

public class UserPlant {

    @SerializedName("$id")
    @Expose
    private String $id;
    @SerializedName("Id")
    @Expose
    private Integer id;
    @SerializedName("PlantId")
    @Expose
    private Integer plantId;
    @SerializedName("Description")
    @Expose
    private String description;
    @SerializedName("Plant")
    @Expose
    private Object plant;
    @SerializedName("Count")
    @Expose
    private Integer count;
    @SerializedName("Gardenid")
    @Expose
    private Integer gardenid;
    @SerializedName("Age")
    @Expose
    private String age;
    @SerializedName("Name")
    @Expose
    private String name;
    @SerializedName("NameLatin")
    @Expose
    private String nameLatin;
    @SerializedName("CustomName")
    @Expose
    private String customName;
    @SerializedName("Synonym")
    @Expose
    private String synonym;
    @SerializedName("IsInPot")
    @Expose
    private Boolean isInPot;
    @SerializedName("Notes")
    @Expose
    private String notes;
    @SerializedName("DatePlanted")
    @Expose
    private String datePlanted;
    @SerializedName("PlantTag")
    @Expose
    private Object plantTag;
    @SerializedName("Badges")
    @Expose
    private List<Badge> badges = null;
    @SerializedName("Images")
    @Expose
    private List<Image> images = null;
    @SerializedName("Todos")
    @Expose
    private List<Todo> todos;
    @SerializedName("CyclicTodos")
    @Expose
    private List<CyclicTodo> cyclicTodos = null;
    @SerializedName("TodosOld")
    @Expose
    private Object todosOld;
    @SerializedName("FaqList")
    @Expose
    private Object faqList;
    @SerializedName("VideoReferenceList")
    @Expose
    private Object videoReferenceList;
    @SerializedName("Articles")
    @Expose
    private Object articles;
    @SerializedName("NotifyForFrost")
    @Expose
    private Boolean notifyForFrost;
    @SerializedName("NotifyForWind")
    @Expose
    private Boolean notifyForWind;
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

    public Integer getPlantId() {
        return plantId;
    }

    public void setPlantId(Integer plantId) {
        this.plantId = plantId;
    }

    public String getDescription() {
        return description;
    }

    public void setDescription(String description) {
        this.description = description;
    }

    public Object getPlant() {
        return plant;
    }

    public void setPlant(Object plant) {
        this.plant = plant;
    }

    public Integer getCount() {
        return count;
    }

    public void setCount(Integer count) {
        this.count = count;
    }

    public Integer getGardenid() {
        return gardenid;
    }

    public void setGardenid(Integer gardenid) {
        this.gardenid = gardenid;
    }

    public String getAge() {
        return age;
    }

    public void setAge(String age) {
        this.age = age;
    }

    public String getName() {
        return name;
    }

    public void setName(String name) {
        this.name = name;
    }

    public String getNameLatin() {
        return nameLatin;
    }

    public void setNameLatin(String nameLatin) {
        this.nameLatin = nameLatin;
    }

    public String getCustomName() {
        return customName;
    }

    public void setCustomName(String customName) {
        this.customName = customName;
    }

    public String getSynonym() {
        return synonym;
    }

    public void setSynonym(String synonym) {
        this.synonym = synonym;
    }

    public Boolean getIsInPot() {
        return isInPot;
    }

    public void setIsInPot(Boolean isInPot) {
        this.isInPot = isInPot;
    }

    public String getNotes() {
        return notes;
    }

    public void setNotes(String notes) {
        this.notes = notes;
    }

    public String getDatePlanted() {
        return datePlanted;
    }

    public void setDatePlanted(String datePlanted) {
        this.datePlanted = datePlanted;
    }

    public Object getPlantTag() {
        return plantTag;
    }

    public void setPlantTag(Object plantTag) {
        this.plantTag = plantTag;
    }

    public List<Badge> getBadges() {
        return badges;
    }

    public void setBadges(List<Badge> badges) {
        this.badges = badges;
    }

    public List<Image> getImages() {
        return images;
    }

    public void setImages(List<Image> images) {
        this.images = images;
    }

    public List<Todo> getTodos() {
        return todos;
    }

    public void setTodos(List<Todo> todos) {
        this.todos = todos;
    }

    public List<CyclicTodo> getCyclicTodos() {
        return cyclicTodos;
    }

    public void setCyclicTodos(List<CyclicTodo> cyclicTodos) {
        this.cyclicTodos = cyclicTodos;
    }

    public Object getTodosOld() {
        return todosOld;
    }

    public void setTodosOld(Object todosOld) {
        this.todosOld = todosOld;
    }

    public Object getFaqList() {
        return faqList;
    }

    public void setFaqList(Object faqList) {
        this.faqList = faqList;
    }

    public Object getVideoReferenceList() {
        return videoReferenceList;
    }

    public void setVideoReferenceList(Object videoReferenceList) {
        this.videoReferenceList = videoReferenceList;
    }

    public Object getArticles() {
        return articles;
    }

    public void setArticles(Object articles) {
        this.articles = articles;
    }

    public Boolean getNotifyForFrost() {
        return notifyForFrost;
    }

    public void setNotifyForFrost(Boolean notifyForFrost) {
        this.notifyForFrost = notifyForFrost;
    }

    public Boolean getNotifyForWind() {
        return notifyForWind;
    }

    public void setNotifyForWind(Boolean notifyForWind) {
        this.notifyForWind = notifyForWind;
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

    public class CyclicTodo {

        @SerializedName("$id")
        @Expose
        private String $id;
        @SerializedName("Id")
        @Expose
        private Integer id;
        @SerializedName("Description")
        @Expose
        private String description;
        @SerializedName("DateStart")
        @Expose
        private String dateStart;
        @SerializedName("DateEnd")
        @Expose
        private String dateEnd;
        @SerializedName("Title")
        @Expose
        private String title;
        @SerializedName("Ignored")
        @Expose
        private Boolean ignored;
        @SerializedName("Finished")
        @Expose
        private Boolean finished;

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

        public String getTitle() {
            return title;
        }

        public void setTitle(String title) {
            this.title = title;
        }

        public Boolean getIgnored() {
            return ignored;
        }

        public void setIgnored(Boolean ignored) {
            this.ignored = ignored;
        }

        public Boolean getFinished() { return finished; }

        public void setFinished(Boolean finished) { this.finished = finished; }
    }

    public class Todo {

        @SerializedName("$id")
        @Expose
        private String $id;
        @SerializedName("UserId")
        @Expose
        private String userId;
        @SerializedName("ReferenceId")
        @Expose
        private Integer referenceId;
        @SerializedName("Notification")
        @Expose
        private Integer notification;
        @SerializedName("ReferenceType")
        @Expose
        private Integer referenceType;
        @SerializedName("Precision")
        @Expose
        private Integer precision;
        @SerializedName("Ignored")
        @Expose
        private Boolean ignored;
        @SerializedName("Finished")
        @Expose
        private Boolean finished;
        @SerializedName("Notes")
        @Expose
        private Object notes;
        @SerializedName("Index")
        @Expose
        private Integer index;
        @SerializedName("CyclicId")
        @Expose
        private Integer cyclicId;
        @SerializedName("Description")
        @Expose
        private String description;
        @SerializedName("DateStart")
        @Expose
        private String dateStart;
        @SerializedName("DateEnd")
        @Expose
        private String dateEnd;
        @SerializedName("Title")
        @Expose
        private String title;
        @SerializedName("PremiumOnly")
        @Expose
        private Boolean premiumOnly;
        @SerializedName("RelatedTodoTemplateId")
        @Expose
        private Integer relatedTodoTemplateId;
        @SerializedName("Id")
        @Expose
        private Integer id;
        @SerializedName("CreatedBy")
        @Expose
        private String createdBy;
        @SerializedName("CreatedDate")
        @Expose
        private String createdDate;
        @SerializedName("EditedBy")
        @Expose
        private String editedBy;
        @SerializedName("EditedDate")
        @Expose
        private String editedDate;
        @SerializedName("Deleted")
        @Expose
        private Boolean deleted;

        public String get$id() {
            return $id;
        }

        public void set$id(String $id) {
            this.$id = $id;
        }

        public String getUserId() {
            return userId;
        }

        public void setUserId(String userId) {
            this.userId = userId;
        }

        public Integer getReferenceId() {
            return referenceId;
        }

        public void setReferenceId(Integer referenceId) {
            this.referenceId = referenceId;
        }

        public Integer getNotification() {
            return notification;
        }

        public void setNotification(Integer notification) {
            this.notification = notification;
        }

        public Integer getReferenceType() {
            return referenceType;
        }

        public void setReferenceType(Integer referenceType) {
            this.referenceType = referenceType;
        }

        public Integer getPrecision() {
            return precision;
        }

        public void setPrecision(Integer precision) {
            this.precision = precision;
        }

        public Boolean getIgnored() {
            return ignored;
        }

        public void setIgnored(Boolean ignored) {
            this.ignored = ignored;
        }

        public Boolean getFinished() {
            return finished;
        }

        public void setFinished(Boolean finished) {
            this.finished = finished;
        }

        public Object getNotes() {
            return notes;
        }

        public void setNotes(Object notes) {
            this.notes = notes;
        }

        public Integer getIndex() {
            return index;
        }

        public void setIndex(Integer index) {
            this.index = index;
        }

        public Integer getCyclicId() {
            return cyclicId;
        }

        public void setCyclicId(Integer cyclicId) {
            this.cyclicId = cyclicId;
        }

        public String getDescription() {
            return description;
        }

        public void setDescription(String description) {
            this.description = description;
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

        public String getTitle() {
            return title;
        }

        public void setTitle(String title) {
            this.title = title;
        }

        public Boolean getPremiumOnly() {
            return premiumOnly;
        }

        public void setPremiumOnly(Boolean premiumOnly) {
            this.premiumOnly = premiumOnly;
        }

        public Integer getRelatedTodoTemplateId() {
            return relatedTodoTemplateId;
        }

        public void setRelatedTodoTemplateId(Integer relatedTodoTemplateId) {
            this.relatedTodoTemplateId = relatedTodoTemplateId;
        }

        public Integer getId() {
            return id;
        }

        public void setId(Integer id) {
            this.id = id;
        }

        public String getCreatedBy() {
            return createdBy;
        }

        public void setCreatedBy(String createdBy) {
            this.createdBy = createdBy;
        }

        public String getCreatedDate() {
            return createdDate;
        }

        public void setCreatedDate(String createdDate) {
            this.createdDate = createdDate;
        }

        public String getEditedBy() {
            return editedBy;
        }

        public void setEditedBy(String editedBy) {
            this.editedBy = editedBy;
        }

        public String getEditedDate() {
            return editedDate;
        }

        public void setEditedDate(String editedDate) {
            this.editedDate = editedDate;
        }

        public Boolean getDeleted() {
            return deleted;
        }

        public void setDeleted(Boolean deleted) {
            this.deleted = deleted;
        }

    }
}