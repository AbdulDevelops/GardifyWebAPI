package com.gardify.android.ui.plantScan.adapters;

import android.content.Context;


import androidx.recyclerview.widget.GridLayoutManager;
import androidx.recyclerview.widget.RecyclerView;


import com.gardify.android.ui.plantScan.models.ItemImage;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.LinkedHashMap;
import java.util.Map;

/**
 * Created by bpncool on 2/23/2016.
 */
public class SectionedExpandableLayoutHelper implements SectionStateChangeListener {

    //data list
    //private LinkedHashMap<Section, ArrayList<Item>> mSectionDataMap = new LinkedHashMap<Section, ArrayList<Item>>();
    private LinkedHashMap<Section, ArrayList<ItemImage>> mSectionDataMap1 = new LinkedHashMap<Section, ArrayList<ItemImage>>();
    private LinkedHashMap<Section, ArrayList<String>> mSectionDataMap2 = new LinkedHashMap<Section, ArrayList<String>>();

    private ArrayList<Object> mDataArrayList = new ArrayList<Object>();

    //section map
    private HashMap<String, Section> mSectionMap = new HashMap<String, Section>();

    //adapter
    private SectionedExpandableGridAdapter mSectionedExpandableGridAdapter;

    //recycler view
    RecyclerView mRecyclerView;

    boolean mGridLayout;

    public SectionedExpandableLayoutHelper(Context context, RecyclerView recyclerView, ItemClickListener itemClickListener,
                                           int gridSpanCount, boolean gridLayout) {

        //setting the recycler view
        GridLayoutManager gridLayoutManager = new GridLayoutManager(context, gridSpanCount);
        recyclerView.setLayoutManager(gridLayoutManager);
        mSectionedExpandableGridAdapter = new SectionedExpandableGridAdapter(context, mDataArrayList,
                gridLayoutManager, itemClickListener, this);
        recyclerView.setAdapter(mSectionedExpandableGridAdapter);

        mRecyclerView = recyclerView;
        mGridLayout = gridLayout;
    }

    public void notifyDataSetChanged() {
        generateDataList();
        mSectionedExpandableGridAdapter.notifyDataSetChanged();
    }

    /*public void addSection(String section, ArrayList<Item> items) {
        Section newSection;
        mSectionMap.put(section, (newSection = new Section(section)));
        mSectionDataMap.put(newSection, items);
    }*/

    public void addSection1(String section, ArrayList<ItemImage> itemImages, ArrayList<String> itemImages2) {
        Section newSection;
        mSectionMap.put(section, (newSection = new Section(section)));
        if (mGridLayout) {
            mSectionDataMap1.put(newSection, itemImages);
        } else {
            mSectionDataMap2.put(newSection, itemImages2);
        }
    }

    public void addItem(String section, ItemImage itemImage) {
        mSectionDataMap1.get(mSectionMap.get(section)).add(itemImage);
    }

    public void removeItem(String section, ItemImage itemImage) {
        mSectionDataMap1.get(mSectionMap.get(section)).remove(itemImage);
    }

    public void removeSection(String section) {
        mSectionDataMap1.remove(mSectionMap.get(section));
        mSectionMap.remove(section);
    }
    public Section getSection(String section) {
        return mSectionMap.get(section);
    }

    private void generateDataList() {
        mDataArrayList.clear();
        if (mGridLayout) {
            for (Map.Entry<Section, ArrayList<ItemImage>> entry : mSectionDataMap1.entrySet()) {
                Section key;
                mDataArrayList.add((key = entry.getKey()));
                if (key.isExpanded)
                    mDataArrayList.addAll(entry.getValue());
            }
        } else {
            for (Map.Entry<Section, ArrayList<String>> entry : mSectionDataMap2.entrySet()) {
                Section key;
                mDataArrayList.add((key = entry.getKey()));
                if (key.isExpanded)
                    mDataArrayList.addAll(entry.getValue());
            }
        }
    }

    @Override
    public void onSectionStateChanged(Section section, boolean isOpen) {
        if (!mRecyclerView.isComputingLayout()) {
            section.isExpanded = isOpen;
            notifyDataSetChanged();
        }
    }
}