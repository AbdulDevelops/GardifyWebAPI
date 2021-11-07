package com.gardify.android.ui.generic.recyclerItem;

import android.annotation.SuppressLint;
import android.view.View;

import androidx.annotation.DrawableRes;
import androidx.annotation.NonNull;
import androidx.annotation.StringRes;

import com.gardify.android.R;
import com.gardify.android.databinding.ItemNoBgHeaderBinding;
import com.xwray.groupie.databinding.BindableItem;

public class NoBgHeaderItem extends BindableItem<ItemNoBgHeaderBinding> {

    @StringRes private int titleStringResId;
    @StringRes private int subtitleResId;
    @DrawableRes private int iconResId;
    String title;
    String count = "";
    private View.OnClickListener onIconClickListener;

    public NoBgHeaderItem(@StringRes int titleStringResId) {
        this(titleStringResId, 0);
    }

    public NoBgHeaderItem(@StringRes int titleStringResId, @StringRes int subtitleResId) {
        this(titleStringResId, 0, null);
    }
    public NoBgHeaderItem(String title, @StringRes int subtitleResId) {
        this(title, 0, null);
    }
    public NoBgHeaderItem(String title, @DrawableRes int iconResId, View.OnClickListener onIconClickListener) {
        this.title = title;
        this.iconResId = iconResId;
        this.onIconClickListener = onIconClickListener;
    }
    public NoBgHeaderItem(@StringRes int titleStringResId,  @DrawableRes int iconResId, View.OnClickListener onIconClickListener) {
        this.titleStringResId = titleStringResId;
        this.iconResId = iconResId;
        this.onIconClickListener = onIconClickListener;
    }

    @Override
    public int getLayout() {
        return R.layout.item_no_bg_header;
    }

    @SuppressLint("ResourceType")
    @Override
    public void bind(@NonNull ItemNoBgHeaderBinding viewBinding, int position) {
        if(titleStringResId==0){
            viewBinding.title.setText(title);
        }else{
            viewBinding.title.setText(titleStringResId);
        }
        if (iconResId > 0) {
            viewBinding.icon.setImageResource(iconResId);
            viewBinding.icon.setOnClickListener(onIconClickListener);
        }
        viewBinding.icon.setVisibility(iconResId > 0 ? View.VISIBLE : View.GONE);
        viewBinding.myGardenCount.setText(count);
    }
    public void setCount (int count) {
        this.count = "(" + count + ")";
        notifyChanged();
    }
}
