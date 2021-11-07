package com.gardify.android.ui.plantSearch.recyclerItems;

import android.content.Context;
import android.view.View;

import androidx.annotation.NonNull;

import com.gardify.android.data.plantSearchFilterData.PlantTags;
import com.gardify.android.R;
import com.gardify.android.databinding.RecyclerViewMyGardenPlantDetailFilterItemBinding;
import com.xwray.groupie.databinding.BindableItem;

public class DetailSearchCardItem extends BindableItem<RecyclerViewMyGardenPlantDetailFilterItemBinding> {

    private CharSequence text;
    private Context context;
    private PlantTags plantTag;
    private OnPlantTagListener onPlantTagListener;
    private boolean isMulti;

    public DetailSearchCardItem(Context context, PlantTags plantTag, boolean _isMulti, CharSequence text, OnPlantTagListener onPlantTagListener) {
        this.context = context;
        this.plantTag = plantTag;
        this.isMulti=_isMulti;
        this.text = text;
        this.onPlantTagListener = onPlantTagListener;
    }

    @Override
    public int getLayout() {
        return R.layout.recycler_view_my_garden_plant_detail_filter_item;
    }

    @Override
    public void bind(@NonNull RecyclerViewMyGardenPlantDetailFilterItemBinding viewBinding, int position) {
        viewBinding.textViewMyGardenPlantFilterName.setText(plantTag.getTitle());
        viewBinding.textViewMyGardenPlantFilterName.setSelected(true);
        viewBinding.imageViewPlantFilterIcon.setVisibility(View.GONE);

        viewBinding.plantDetailFilterCheckbox.setChecked(plantTag.isChecked());

        viewBinding.plantDetailFilterCheckbox.setOnClickListener(v -> {
            plantTag.setChecked(!plantTag.isChecked());
            onPlantTagListener.onClick(plantTag);
        });
    }


    public void setText(CharSequence text) {
        this.text = text;
    }

    public CharSequence getText() {
        return text;
    }

    public interface OnPlantTagListener {
        void onClick(PlantTags plantTags);
    }

}
