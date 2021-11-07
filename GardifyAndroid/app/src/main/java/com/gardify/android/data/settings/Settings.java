
package com.gardify.android.data.settings;

import com.google.gson.annotations.Expose;
import com.google.gson.annotations.SerializedName;

public class Settings {

    @SerializedName("$id")
    @Expose
    private String $id;
    @SerializedName("UserId")
    @Expose
    private String userId;
    @SerializedName("ActiveStormAlert")
    @Expose
    private Boolean activeStormAlert;
    @SerializedName("ActiveFrostAlert")
    @Expose
    private Boolean activeFrostAlert;
    @SerializedName("ActiveNewPlantAlert")
    @Expose
    private Boolean activeNewPlantAlert;
    @SerializedName("AlertByEmail")
    @Expose
    private Boolean alertByEmail;
    @SerializedName("AlertByPush")
    @Expose
    private Boolean alertByPush;
    @SerializedName("FrostDegreeBuffer")
    @Expose
    private Integer frostDegreeBuffer;

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

    public Boolean getActiveStormAlert() {
        return activeStormAlert;
    }

    public void setActiveStormAlert(Boolean activeStormAlert) {
        this.activeStormAlert = activeStormAlert;
    }

    public Boolean getActiveFrostAlert() {
        return activeFrostAlert;
    }

    public void setActiveFrostAlert(Boolean activeFrostAlert) {
        this.activeFrostAlert = activeFrostAlert;
    }

    public Boolean getActiveNewPlantAlert() {
        return activeNewPlantAlert;
    }

    public void setActiveNewPlantAlert(Boolean activeNewPlantAlert) {
        this.activeNewPlantAlert = activeNewPlantAlert;
    }

    public Boolean getAlertByEmail() {
        return alertByEmail;
    }

    public void setAlertByEmail(Boolean alertByEmail) {
        this.alertByEmail = alertByEmail;
    }

    public Boolean getAlertByPush() {
        return alertByPush;
    }

    public void setAlertByPush(Boolean alertByPush) {
        this.alertByPush = alertByPush;
    }

    public Integer getFrostDegreeBuffer() {
        return frostDegreeBuffer;
    }

    public void setFrostDegreeBuffer(Integer frostDegreeBuffer) {
        this.frostDegreeBuffer = frostDegreeBuffer;
    }

}
