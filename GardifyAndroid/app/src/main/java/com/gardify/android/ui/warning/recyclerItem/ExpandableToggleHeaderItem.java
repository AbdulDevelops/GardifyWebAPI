package com.gardify.android.ui.warning.recyclerItem;

import android.content.Context;
import android.util.Log;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ImageView;
import android.widget.TextView;

import androidx.annotation.NonNull;
import androidx.annotation.StringRes;

import com.gardify.android.R;
import com.gardify.android.data.warning.Warning;
import com.gardify.android.databinding.ItemToggleableHeaderBinding;
import com.gardify.android.ui.MainActivity;
import com.gardify.android.ui.generic.recyclerItem.HeaderToggleItem;
import com.gardify.android.ui.news.recyclerItem.NewsCardItem;
import com.gardify.android.utils.PreferencesUtility;
import com.xwray.groupie.ExpandableGroup;
import com.xwray.groupie.ExpandableItem;

import static com.gardify.android.ui.warning.WarningFragment.OBJECT_TYPE_PLANT;

public class ExpandableToggleHeaderItem extends HeaderToggleItem implements ExpandableItem {

    private ExpandableGroup expandableGroup;
    private int titleStringResId;
    private OnExpandableHeaderListener onExpandableHeaderListener;
    private OnExpandableHeaderSwitchListener onExpandableHeaderSwitchListener;
    private int backgroundColor;
    private int headerTextColor;
    private Context context;
    private Warning warning;
    private boolean isFrostWarning;
    private int newWarningCount;
    private OnWarningClickListener onWarningClickListener;

    public ExpandableToggleHeaderItem(Context context, int headerTextColor, int backGroundColor, String title, Warning warning,
                                      boolean isFrostWarning, OnExpandableHeaderListener onExpandableHeaderListener, OnExpandableHeaderSwitchListener onExpandableHeaderSwitchListener, OnWarningClickListener onWarningClickListener) {
        super(title, 0);
        this.headerTextColor = headerTextColor;
        this.warning = warning;
        this.isFrostWarning = isFrostWarning;
        this.backgroundColor = backGroundColor;
        this.context = context;
        this.onExpandableHeaderListener = onExpandableHeaderListener;
        this.onExpandableHeaderSwitchListener = onExpandableHeaderSwitchListener;
        this.onWarningClickListener = onWarningClickListener;
    }


    @Override
    public void bind(@NonNull final ItemToggleableHeaderBinding viewBinding, int position) {
        super.bind(viewBinding, position);

        if (headerTextColor != 0) {
            viewBinding.title.setTextColor(context.getResources().getColor(headerTextColor, null));
        }
        if (headerTextColor != 0) {
            viewBinding.cardViewExpandableHeader.setCardBackgroundColor(context.getResources().getColor(backgroundColor, null));
        }
        bindDropdownIcon(viewBinding);

        viewBinding.title.setSelected(true);

        viewBinding.icon.setOnClickListener(view -> {
            expandableGroup.onToggleExpanded();
            bindDropdownIcon(viewBinding);
            onExpandableHeaderListener.onClick(titleStringResId);
        });

        enableChildComponents(true, viewBinding.cardViewExpandableHeader);

        //is in pot status
        if (warning.getObjectType() == OBJECT_TYPE_PLANT) {
            viewBinding.icon.setVisibility(View.VISIBLE);
            showPotIcon(viewBinding.imageViewIsInPot);

            if (isSturmWarnungBeetpflanzen()) {
                enableChildComponents(false, viewBinding.cardViewExpandableHeader);
            }
        } else {
            //for devices hide icon and isInPot image
            viewBinding.imageViewIsInPot.setVisibility(View.INVISIBLE);
            viewBinding.icon.setVisibility(View.INVISIBLE);

        }
        viewBinding.tvAlertConditionValue.setText(" ≤ " + warning.getAlertConditionValue() + " °");

        if (isFrostWarning) {
            viewBinding.switchWarning.setChecked(warning.getNotifyForFrost());
        } else {
            viewBinding.switchWarning.setChecked(warning.getNotifyForWind());
        }
        viewBinding.switchWarning.setOnClickListener(v -> onExpandableHeaderSwitchListener.onClickSwitch(warning.getRelatedObjectId(), viewBinding.switchWarning.isChecked()));

        showWarningIcon(viewBinding);

    }

    private boolean isSturmWarnungBeetpflanzen() {
        return !isFrostWarning && !warning.getIsInPot();
    }

    private void showWarningIcon(@NonNull ItemToggleableHeaderBinding viewBinding) {
        // if dismissed is false show warning icon
        if (warning.getDismissed()) {
            viewBinding.imageViewWarningIcon.setVisibility(View.GONE);
        } else {
            viewBinding.imageViewWarningIcon.setVisibility(View.VISIBLE);
            int currentWarningCount = Integer.parseInt(PreferencesUtility.getWarningCount(context));
            viewBinding.imageViewWarningIcon.setOnClickListener(v -> {
                warning.setDismissed(true);
                viewBinding.imageViewWarningIcon.setVisibility(View.GONE);

                newWarningCount = currentWarningCount - 1;
                onWarningClickListener.onClick(newWarningCount);
            });
        }
    }

    public interface OnWarningClickListener {
        void onClick(int newWarningCount);
    }

    private void showPotIcon(@NonNull ImageView imageViewIsInPot) {
        imageViewIsInPot.setVisibility(View.VISIBLE);

        if (warning.getIsInPot()) {
            imageViewIsInPot.setImageResource(R.drawable.warnungen_topfpflanzen);
        } else {
            imageViewIsInPot.setImageResource(R.drawable.warnungen_beetpflanzen);
        }
    }

    private void bindDropdownIcon(ItemToggleableHeaderBinding viewBinding) {
        viewBinding.icon.setImageResource(expandableGroup.isExpanded() ? R.drawable.collapse : R.drawable.expand);

    }

    @Override
    public void setExpandableGroup(@NonNull ExpandableGroup onToggleListener) {
        this.expandableGroup = onToggleListener;
    }

    public interface OnExpandableHeaderListener {
        void onClick(@StringRes int titleStringResId);
    }

    public interface OnExpandableHeaderSwitchListener {
        void onClickSwitch(int id, boolean isChecked);
    }

    private void enableChildComponents(boolean enable, ViewGroup vg) {
        for (int i = 0; i < vg.getChildCount(); i++) {
            View child = vg.getChildAt(i);
            child.setEnabled(enable);
            child.setClickable(enable);
            if (child instanceof ViewGroup) {
                enableChildComponents(enable, (ViewGroup) child);
            }
        }
    }
}
