package com.gardify.android.data.plantSearchFilterData;

import com.google.gson.annotations.Expose;
import com.google.gson.annotations.SerializedName;

import java.util.List;

public class PlantGroup {

    @SerializedName("$id")
    @Expose
    private String $id;
    @SerializedName("Groups")
    @Expose
    private List<Group> groups = null;
    @SerializedName("GardenGroups")
    @Expose
    private List<GardenGroup> gardenGroups = null;

    public String get$id() {
        return $id;
    }

    public void set$id(String $id) {
        this.$id = $id;
    }

    public List<Group> getGroups() {
        return groups;
    }

    public void setGroups(List<Group> groups) {
        this.groups = groups;
    }

    public List<GardenGroup> getGardenGroups() {
        return gardenGroups;
    }

    public void setGardenGroups(List<GardenGroup> gardenGroups) {
        this.gardenGroups = gardenGroups;
    }


    public class Group {

        @SerializedName("$id")
        @Expose
        private String $id;
        @SerializedName("Name")
        @Expose
        private String name;
        @SerializedName("PlantsWithThisGroupd")
        @Expose
        private Object plantsWithThisGroupd;
        @SerializedName("SuperCategories")
        @Expose
        private List<Object> superCategories = null;
        @SerializedName("Id")
        @Expose
        private Integer id;
        @SerializedName("CreatedBy")
        @Expose
        private Object createdBy;
        @SerializedName("CreatedDate")
        @Expose
        private String createdDate;
        @SerializedName("EditedBy")
        @Expose
        private Object editedBy;
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

        public String getName() {
            return name;
        }

        public void setName(String name) {
            this.name = name;
        }

        public Object getPlantsWithThisGroupd() {
            return plantsWithThisGroupd;
        }

        public void setPlantsWithThisGroupd(Object plantsWithThisGroupd) {
            this.plantsWithThisGroupd = plantsWithThisGroupd;
        }

        public List<Object> getSuperCategories() {
            return superCategories;
        }

        public void setSuperCategories(List<Object> superCategories) {
            this.superCategories = superCategories;
        }

        public Integer getId() {
            return id;
        }

        public void setId(Integer id) {
            this.id = id;
        }

        public Object getCreatedBy() {
            return createdBy;
        }

        public void setCreatedBy(Object createdBy) {
            this.createdBy = createdBy;
        }

        public String getCreatedDate() {
            return createdDate;
        }

        public void setCreatedDate(String createdDate) {
            this.createdDate = createdDate;
        }

        public Object getEditedBy() {
            return editedBy;
        }

        public void setEditedBy(Object editedBy) {
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

        /**
         * * * * * * * * * * * * * * * * * * * * * * *
         * following data types are for view model only
         * * * * * * * * * * * * * * * * * * * * * * *
         **/

        private boolean checked = false;

        public boolean isChecked() {
            return checked;
        }

        public void setChecked(boolean checked) {
            this.checked = checked;
        }

    }

    public class GardenGroup {

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


}
