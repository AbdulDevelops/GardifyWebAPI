package com.gardify.android.data.todos;

import com.google.gson.annotations.Expose;
import com.google.gson.annotations.SerializedName;

public class TodoCount {

    @SerializedName("$id")
    @Expose
    private String $id;
    @SerializedName("Finished")
    @Expose
    private Integer finished;
    @SerializedName("Open")
    @Expose
    private Integer open;
    @SerializedName("AllTodos")
    @Expose
    private Integer allTodos;
    @SerializedName("AllTodosOfTheMonth")
    @Expose
    private Integer allTodosOfTheMonth;

    public String get$id() {
        return $id;
    }

    public void set$id(String $id) {
        this.$id = $id;
    }

    public Integer getFinished() {
        return finished;
    }

    public void setFinished(Integer finished) {
        this.finished = finished;
    }

    public Integer getOpen() {
        return open;
    }

    public void setOpen(Integer open) {
        this.open = open;
    }

    public Integer getAllTodos() {
        return allTodos;
    }

    public void setAllTodos(Integer allTodos) {
        this.allTodos = allTodos;
    }

    public Integer getAllTodosOfTheMonth() {
        return allTodosOfTheMonth;
    }

    public void setAllTodosOfTheMonth(Integer allTodosOfTheMonth) {
        this.allTodosOfTheMonth = allTodosOfTheMonth;
    }

}