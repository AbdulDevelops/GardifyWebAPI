package com.gardify.android.ui.myGarden.recyclerItems;

import android.content.Context;
import android.view.View;

import androidx.annotation.NonNull;
import androidx.annotation.StringRes;

import com.gardify.android.R;
import com.gardify.android.databinding.RecyclerItemCardBinding;
import com.xwray.groupie.databinding.BindableItem;

import static com.gardify.android.ui.myGarden.MyGardenFragment.INSET;
import static com.gardify.android.ui.myGarden.MyGardenFragment.INSET_TYPE_KEY;

public class CardAddItem extends BindableItem<RecyclerItemCardBinding> {

    private int bgColor;
    @StringRes
    private int textStringResId;
    private Context context;
    private OnCardClickListener onCardClickListener;

    public CardAddItem(Context context, int bgColor, int textResId, OnCardClickListener onCardClickListener) {
        this.bgColor = bgColor;
        this.textStringResId =textResId;
        this.context=context;
        this.onCardClickListener=onCardClickListener;
        getExtras().put(INSET_TYPE_KEY, INSET);
    }

    @Override
    public int getLayout() {
        return R.layout.recycler_item_card;
    }

    @Override
    public void bind(@NonNull RecyclerItemCardBinding viewBinding, int position) {
        viewBinding.addIcon.setVisibility(View.VISIBLE);
        viewBinding.linearLayout.setBackgroundColor(context.getResources().getColor(bgColor,null));
        viewBinding.text.setText(context.getResources().getString(textStringResId));
        viewBinding.linearLayout.setOnClickListener(v -> onCardClickListener.onClick(textStringResId));
        viewBinding.addIcon.setOnClickListener(v -> onCardClickListener.onClick(textStringResId));
    }

    public interface OnCardClickListener {
        void onClick(int textResId);
    }
}
