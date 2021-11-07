package com.gardify.android.ui.generic;

import android.content.Context;
import android.graphics.PorterDuff;
import android.graphics.PorterDuffColorFilter;
import android.view.View;

import androidx.annotation.NonNull;
import androidx.annotation.StringRes;
import androidx.core.content.ContextCompat;

import com.gardify.android.R;
import com.gardify.android.ui.generic.recyclerItem.HeaderItemTodo;
import com.gardify.android.viewModelData.todos.TodoCalendarViewModel;
import com.gardify.android.databinding.ItemHeaderTodoBinding;
import com.xwray.groupie.ExpandableGroup;
import com.xwray.groupie.ExpandableItem;

public class ExpandableHeaderTodoItem extends HeaderItemTodo implements ExpandableItem {

    private ExpandableGroup expandableGroup;
    private OnExpandableHeaderListener onExpandableHeaderListener;

    private int backgroundColor;
    private int headerTextColor;
    private Context context;
    private int iconColor;
    private TodoCalendarViewModel todos;
    public ExpandableHeaderTodoItem(Context context, int iconColor, int headerTextColor, int backGroundColor, TodoCalendarViewModel todos, @StringRes int subtitleResId, OnExpandableHeaderListener onExpandableHeaderListener) {
        super(todos.getTitle(), subtitleResId);
        this.todos = todos;
        this.headerTextColor = headerTextColor;
        this.backgroundColor = backGroundColor;
        this.iconColor = iconColor;
        this.context = context;
        this.onExpandableHeaderListener = onExpandableHeaderListener;
    }

    @Override
    public void bind(@NonNull final ItemHeaderTodoBinding binding, int position) {
        super.bind(binding, position);

        // Initial icon state -- not animated.
        binding.icon.setVisibility(View.VISIBLE);
        if (headerTextColor != 0) {
            binding.title.setTextColor(context.getResources().getColor(headerTextColor, null));
        }
        if (backgroundColor != 0) {
            binding.linearLayoutMain.setBackgroundColor(context.getResources().getColor(backgroundColor, null));
        }
        bindDropdownIcon(binding);
        binding.icon.setColorFilter(getDrawableFilter(iconColor));
        binding.icon.setOnClickListener(view -> {
            expandableGroup.onToggleExpanded();
            bindDropdownIcon(binding);
            onExpandableHeaderListener.onClick(todos, binding, binding.icon);
        });
        binding.todoCheckbox.setChecked(todos.isFinished());
        binding.linearLayoutMain.setOnClickListener(view -> {
            expandableGroup.onToggleExpanded();
            bindDropdownIcon(binding);
            onExpandableHeaderListener.onClick(todos, binding, binding.linearLayoutMain);
        });

        binding.todoOptions.setOnClickListener(v -> {
            onExpandableHeaderListener.onClick(todos, binding, binding.todoOptions);
        });
        binding.todoCheckbox.setOnClickListener(v -> {
            onExpandableHeaderListener.onClick(todos, binding, binding.todoCheckbox);
        });

    }

    private void bindDropdownIcon(ItemHeaderTodoBinding viewBinding) {
        viewBinding.icon.setVisibility(View.VISIBLE);
        viewBinding.icon.setImageResource(expandableGroup.isExpanded() ? R.drawable.collapse_todo : R.drawable.expand_todo);

    }
    public PorterDuffColorFilter getDrawableFilter(int color){
        return new PorterDuffColorFilter(ContextCompat.getColor(context, color), PorterDuff.Mode.SRC_ATOP);
    }

    @Override
    public void setExpandableGroup(@NonNull ExpandableGroup onToggleListener) {
        this.expandableGroup = onToggleListener;
    }

    public interface OnExpandableHeaderListener {
        void onClick(TodoCalendarViewModel todos, ItemHeaderTodoBinding binding, View view);
    }
}
