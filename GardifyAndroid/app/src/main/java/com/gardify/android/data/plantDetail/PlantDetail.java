package com.gardify.android.data.plantDetail;

import com.google.gson.annotations.SerializedName;

import java.util.List;

import com.google.gson.annotations.Expose;

public class PlantDetail {

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
    private String description;
    @SerializedName("IsInUserGarden")
    @Expose
    private Boolean isInUserGarden;
    @SerializedName("GenusTaxon")
    @Expose
    private GenusTaxon genusTaxon;
    @SerializedName("PlantTags")
    @Expose
    private List<PlantTag> plantTags = null;
    @SerializedName("PlantCharacteristics")
    @Expose
    private List<PlantCharacteristic> plantCharacteristics = null;
    @SerializedName("Images")
    @Expose
    private List<Image> images = null;
    @SerializedName("Articles")
    @Expose
    private List<Object> articles = null;
    @SerializedName("TodoTemplates")
    @Expose
    private List<TodoTemplate> todoTemplates = null;
    @SerializedName("Synonym")
    @Expose
    private String synonym;
    @SerializedName("Family")
    @Expose
    private String family;
    @SerializedName("Herkunft")
    @Expose
    private String herkunft;
    @SerializedName("Colors")
    @Expose
    private List<String> colors = null;
    @SerializedName("PlantGroups")
    @Expose
    private List<PlantGroup> plantGroups = null;
    @SerializedName("GardenCategory")
    @Expose
    private Object gardenCategory;

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

    public String getDescription() {
        return description;
    }

    public void setDescription(String description) {
        this.description = description;
    }

    public Boolean getIsInUserGarden() {
        return isInUserGarden;
    }

    public void setIsInUserGarden(Boolean isInUserGarden) {
        this.isInUserGarden = isInUserGarden;
    }

    public GenusTaxon getGenusTaxon() {
        return genusTaxon;
    }

    public void setGenusTaxon(GenusTaxon genusTaxon) {
        this.genusTaxon = genusTaxon;
    }

    public List<PlantTag> getPlantTags() {
        return plantTags;
    }

    public void setPlantTags(List<PlantTag> plantTags) {
        this.plantTags = plantTags;
    }

    public List<PlantCharacteristic> getPlantCharacteristics() {
        return plantCharacteristics;
    }

    public void setPlantCharacteristics(List<PlantCharacteristic> plantCharacteristics) {
        this.plantCharacteristics = plantCharacteristics;
    }

    public List<Image> getImages() {
        return images;
    }

    public void setImages(List<Image> images) {
        this.images = images;
    }

    public List<Object> getArticles() {
        return articles;
    }

    public void setArticles(List<Object> articles) {
        this.articles = articles;
    }

    public List<TodoTemplate> getTodoTemplates() {
        return todoTemplates;
    }

    public void setTodoTemplates(List<TodoTemplate> todoTemplates) {
        this.todoTemplates = todoTemplates;
    }

    public String getSynonym() {
        return synonym;
    }

    public void setSynonym(String synonym) {
        this.synonym = synonym;
    }

    public String getFamily() {
        return family;
    }

    public void setFamily(String family) {
        this.family = family;
    }

    public String getHerkunft() {
        return herkunft;
    }

    public void setHerkunft(String herkunft) {
        this.herkunft = herkunft;
    }

    public List<String> getColors() {
        return colors;
    }

    public void setColors(List<String> colors) {
        this.colors = colors;
    }

    public List<PlantGroup> getPlantGroups() {
        return plantGroups;
    }

    public void setPlantGroups(List<PlantGroup> plantGroups) {
        this.plantGroups = plantGroups;
    }

    public Object getGardenCategory() {
        return gardenCategory;
    }

    public void setGardenCategory(Object gardenCategory) {
        this.gardenCategory = gardenCategory;
    }

    public class PlantTag {

        @SerializedName("$id")
        @Expose
        private String $id;
        @SerializedName("Category")
        @Expose
        private Category category;
        @SerializedName("SuperCategories")
        @Expose
        private List<Object> superCategories = null;
        @SerializedName("CategoryId")
        @Expose
        private Integer categoryId;
        @SerializedName("Title")
        @Expose
        private String title;
        @SerializedName("TagImage")
        @Expose
        private Object tagImage;
        @SerializedName("Selected")
        @Expose
        private Boolean selected;
        @SerializedName("Count")
        @Expose
        private Integer count;
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

        public Category getCategory() {
            return category;
        }

        public void setCategory(Category category) {
            this.category = category;
        }

        public List<Object> getSuperCategories() {
            return superCategories;
        }

        public void setSuperCategories(List<Object> superCategories) {
            this.superCategories = superCategories;
        }

        public Integer getCategoryId() {
            return categoryId;
        }

        public void setCategoryId(Integer categoryId) {
            this.categoryId = categoryId;
        }

        public String getTitle() {
            return title;
        }

        public void setTitle(String title) {
            this.title = title;
        }

        public Object getTagImage() {
            return tagImage;
        }

        public void setTagImage(Object tagImage) {
            this.tagImage = tagImage;
        }

        public Boolean getSelected() {
            return selected;
        }

        public void setSelected(Boolean selected) {
            this.selected = selected;
        }

        public Integer getCount() {
            return count;
        }

        public void setCount(Integer count) {
            this.count = count;
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

    public class PlantCharacteristic {

        @SerializedName("$id")
        @Expose
        private String $id;
        @SerializedName("Id")
        @Expose
        private Integer id;
        @SerializedName("CategoryId")
        @Expose
        private Integer categoryId;
        @SerializedName("Min")
        @Expose
        private Double min;
        @SerializedName("Max")
        @Expose
        private Double max;
        @SerializedName("PlantId")
        @Expose
        private Integer plantId;
        @SerializedName("Category")
        @Expose
        private Category_ category;
        @SerializedName("CategoryTitle")
        @Expose
        private String categoryTitle;
        @SerializedName("PlantTagCategoryId")
        @Expose
        private Integer plantTagCategoryId;

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

        public Integer getCategoryId() {
            return categoryId;
        }

        public void setCategoryId(Integer categoryId) {
            this.categoryId = categoryId;
        }

        public Double getMin() {
            return min;
        }

        public void setMin(Double min) {
            this.min = min;
        }

        public Double getMax() {
            return max;
        }

        public void setMax(Double max) {
            this.max = max;
        }

        public Integer getPlantId() {
            return plantId;
        }

        public void setPlantId(Integer plantId) {
            this.plantId = plantId;
        }

        public Category_ getCategory() {
            return category;
        }

        public void setCategory(Category_ category) {
            this.category = category;
        }

        public String getCategoryTitle() {
            return categoryTitle;
        }

        public void setCategoryTitle(String categoryTitle) {
            this.categoryTitle = categoryTitle;
        }

        public Integer getPlantTagCategoryId() {
            return plantTagCategoryId;
        }

        public void setPlantTagCategoryId(Integer plantTagCategoryId) {
            this.plantTagCategoryId = plantTagCategoryId;
        }

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

    public class TodoTemplate {

        @SerializedName("$id")
        @Expose
        private String $id;
        @SerializedName("ArticleReferences")
        @Expose
        private List<Object> articleReferences = null;
        @SerializedName("SuperCategories")
        @Expose
        private List<Object> superCategories = null;
        @SerializedName("ReferenceId")
        @Expose
        private Integer referenceId;
        @SerializedName("ReferenceType")
        @Expose
        private Integer referenceType;
        @SerializedName("Precision")
        @Expose
        private Integer precision;
        @SerializedName("Cycle")
        @Expose
        private Integer cycle;
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
        @SerializedName("TaxonomicTreeId")
        @Expose
        private Object taxonomicTreeId;
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

        public List<Object> getArticleReferences() {
            return articleReferences;
        }

        public void setArticleReferences(List<Object> articleReferences) {
            this.articleReferences = articleReferences;
        }

        public List<Object> getSuperCategories() {
            return superCategories;
        }

        public void setSuperCategories(List<Object> superCategories) {
            this.superCategories = superCategories;
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

        public Integer getPrecision() {
            return precision;
        }

        public void setPrecision(Integer precision) {
            this.precision = precision;
        }

        public Integer getCycle() {
            return cycle;
        }

        public void setCycle(Integer cycle) {
            this.cycle = cycle;
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

        public Object getTaxonomicTreeId() {
            return taxonomicTreeId;
        }

        public void setTaxonomicTreeId(Object taxonomicTreeId) {
            this.taxonomicTreeId = taxonomicTreeId;
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

    public class PlantGroup {

        @SerializedName("$id")
        @Expose
        private String $id;
        @SerializedName("Name")
        @Expose
        private String name;
        @SerializedName("Id")
        @Expose
        private Integer id;

        public String get$id() {
            return $id;
        }

        public void set$id(String $id) {
            this.$id = $id;
        }

        public String getName() {
            return name;
        }

        public void setName(String name) {
            this.name = name;
        }

        public Integer getId() {
            return id;
        }

        public void setId(Integer id) {
            this.id = id;
        }

    }

    public class Category_ {

        @SerializedName("$id")
        @Expose
        private String $id;
        @SerializedName("Parent")
        @Expose
        private Object parent;
        @SerializedName("SuperCategories")
        @Expose
        private List<Object> superCategories = null;
        @SerializedName("ParentId")
        @Expose
        private Object parentId;
        @SerializedName("Title")
        @Expose
        private String title;
        @SerializedName("Color")
        @Expose
        private String color;
        @SerializedName("Count")
        @Expose
        private Integer count;
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
        @SerializedName("$ref")
        @Expose
        private String $ref;

        public String get$id() {
            return $id;
        }

        public void set$id(String $id) {
            this.$id = $id;
        }

        public Object getParent() {
            return parent;
        }

        public void setParent(Object parent) {
            this.parent = parent;
        }

        public List<Object> getSuperCategories() {
            return superCategories;
        }

        public void setSuperCategories(List<Object> superCategories) {
            this.superCategories = superCategories;
        }

        public Object getParentId() {
            return parentId;
        }

        public void setParentId(Object parentId) {
            this.parentId = parentId;
        }

        public String getTitle() {
            return title;
        }

        public void setTitle(String title) {
            this.title = title;
        }

        public String getColor() {
            return color;
        }

        public void setColor(String color) {
            this.color = color;
        }

        public Integer getCount() {
            return count;
        }

        public void setCount(Integer count) {
            this.count = count;
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

        public String get$ref() {
            return $ref;
        }

        public void set$ref(String $ref) {
            this.$ref = $ref;
        }

    }

}
