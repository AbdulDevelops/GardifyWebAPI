package com.gardify.android.ui.generic.recyclerItem;

import android.content.Context;

import androidx.annotation.NonNull;

import com.gardify.android.R;
import com.gardify.android.databinding.ItemHeaderTwoIconBinding;
import com.xwray.groupie.databinding.BindableItem;

import static com.gardify.android.ui.myGarden.MyGardenFragment.INSET;
import static com.gardify.android.ui.myGarden.MyGardenFragment.INSET_TYPE_KEY;

public class HeaderTwoIcons extends BindableItem<ItemHeaderTwoIconBinding> {

    private int header;
    private String headerString;
    private onIconClickListener onIconClickListener;
    private int imageOne, imageTwo;
    private boolean isGrid;
    private Context context;
    private ItemHeaderTwoIconBinding _binding;

    public HeaderTwoIcons(int header, Context context, int imageOne, boolean isGrid, int imageTwo, onIconClickListener onIconClickListener) {
        this.header = header;
        this.context = context;
        this.isGrid = isGrid;
        this.imageOne = imageOne;
        this.imageTwo = imageTwo;
        this.onIconClickListener = onIconClickListener;
        getExtras().put(INSET_TYPE_KEY, INSET);
    }

    public HeaderTwoIcons(String headerString, Context context, boolean isGrid, int imageOne, int imageTwo, onIconClickListener onIconClickListener) {
        this.headerString = headerString;
        this.context = context;
        this.imageOne = imageOne;
        this.isGrid = isGrid;
        this.imageTwo = imageTwo;
        this.onIconClickListener = onIconClickListener;
        getExtras().put(INSET_TYPE_KEY, INSET);
    }

    @Override
    public int getLayout() {
        return R.layout.item_header_two_icon;
    }

    @Override
    public void bind(@NonNull ItemHeaderTwoIconBinding viewBinding, int position) {
        _binding= viewBinding;
        viewBinding.text.setText(headerString);

        viewBinding.imageOne.setBackgroundResource(imageOne);
        viewBinding.imageTwo.setBackgroundResource(imageTwo);
        if (isGrid) {
            viewBinding.imageOne.setSelected(true);
            viewBinding.imageTwo.setSelected(false);
        } else {
            viewBinding.imageOne.setSelected(false);
            viewBinding.imageTwo.setSelected(true);
        }

        viewBinding.imageOne.setOnClickListener(v -> {
            onIconClickListener.onClick(true, viewBinding);
        });
        viewBinding.imageTwo.setOnClickListener(v -> {
            onIconClickListener.onClick(false, viewBinding);
        });

    }

    public void setText(String headerTitle) {
        _binding.text.setText(headerTitle);
        headerString = headerTitle;
        _binding.notifyChange();
    }

    public void setGridFlag(boolean isGrid) {
        this.isGrid=isGrid;
    }

    public String getText() {
        return headerString;
    }

    public interface onIconClickListener {
        void onClick(boolean isButtonOne, ItemHeaderTwoIconBinding binding);
    }
}
