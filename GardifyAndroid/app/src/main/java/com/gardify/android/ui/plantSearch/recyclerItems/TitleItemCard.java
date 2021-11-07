package com.gardify.android.ui.plantSearch.recyclerItems;

import android.content.Context;
import android.graphics.Typeface;
import android.util.TypedValue;
import android.view.View;

import androidx.annotation.NonNull;
import androidx.annotation.StringRes;

import com.gardify.android.R;
import com.gardify.android.databinding.RecyclerPlantSearchTitleCardBinding;
import com.xwray.groupie.databinding.BindableItem;

import static com.gardify.android.ui.myGarden.MyGardenFragment.INSET;
import static com.gardify.android.ui.myGarden.MyGardenFragment.INSET_TYPE_KEY;

public class TitleItemCard extends BindableItem<RecyclerPlantSearchTitleCardBinding> {

    private int bgColor;
    @StringRes
    private int textStringResId;
    private Context context;
    private OnCardClickListener onCardClickListener;

    public TitleItemCard(Context context, int bgColor, int textResId, OnCardClickListener onCardClickListener) {
        this.bgColor = bgColor;
        this.textStringResId =textResId;
        this.context=context;
        this.onCardClickListener=onCardClickListener;
        getExtras().put(INSET_TYPE_KEY, INSET);
    }

    @Override
    public int getLayout() {
        return R.layout.recycler_plant_search_title_card;
    }

    @Override
    public void bind(@NonNull RecyclerPlantSearchTitleCardBinding viewBinding, int position) {
        viewBinding.addIcon.setVisibility(View.INVISIBLE);
        viewBinding.cardView.setCardBackgroundColor(context.getResources().getColor(bgColor,null));
        viewBinding.text.setText(context.getResources().getString(textStringResId));
        viewBinding.text.setTypeface(viewBinding.text.getTypeface(), Typeface.BOLD);
        viewBinding.text.setTextSize(TypedValue.COMPLEX_UNIT_PX, context.getResources().getDimension(R.dimen.textSize_title));

        if(onCardClickListener!=null)
        viewBinding.cardView.setOnClickListener(v -> onCardClickListener.onClick(textStringResId));
    }

    public interface OnCardClickListener {
        void onClick(int textResId);
    }
}
