package com.gardify.android.ui.myGarden;

import androidx.lifecycle.LiveData;
import androidx.lifecycle.MutableLiveData;
import androidx.lifecycle.ViewModel;

import com.gardify.android.data.myGarden.MyGarden;

import java.util.List;

public class MyGardenPersistDataViewModel extends ViewModel {
    private MutableLiveData<Integer> MyGardenState = new MutableLiveData<>();
    private MutableLiveData<List<MyGarden>> myGardenList = new MutableLiveData<>();

    public LiveData<List<MyGarden>> getMyGardenList() {
        return myGardenList;
    }

    public void setMyGardenList(List<MyGarden> _myGardenList) {
        myGardenList.setValue(_myGardenList);
    }

    public LiveData<Integer> getMyGardenState() {
        return MyGardenState;
    }

    public void setMyGardenState(Integer expandedMenuState) {
        MyGardenState.setValue(expandedMenuState);
    }
}
