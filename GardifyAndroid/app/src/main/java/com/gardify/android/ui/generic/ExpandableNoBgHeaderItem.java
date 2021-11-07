package com.gardify.android.ui.generic;

import android.content.Context;
import android.graphics.Color;
import android.view.View;

import androidx.annotation.NonNull;
import androidx.annotation.StringRes;

import com.gardify.android.R;
import com.gardify.android.databinding.ItemNoBgHeaderBinding;
import com.gardify.android.ui.generic.interfaces.OnExpandableHeaderListener;
import com.gardify.android.ui.generic.recyclerItem.NoBgHeaderItem;
import com.xwray.groupie.ExpandableGroup;
import com.xwray.groupie.ExpandableItem;

public class ExpandableNoBgHeaderItem extends NoBgHeaderItem implements ExpandableItem {

    private ExpandableGroup expandableGroup;
    private int titleStringResId;
    private OnExpandableHeaderListener onExpandableHeaderListener;

    private int backgroundColor;
    private int headerTextColor;
    private Context context;
    private String title;
    private int count;
    private boolean isInfoVisible;

    public ExpandableNoBgHeaderItem(Context context, int headerTextColor, int backGroundColor, @StringRes int titleStringResId, @StringRes int subtitleResId, OnExpandableHeaderListener onExpandableHeaderListener) {
        super(titleStringResId, subtitleResId);
        this.titleStringResId = titleStringResId;
        this.backgroundColor = backGroundColor;
        this.headerTextColor = headerTextColor;
        this.context = context;
        this.onExpandableHeaderListener = onExpandableHeaderListener;
    }

    public ExpandableNoBgHeaderItem(Context context, int headerTextColor, int backGroundColor, String title, boolean isInfoVisible, OnExpandableHeaderListener onExpandableHeaderListener) {
        super(title, 0);
        this.title = title;
        this.headerTextColor = headerTextColor;
        this.backgroundColor = backGroundColor;
        this.context = context;
        this.isInfoVisible = isInfoVisible;
        this.onExpandableHeaderListener = onExpandableHeaderListener;
    }

    public ExpandableNoBgHeaderItem(Context context, int headerTextColor, int backGroundColor, @StringRes int titleStringResId) {
        super(titleStringResId, 0);
        this.titleStringResId = titleStringResId;
        this.backgroundColor = backGroundColor;
        this.headerTextColor = headerTextColor;
        this.context = context;
        this.onExpandableHeaderListener = onExpandableHeaderListener;
    }

    @Override
    public void bind(@NonNull final ItemNoBgHeaderBinding viewBinding, int position) {
        super.bind(viewBinding, position);

        // Initial icon state -- not animated.
        viewBinding.icon.setVisibility(View.VISIBLE);
        if (headerTextColor != 0) {
            viewBinding.title.setTextColor(context.getResources().getColor(headerTextColor, null));
        }

        bindDropdownIcon(viewBinding);


        viewBinding.icon.setOnClickListener(view -> {
            expandableGroup.onToggleExpanded();
            bindDropdownIcon(viewBinding);
            if (onExpandableHeaderListener != null)
                onExpandableHeaderListener.onClick(titleStringResId);
        });
        viewBinding.linearLayoutHeader.setOnClickListener(view -> {
            expandableGroup.onToggleExpanded();
            bindDropdownIcon(viewBinding);
            if (onExpandableHeaderListener != null)
                onExpandableHeaderListener.onClick(titleStringResId);
        });

    }

    private void bindDropdownIcon(ItemNoBgHeaderBinding viewBinding) {
        viewBinding.icon.setVisibility(View.VISIBLE);
        viewBinding.icon.setImageResource(expandableGroup.isExpanded() ? R.drawable.collapse : R.drawable.expand);
    }

    @Override
    public void setExpandableGroup(@NonNull ExpandableGroup onToggleListener) {
        this.expandableGroup = onToggleListener;
    }

    public void setCount(int count) {
        if(count > 0 ) {
            super.setCount(count);
        }
    }
}
