package com.gardify.android.ui.plantScan.models;

public class ItemString {
    private final String name;
    private final int id;

    public ItemString(String name, int id) {
        this.name = name;
        this.id = id;
    }

    public int getId() {
        return id;
    }

    public String getName() {
        return name;
    }
}
