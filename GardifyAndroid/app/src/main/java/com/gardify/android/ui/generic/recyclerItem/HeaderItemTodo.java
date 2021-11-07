package com.gardify.android.ui.generic.recyclerItem;

import android.annotation.SuppressLint;
import android.view.View;

import androidx.annotation.DrawableRes;
import androidx.annotation.NonNull;
import androidx.annotation.StringRes;

import com.gardify.android.R;
import com.gardify.android.databinding.ItemHeaderTodoBinding;
import com.xwray.groupie.databinding.BindableItem;

public class HeaderItemTodo extends BindableItem<ItemHeaderTodoBinding> {

    @StringRes private int titleStringResId;
    @StringRes private int subtitleResId;
    @DrawableRes private int iconResId;
    String title;
    private View.OnClickListener onIconClickListener;

    public HeaderItemTodo(@StringRes int titleStringResId) {
        this(titleStringResId, 0);
    }

    public HeaderItemTodo(@StringRes int titleStringResId, @StringRes int subtitleResId) {
        this(titleStringResId, subtitleResId, 0, null);
    }
    public HeaderItemTodo(String title, @StringRes int subtitleResId) {
        this(title, subtitleResId, 0, null);
    }
    public HeaderItemTodo(String title, @StringRes int subtitleResId, @DrawableRes int iconResId, View.OnClickListener onIconClickListener) {
        this.title = title;
        this.subtitleResId = subtitleResId;
        this.iconResId = iconResId;
        this.onIconClickListener = onIconClickListener;
    }
    public HeaderItemTodo(@StringRes int titleStringResId, @StringRes int subtitleResId, @DrawableRes int iconResId, View.OnClickListener onIconClickListener) {
        this.titleStringResId = titleStringResId;
        this.subtitleResId = subtitleResId;
        this.iconResId = iconResId;
        this.onIconClickListener = onIconClickListener;
    }

    @Override
    public int getLayout() {
        return R.layout.item_header_todo;
    }

    @SuppressLint("ResourceType")
    @Override
    public void bind(@NonNull ItemHeaderTodoBinding viewBinding, int position) {
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
    }
}
