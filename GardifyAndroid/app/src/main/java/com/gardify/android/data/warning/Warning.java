package com.gardify.android.data.warning;

import com.google.gson.annotations.Expose;
import com.google.gson.annotations.SerializedName;

public class Warning {

    @SerializedName("$id")
    @Expose
    private String $id;
    @SerializedName("RelatedObjectId")
    @Expose
    private Integer relatedObjectId;
    @SerializedName("RelatedObjectName")
    @Expose
    private String relatedObjectName;
    @SerializedName("NotifyForWind")
    @Expose
    private Boolean notifyForWind;
    @SerializedName("NotifyForFrost")
    @Expose
    private Boolean notifyForFrost;
    @SerializedName("IsInPot")
    @Expose
    private Boolean isInPot;
    @SerializedName("Title")
    @Expose
    private String title;
    @SerializedName("Text")
    @Expose
    private String text;
    @SerializedName("AlertConditionValue")
    @Expose
    private Double alertConditionValue;
    @SerializedName("ObjectType")
    @Expose
    private Integer objectType;
    @SerializedName("Dismissed")
    @Expose
    private Boolean dismissed;

    public String get$id() {
        return $id;
    }

    public void set$id(String $id) {
        this.$id = $id;
    }

    public Integer getRelatedObjectId() {
        return relatedObjectId;
    }

    public void setRelatedObjectId(Integer relatedObjectId) {
        this.relatedObjectId = relatedObjectId;
    }

    public String getRelatedObjectName() {
        return relatedObjectName;
    }

    public void setRelatedObjectName(String relatedObjectName) {
        this.relatedObjectName = relatedObjectName;
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

    public Boolean getIsInPot() {
        return isInPot;
    }

    public void setIsInPot(Boolean isInPot) {
        this.isInPot = isInPot;
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

    public Double getAlertConditionValue() {
        return alertConditionValue;
    }

    public void setAlertConditionValue(Double alertConditionValue) {
        this.alertConditionValue = alertConditionValue;
    }

    public Integer getObjectType() {
        return objectType;
    }

    public void setObjectType(Integer objectType) {
        this.objectType = objectType;
    }

    public Boolean getDismissed() {
        return dismissed;
    }

    public void setDismissed(Boolean dismissed) {
        this.dismissed = dismissed;
    }
}

