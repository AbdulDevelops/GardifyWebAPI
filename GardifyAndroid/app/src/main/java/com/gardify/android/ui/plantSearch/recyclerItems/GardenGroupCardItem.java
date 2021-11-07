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

public class GardenGroupCardItem extends BindableItem<RecyclerItemCardBinding> {

    private int bgColor;
    private Context context;
    private OnGardenGroupClickListener onGardenGroupClickListener;
    private PlantGroup.GardenGroup gardenGroup;

    public GardenGroupCardItem(Context context, int bgColor, PlantGroup.GardenGroup gardenGroup, OnGardenGroupClickListener onGardenGroupClickListener) {
        this.bgColor = bgColor;
        this.gardenGroup =gardenGroup;
        this.context=context;
        this.onGardenGroupClickListener = onGardenGroupClickListener;
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
        viewBinding.text.setText(gardenGroup.getName());
        viewBinding.linearLayout.setOnClickListener(v -> onGardenGroupClickListener.onClick(gardenGroup.getId()));
    }

    public interface OnGardenGroupClickListener {
        void onClick(int groupId);
    }
}
