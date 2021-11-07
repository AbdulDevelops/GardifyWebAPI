package com.gardify.android.ui.generic.recyclerItem;

import android.content.Context;

import androidx.annotation.NonNull;
import androidx.annotation.StringRes;

import com.gardify.android.R;
import com.gardify.android.databinding.RecyclerItemCardNoResultBinding;
import com.xwray.groupie.databinding.BindableItem;

import static com.gardify.android.ui.myGarden.MyGardenFragment.INSET;
import static com.gardify.android.ui.myGarden.MyGardenFragment.INSET_TYPE_KEY;

public class CardItemNoResult extends BindableItem<RecyclerItemCardNoResultBinding> {

    @StringRes
    private int textStringResId, buttonStringResId, hintStringResId;
    private Context context;
    private OnCardClickListener onCardClickListener;

    public CardItemNoResult(Context context, int textId, int buttonTextId, OnCardClickListener onCardClickListener) {
        this.textStringResId = textId;
        this.buttonStringResId=buttonTextId;
        this.context = context;
        this.onCardClickListener = onCardClickListener;
        getExtras().put(INSET_TYPE_KEY, INSET);
    }
    public CardItemNoResult(Context context, int textId, int buttonTextId, int hintStringResId, OnCardClickListener onCardClickListener) {
        this.textStringResId = textId;
        this.buttonStringResId=buttonTextId;
        this.hintStringResId=hintStringResId;
        this.context = context;
        this.onCardClickListener = onCardClickListener;
        getExtras().put(INSET_TYPE_KEY, INSET);
    }

    @Override
    public int getLayout() {
        return R.layout.recycler_item_card_no_result;
    }

    @Override
    public void bind(@NonNull RecyclerItemCardNoResultBinding viewBinding, int position) {
        viewBinding.textRecyclerItemCardNoResult.setText(context.getResources().getString(textStringResId));
        viewBinding.buttonRecyclerItemCardNoResult.setText(context.getResources().getString(buttonStringResId));
        viewBinding.textRecyclerItemCardNoResultSuggestionHint.setText(context.getResources().getString(hintStringResId));

        viewBinding.buttonRecyclerItemCardNoResult.setOnClickListener(v -> onCardClickListener.onClick(buttonStringResId));
    }

    public interface OnCardClickListener {
        void onClick(int textResId);
    }
}
