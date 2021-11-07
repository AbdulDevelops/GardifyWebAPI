package com.gardify.android.ui.generic;

import android.annotation.SuppressLint;
import android.view.View;

import androidx.annotation.DrawableRes;
import androidx.annotation.NonNull;
import androidx.annotation.StringRes;

import com.gardify.android.R;
import com.gardify.android.databinding.ItemInputHeaderBinding;
import com.xwray.groupie.databinding.BindableItem;

public class InputHeaderItem extends BindableItem<ItemInputHeaderBinding> {

    @StringRes private int titleStringResId;
    @StringRes private int subtitleResId;
    @DrawableRes private int iconResId;
    private String title;
    private String itemCount=null;
    private View.OnClickListener onIconClickListener;

    public InputHeaderItem(@StringRes int titleStringResId) {
        this(titleStringResId, 0);
    }

    public InputHeaderItem(@StringRes int titleStringResId, @StringRes int subtitleResId) {
        this(titleStringResId, subtitleResId, 0, null);
    }
    public InputHeaderItem(String title, @StringRes int subtitleResId) {
        this(title, subtitleResId, 0, null);
    }
    public InputHeaderItem(String title, @StringRes int subtitleResId, @DrawableRes int iconResId, View.OnClickListener onIconClickListener) {
        this.title = title;
        this.subtitleResId = subtitleResId;
        this.iconResId = iconResId;
        this.onIconClickListener = onIconClickListener;
    }
    public InputHeaderItem(@StringRes int titleStringResId, @StringRes int subtitleResId, @DrawableRes int iconResId, View.OnClickListener onIconClickListener) {
        this.titleStringResId = titleStringResId;
        this.subtitleResId = subtitleResId;
        this.iconResId = iconResId;
        this.onIconClickListener = onIconClickListener;
    }

    @Override
    public int getLayout() {
        return R.layout.item_input_header;
    }

    @SuppressLint("ResourceType")
    @Override
    public void bind(@NonNull ItemInputHeaderBinding viewBinding, int position) {
        if(titleStringResId==0){
            viewBinding.title.setText(title);
        }else{
            viewBinding.title.setText(titleStringResId);
        }
        if (subtitleResId > 0) {
            viewBinding.autoCompleteEditTextName.setHint(subtitleResId);
        }
        viewBinding.autoCompleteEditTextName.setVisibility(subtitleResId > 0 ? View.VISIBLE : View.GONE);

        if (iconResId > 0) {
            viewBinding.icon.setImageResource(iconResId);
            viewBinding.icon.setOnClickListener(onIconClickListener);
        }
        viewBinding.icon.setVisibility(iconResId > 0 ? View.VISIBLE : View.GONE);

        if(itemCount!=null){
            viewBinding.count.setText(itemCount);
        }
    }

    public void setTitle(int count) {
        this.itemCount = "(" + count + ")";
        notifyChanged();
    }
}
