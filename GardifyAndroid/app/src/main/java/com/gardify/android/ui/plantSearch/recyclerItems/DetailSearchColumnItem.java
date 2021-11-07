package com.gardify.android.ui.plantSearch.recyclerItems;

import android.content.Context;

import com.gardify.android.data.plantSearchFilterData.PlantTags;

public class DetailSearchColumnItem extends DetailSearchCardItem {

    public DetailSearchColumnItem(Context context, PlantTags plantTag, boolean isMulti, CharSequence text, OnPlantTagListener onPlantTagListener) {
        super(context, plantTag, isMulti, text, onPlantTagListener);
    }

    @Override public int getSpanSize(int spanCount, int position) {
        return spanCount / 2;
    }

}
