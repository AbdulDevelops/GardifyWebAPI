package com.gardify.android.ui.weather;

import android.content.Context;
import android.util.AttributeSet;
import android.widget.LinearLayout;
import android.widget.TextView;

import androidx.annotation.Nullable;

import com.gardify.android.data.weather.Forecast;
import com.gardify.android.R;
import com.gardify.android.generic.RecycleViewGenericAdapter;
import com.gardify.android.utils.TimeUtils;

public class WeeklyForecastCol extends LinearLayout implements RecycleViewGenericAdapter.RecyclerViewRow<Forecast> {
    private TextView dayTxt, dateTxt, minTempTxt, maxTempTxt;


    public WeeklyForecastCol(Context context) {
        super(context);
    }

    public WeeklyForecastCol(Context context, @Nullable AttributeSet attrs) {
        super(context, attrs);
    }

    public WeeklyForecastCol(Context context, @Nullable AttributeSet attrs, int defStyleAttr) {
        super(context, attrs, defStyleAttr);
    }

    @Override
    protected void onFinishInflate() {
        super.onFinishInflate();
        dayTxt = findViewById(R.id.text_view_weather_daily_forecast_day);
        dateTxt = findViewById(R.id.text_view_weather_daily_forecast_date);
        maxTempTxt = findViewById(R.id.text_view_weather_daily_forecast_max);
        minTempTxt = findViewById(R.id.text_view_weather_daily_forecast_min);
    }

    @Override
    public void showData(Forecast item) {


        String formattedDay = TimeUtils.dateToString(item.getValidFrom(), "yyyy-MM-dd'T'HH:00:00XXX", "EEEE");
        String formattedDate = TimeUtils.dateToString(item.getValidFrom(), "yyyy-MM-dd'T'HH:00:00XXX", "dd.MM");

        dayTxt.setText(formattedDay);
        dateTxt.setText(formattedDate);
        maxTempTxt.setText(item.getMaxAirTemperatureInCelsius()+"");
        minTempTxt.setText(item.getMinAirTemperatureInCelsius()+"");
    }

}
