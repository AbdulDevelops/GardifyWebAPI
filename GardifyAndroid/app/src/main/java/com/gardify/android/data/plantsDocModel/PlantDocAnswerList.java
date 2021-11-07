
package com.gardify.android.data.plantsDocModel;

import com.google.gson.annotations.Expose;
import com.google.gson.annotations.SerializedName;

public class PlantDocAnswerList {

    @SerializedName("$id")
    @Expose
    private String $id;
    @SerializedName("AnswerText")
    @Expose
    private String answerText;
    @SerializedName("AutorName")
    @Expose
    private String autorName;
    @SerializedName("Date")
    @Expose
    private String date;
    @SerializedName("AnswerImages")
    @Expose
    private Object answerImages;
    @SerializedName("AnswerId")
    @Expose
    private Integer answerId;
    @SerializedName("EnableToEdit")
    @Expose
    private Boolean enableToEdit;
    @SerializedName("IsEdited")
    @Expose
    private Boolean isEdited;
    @SerializedName("OriginalAnswer")
    @Expose
    private Object originalAnswer;
    @SerializedName("IsAdminAnswer")
    @Expose
    private Boolean isAdminAnswer;

    public String get$id() {
        return $id;
    }

    public void set$id(String $id) {
        this.$id = $id;
    }

    public String getAnswerText() {
        return answerText;
    }

    public void setAnswerText(String answerText) {
        this.answerText = answerText;
    }

    public String getAutorName() {
        return autorName;
    }

    public void setAutorName(String autorName) {
        this.autorName = autorName;
    }

    public String getDate() {
        return date;
    }

    public void setDate(String date) {
        this.date = date;
    }

    public Object getAnswerImages() {
        return answerImages;
    }

    public void setAnswerImages(Object answerImages) {
        this.answerImages = answerImages;
    }

    public Integer getAnswerId() {
        return answerId;
    }

    public void setAnswerId(Integer answerId) {
        this.answerId = answerId;
    }

    public Boolean getEnableToEdit() {
        return enableToEdit;
    }

    public void setEnableToEdit(Boolean enableToEdit) {
        this.enableToEdit = enableToEdit;
    }

    public Boolean getIsEdited() {
        return isEdited;
    }

    public void setIsEdited(Boolean isEdited) {
        this.isEdited = isEdited;
    }

    public Object getOriginalAnswer() {
        return originalAnswer;
    }

    public void setOriginalAnswer(Object originalAnswer) {
        this.originalAnswer = originalAnswer;
    }

    public Boolean getIsAdminAnswer() {
        return isAdminAnswer;
    }

    public void setIsAdminAnswer(Boolean isAdminAnswer) {
        this.isAdminAnswer = isAdminAnswer;
    }

}
