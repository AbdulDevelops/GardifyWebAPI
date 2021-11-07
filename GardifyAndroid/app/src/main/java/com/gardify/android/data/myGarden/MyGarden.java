
package com.gardify.android.data.myGarden;

import java.util.List;
import com.google.gson.annotations.Expose;
import com.google.gson.annotations.SerializedName;

public class MyGarden {

    @SerializedName("$id")
    @Expose
    private String $id;
    @SerializedName("UserPlant")
    @Expose
    private UserPlant userPlant;
    @SerializedName("ListName")
    @Expose
    private String listName;
    @SerializedName("ListNames")
    @Expose
    private List<String> listNames = null;
    @SerializedName("ListIds")
    @Expose
    private List<Integer> listIds = null;
    @SerializedName("ListId")
    @Expose
    private Integer listId;
    @SerializedName("Count")
    @Expose
    private Integer count;

    public String get$id() {
        return $id;
    }

    public void set$id(String $id) {
        this.$id = $id;
    }

    public UserPlant getUserPlant() {
        return userPlant;
    }

    public void setUserPlant(UserPlant userPlant) {
        this.userPlant = userPlant;
    }

    public Object getListName() {
        return listName;
    }

    public void setListName(String listName) {
        this.listName = listName;
    }

    public List<String> getListNames() {
        return listNames;
    }

    public void setListNames(List<String> listNames) {
        this.listNames = listNames;
    }

    public List<Integer> getListIds() {
        return listIds;
    }

    public void setListIds(List<Integer> listIds) {
        this.listIds = listIds;
    }

    public Integer getListId() {
        return listId;
    }

    public void setListId(Integer listId) {
        this.listId = listId;
    }

    public Integer getCount() {
        return count;
    }

    public void setCount(Integer count) {
        this.count = count;
    }

}
