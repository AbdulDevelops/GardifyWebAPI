
package com.gardify.android.data.plantsDocModel;

import java.util.List;
import com.google.gson.annotations.Expose;
import com.google.gson.annotations.SerializedName;

public class PlantDocViewModel {
    @SerializedName("$id")
    @Expose
    private String $id;
    @SerializedName("QuestionAuthorId")
    @Expose
    private String questionAuthorId;
    @SerializedName("QuestionText")
    @Expose
    private String questionText;
    @SerializedName("UserAllowsPublishment")
    @Expose
    private Boolean userAllowsPublishment;
    @SerializedName("AdminAllowsPublishment")
    @Expose
    private Boolean adminAllowsPublishment;
    @SerializedName("IsOwnFoto")
    @Expose
    private Boolean isOwnFoto;
    @SerializedName("Thema")
    @Expose
    private String thema;
    @SerializedName("Headline")
    @Expose
    private String headline;
    @SerializedName("Description")
    @Expose
    private String description;
    @SerializedName("PublishDate")
    @Expose
    private String publishDate;
    @SerializedName("Images")
    @Expose
    private List<Image> images = null;
    @SerializedName("QuestionId")
    @Expose
    private Integer questionId;
    @SerializedName("TotalAnswers")
    @Expose
    private Integer totalAnswers;
    @SerializedName("SeenAnswers")
    @Expose
    private Integer seenAnswers;
    @SerializedName("isEdited")
    @Expose
    private Boolean isEdited;

    public String get$id() {
        return $id;
    }

    public void set$id(String $id) {
        this.$id = $id;
    }

    public String getQuestionAuthorId() {
        return questionAuthorId;
    }

    public void setQuestionAuthorId(String questionAuthorId) {
        this.questionAuthorId = questionAuthorId;
    }

    public String getQuestionText() {
        return questionText;
    }

    public void setQuestionText(String questionText) {
        this.questionText = questionText;
    }

    public Boolean getUserAllowsPublishment() {
        return userAllowsPublishment;
    }

    public void setUserAllowsPublishment(Boolean userAllowsPublishment) {
        this.userAllowsPublishment = userAllowsPublishment;
    }

    public Boolean getAdminAllowsPublishment() {
        return adminAllowsPublishment;
    }

    public void setAdminAllowsPublishment(Boolean adminAllowsPublishment) {
        this.adminAllowsPublishment = adminAllowsPublishment;
    }

    public Boolean getIsOwnFoto() {
        return isOwnFoto;
    }

    public void setIsOwnFoto(Boolean isOwnFoto) {
        this.isOwnFoto = isOwnFoto;
    }

    public String getThema() {
        return thema;
    }

    public void setThema(String thema) {
        this.thema = thema;
    }

    public String getHeadline() {
        return headline;
    }

    public void setHeadline(String headline) {
        this.headline = headline;
    }

    public String getDescription() {
        return description;
    }

    public void setDescription(String description) {
        this.description = description;
    }

    public String getPublishDate() {
        return publishDate;
    }

    public void setPublishDate(String publishDate) {
        this.publishDate = publishDate;
    }

    public List<Image> getImages() {
        return images;
    }

    public void setImages(List<Image> images) {
        this.images = images;
    }

    public Integer getQuestionId() {
        return questionId;
    }

    public void setQuestionId(Integer questionId) {
        this.questionId = questionId;
    }

    public Integer getTotalAnswers() {
        return totalAnswers;
    }

    public void setTotalAnswers(Integer totalAnswers) {
        this.totalAnswers = totalAnswers;
    }

    public Integer getSeenAnswers() {
        return seenAnswers;
    }

    public void setSeenAnswers(Integer seenAnswers) {
        this.seenAnswers = seenAnswers;
    }

    public Boolean getIsEdited() {
        return isEdited;
    }

    public void setIsEdited(Boolean isEdited) {
        this.isEdited = isEdited;
    }

}
