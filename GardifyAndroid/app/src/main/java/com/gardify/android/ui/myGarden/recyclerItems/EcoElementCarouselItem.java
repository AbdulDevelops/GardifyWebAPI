package com.gardify.android.ui.myGarden.recyclerItems;

import android.content.Context;
import android.view.View;

import androidx.annotation.NonNull;

import com.gardify.android.data.myGarden.Eco.EcoElement;
import com.gardify.android.R;
import com.gardify.android.databinding.RecyclerViewMyGardenEcoItemBinding;
import com.xwray.groupie.databinding.BindableItem;

/**
 * A card item with a fixed width so it can be used with a horizontal layout manager.
 */
public class EcoElementCarouselItem extends BindableItem<RecyclerViewMyGardenEcoItemBinding> {

    private EcoElement ecoElement;
    private OnEcoClickListener onEcoClickListener;
    private Context context;

    public EcoElementCarouselItem(Context context, EcoElement ecoElement, OnEcoClickListener onEcoClickListener) {
        this.ecoElement=ecoElement;
        this.onEcoClickListener=onEcoClickListener;
        this.context=context;
    }

    @Override
    public int getLayout() {
        return R.layout.recycler_view_my_garden_eco_item;
    }

    @Override
    public void bind(@NonNull RecyclerViewMyGardenEcoItemBinding viewBinding, int position) {
        viewBinding.textViewMyGardenEcoElementName.setText(ecoElement.getName());
        viewBinding.textViewMyGardenEcoElementDesc.setText(ecoElement.getDescription());
        viewBinding.switchMyGardenEcoElement.setChecked(ecoElement.getChecked());
        int resId = getResId();
        if(resId!=0){
            viewBinding.imageViewEcoIcon.setImageDrawable(context.getResources().getDrawable(resId,null));
        }
        viewBinding.buttonMyGardenEcoMoreDetail.setOnClickListener(v->{
            onEcoClickListener.onClick(ecoElement, viewBinding, viewBinding.buttonMyGardenEcoMoreDetail);
        });
        viewBinding.switchMyGardenEcoElement.setOnClickListener(v->{
            onEcoClickListener.onClick(ecoElement, viewBinding, viewBinding.switchMyGardenEcoElement);
        });
    }

    private int getResId() {
        return context.getResources().getIdentifier("eco_" + ecoElement.getName().toLowerCase(), "drawable", context.getApplicationInfo().packageName);
    }

    public interface OnEcoClickListener {
        void onClick(EcoElement item, RecyclerViewMyGardenEcoItemBinding viewBinding, View view);
    }
}
