package com.gardify.android.ui.warning.recyclerItem;

import android.content.Context;
import android.view.View;

import androidx.annotation.NonNull;
import androidx.core.content.ContextCompat;

import com.gardify.android.R;
import com.gardify.android.databinding.ItemHeaderIconsWarningBinding;
import com.xwray.groupie.databinding.BindableItem;

import static com.gardify.android.ui.myGarden.MyGardenFragment.INSET;
import static com.gardify.android.ui.myGarden.MyGardenFragment.INSET_TYPE_KEY;

public class HeaderIcons extends BindableItem<ItemHeaderIconsWarningBinding> {

    private int header;
    private String headerString;
    private OnIconClickListener onIconClickListener;
    private int imageOne, imageTwo;
    private Context context;
    private ItemHeaderIconsWarningBinding _binding;

    public HeaderIcons(String headerString, Context context, int imageOne, int imageTwo, OnIconClickListener onIconClickListener) {
        this.headerString = headerString;
        this.context = context;
        this.imageOne = imageOne;
        this.imageTwo = imageTwo;
        this.onIconClickListener = onIconClickListener;
        getExtras().put(INSET_TYPE_KEY, INSET);
    }

    @Override
    public int getLayout() {
        return R.layout.item_header_icons_warning;
    }

    @Override
    public void bind(@NonNull ItemHeaderIconsWarningBinding viewBinding, int position) {
        _binding = viewBinding;

        viewBinding.imageOne.setImageDrawable(ContextCompat.getDrawable(context, imageOne));
        viewBinding.imageTwo.setImageDrawable(ContextCompat.getDrawable(context, imageTwo));

        viewBinding.imageOne.setOnClickListener(v -> {
            onIconClickListener.onClick(viewBinding, viewBinding.imageOne);
        });
        viewBinding.imageTwo.setOnClickListener(v -> {
            onIconClickListener.onClick(viewBinding, viewBinding.imageTwo);
        });

        //reset warning
        viewBinding.cardViewResetWarning.setOnClickListener(v -> {
            onIconClickListener.onClick(viewBinding, viewBinding.cardViewResetWarning);
        });

    }

    public int getText() {
        return header;
    }

    public interface OnIconClickListener {
        void onClick(ItemHeaderIconsWarningBinding viewBinding, View view);
    }
}
