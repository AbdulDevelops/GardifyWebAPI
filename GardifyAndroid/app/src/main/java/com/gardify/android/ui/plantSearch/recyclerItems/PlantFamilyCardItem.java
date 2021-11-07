package com.gardify.android.ui.plantSearch.recyclerItems;

import android.content.Context;
import android.view.View;

import androidx.annotation.NonNull;

import com.gardify.android.R;
import com.gardify.android.databinding.RecyclerItemCardBinding;
import com.xwray.groupie.databinding.BindableItem;

import static com.gardify.android.ui.myGarden.MyGardenFragment.INSET;
import static com.gardify.android.ui.myGarden.MyGardenFragment.INSET_TYPE_KEY;

public class PlantFamilyCardItem extends BindableItem<RecyclerItemCardBinding> {

    private int bgColor;
    private Context context;
    private OnPlantFamilyClickListener onPlantFamilyClickListener;
    private String plantFamilyName;

    public PlantFamilyCardItem(Context context, int bgColor, String plantFamilyName, OnPlantFamilyClickListener onPlantFamilyClickListener) {
        this.bgColor = bgColor;
        this.plantFamilyName =plantFamilyName;
        this.context=context;
        this.onPlantFamilyClickListener=onPlantFamilyClickListener;
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
        viewBinding.text.setText(plantFamilyName);
        viewBinding.linearLayout.setOnClickListener(v -> onPlantFamilyClickListener.onClick(plantFamilyName));
    }

    public interface OnPlantFamilyClickListener {
        void onClick(String plantFamilyName);
    }
}
