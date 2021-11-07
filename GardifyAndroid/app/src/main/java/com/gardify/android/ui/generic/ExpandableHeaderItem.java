package com.gardify.android.ui.generic;

import android.content.Context;
import android.view.View;

import androidx.annotation.NonNull;
import androidx.annotation.StringRes;

import com.gardify.android.R;
import com.gardify.android.ui.generic.interfaces.OnExpandableHeaderListener;
import com.gardify.android.ui.generic.recyclerItem.HeaderItem;
import com.gardify.android.databinding.ItemHeaderBinding;
import com.xwray.groupie.ExpandableGroup;
import com.xwray.groupie.ExpandableItem;

public class ExpandableHeaderItem extends HeaderItem implements ExpandableItem {

    private ExpandableGroup expandableGroup;
    private int titleStringResId;
    private OnExpandableHeaderListener onExpandableHeaderListener;

    private int backgroundColor;
    private int headerTextColor;
    private Context context;
    private String title;
    private int count;
    private boolean isInfoVisible;

    public ExpandableHeaderItem(Context context, int headerTextColor, int backGroundColor, @StringRes int titleStringResId, @StringRes int subtitleResId, OnExpandableHeaderListener onExpandableHeaderListener) {
        super(titleStringResId, subtitleResId);
        this.titleStringResId = titleStringResId;
        this.backgroundColor = backGroundColor;
        this.headerTextColor = headerTextColor;
        this.context = context;
        this.onExpandableHeaderListener = onExpandableHeaderListener;
    }

    public ExpandableHeaderItem(Context context, int headerTextColor, int backGroundColor, String title, boolean isInfoVisible, OnExpandableHeaderListener onExpandableHeaderListener) {
        super(title, 0);
        this.title = title;
        this.headerTextColor = headerTextColor;
        this.backgroundColor = backGroundColor;
        this.context = context;
        this.isInfoVisible = isInfoVisible;
        this.onExpandableHeaderListener = onExpandableHeaderListener;
    }

    public ExpandableHeaderItem(int count, Context context, int headerTextColor, int backGroundColor, String title, @StringRes int subtitleResId, OnExpandableHeaderListener onExpandableHeaderListener) {
        super(title, subtitleResId);
        this.title = title;
        this.headerTextColor = headerTextColor;
        this.backgroundColor = backGroundColor;
        this.context = context;
        this.onExpandableHeaderListener = onExpandableHeaderListener;
        this.count = count;
    }

    @Override
    public void bind(@NonNull final ItemHeaderBinding viewBinding, int position) {
        super.bind(viewBinding, position);

        // Initial icon state -- not animated.
        viewBinding.icon.setVisibility(View.VISIBLE);

        if (headerTextColor != 0) {
            viewBinding.title.setTextColor(context.getResources().getColor(headerTextColor, null));
            viewBinding.count.setTextColor(context.getResources().getColor(headerTextColor, null));
        }
        if (backgroundColor != 0) {
            viewBinding.cardViewExpandableHeader.setCardBackgroundColor(context.getResources().getColor(backgroundColor, null));
        }

        bindDropdownIcon(viewBinding);

        // show notification icon
        if (isInfoVisible)
            viewBinding.iconNotification.setVisibility(View.VISIBLE);
        else
            viewBinding.iconNotification.setVisibility(View.GONE);

        viewBinding.iconNotification.setImageResource(R.drawable.ic_info);

        viewBinding.icon.setOnClickListener(view -> {
            expandableGroup.onToggleExpanded();
            bindDropdownIcon(viewBinding);
            onExpandableHeaderListener.onClick(titleStringResId);
        });
        viewBinding.cardViewExpandableHeader.setOnClickListener(view -> {
            expandableGroup.onToggleExpanded();
            bindDropdownIcon(viewBinding);
            onExpandableHeaderListener.onClick(titleStringResId);
        });

    }

    private void bindDropdownIcon(ItemHeaderBinding viewBinding) {
        viewBinding.icon.setVisibility(View.VISIBLE);
        viewBinding.icon.setImageResource(expandableGroup.isExpanded() ? R.drawable.collapse : R.drawable.expand);
    }

    @Override
    public void setExpandableGroup(@NonNull ExpandableGroup onToggleListener) {
        this.expandableGroup = onToggleListener;
    }

    @Override
    public void setTitle(int count) {
        if (count >= 0)
            super.setTitle(count);
    }
}
