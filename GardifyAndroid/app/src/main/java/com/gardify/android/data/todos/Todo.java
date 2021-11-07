
package com.gardify.android.data.todos;

import java.util.List;
import com.google.gson.annotations.Expose;
import com.google.gson.annotations.SerializedName;

public class Todo {

    @SerializedName("$id")
    @Expose
    private String $id;
    @SerializedName("TodoList")
    @Expose
    private List<TodoList> todoList = null;
    @SerializedName("StartDate")
    @Expose
    private String startDate;
    @SerializedName("EndDate")
    @Expose
    private String endDate;
    @SerializedName("ShopcartCounter")
    @Expose
    private Integer shopcartCounter;
    @SerializedName("PlantCount")
    @Expose
    private Integer plantCount;
    @SerializedName("Points")
    @Expose
    private Integer points;
    @SerializedName("CurrentTodoCount")
    @Expose
    private Integer currentTodoCount;
    @SerializedName("NewMessages")
    @Expose
    private Integer newMessages;

    public String get$id() {
        return $id;
    }

    public void set$id(String $id) {
        this.$id = $id;
    }

    public List<TodoList> getTodoList() {
        return todoList;
    }

    public void setTodoList(List<TodoList> todoList) {
        this.todoList = todoList;
    }

    public String getStartDate() {
        return startDate;
    }

    public void setStartDate(String startDate) {
        this.startDate = startDate;
    }

    public String getEndDate() {
        return endDate;
    }

    public void setEndDate(String endDate) {
        this.endDate = endDate;
    }

    public Integer getShopcartCounter() {
        return shopcartCounter;
    }

    public void setShopcartCounter(Integer shopcartCounter) {
        this.shopcartCounter = shopcartCounter;
    }

    public Integer getPlantCount() {
        return plantCount;
    }

    public void setPlantCount(Integer plantCount) {
        this.plantCount = plantCount;
    }

    public Integer getPoints() {
        return points;
    }

    public void setPoints(Integer points) {
        this.points = points;
    }

    public Integer getCurrentTodoCount() {
        return currentTodoCount;
    }

    public void setCurrentTodoCount(Integer currentTodoCount) {
        this.currentTodoCount = currentTodoCount;
    }

    public Integer getNewMessages() {
        return newMessages;
    }

    public void setNewMessages(Integer newMessages) {
        this.newMessages = newMessages;
    }

}
