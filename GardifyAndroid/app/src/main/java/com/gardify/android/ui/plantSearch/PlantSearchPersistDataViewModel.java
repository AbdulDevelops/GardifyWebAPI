package com.gardify.android.ui.plantSearch;

import androidx.lifecycle.LiveData;
import androidx.lifecycle.MutableLiveData;
import androidx.lifecycle.ViewModel;

import com.google.android.material.chip.Chip;

import java.util.List;

public class PlantSearchPersistDataViewModel extends ViewModel {
    private MutableLiveData<List<Chip>> chipListLiveData = new MutableLiveData<>();
    private MutableLiveData<Boolean> isFilterAppliedLiveData = new MutableLiveData<>();
    private MutableLiveData<Boolean> needToScrollLiveData = new MutableLiveData<>();
    private MutableLiveData<String> searchTextLiveData = new MutableLiveData<>();
    private MutableLiveData<Integer> recyclerViewPositionLiveData = new MutableLiveData<>();

    public LiveData<List<Chip>> getChipListLiveData() {
        return chipListLiveData;
    }
    public LiveData<Boolean> getIsFilterAppliedLiveData() {
        return isFilterAppliedLiveData;
    }
    public LiveData<Boolean> getNeedToScrollLiveData() {
        return needToScrollLiveData;
    }
    public LiveData<String> getSearchedTextLiveData() {
        return searchTextLiveData;
    }
    public LiveData<Integer> getRecyclerViewPositionLiveData() {
        return recyclerViewPositionLiveData;
    }

    public void setChipListLiveData(List<Chip> chipList) {
        chipListLiveData.setValue(chipList);
    }
    public void setIsFilterAppliedLiveData(Boolean value) {
        isFilterAppliedLiveData.setValue(value);
    }
    public void setNeedToScrollLiveData(Boolean value) {
        needToScrollLiveData.setValue(value);
    }
    public void setSearchedTextLiveData(String value) {
        searchTextLiveData.setValue(value);
    }
    public void setRecyclerViewPositionLiveData(Integer value) {
        recyclerViewPositionLiveData.setValue(value);
    }

    public void clearChipListLiveData(){
        chipListLiveData.setValue(null);
    }
}
