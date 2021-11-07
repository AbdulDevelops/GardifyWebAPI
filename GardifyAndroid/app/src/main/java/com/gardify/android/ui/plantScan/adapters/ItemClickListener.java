package com.gardify.android.ui.plantScan.adapters;


import com.gardify.android.ui.plantScan.models.ItemImage;

public interface ItemClickListener {

    void itemClicked(ItemImage itemImage);
    void itemClicked(String itemText);
    void itemClicked(Section section);


}
