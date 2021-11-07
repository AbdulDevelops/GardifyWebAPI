package com.gardify.android.ui.generic.recyclerItem;

import android.view.View;

import androidx.annotation.NonNull;
import androidx.annotation.StringRes;

import com.gardify.android.R;
import com.gardify.android.databinding.ItemHeaderTitleBinding;
import com.xwray.groupie.databinding.BindableItem;

import static com.gardify.android.ui.myGarden.MyGardenFragment.INSET;
import static com.gardify.android.ui.myGarden.MyGardenFragment.INSET_TYPE_KEY;

public class HeaderTitle extends BindableItem<ItemHeaderTitleBinding> {

    public static final int TOP_MOST_POSITION_TODO = 2;
    @StringRes
    private int header;
    private String headerString;
    private boolean isCheckVisible =false;
    public HeaderTitle(int header) {
        this.header = header;
        getExtras().put(INSET_TYPE_KEY, INSET);
    }

    public HeaderTitle(String headerString, boolean isCheckVisible) {
        this.headerString = headerString;
        this.isCheckVisible = isCheckVisible;
        getExtras().put(INSET_TYPE_KEY, INSET);
    }

    @Override
    public int getLayout() {
        return R.layout.item_header_title;
    }

    @Override
    public void bind(@NonNull ItemHeaderTitleBinding viewBinding, int position) {

        if (header != 0) {
            viewBinding.text.setText(header);
        } else {
            viewBinding.text.setText(headerString);
        }

        if (isCheckVisible) {
            if (position == TOP_MOST_POSITION_TODO) {
                viewBinding.imageViewCheckedIcon.setVisibility(View.VISIBLE);
            } else {
                viewBinding.imageViewCheckedIcon.setVisibility(View.GONE);
            }
        }

    }

}
