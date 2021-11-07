package com.gardify.android.viewModelData.todos;


import com.gardify.android.data.todos.EntryImage;

import java.util.List;

public class TodoCalendarViewModel {

    int id, cyclicId, referenceId;
    private String date, title, description, imageUrl, notes, dateEnd, dateStart, cyclicDateEnd;
    boolean deleted, ignored, finished;
    private Integer referenceType;

    private List<EntryImage> entryImages = null;

    private Integer newMessages, currentTodoCount, points, plantCount, shopcartCounter, relatedPlantId, notification;

    private String relatedPlantName, referenceText;

    public TodoCalendarViewModel setId(int id) {
        this.id = id;
        return this;
    }

    public TodoCalendarViewModel setCyclicId(int cyclicId) {
        this.cyclicId = cyclicId;
        return this;
    }

    public TodoCalendarViewModel setReferenceId(int referenceId) {
        this.referenceId = referenceId;
        return this;
    }

    public TodoCalendarViewModel setReferenceType(Integer referenceType) {
        this.referenceType = referenceType;
        return this;
    }

    public TodoCalendarViewModel setDate(String date) {
        this.date = date;
        return this;
    }

    public TodoCalendarViewModel setTitle(String title) {
        this.title = title;
        return this;
    }

    public TodoCalendarViewModel setDescription(String description) {
        this.description = description;
        return this;
    }

    public TodoCalendarViewModel setImageUrl(String imageUrl) {
        this.imageUrl = imageUrl;
        return this;
    }

    public TodoCalendarViewModel setDeleted(boolean deleted) {
        this.deleted = deleted;
        return this;
    }

    public TodoCalendarViewModel setFinished(boolean finished) {
        this.finished = finished;
        return this;
    }

    public TodoCalendarViewModel setIgnored(boolean ignored) {
        this.ignored = ignored;
        return this;
    }

    public TodoCalendarViewModel setNotes(String notes) {
        this.notes = notes;
        return this;
    }

    public TodoCalendarViewModel setDateStart(String startDate) {
        this.dateStart = startDate;
        return this;
    }

    public TodoCalendarViewModel setDateEnd(String endDate) {
        this.dateEnd = endDate;
        return this;
    }

    public TodoCalendarViewModel setNewMessages(Integer newMessages) {
        this.newMessages = newMessages;
        return this;
    }

    public TodoCalendarViewModel setEntryImages(List<EntryImage> entryImages) {
        this.entryImages = entryImages;
        return this;

    }

    public TodoCalendarViewModel setCyclicDateEnd(String cyclicDateEnd) {
        this.cyclicDateEnd = cyclicDateEnd;
        return this;

    }

    public TodoCalendarViewModel setCurrentTodoCount(Integer currentTodoCount) {
        this.currentTodoCount = currentTodoCount;
        return this;

    }

    public TodoCalendarViewModel setPoints(Integer points) {
        this.points = points;
        return this;

    }

    public TodoCalendarViewModel setPlantCount(Integer plantCount) {
        this.plantCount = plantCount;
        return this;

    }

    public TodoCalendarViewModel setReferenceText(String referenceText) {
        this.referenceText = referenceText;
        return this;
    }

    public TodoCalendarViewModel setRelatedPlantName(String relatedPlantName) {
        this.relatedPlantName = relatedPlantName;
        return this;
    }

    public TodoCalendarViewModel setRelatedPlantId(Integer relatedPlantId) {
        this.relatedPlantId = relatedPlantId;
        return this;
    }

    public TodoCalendarViewModel setShopcartCounter(Integer shopcartCounter) {
        this.shopcartCounter = shopcartCounter;
        return this;
    }
    public TodoCalendarViewModel setNotification(Integer notification) {
        this.notification = notification;
        return this;
    }

    //... build
    public TodoCalendarViewModel build() {
        return this;
    }

    //... getters

    public int getId() {
        return id;
    }

    public int getCyclicId() {
        return cyclicId;
    }

    public int getReferenceId() {
        return referenceId;
    }

    public Integer getReferenceType() {
        return referenceType;
    }

    public String getDate() {
        return date;
    }

    public String getTitle() {
        return title;
    }

    public String getDescription() {
        return description;
    }

    public String getImageUrl() {
        return imageUrl;
    }

    public String getDateEnd() {
        return dateEnd;
    }

    public String getDateStart() {
        return dateStart;
    }

    public boolean isDeleted() {
        return deleted;
    }

    public boolean isIgnored() {
        return ignored;
    }

    public boolean isFinished() {
        return finished;
    }

    public String getNotes() {
        return notes;
    }


    public String getCyclicDateEnd() {
        return cyclicDateEnd;
    }

    public List<EntryImage> getEntryImages() {
        return entryImages;
    }


    public Integer getNewMessages() {
        return newMessages;
    }

    public Integer getCurrentTodoCount() {
        return currentTodoCount;
    }

    public Integer getPoints() {
        return points;
    }

    public Integer getPlantCount() {
        return plantCount;
    }

    public Integer getShopcartCounter() {
        return shopcartCounter;
    }

    public Integer getRelatedPlantId() {
        return relatedPlantId;
    }

    public String getRelatedPlantName() {
        return relatedPlantName;
    }

    public String getReferenceText() {
        return referenceText;
    }

    public Integer getNotification() {
        return notification;
    }
}
