package com.gardify.android.data.plantSearchModel;

import com.google.gson.annotations.Expose;
import com.google.gson.annotations.SerializedName;

import java.util.List;

public class PlantsSearchModel {
    @SerializedName("$id")
    @Expose
    private String $id;
    @SerializedName("CategoryId")
    @Expose
    private Integer categoryId;
    @SerializedName("SubCategoryId")
    @Expose
    private Integer subCategoryId;
    @SerializedName("SelectedPositiveFilterTagId")
    @Expose
    private Object selectedPositiveFilterTagId;
    @SerializedName("SelectedTagId")
    @Expose
    private Object selectedTagId;
    @SerializedName("Input_search")
    @Expose
    private Object inputSearch;
    @SerializedName("TaxonId")
    @Expose
    private Object taxonId;
    @SerializedName("SelHmin")
    @Expose
    private Integer selHmin;
    @SerializedName("SelHmax")
    @Expose
    private Integer selHmax;
    @SerializedName("SelMinMonth")
    @Expose
    private Object selMinMonth;
    @SerializedName("SelMaxMonth")
    @Expose
    private Object selMaxMonth;
    @SerializedName("Fm")
    @Expose
    private Object fm;
    @SerializedName("AppliedFilters")
    @Expose
    private List<Object> appliedFilters = null;
    @SerializedName("TreeRoot")
    @Expose
    private Object treeRoot;
    @SerializedName("CategoryList")
    @Expose
    private List<Object> categoryList = null;
    @SerializedName("SubCategoryList")
    @Expose
    private List<Object> subCategoryList = null;
    @SerializedName("TagsList")
    @Expose
    private Object tagsList;
    @SerializedName("PositiveFilterTagsList")
    @Expose
    private Object positiveFilterTagsList;
    @SerializedName("SuperCategories")
    @Expose
    private List<Object> superCategories = null;
    @SerializedName("Plants")
    @Expose
    private List<Plant> plants = null;
    @SerializedName("PlantsOld")
    @Expose
    private Object plantsOld;
    @SerializedName("PlantList")
    @Expose
    private Object plantList;
    @SerializedName("MonthCheckboxes")
    @Expose
    private Object monthCheckboxes;
    @SerializedName("HeightMin")
    @Expose
    private Integer heightMin;
    @SerializedName("HeightMax")
    @Expose
    private Integer heightMax;
    @SerializedName("MinMonth")
    @Expose
    private Object minMonth;
    @SerializedName("MaxMonth")
    @Expose
    private Object maxMonth;
    @SerializedName("SearchQueries")
    @Expose
    private Object searchQueries;
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

    public Integer getCategoryId() {
        return categoryId;
    }

    public void setCategoryId(Integer categoryId) {
        this.categoryId = categoryId;
    }

    public Integer getSubCategoryId() {
        return subCategoryId;
    }

    public void setSubCategoryId(Integer subCategoryId) {
        this.subCategoryId = subCategoryId;
    }

    public Object getSelectedPositiveFilterTagId() {
        return selectedPositiveFilterTagId;
    }

    public void setSelectedPositiveFilterTagId(Object selectedPositiveFilterTagId) {
        this.selectedPositiveFilterTagId = selectedPositiveFilterTagId;
    }

    public Object getSelectedTagId() {
        return selectedTagId;
    }

    public void setSelectedTagId(Object selectedTagId) {
        this.selectedTagId = selectedTagId;
    }

    public Object getInputSearch() {
        return inputSearch;
    }

    public void setInputSearch(Object inputSearch) {
        this.inputSearch = inputSearch;
    }

    public Object getTaxonId() {
        return taxonId;
    }

    public void setTaxonId(Object taxonId) {
        this.taxonId = taxonId;
    }

    public Integer getSelHmin() {
        return selHmin;
    }

    public void setSelHmin(Integer selHmin) {
        this.selHmin = selHmin;
    }

    public Integer getSelHmax() {
        return selHmax;
    }

    public void setSelHmax(Integer selHmax) {
        this.selHmax = selHmax;
    }

    public Object getSelMinMonth() {
        return selMinMonth;
    }

    public void setSelMinMonth(Object selMinMonth) {
        this.selMinMonth = selMinMonth;
    }

    public Object getSelMaxMonth() {
        return selMaxMonth;
    }

    public void setSelMaxMonth(Object selMaxMonth) {
        this.selMaxMonth = selMaxMonth;
    }

    public Object getFm() {
        return fm;
    }

    public void setFm(Object fm) {
        this.fm = fm;
    }

    public List<Object> getAppliedFilters() {
        return appliedFilters;
    }

    public void setAppliedFilters(List<Object> appliedFilters) {
        this.appliedFilters = appliedFilters;
    }

    public Object getTreeRoot() {
        return treeRoot;
    }

    public void setTreeRoot(Object treeRoot) {
        this.treeRoot = treeRoot;
    }

    public List<Object> getCategoryList() {
        return categoryList;
    }

    public void setCategoryList(List<Object> categoryList) {
        this.categoryList = categoryList;
    }

    public List<Object> getSubCategoryList() {
        return subCategoryList;
    }

    public void setSubCategoryList(List<Object> subCategoryList) {
        this.subCategoryList = subCategoryList;
    }

    public Object getTagsList() {
        return tagsList;
    }

    public void setTagsList(Object tagsList) {
        this.tagsList = tagsList;
    }

    public Object getPositiveFilterTagsList() {
        return positiveFilterTagsList;
    }

    public void setPositiveFilterTagsList(Object positiveFilterTagsList) {
        this.positiveFilterTagsList = positiveFilterTagsList;
    }

    public List<Object> getSuperCategories() {
        return superCategories;
    }

    public void setSuperCategories(List<Object> superCategories) {
        this.superCategories = superCategories;
    }

    public List<Plant> getPlants() {
        return plants;
    }

    public void setPlants(List<Plant> plants) {
        this.plants = plants;
    }

    public Object getPlantsOld() {
        return plantsOld;
    }

    public void setPlantsOld(Object plantsOld) {
        this.plantsOld = plantsOld;
    }

    public Object getPlantList() {
        return plantList;
    }

    public void setPlantList(Object plantList) {
        this.plantList = plantList;
    }

    public Object getMonthCheckboxes() {
        return monthCheckboxes;
    }

    public void setMonthCheckboxes(Object monthCheckboxes) {
        this.monthCheckboxes = monthCheckboxes;
    }

    public Integer getHeightMin() {
        return heightMin;
    }

    public void setHeightMin(Integer heightMin) {
        this.heightMin = heightMin;
    }

    public Integer getHeightMax() {
        return heightMax;
    }

    public void setHeightMax(Integer heightMax) {
        this.heightMax = heightMax;
    }

    public Object getMinMonth() {
        return minMonth;
    }

    public void setMinMonth(Object minMonth) {
        this.minMonth = minMonth;
    }

    public Object getMaxMonth() {
        return maxMonth;
    }

    public void setMaxMonth(Object maxMonth) {
        this.maxMonth = maxMonth;
    }

    public Object getSearchQueries() {
        return searchQueries;
    }

    public void setSearchQueries(Object searchQueries) {
        this.searchQueries = searchQueries;
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
