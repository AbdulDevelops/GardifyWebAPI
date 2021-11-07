package com.gardify.android.ui.plantSearch.recyclerItems;

import android.content.Context;
import android.view.View;
import android.widget.SeekBar;

import androidx.annotation.NonNull;

import com.gardify.android.R;
import com.gardify.android.databinding.RecyclerViewPlantSearchFrostHardBinding;
import com.xwray.groupie.databinding.BindableItem;

import org.jetbrains.annotations.NotNull;

public class FrostHadyCardItem extends BindableItem<RecyclerViewPlantSearchFrostHardBinding> {

    private OnFrostHardClickListener onFrostHardClickListener;
    private Context context;
    String[] temperatureArray = {"10 °C", "5 °C", "0 °C", "-5 °C", "-10 °C", "-15 °C", "-20°C",
            "-25 °C", "-30 °C", "-35 °C", "-40 °C", "-45 °C"};

    public FrostHadyCardItem(Context context, OnFrostHardClickListener onFrostHardClickListener) {
        this.context = context;
        this.onFrostHardClickListener = onFrostHardClickListener;
    }

    @Override
    public int getLayout() {
        return R.layout.recycler_view_plant_search_frost_hard;
    }

    @Override
    public void bind(@NonNull final RecyclerViewPlantSearchFrostHardBinding binding, int position) {
        binding.seekBarFrostHardTemperature.setOnSeekBarChangeListener(seekBarChange(binding));
        binding.seekBarFrostHardZone.setOnSeekBarChangeListener(seekBarChange(binding));
    }

    @NotNull
    private SeekBar.OnSeekBarChangeListener seekBarChange(RecyclerViewPlantSearchFrostHardBinding binding) {
        return new SeekBar.OnSeekBarChangeListener() {
            @Override
            public void onProgressChanged(SeekBar seekBar, int progress, boolean fromUser) {
                binding.seekBarFrostHardZone.setProgress(progress);
                binding.seekBarFrostHardTemperature.setProgress(progress);
                if (seekBar.getProgress() != 0)
                    binding.imageViewSeekbarArrow.setVisibility(View.GONE);
                else binding.imageViewSeekbarArrow.setVisibility(View.VISIBLE);
            }

            @Override
            public void onStartTrackingTouch(SeekBar seekBar) {

            }

            @Override
            public void onStopTrackingTouch(SeekBar seekBar) {
                onFrostHardClickListener.onClick(seekBar.getProgress(), temperatureArray);
            }
        };
    }

    public interface OnFrostHardClickListener {
        void onClick(int progressPosition, String[] temperatureArray);
    }

}
