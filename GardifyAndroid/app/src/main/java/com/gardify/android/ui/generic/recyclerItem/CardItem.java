package com.gardify.android.ui.generic.recyclerItem;

import android.content.Context;
import android.view.View;

import androidx.annotation.NonNull;
import androidx.annotation.StringRes;

import com.gardify.android.R;
import com.gardify.android.databinding.RecyclerItemCardBinding;
import com.xwray.groupie.databinding.BindableItem;

import static com.gardify.android.ui.myGarden.MyGardenFragment.INSET;
import static com.gardify.android.ui.myGarden.MyGardenFragment.INSET_TYPE_KEY;

public class CardItem extends BindableItem<RecyclerItemCardBinding> {

    private int bgColor;
    @StringRes
    private int textStringResId;
    private Context context;
    private OnCardClickListener onCardClickListener;
    private String text;
    private RecyclerItemCardBinding _binding;

    public CardItem(Context context, int bgColor, int textResId, OnCardClickListener onCardClickListener) {
        this.bgColor = bgColor;
        this.textStringResId = textResId;
        this.context = context;
        this.onCardClickListener = onCardClickListener;
        getExtras().put(INSET_TYPE_KEY, INSET);
    }
    public CardItem(Context context, int bgColor, int textResId) {
        this.bgColor = bgColor;
        this.textStringResId = textResId;
        this.context = context;
        getExtras().put(INSET_TYPE_KEY, INSET);
    }
    public CardItem(Context context, int bgColor, String text) {
        this.bgColor = bgColor;
        this.text = text;
        this.context = context;
        getExtras().put(INSET_TYPE_KEY, INSET);
    }

    @Override
    public int getLayout() {
        return R.layout.recycler_item_card;
    }

    @Override
    public void bind(@NonNull RecyclerItemCardBinding viewBinding, int position) {
        _binding = viewBinding;
        viewBinding.addIcon.setVisibility(View.INVISIBLE);
        viewBinding.linearLayout.setBackgroundColor(context.getResources().getColor(bgColor, null));
        if (textStringResId != 0) {
            viewBinding.text.setText(context.getResources().getString(textStringResId));
        } else {
            viewBinding.text.setText(text);
        }
        if (onCardClickListener != null)
            viewBinding.linearLayout.setOnClickListener(v -> onCardClickListener.onClick(textStringResId));
    }

    public interface OnCardClickListener {
        void onClick(int textResId);
    }

    public void setTitle(String title){
        _binding.text.setText(title);
    }
}
