package com.gardify.android.ui.generic.recyclerItem;

import android.annotation.SuppressLint;
import android.view.View;

import androidx.annotation.DrawableRes;
import androidx.annotation.NonNull;
import androidx.annotation.StringRes;

import com.gardify.android.R;
import com.gardify.android.databinding.ItemToggleableHeaderBinding;
import com.xwray.groupie.databinding.BindableItem;

public class HeaderToggleItem extends BindableItem<ItemToggleableHeaderBinding> {

    @StringRes
    private int titleStringResId;
    @StringRes
    private int subtitleResId;
    @DrawableRes
    private int iconResId;
    String title;
    private View.OnClickListener onIconClickListener;


    public HeaderToggleItem(String title, @StringRes int subtitleResId) {
        this(title, subtitleResId, 0, null);
    }

    public HeaderToggleItem(String title, @StringRes int subtitleResId, @DrawableRes int iconResId, View.OnClickListener onIconClickListener) {
        this.title = title;
        this.subtitleResId = subtitleResId;
        this.iconResId = iconResId;
        this.onIconClickListener = onIconClickListener;
    }

    @Override
    public int getLayout() {
        return R.layout.item_toggleable_header;
    }

    @SuppressLint("ResourceType")
    @Override
    public void bind(@NonNull ItemToggleableHeaderBinding viewBinding, int position) {
        if (titleStringResId == 0) {
            viewBinding.title.setText(title);
        } else {
            viewBinding.title.setText(titleStringResId);
        }
        if (iconResId > 0) {
            viewBinding.icon.setImageResource(iconResId);
            viewBinding.icon.setOnClickListener(onIconClickListener);
        }
        viewBinding.icon.setVisibility(iconResId > 0 ? View.VISIBLE : View.GONE);

    }
}
