package com.gardify.android.ui.plantSearch.recyclerItems;

import android.content.Context;
import android.util.Log;
import android.view.View;

import androidx.annotation.NonNull;

import com.gardify.android.R;
import com.gardify.android.databinding.RecyclerViewPlantSearchRangeSliderBinding;
import com.google.android.material.slider.RangeSlider;
import com.xwray.groupie.databinding.BindableItem;

import static com.gardify.android.utils.StringUtils.getShortMonthNameByNumber;


public class RangeSliderCardItem extends BindableItem<RecyclerViewPlantSearchRangeSliderBinding> {

    public static final float PLANT_MIN_HEIGHT = 0f;
    public static final float PLANT_MAX_HEIGHT = 800f;
    public static final float JAN_MONTH_INDEX = 0f;
    public static final float DEC_MONTH_INDEX = 11f;

    private Context context;
    private OnSliderChangeListener onSliderChangeListener;

    private int sliderType;

    public RangeSliderCardItem(Context context, int _sliderType, OnSliderChangeListener onSliderChangeListener) {
        this.context = context;
        this.sliderType = _sliderType;
        this.onSliderChangeListener = onSliderChangeListener;
    }

    @Override
    public int getLayout() {
        return R.layout.recycler_view_plant_search_range_slider;
    }

    @Override
    public void bind(@NonNull RecyclerViewPlantSearchRangeSliderBinding binding, int position) {
        String sliderTitle = "";
        switch (sliderType) {
            case R.string.plantSearch_floweringPeriod:
                binding.rangeSliderMonth.setVisibility(View.VISIBLE);
                binding.rangeSliderHeight.setVisibility(View.GONE);
                binding.textViewRecyclerViewPlantSearchRangeSliderUnit.setVisibility(View.GONE);

                binding.rangeSliderMonth.setValues(JAN_MONTH_INDEX, DEC_MONTH_INDEX);
                binding.rangeSliderMonth.setValueFrom(JAN_MONTH_INDEX);
                binding.rangeSliderMonth.setValueTo(DEC_MONTH_INDEX);
                sliderTitle = getShortMonthNameByNumber(Math.round(JAN_MONTH_INDEX)) + " - " + getShortMonthNameByNumber(Math.round(DEC_MONTH_INDEX));
                binding.textViewRecyclerViewPlantSearchRangeSliderTitle.setText(sliderTitle);
                binding.rangeSliderMonth.setStepSize(1);
                binding.rangeSliderMonth.setLabelFormatter(value -> getShortMonthNameByNumber(Math.round(value)));

                binding.rangeSliderMonth.addOnSliderTouchListener(new RangeSlider.OnSliderTouchListener() {
                    @Override
                    public void onStartTrackingTouch(@NonNull RangeSlider slider) {
                    }

                    @Override
                    public void onStopTrackingTouch(@NonNull RangeSlider sliderMonth) {
                        String sliderTitle = getShortMonthNameByNumber(Math.round(sliderMonth.getValues().get(0))) + " - " + getShortMonthNameByNumber(Math.round(sliderMonth.getValues().get(1)));
                        binding.textViewRecyclerViewPlantSearchRangeSliderTitle.setText(sliderTitle);
                        onSliderChangeListener.onClick(sliderType, sliderMonth, null);
                        Log.d("rangeSlider:", String.valueOf(sliderMonth.getValues()));
                    }
                });

                break;
            case R.string.plantSearch_growthHeight:
                binding.rangeSliderMonth.setVisibility(View.GONE);
                binding.rangeSliderHeight.setVisibility(View.VISIBLE);
                sliderTitle = Math.round(PLANT_MIN_HEIGHT) + " - " + Math.round(PLANT_MAX_HEIGHT);
                binding.textViewRecyclerViewPlantSearchRangeSliderTitle.setText(sliderTitle);

                binding.rangeSliderHeight.setValues(PLANT_MIN_HEIGHT, PLANT_MAX_HEIGHT);
                binding.rangeSliderHeight.setValueFrom(PLANT_MIN_HEIGHT);
                binding.rangeSliderHeight.setValueTo(PLANT_MAX_HEIGHT);
                binding.rangeSliderHeight.setStepSize(0);
                binding.rangeSliderHeight.setLabelFormatter(value -> Math.round(value) + " cm");

                binding.rangeSliderHeight.addOnSliderTouchListener(new RangeSlider.OnSliderTouchListener() {
                    @Override
                    public void onStartTrackingTouch(@NonNull RangeSlider slider) {
                    }

                    @Override
                    public void onStopTrackingTouch(@NonNull RangeSlider sliderHeight) {
                        String sliderTitle = Math.round(sliderHeight.getValues().get(0)) + " - " + Math.round(sliderHeight.getValues().get(1));
                        binding.textViewRecyclerViewPlantSearchRangeSliderTitle.setText(sliderTitle);

                        onSliderChangeListener.onClick(sliderType, null, sliderHeight);
                        Log.d("rangeSlider:", String.valueOf(sliderHeight.getValues()));
                    }
                });

                break;
        }
    }


    public interface OnSliderChangeListener {
        void onClick(int sliderType, RangeSlider sliderValuesMonth, RangeSlider sliderValuesHeight);
    }

}
