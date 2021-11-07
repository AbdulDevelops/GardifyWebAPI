package com.gardify.android.ui.myGarden.interfaces;

import android.view.View;
import com.gardify.android.data.myGarden.MyGarden;
import com.gardify.android.databinding.ItemGardenCardBinding;
import com.gardify.android.databinding.ItemGenericGridImageBinding;

public interface OnPlantClickListener {
    void onClick(MyGarden myGarden, ItemGardenCardBinding cardBinding, ItemGenericGridImageBinding gridBinding, View view, int Pos);
}