
package com.gardify.android.data.myGarden.UserDevice;

import java.util.List;
import com.google.gson.annotations.Expose;
import com.google.gson.annotations.SerializedName;

public class UserDevice {

    @SerializedName("$id")
    @Expose
    private String $id;
    @SerializedName("Id")
    @Expose
    private Integer id;
    @SerializedName("Name")
    @Expose
    private String name;
    @SerializedName("isActive")
    @Expose
    private Boolean isActive;
    @SerializedName("notifyForWind")
    @Expose
    private Boolean notifyForWind;
    @SerializedName("notifyForFrost")
    @Expose
    private Boolean notifyForFrost;
    @SerializedName("Note")
    @Expose
    private String note;
    @SerializedName("Gardenid")
    @Expose
    private Integer gardenid;
    @SerializedName("AdminDevId")
    @Expose
    private Integer adminDevId;
    @SerializedName("CreatedBy")
    @Expose
    private String createdBy;
    @SerializedName("EditedBy")
    @Expose
    private String editedBy;
    @SerializedName("UserDevListId")
    @Expose
    private Integer userDevListId;
    @SerializedName("Date")
    @Expose
    private String date;
    @SerializedName("Count")
    @Expose
    private Integer count;
    @SerializedName("EntryImages")
    @Expose
    private List<EntryImage> entryImages = null;
    @SerializedName("Todos")
    @Expose
    private Object todos;

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

    public Boolean getIsActive() {
        return isActive;
    }

    public void setIsActive(Boolean isActive) {
        this.isActive = isActive;
    }

    public Boolean getNotifyForWind() {
        return notifyForWind;
    }

    public void setNotifyForWind(Boolean notifyForWind) {
        this.notifyForWind = notifyForWind;
    }

    public Boolean getNotifyForFrost() {
        return notifyForFrost;
    }

    public void setNotifyForFrost(Boolean notifyForFrost) {
        this.notifyForFrost = notifyForFrost;
    }

    public String getNote() {
        return note;
    }

    public void setNote(String note) {
        this.note = note;
    }

    public Integer getGardenid() {
        return gardenid;
    }

    public void setGardenid(Integer gardenid) {
        this.gardenid = gardenid;
    }

    public Integer getAdminDevId() {
        return adminDevId;
    }

    public void setAdminDevId(Integer adminDevId) {
        this.adminDevId = adminDevId;
    }

    public String getCreatedBy() {
        return createdBy;
    }

    public void setCreatedBy(String createdBy) {
        this.createdBy = createdBy;
    }

    public String getEditedBy() {
        return editedBy;
    }

    public void setEditedBy(String editedBy) {
        this.editedBy = editedBy;
    }

    public Integer getUserDevListId() {
        return userDevListId;
    }

    public void setUserDevListId(Integer userDevListId) {
        this.userDevListId = userDevListId;
    }

    public String getDate() {
        return date;
    }

    public void setDate(String date) {
        this.date = date;
    }

    public Integer getCount() {
        return count;
    }

    public void setCount(Integer count) {
        this.count = count;
    }

    public List<EntryImage> getEntryImages() {
        return entryImages;
    }

    public void setEntryImages(List<EntryImage> entryImages) {
        this.entryImages = entryImages;
    }

    public Object getTodos() {
        return todos;
    }

    public void setTodos(Object todos) {
        this.todos = todos;
    }

}
