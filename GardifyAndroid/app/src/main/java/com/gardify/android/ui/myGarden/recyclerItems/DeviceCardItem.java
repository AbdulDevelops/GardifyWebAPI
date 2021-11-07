package com.gardify.android.ui.myGarden.recyclerItems;

import android.content.Context;
import android.view.View;

import androidx.annotation.NonNull;

import com.gardify.android.data.myGarden.UserDevice.UserDevice;
import com.gardify.android.R;
import com.gardify.android.viewModelData.DeviceIconsVM;
import com.gardify.android.databinding.RecyclerViewMyGardenDeviceItemBinding;
import com.xwray.groupie.databinding.BindableItem;

import java.util.Optional;

import static com.gardify.android.viewModelData.DeviceIconsVM.deviceIconsList;

/**
 * A card item with a fixed width so it can be used with a horizontal layout manager.
 */
public class DeviceCardItem extends BindableItem<RecyclerViewMyGardenDeviceItemBinding> {

    private UserDevice userDevice;
    private OnDeviceClickListener onDeviceClickListener;
    private Context context;

    public DeviceCardItem(Context context, UserDevice userDevice, OnDeviceClickListener onDeviceClickListener) {
        this.userDevice = userDevice;
        this.onDeviceClickListener = onDeviceClickListener;
        this.context = context;
    }

    @Override
    public int getLayout() {
        return R.layout.recycler_view_my_garden_device_item;
    }

    @Override
    public void bind(@NonNull RecyclerViewMyGardenDeviceItemBinding viewBinding, int position) {
        viewBinding.textViewMyGardenDeviceName.setText(userDevice.getName());
        viewBinding.buttonMyGardenDeviceItemDeviceCount.setText(String.valueOf(userDevice.getCount()));
        viewBinding.switchMyGardenDeviceFrost.setChecked(userDevice.getNotifyForFrost());
        viewBinding.switchMyGardenDeviceStorm.setChecked(userDevice.getNotifyForWind());
        viewBinding.textViewMyGardenDeviceItemMinusButton.setOnClickListener(v -> {
            onDeviceClickListener.onClick(userDevice, viewBinding, viewBinding.textViewMyGardenDeviceItemMinusButton,position);
        });
        viewBinding.textViewMyGardenDeviceItemPlusButton.setOnClickListener(v -> {
            onDeviceClickListener.onClick(userDevice, viewBinding, viewBinding.textViewMyGardenDeviceItemPlusButton,position);
        });
        viewBinding.switchMyGardenDeviceFrost.setOnClickListener(v -> {
            onDeviceClickListener.onClick(userDevice, viewBinding, viewBinding.switchMyGardenDeviceFrost,position);
        });
        viewBinding.switchMyGardenDeviceStorm.setOnClickListener(v -> {
            onDeviceClickListener.onClick(userDevice, viewBinding, viewBinding.switchMyGardenDeviceStorm,position);
        });
        viewBinding.textviewMyGardenDelete.setOnClickListener(v -> {
            onDeviceClickListener.onClick(userDevice, viewBinding, viewBinding.textviewMyGardenDelete,position);
        });

        getTitleIconIfExists(viewBinding);
    }

    public interface OnDeviceClickListener {
        void onClick(UserDevice userDevice, RecyclerViewMyGardenDeviceItemBinding viewBinding, View view, int Pos);
    }

    private void getTitleIconIfExists(RecyclerViewMyGardenDeviceItemBinding binding) {

        Optional<DeviceIconsVM> matchingObject = deviceIconsList.stream().filter(p -> p.getName().equalsIgnoreCase(userDevice.getName())).findFirst();
        if (matchingObject.isPresent()) {
            DeviceIconsVM deviceIconsVM = matchingObject.get();
            binding.titleIcon.setVisibility(View.VISIBLE);
            binding.titleIcon.setImageDrawable(context.getResources().getDrawable(deviceIconsVM.getImagePath(), null));
        }

    }

}
