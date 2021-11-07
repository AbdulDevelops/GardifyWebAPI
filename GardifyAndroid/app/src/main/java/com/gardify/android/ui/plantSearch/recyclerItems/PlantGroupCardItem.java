package com.gardify.android.ui.plantSearch.recyclerItems;

import android.content.Context;
import android.view.View;

import androidx.annotation.NonNull;

import com.gardify.android.data.plantSearchFilterData.PlantGroup;
import com.gardify.android.R;
import com.gardify.android.databinding.RecyclerItemCardBinding;
import com.xwray.groupie.databinding.BindableItem;

import static com.gardify.android.ui.myGarden.MyGardenFragment.INSET;
import static com.gardify.android.ui.myGarden.MyGardenFragment.INSET_TYPE_KEY;

public class PlantGroupCardItem extends BindableItem<RecyclerItemCardBinding> {

    private int bgColor;
    private Context context;
    private OnPlantGroupClickListener onPlantGroupClickListener;
    private PlantGroup.Group plantGroup;

    public PlantGroupCardItem(Context context, int bgColor, PlantGroup.Group plantGroup, OnPlantGroupClickListener onPlantGroupClickListener) {
        this.bgColor = bgColor;
        this.plantGroup =plantGroup;
        this.context=context;
        this.onPlantGroupClickListener=onPlantGroupClickListener;
        getExtras().put(INSET_TYPE_KEY, INSET);
    }

    @Override
    public int getLayout() {
        return R.layout.recycler_item_card;
    }

    @Override
    public void bind(@NonNull RecyclerItemCardBinding viewBinding, int position) {
        viewBinding.addIcon.setVisibility(View.INVISIBLE);
        viewBinding.linearLayout.setBackgroundColor(context.getResources().getColor(bgColor,null));
        viewBinding.text.setText(plantGroup.getName());
        viewBinding.linearLayout.setOnClickListener(v -> onPlantGroupClickListener.onClick(plantGroup.getId()));
    }

    public interface OnPlantGroupClickListener {
        void onClick(int groupId);
    }
}
