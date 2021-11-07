
package com.gardify.android.data.plantsDocModel;

import java.util.List;
import com.google.gson.annotations.Expose;
import com.google.gson.annotations.SerializedName;

public class PlanDocViewModelAnswer {

    @SerializedName("$id")
    @Expose
    private String $id;
    @SerializedName("PlantDocViewModel")
    @Expose
    private PlantDocViewModel plantDocViewModel;
    @SerializedName("PlantDocAnswerList")
    @Expose
    private List<PlantDocAnswerList> plantDocAnswerList = null;
    @SerializedName("NewAnswer")
    @Expose
    private Object newAnswer;
    @SerializedName("Thema")
    @Expose
    private Object thema;

    public String get$id() {
        return $id;
    }

    public void set$id(String $id) {
        this.$id = $id;
    }

    public PlantDocViewModel getPlantDocViewModel() {
        return plantDocViewModel;
    }

    public void setPlantDocViewModel(PlantDocViewModel plantDocViewModel) {
        this.plantDocViewModel = plantDocViewModel;
    }

    public List<PlantDocAnswerList> getPlantDocAnswerList() {
        return plantDocAnswerList;
    }

    public void setPlantDocAnswerList(List<PlantDocAnswerList> plantDocAnswerList) {
        this.plantDocAnswerList = plantDocAnswerList;
    }

    public Object getNewAnswer() {
        return newAnswer;
    }

    public void setNewAnswer(Object newAnswer) {
        this.newAnswer = newAnswer;
    }

    public Object getThema() {
        return thema;
    }

    public void setThema(Object thema) {
        this.thema = thema;
    }

}
