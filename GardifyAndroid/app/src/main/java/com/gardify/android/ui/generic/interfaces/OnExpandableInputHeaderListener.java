package com.gardify.android.ui.generic.interfaces;

import androidx.annotation.StringRes;

public interface OnExpandableInputHeaderListener {
    void onClick(@StringRes int titleStringResId, String typedText);
}