package com.gardify.android.ui.generic.recyclerItem;

import android.content.Context;
import android.content.res.ColorStateList;
import android.util.TypedValue;
import android.view.ContextThemeWrapper;
import android.view.View;
import android.widget.LinearLayout;

import androidx.annotation.NonNull;
import androidx.annotation.StringRes;
import androidx.annotation.StyleRes;

import com.gardify.android.R;
import com.gardify.android.databinding.ItemGenericButtonBinding;
import com.google.android.material.button.MaterialButton;
import com.xwray.groupie.databinding.BindableItem;

import java.util.ArrayList;
import java.util.List;

public class GenericButton extends BindableItem<ItemGenericButtonBinding> {

    private static final int DEFAULT_MARGIN_8 = 8;
    private static final int DEFAULT_MARGIN_16 = 16;

    private Context context;
    private List<MaterialButton> buttonList;
    private View selectedButton;
    private int buttonColorState = 0;
    private int buttonTextColorState = 0;
    private ItemGenericButtonBinding binding;
    private int orientation;

    private GenericButton(Builder builder) {
        this.context = builder.context;
        this.buttonList = builder.buttonList;
        this.buttonColorState = builder.buttonColorState;
        this.buttonTextColorState = builder.buttonTextColorState;
        this.orientation = builder.orientation;
    }

    @Override
    public int getLayout() {
        return R.layout.item_generic_button;
    }

    @Override
    public void bind(@NonNull ItemGenericButtonBinding viewBinding, int position) {
        binding = viewBinding;

        if (viewBinding.llButtonContainer.getChildCount() == 0) {
            displayButtons(viewBinding);
        }

        if (buttonColorState != 0) {
            defaultButtonSelection(viewBinding);
            updateSelectedButtonColor(binding);
        }

    }

    private void defaultButtonSelection(ItemGenericButtonBinding binding) {
        MaterialButton button = (MaterialButton) binding.llButtonContainer.getChildAt(0);
        selectedButton = button;
    }

    private void displayButtons(ItemGenericButtonBinding binding) {
        binding.llButtonContainer.setOrientation(orientation);

        for (int position = 0; position < buttonList.size(); position++) {
            View button = buttonList.get(position);

            setButtonPadding(position, button);

            binding.llButtonContainer.addView(button);
        }
    }

    private void setButtonPadding(int position, View button) {
        int padding = (int) context.getResources().getDimension(R.dimen.marginPaddingSize_16sdp);
        button.setPadding(0, padding, 0, padding);
    }

    public void setSelectedButton(View selectedButton) {
        if (buttonColorState != 0) {
            this.selectedButton = selectedButton;
            updateSelectedButtonColor(binding);
            notifyChanged();
        }
    }

    private void updateSelectedButtonColor(@NonNull ItemGenericButtonBinding viewBinding) {

        for (int i = 0; i < viewBinding.llButtonContainer.getChildCount(); i++) {
            MaterialButton v = (MaterialButton) viewBinding.llButtonContainer.getChildAt(i);
            ColorStateList bgColorStateList = context.getResources().getColorStateList(buttonColorState, null);
            ColorStateList textColorStateList = context.getResources().getColorStateList(buttonTextColorState, null);
            v.setBackgroundTintList(bgColorStateList);
            v.setTextColor(textColorStateList);

            if (v == selectedButton) {
                v.setSelected(true);
            } else {
                v.setSelected(false);
            }
        }
    }

    public static class Builder {

        private Context context;
        private List<MaterialButton> buttonList = new ArrayList<>();
        private int buttonColorState, buttonTextColorState;
        private int orientation;
        public Builder(Context context) {
            this.context = context;
        }

        public Builder addNewButton(@StyleRes int style, int buttonText, int textSizeId, final HeaderButtonClickListener headerButtonClickListener) {
            MaterialButton addButton = new MaterialButton(new ContextThemeWrapper(context, style), null, style);
            LinearLayout.LayoutParams layoutParams = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MATCH_PARENT, LinearLayout.LayoutParams.WRAP_CONTENT, 1f);
            layoutParams.setMargins(8, 3, 8, 3);
            addButton.setLayoutParams(layoutParams);
            addButton.setText(buttonText);
            addButton.setTextSize(TypedValue.COMPLEX_UNIT_PX, context.getResources().getDimension(textSizeId));
            addButton.setOnClickListener(view -> headerButtonClickListener.onClick(buttonText, addButton));
            buttonList.add(addButton);
            return this;
        }

        public Builder setButtonColorState(int buttonBgColorState, int buttonTextColorState) {
            this.buttonColorState = buttonBgColorState;
            this.buttonTextColorState = buttonTextColorState;
            return this;
        }

        public Builder setOrientation(int orientation) {
            this.orientation = orientation;
            return this;
        }

        private Builder setButtonList(List<MaterialButton> buttonList) {
            this.buttonList = buttonList;
            return this;
        }

        public GenericButton build() {
            setButtonList(buttonList);
            return new GenericButton(this);
        }

    }

    public interface HeaderButtonClickListener {
        void onClick(@StringRes int buttonString, View view);
    }
}
