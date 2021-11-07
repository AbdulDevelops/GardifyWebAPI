package com.gardify.android.ui.generic;

import androidx.annotation.ColorInt;

import com.gardify.android.R;

public class HeaderItemDecoration extends com.gardify.android.ui.generic.decoration.HeaderItemDecoration {
    public HeaderItemDecoration(@ColorInt int background, int sidePaddingPixels) {
        super(background, sidePaddingPixels, R.layout.item_header);
    }
}
