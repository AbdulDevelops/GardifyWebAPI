package com.gardify.android.ui.myGarden.recyclerItems;

import android.content.Context;
import android.view.View;

import androidx.annotation.NonNull;

import com.gardify.android.R;
import com.gardify.android.viewModelData.BadgesIconVM;
import com.gardify.android.databinding.RecyclerViewMyGardenEcoFilterItemBinding;
import com.xwray.groupie.databinding.BindableItem;

public class PlantEcoFilterCardItem extends BindableItem<RecyclerViewMyGardenEcoFilterItemBinding> {

    private Context context;
    private BadgesIconVM badgesIconVM;
    private OnFilterEcoClickListener onFilterEcoClickListener;
    private boolean resetSelected = false;
    private boolean isMulti;

    public PlantEcoFilterCardItem(Context context, BadgesIconVM badgesIconVM, boolean isMulti, OnFilterEcoClickListener onFilterEcoClickListener) {
        this.context = context;
        this.badgesIconVM = badgesIconVM;
        this.isMulti = isMulti;
        this.onFilterEcoClickListener = onFilterEcoClickListener;
    }

    public PlantEcoFilterCardItem(Context context, boolean resetSelected, OnFilterEcoClickListener onFilterEcoClickListener) {
        this.context = context;
        this.resetSelected = resetSelected;
        this.onFilterEcoClickListener = onFilterEcoClickListener;
    }

    @Override
    public int getLayout() {
        return R.layout.recycler_view_my_garden_eco_filter_item;
    }

    @Override
    public void bind(@NonNull RecyclerViewMyGardenEcoFilterItemBinding viewBinding, int position) {

        if (!resetSelected) {
            viewBinding.textViewMyGardenPlantFilterName.setText(badgesIconVM.getName());
            viewBinding.textViewMyGardenPlantFilterName.setTextColor(context.getResources().getColor(R.color.text_all_gunmetal, null));
            viewBinding.imageViewPlantFilterIcon.setVisibility(View.VISIBLE);
            viewBinding.plantEcoFilterCheckbox.setVisibility(View.GONE);

            int resId = getResId();
            if (resId != 0) {
                viewBinding.imageViewPlantFilterIcon.setImageDrawable(context.getResources().getDrawable(resId, null));
            }
            viewBinding.plantEcoFilterCheckbox.setChecked(badgesIconVM.isChecked());
            viewBinding.linearLayoutRecyclerViewMyGardenEcoFilterItem.setOnClickListener(v -> {
                badgesIconVM.setChecked(!badgesIconVM.isChecked());
                onFilterEcoClickListener.onClick(badgesIconVM, false);
                if (isMulti)
                    viewBinding.plantEcoFilterCheckbox.performClick();
            });
            viewBinding.plantEcoFilterCheckbox.setOnClickListener(v -> {
                badgesIconVM.setChecked(!badgesIconVM.isChecked());
                onFilterEcoClickListener.onClick(badgesIconVM, false);
            });
        } else {
            viewBinding.textViewMyGardenPlantFilterName.setText(R.string.myGarden_resetFilter);
            viewBinding.textViewMyGardenPlantFilterName.setTextColor(context.getResources().getColor(R.color.text_all_jasper, null));
            viewBinding.imageViewPlantFilterIcon.setVisibility(View.GONE);
            viewBinding.textViewMyGardenPlantFilterName.setOnClickListener(v -> {
                onFilterEcoClickListener.onClick(null, true);
                PlantEcoFilterIconsCardItem.appliedFilterList.clear();
            });
        }

        if (isMulti) {
            viewBinding.plantEcoFilterCheckbox.setVisibility(View.VISIBLE);
        }
    }

    private int getResId() {
        return context.getResources().getIdentifier("gardify_app_icon_" + badgesIconVM.getImage().toLowerCase(), "drawable", context.getApplicationInfo().packageName);
    }

    public interface OnFilterEcoClickListener {
        void onClick(BadgesIconVM badgesIconVM, boolean reset);
    }

}
