package com.gardify.android.data.plantSearchFilterData;

import com.google.gson.annotations.Expose;
import com.google.gson.annotations.SerializedName;

public class Cats {


    @Expose
    @SerializedName("Deleted")
    private boolean Deleted;
    @Expose
    @SerializedName("EditedDate")
    private String EditedDate;
    @Expose
    @SerializedName("CreatedDate")
    private String CreatedDate;
    @Expose
    @SerializedName("Id")
    private int Id;
    @Expose
    @SerializedName("Count")
    private int Count;
    @Expose
    @SerializedName("Title")
    private String Title;
    @Expose
    @SerializedName("$id")
    private String $id;

    public boolean getDeleted() {
        return Deleted;
    }

    public void setDeleted(boolean Deleted) {
        this.Deleted = Deleted;
    }

    public String getEditedDate() {
        return EditedDate;
    }

    public void setEditedDate(String EditedDate) {
        this.EditedDate = EditedDate;
    }

    public String getCreatedDate() {
        return CreatedDate;
    }

    public void setCreatedDate(String CreatedDate) {
        this.CreatedDate = CreatedDate;
    }

    public int getId() {
        return Id;
    }

    public void setId(int Id) {
        this.Id = Id;
    }

    public int getCount() {
        return Count;
    }

    public void setCount(int Count) {
        this.Count = Count;
    }

    public String getTitle() {
        return Title;
    }

    public void setTitle(String Title) {
        this.Title = Title;
    }

    public String get$id() {
        return $id;
    }

    public void set$id(String $id) {
        this.$id = $id;
    }
}
