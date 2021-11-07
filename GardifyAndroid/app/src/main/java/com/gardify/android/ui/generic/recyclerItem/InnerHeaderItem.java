package com.gardify.android.ui.generic.recyclerItem;

import android.annotation.SuppressLint;
import android.view.View;

import androidx.annotation.DrawableRes;
import androidx.annotation.NonNull;
import androidx.annotation.StringRes;

import com.gardify.android.R;
import com.gardify.android.databinding.ItemInnerHeaderBinding;
import com.xwray.groupie.databinding.BindableItem;

public class InnerHeaderItem extends BindableItem<ItemInnerHeaderBinding> {

    @StringRes private int titleStringResId;
    @StringRes private int subtitleResId;
    @DrawableRes private int iconResId;
    String title;
    private View.OnClickListener onIconClickListener;

    public InnerHeaderItem(@StringRes int titleStringResId) {
        this(titleStringResId, 0);
    }

    public InnerHeaderItem(@StringRes int titleStringResId, @StringRes int subtitleResId) {
        this(titleStringResId, subtitleResId, 0, null);
    }
    public InnerHeaderItem(String title, @StringRes int subtitleResId) {
        this(title, subtitleResId, 0, null);
    }
    public InnerHeaderItem(String title, @StringRes int subtitleResId, @DrawableRes int iconResId, View.OnClickListener onIconClickListener) {
        this.title = title;
        this.subtitleResId = subtitleResId;
        this.iconResId = iconResId;
        this.onIconClickListener = onIconClickListener;
    }
    public InnerHeaderItem(@StringRes int titleStringResId, @StringRes int subtitleResId, @DrawableRes int iconResId, View.OnClickListener onIconClickListener) {
        this.titleStringResId = titleStringResId;
        this.subtitleResId = subtitleResId;
        this.iconResId = iconResId;
        this.onIconClickListener = onIconClickListener;
    }

    @Override
    public int getLayout() {
        return R.layout.item_inner_header;
    }

    @SuppressLint("ResourceType")
    @Override
    public void bind(@NonNull ItemInnerHeaderBinding viewBinding, int position) {
        if(titleStringResId==0){
            viewBinding.title.setText(title);
        }else{
            viewBinding.title.setText(titleStringResId);
        }
        if (subtitleResId > 0) {
            viewBinding.subtitle.setText(subtitleResId);
        }
        viewBinding.subtitle.setVisibility(subtitleResId > 0 ? View.VISIBLE : View.GONE);

        if (iconResId > 0) {
            viewBinding.icon.setImageResource(iconResId);
            viewBinding.icon.setOnClickListener(onIconClickListener);
        }
        viewBinding.icon.setVisibility(iconResId > 0 ? View.VISIBLE : View.GONE);
    }
}
