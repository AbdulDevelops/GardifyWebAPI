package com.gardify.android.ui.generic;

import android.content.Context;
import android.view.View;

import androidx.annotation.NonNull;
import androidx.annotation.StringRes;

import com.gardify.android.R;
import com.gardify.android.ui.generic.recyclerItem.InnerHeaderItem;
import com.gardify.android.viewModelData.CategoriesIconVM;
import com.gardify.android.databinding.ItemInnerHeaderBinding;
import com.xwray.groupie.ExpandableGroup;
import com.xwray.groupie.ExpandableItem;

import java.util.Optional;

import static com.gardify.android.viewModelData.CategoriesIconVM.filterCategories;

public class ExpandableInnerHeaderItem extends InnerHeaderItem implements ExpandableItem {

    private ExpandableGroup expandableGroup;
    private int titleStringResId;
    String title;
    private int bgColor;
    private Context context;

    public ExpandableInnerHeaderItem(Context context, int bgColor, @StringRes int titleStringResId, @StringRes int subtitleResId) {
        super(titleStringResId, subtitleResId);
        this.titleStringResId = titleStringResId;
        this.bgColor = bgColor;
        this.context = context;
    }

    public ExpandableInnerHeaderItem(Context context, int bgColor, String title, @StringRes int subtitleResId) {
        super(title, subtitleResId);
        this.title = title;
        this.bgColor = bgColor;
        this.context = context;
    }

    @Override
    public void bind(@NonNull final ItemInnerHeaderBinding viewBinding, int position) {
        super.bind(viewBinding, position);

        viewBinding.linearLayout.setBackgroundColor(context.getResources().getColor(bgColor, null));
        viewBinding.icon.setVisibility(View.VISIBLE);
        bindDropdownIcon(viewBinding);
        viewBinding.icon.setOnClickListener(view -> {
            expandableGroup.onToggleExpanded();
            bindDropdownIcon(viewBinding);
        });
        viewBinding.linearLayout.setOnClickListener(view -> {
            expandableGroup.onToggleExpanded();
            bindDropdownIcon(viewBinding);
        });

        getTitleIconIfExists(viewBinding);
    }

    private void getTitleIconIfExists(ItemInnerHeaderBinding binding) {

        Optional<CategoriesIconVM> matchingObject = filterCategories.stream().filter(p -> p.getName().equalsIgnoreCase(title)).findFirst();
        if (matchingObject.isPresent()) {
            CategoriesIconVM categoriesIconVM = matchingObject.get();
            binding.titleIcon.setVisibility(View.VISIBLE);
            binding.titleIcon.setImageDrawable(context.getResources().getDrawable(categoriesIconVM.getImagePath(), null));
        }

    }

    private void bindDropdownIcon(ItemInnerHeaderBinding viewBinding) {
        viewBinding.icon.setVisibility(View.VISIBLE);
        viewBinding.icon.setImageResource(expandableGroup.isExpanded() ? R.drawable.collapse : R.drawable.expand);

    }

    @Override
    public void setExpandableGroup(@NonNull ExpandableGroup onToggleListener) {
        this.expandableGroup = onToggleListener;
    }

    public void setTitle(String _title) {
        this.title = _title;
        notifyChanged();
    }
}
