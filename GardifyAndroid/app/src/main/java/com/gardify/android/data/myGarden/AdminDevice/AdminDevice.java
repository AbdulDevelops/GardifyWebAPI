
package com.gardify.android.data.myGarden.AdminDevice;

import java.util.List;
import com.google.gson.annotations.Expose;
import com.google.gson.annotations.SerializedName;

public class AdminDevice {

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
    @SerializedName("DevicesImages")
    @Expose
    private List<DevicesImage> devicesImages = null;
    @SerializedName("Note")
    @Expose
    private Object note;
    @SerializedName("StatusMessage")
    @Expose
    private Object statusMessage;

    private boolean checkedFlag = false;

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

    public List<DevicesImage> getDevicesImages() {
        return devicesImages;
    }

    public void setDevicesImages(List<DevicesImage> devicesImages) {
        this.devicesImages = devicesImages;
    }

    public Object getNote() {
        return note;
    }

    public void setNote(Object note) {
        this.note = note;
    }

    public Object getStatusMessage() {
        return statusMessage;
    }

    public void setStatusMessage(Object statusMessage) {
        this.statusMessage = statusMessage;
    }

    public boolean isCheckedFlag() {
        return checkedFlag;
    }

    public void setCheckedFlag(boolean checkedFlag) {
        this.checkedFlag = checkedFlag;
    }
}
