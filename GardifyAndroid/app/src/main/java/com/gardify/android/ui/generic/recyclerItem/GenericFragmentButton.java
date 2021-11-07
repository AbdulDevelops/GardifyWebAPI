package com.gardify.android.ui.generic.recyclerItem;

import android.content.Context;
import android.content.res.Configuration;
import android.view.View;

import androidx.annotation.NonNull;

import com.gardify.android.R;
import com.gardify.android.databinding.ItemGenericFragmentButtonBinding;
import com.xwray.groupie.databinding.BindableItem;

public class GenericFragmentButton extends BindableItem<ItemGenericFragmentButtonBinding> {

    private Context context;
    private int buttonIconId = 0;
    private int textTitleId = 0;
    private int textSubTitleId = 0;
    private int bgColorId;
    private int notificationCount = 0;
    private boolean isNotificationCountEnabled = false;
    private FragmentButtonClickListener buttonClickListener;
    private ItemGenericFragmentButtonBinding binding;

    private GenericFragmentButton(Builder builder) {
        this.context = builder.context;
        this.buttonIconId = builder.buttonIconId;
        this.textTitleId = builder.textTitleId;
        this.textSubTitleId = builder.textSubTitleId;
        this.bgColorId = builder.bgColorId;
        this.isNotificationCountEnabled = builder.isNotificationCountEnabled;
        this.buttonClickListener = builder.buttonClickListener;
    }

    @Override
    public int getLayout() {
        return R.layout.item_generic_fragment_button;
    }

    @Override
    public void bind(@NonNull ItemGenericFragmentButtonBinding viewBinding, int position) {
        binding = viewBinding;
        setListener(viewBinding);
        displayIcon(viewBinding);
        displayTitle(viewBinding);
        displaySubtitle(viewBinding);
        changeBackground(viewBinding);
        displayNotificationCount(viewBinding);

    }

    private void setListener(@NonNull ItemGenericFragmentButtonBinding viewBinding) {
        viewBinding.cardViewBackground.setOnClickListener(v -> buttonClickListener.onClick(viewBinding.cardViewBackground));
    }

    @Override
    public int getSpanSize(int spanCount, int position) {
        int currentOrientation = context.getResources().getConfiguration().orientation;
        if (currentOrientation == Configuration.ORIENTATION_LANDSCAPE) {
            // Landscape
            return spanCount / 3;
        } else {
            // Portrait
            return spanCount / 2;
        }
    }

    private void displayIcon(@NonNull ItemGenericFragmentButtonBinding binding) {
        if (buttonIconId != 0) {
            binding.imageViewIcon.setImageResource(buttonIconId);
        }
    }

    private void displayTitle(@NonNull ItemGenericFragmentButtonBinding binding) {
        if (textTitleId != 0) {
            binding.textViewTitle.setText(context.getResources().getString(textTitleId));
        }
    }

    private void displaySubtitle(@NonNull ItemGenericFragmentButtonBinding binding) {
        if (textSubTitleId != 0) {
            binding.textViewSubTitle.setText(context.getResources().getString(textSubTitleId));
        }
    }

    private void changeBackground(@NonNull ItemGenericFragmentButtonBinding binding) {
        if (bgColorId != 0) {
            binding.cardViewBackground.setCardBackgroundColor(context.getResources().getColor(bgColorId, null));
        }
    }

    private void displayNotificationCount(@NonNull ItemGenericFragmentButtonBinding binding) {
        if (isNotificationCountEnabled && notificationCount > 0) {
            binding.textViewNotificationCount.setVisibility(View.VISIBLE);
            binding.textViewNotificationCount.setText(String.valueOf(notificationCount));
        } else {
            binding.textViewNotificationCount.setVisibility(View.INVISIBLE);
        }
    }

    public void setNotificationCount(int count) {
        this.notificationCount = count;
        displayNotificationCount(binding);
        notifyChanged();
    }

    public View getView() {
        String title = context.getResources().getString(textTitleId);

        return binding.cardViewBackground;
    }

    public static class Builder {

        private Context context;
        private int buttonIconId;
        private int textTitleId;
        private int textSubTitleId;
        private boolean isNotificationCountEnabled;
        private int bgColorId;
        private FragmentButtonClickListener buttonClickListener;

        public Builder(Context context) {
            this.context = context;
        }

        public Builder setButtonIcon(int buttonIconId) {
            this.buttonIconId = buttonIconId;
            return this;
        }

        public Builder setTitle(int textTitleId) {
            this.textTitleId = textTitleId;
            return this;
        }

        public Builder setBackgroundColor(int bgColorId) {
            this.bgColorId = bgColorId;
            return this;
        }

        public Builder setSubTitle(int textSubTitleId) {
            this.textSubTitleId = textSubTitleId;
            return this;
        }

        public Builder enableNotificationCount(boolean isNotificationCountEnabled) {
            this.isNotificationCountEnabled = isNotificationCountEnabled;
            return this;
        }

        public Builder setButtonClickListener(FragmentButtonClickListener buttonClickListener) {
            this.buttonClickListener = buttonClickListener;
            return this;
        }

        public GenericFragmentButton build() {
            return new GenericFragmentButton(this);
        }

    }

    public interface FragmentButtonClickListener {
        void onClick(View view);
    }
}
