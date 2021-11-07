package com.gardify.android.ui.generic;

import android.content.Context;
import android.view.View;

import androidx.annotation.NonNull;
import androidx.annotation.StringRes;

import com.gardify.android.R;
import com.gardify.android.databinding.ItemInputHeaderBinding;
import com.gardify.android.ui.generic.interfaces.OnExpandableInputHeaderListener;
import com.xwray.groupie.ExpandableGroup;
import com.xwray.groupie.ExpandableItem;

public class ExpandableInputHeaderItem extends InputHeaderItem implements ExpandableItem {

    private ExpandableGroup expandableGroup;
    private int titleStringResId;
    private OnExpandableInputHeaderListener onExpandableInputHeaderListener;
    private int backgroundColor;
    private int headerTextColor;
    private Context context;
    public ExpandableInputHeaderItem(Context context, int headerTextColor, int backGroundColor, @StringRes int titleStringResId, @StringRes int subtitleResId, OnExpandableInputHeaderListener onExpandableInputHeaderListener) {
        super(titleStringResId, subtitleResId);
        this.titleStringResId = titleStringResId;
        this.backgroundColor = backGroundColor;
        this.headerTextColor = headerTextColor;
        this.context = context;
        this.onExpandableInputHeaderListener = onExpandableInputHeaderListener;
    }


    @Override
    public void bind(@NonNull final ItemInputHeaderBinding _binding, int position) {
        super.bind(_binding, position);
        // Initial icon state -- not animated.
        _binding.linearLayoutExpandableHeader.setBackgroundColor(context.getResources().getColor(backgroundColor, null));
        _binding.title.setTextColor(context.getResources().getColor(headerTextColor, null));
       bindDropdownIcon(_binding);
        _binding.icon.setOnClickListener(view -> {
            expandableGroup.onToggleExpanded();
            bindDropdownIcon(_binding);
            onExpandableInputHeaderListener.onClick(titleStringResId, "");
        });
        _binding.linearLayoutExpandableHeader.setOnClickListener(view -> {
            expandableGroup.onToggleExpanded();
            bindDropdownIcon(_binding);
            onExpandableInputHeaderListener.onClick(titleStringResId, "");
        });

        _binding.icon.setVisibility(View.VISIBLE);
        _binding.count.setVisibility(View.VISIBLE);
        _binding.autoCompleteEditTextName.setEnabled(false);
    }

    private void bindDropdownIcon(ItemInputHeaderBinding viewBinding) {
        viewBinding.icon.setVisibility(View.VISIBLE);
        viewBinding.icon.setImageResource(expandableGroup.isExpanded() ? R.drawable.collapse : R.drawable.expand);
    }

    @Override
    public void setExpandableGroup(@NonNull ExpandableGroup onToggleListener) {
        this.expandableGroup = onToggleListener;
    }

    @Override
    public void setTitle(int count) {
        if (count > 0)
            super.setTitle(count);
    }

}
